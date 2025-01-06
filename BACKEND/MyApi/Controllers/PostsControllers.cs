using Microsoft.AspNetCore.Mvc;
using MyApi.Data;
using MyApi.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

[Route("api/[controller]")]
[ApiController]
public class PostsController : ControllerBase
{
    private readonly MyDbContext _context;
    private readonly ILogger<PostsController> _logger;

    public PostsController(MyDbContext context, ILogger<PostsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: api/Posts OBTENER TODOS LOS POST
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Post>>> GetPosts()
    {
        _logger.LogInformation("GetPosts method called.");

        var posts = await _context.Posts.ToListAsync();
        _logger.LogInformation("Number of posts retrieved: {Count}", posts.Count);

        return posts;
    }

    // GET: api/Posts/{Id} OBTENER POST POR ID
    [HttpGet("{id}")]
    public async Task<ActionResult<Post>> GetPost(int id)
    {
        _logger.LogInformation("GetPost method called for id: {Id}", id);

        var post = await _context.Posts.FindAsync(id);

        if (post == null)
        {
            _logger.LogWarning("Post with id: {Id} not found.", id);
            return NotFound();
        }

        _logger.LogInformation("Post retrieved: {Post}", post);
        return post;
    }


  // GET: api/Posts/ByName/{name} OBTENER POSTS POR NOMBRE
[HttpGet("ByName/{name}")]
public async Task<ActionResult<IEnumerable<Post>>> GetPostsByName(string name)
{
    _logger.LogInformation("GetPostsByName method called for name: {Name}", name);

    var posts = await _context.Posts.ToListAsync();
    var filteredPosts = posts
        .Where(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
        .ToList();

    _logger.LogInformation("Number of posts retrieved with name {Name}: {Count}", name, filteredPosts.Count);

    if (filteredPosts.Count == 0)
    {
        return NotFound();
    }

    return Ok(filteredPosts);
}




    // POST: api/Posts y JSON AGREGAR POST
    [HttpPost]
    public async Task<ActionResult<Post>> PostPost(Post post)
    {
        _logger.LogInformation("PostPost method called.");

        _context.Posts.Add(post);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Post created with ID: {Id}", post.Id);
        return CreatedAtAction(nameof(GetPost), new { id = post.Id }, post);
    }

    // PUT: api/Posts/{Id} y JSON EDITAR POST
[HttpPut("{id}")]
public async Task<IActionResult> PutPost(int id, Post post)
{
    if (id != post.Id)
    {
        return BadRequest();
    }

    _context.Entry(post).State = EntityState.Modified;

    try
    {
        await _context.SaveChangesAsync();
        _logger.LogInformation("Post with ID: {Id} updated.", id);
    }
    catch (DbUpdateConcurrencyException)
    {
        if (!PostExists(id))
        {
            return NotFound();
        }
        else
        {
            throw;
        }
    }

    // Retornar el post actualizado
    return Ok(post);
}

    // DELETE: api/Posts/{Id} ELIMINAR POST
[HttpDelete("{id}")]
public async Task<ActionResult<Post>> DeletePost(int id)
{
    var post = await _context.Posts.FindAsync(id);
    if (post == null)
    {
        return NotFound();
    }

    _context.Posts.Remove(post);
    await _context.SaveChangesAsync();

    return post;
}

    


    private bool PostExists(int id)
    {
        return _context.Posts.Any(e => e.Id == id);
    }
}
