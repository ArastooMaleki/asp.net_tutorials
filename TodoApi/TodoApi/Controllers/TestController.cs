using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

namespace TodoApi.Controllers;


[Route("apiTest/[controller]")]
[ApiController]
public class TestController : ControllerBase
{
    private readonly TodoContext _context;
    public TestController(TodoContext context)
    {
        _context = context;
        if (_context.TodoItems.Count() ==0 ) {
            for (int i = 1; i < 5; i++)
            {
                TodoItem obj = new TodoItem();
                obj.Name = $"task{i}"; obj.IsComplete = false;
                PostTodoItem(obj);
            }
        }
    }
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TodoItem>>> GetTodoItems()
    {
        return await _context.TodoItems.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TodoItem>> GetTodoItem(int id)
    {
        TodoItem item = await _context.TodoItems.FindAsync(id);
        if (item == null)
        {
            return NotFound();
        }
        else
        {
            return item;
        }

    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutTodoItem(int id , TodoItem newItem)
    {
        if (id != newItem.ID)
        {
            return BadRequest();
        }
        _context.Entry(newItem).State = EntityState.Modified;
        try
        {
            await _context.SaveChangesAsync();
        }
        catch(Exception ex)
        {
            if (!(_context.TodoItems.Any(e=>e.ID == id))) {
                return NotFound();
            }
            else
            {
                throw;
            }
        }
        return NoContent();
    }

    [HttpPost]
    public async Task<ActionResult<TodoItem>> PostTodoItem(TodoItem item)
    {
        _context.TodoItems.AddAsync(item);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(PostTodoItem), new { id = item.ID }, item);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTodoItem(int id)
    {
        var item = await _context.TodoItems.FindAsync(id);
        if (item == null)
        {
            return NotFound();
        }
        _context.TodoItems.Remove(item);
        await _context.SaveChangesAsync();
        return NoContent();
    }

}