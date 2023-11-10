using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Invoice.Models;
using Invoice.Data;
using Invoice.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Invoice.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ItemsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ItemsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/Items
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Item>>> GetItems()
    {
        return await _context.Items.ToListAsync();
    }

    // POST: api/Items
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<Item>> PostItem([FromBody] ItemDTO itemDTO)
    {
        var item = new Item
        {
            Name = itemDTO.Name,
            Price = itemDTO.Price
        };

        _context.Items.Add(item);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetItem), new { id = item.ItemId }, item);
    }

    // GET: api/Items/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Item>> GetItem(int id)
    {
        var item = await _context.Items.FindAsync(id);

        if (item == null)
        {
            return NotFound();
        }

        return item;
    }

    // PUT: api/Items/5
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateItem(int id, [FromBody] ItemDTO itemDTO)
    {
        var item = await _context.Items.FindAsync(id);
        if (item == null)
        {
            return NotFound();
        }

        item.Name = itemDTO.Name;
        item.Price = itemDTO.Price;

        _context.Entry(item).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException) when (!ItemExists(id))
        {
            return NotFound();
        }

        return NoContent();
    }

    // DELETE: api/Items/5
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteItem(int id)
    {
        var item = await _context.Items.FindAsync(id);
        if (item == null)
        {
            return NotFound();
        }

        _context.Items.Remove(item);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool ItemExists(int id) =>
         _context.Items.Any(e => e.ItemId == id);
}
