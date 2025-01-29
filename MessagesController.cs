using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;

[ApiController]
[Route("api/[controller]")]
public class MessagesController : ControllerBase
{
    private readonly cvContext _context;

    // Constructor to inject the database context
    public MessagesController(cvContext context)
    {
        _context = context;
    }

    [HttpGet(Name = "GetMessage")]
    public IEnumerable<Message> Get()
    {
        return _context.Messages.ToList();
    }

    // POST method to save the message
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Message message)
    {
        if (message == null)
        {
            return BadRequest("Invalid message data.");
        }

        // Generate ID for the message
        message.MessesageId = Guid.NewGuid();

        // Add the new message to the database
        _context.Messages.Add(message);

        // Save the changes to the database
        try
        {
            await _context.SaveChangesAsync();
            return Ok(new { success = true, message = "Message saved successfully!" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, "An error occurred while saving the message.");
        }
    }
    // PUT: Update an existing message
    [HttpPut("{id}")]
    public async Task<IActionResult> Put(Guid id, [FromBody] Message updatedMessage)
    {
        if (updatedMessage == null || id == Guid.Empty)
        {
            return BadRequest("Invalid message data.");
        }

        var existingMessage = await _context.Messages.FindAsync(id);
        if (existingMessage == null)
        {
            return NotFound("Message not found.");
        }

        // Update the message properties
        existingMessage.Message1 = updatedMessage.Message1;

        try
        {
            await _context.SaveChangesAsync();
            return Ok(new { success = true, message = "Message updated successfully!" });
        }
        catch (Exception)
        {
            return StatusCode(500, "An error occurred while updating the message.");
        }
    }

    // DELETE: Remove a message by ID
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var message = await _context.Messages.FindAsync(id);
        if (message == null)
        {
            return NotFound("Message not found.");
        }

        _context.Messages.Remove(message);

        try
        {
            await _context.SaveChangesAsync();
            return Ok(new { success = true, message = "Message deleted successfully!" });
        }
        catch (Exception)
        {
            return StatusCode(500, "An error occurred while deleting the message.");
        }
    }
    [HttpGet("sorted")]
    public IActionResult GetSortedMessages([FromQuery] string sortOrder)
    {
        var messages = _context.Messages.AsQueryable();

        if (string.IsNullOrEmpty(sortOrder) || sortOrder.ToLower() == "asc")
        {
            // Ascending order by CreatedDateTime
            messages = messages.OrderBy(m => m.CreatedDateTime);
        }
        else if (sortOrder.ToLower() == "desc")
        {
            // Descending order by CreatedDateTime
            messages = messages.OrderByDescending(m => m.CreatedDateTime);
        }
        else
        {
            return BadRequest("Invalid sort order. Use 'asc' or 'desc'.");
        }

        return Ok(messages.ToList());
    }
}
