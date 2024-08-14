using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Models;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using Microsoft.AspNetCore.JsonPatch;


namespace TodoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoItemsController : ControllerBase
    {
        private readonly TodoContext _context;

        public TodoItemsController(TodoContext context)
        {
            _context = context;
        }

        // GET: api/TodoItems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItem>>> GetTodoItems()
        {
            return await _context.TodoItems.ToListAsync();
        }

        // GET: api/TodoItems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItem>> GetTodoItem(long id)
        {
            var todoItem = await _context.TodoItems.FindAsync(id);

            if (todoItem == null)
            {
                return NotFound();
            }

            return todoItem;
        }
         [HttpGet("users")]
        public IActionResult GetUsers()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "veriler.csv");
            IEnumerable<TodoItem> users;

            try
            {
                using (var reader = new StreamReader(path))
                using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)))
                {
                    users = csv.GetRecords<TodoItem>().ToList();
                }

                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
         [HttpGet("users/{id}")]
public IActionResult GetUser(int id)
{
    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "veriler.csv");
    IEnumerable<TodoItem> users;

    try
    {
        // CSV dosyasını oku ve verileri al
        using (var reader = new StreamReader(path))
        using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)))
        {
            users = csv.GetRecords<TodoItem>().ToList();
        }

        // Belirli bir ID'ye sahip kullanıcıyı bul
        var user = users.FirstOrDefault(u => u.Id == id);
        if (user == null)
        {
            return NotFound($"User with ID {id} not found.");
        }

        return Ok(user);
    }
    catch (Exception ex)
    {
        return StatusCode(500, $"Internal server error: {ex.Message}");
    }
}
        [HttpDelete("users/{id}")]
public IActionResult DeleteUser(int id)
{
    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "veriler.csv");
    List<TodoItem> users;

    try
    {
        // CSV dosyasını oku ve verileri al
        using (var reader = new StreamReader(path))
        using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)))
        {
            users = csv.GetRecords<TodoItem>().ToList();
        }

        // Silinmek istenen kaydı bul
        var userToDelete = users.FirstOrDefault(u => u.Id == id);
        if (userToDelete == null)
        {
            return NotFound($"User with ID {id} not found.");
        }

        // Kaydı listeden çıkar
        users.Remove(userToDelete);

        // Güncellenmiş veriyi tekrar CSV dosyasına yaz
        using (var writer = new StreamWriter(path))
        using (var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)))
        {
            csv.WriteRecords(users);
        }

        return NoContent(); // Başarılı silme işleminde 204 No Content döndürülür
    }
    catch (Exception ex)
    {
        return StatusCode(500, $"Internal server error: {ex.Message}");
    }
}
[HttpPut("users/{id}")]
public IActionResult UpdateUser(int id, [FromBody] TodoItem updatedUser)
{
    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "veriler.csv");
    List<TodoItem> users;

    try
    {
        // CSV dosyasını oku ve verileri al
        using (var reader = new StreamReader(path))
        using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)))
        {
            users = csv.GetRecords<TodoItem>().ToList();
        }

        // Güncellenecek kaydı bul
        var userToUpdate = users.FirstOrDefault(u => u.Id == id);
        if (userToUpdate == null)
        {
            return NotFound($"User with ID {id} not found.");
        }

        // Kaydı güncelle
        userToUpdate.Name = updatedUser.Name;
        userToUpdate.IsComplete = updatedUser.IsComplete;

        // Güncellenmiş veriyi tekrar CSV dosyasına yaz
        using (var writer = new StreamWriter(path))
        using (var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)))
        {
            csv.WriteRecords(users);
        }

        return NoContent(); // Başarılı güncelleme işleminde 204 No Content döndürülür
    }
    catch (Exception ex)
    {
        return StatusCode(500, $"Internal server error: {ex.Message}");
    }
}
[HttpPatch("users/{id}")]
public IActionResult PatchUser(int id, [FromBody] JsonPatchDocument<TodoItem> patchDoc)
{
    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "veriler.csv");
    List<TodoItem> users;

    try
    {
        // CSV dosyasını oku ve verileri al
        using (var reader = new StreamReader(path))
        using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)))
        {
            users = csv.GetRecords<TodoItem>().ToList();
        }

        // Güncellenecek kaydı bul
        var userToPatch = users.FirstOrDefault(u => u.Id == id);
        if (userToPatch == null)
        {
            return NotFound($"User with ID {id} not found.");
        }

        // Kaydı patch ile güncelle
        patchDoc.ApplyTo(userToPatch);

        // Güncellenmiş veriyi tekrar CSV dosyasına yaz
        using (var writer = new StreamWriter(path))
        using (var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)))
        {
            csv.WriteRecords(users);
        }

        return NoContent(); // Başarılı güncelleme işleminde 204 No Content döndürülür
    }
    catch (Exception ex)
    {
        return StatusCode(500, $"Internal server error: {ex.Message}");
    }
}
[HttpPost("users")]
public IActionResult CreateUser([FromBody] TodoItem newUser)
{
    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "veriler.csv");
    List<TodoItem> users;

    try
    {
        // CSV dosyasını oku ve mevcut verileri al
        using (var reader = new StreamReader(path))
        using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)))
        {
            users = csv.GetRecords<TodoItem>().ToList();
        }

        // Yeni kullanıcının ID'sinin benzersiz olduğundan emin ol
        if (users.Any(u => u.Id == newUser.Id))
        {
            return BadRequest($"User with ID {newUser.Id} already exists.");
        }

        // Yeni kullanıcıyı listeye ekle
        users.Add(newUser);

        // Güncellenmiş veriyi tekrar CSV dosyasına yaz
        using (var writer = new StreamWriter(path))
        using (var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)))
        {
            csv.WriteRecords(users);
        }

        // 201 Created ve yeni kaydın URL'si ile birlikte döndür
        return CreatedAtAction(nameof(CreateUser), new { id = newUser.Id }, newUser);
    }
    catch (Exception ex)
    {
        return StatusCode(500, $"Internal server error: {ex.Message}");
    }
}
         

        // PUT: api/TodoItems/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTodoItem(long id, TodoItem todoItem)
        {
            if (id != todoItem.Id)
            {
                return BadRequest();
            }

            _context.Entry(todoItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TodoItemExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/TodoItems
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TodoItem>> PostTodoItem(TodoItem todoItem)
        {
            _context.TodoItems.Add(todoItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTodoItem), new { id = todoItem.Id }, todoItem);
        }

        // DELETE: api/TodoItems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodoItem(long id)
        {
            var todoItem = await _context.TodoItems.FindAsync(id);
            if (todoItem == null)
            {
                return NotFound();
            }

            _context.TodoItems.Remove(todoItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TodoItemExists(long id)
        {
            return _context.TodoItems.Any(e => e.Id == id);
        }
    }
}
