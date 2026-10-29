using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Mvc;
using TaskTracker.Api.Models;
using TaskTracker.Api.DTOs;
using TaskTracker.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace TaskTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{

    private readonly AppDbContext _db;
    private readonly ILogger<TasksController> _logger;

    public TasksController(AppDbContext db, ILogger<TasksController> logger)
    {
        _db = db;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<List<TaskDTO>>> GetTasks(bool? isCompleted, string? sortBy, string? search, bool descending = false)
    {
        IQueryable<TaskItem> query = _db.Tasks.AsQueryable().AsNoTracking();

        if(isCompleted != null)
            query = query.Where(t => t.IsCompleted == isCompleted.Value);

        if (String.IsNullOrEmpty(sortBy))
        {
            query = query.OrderBy(t => t.Id);
        }
        else if (sortBy == "title")
        {
            query = descending ? query.OrderByDescending(t => t.Title) : query.OrderBy(t => t.Title);
        }
        else if (sortBy == "createdAt")
        {
            query = descending ? query.OrderByDescending(t => t.CreatedAt) : query.OrderBy(t => t.CreatedAt);
        }
        else
        {
            return BadRequest("Invalid sortBy value");
        }

        if(!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(t => t.Title.Contains(search));
        }   
        
        var tasks = await query.ToListAsync();

        return Ok(tasks.Select(MapToDTO).ToList());
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TaskDTO>> GetTask(int id)
    {
        var task = await _db.Tasks.FindAsync(id);

        if(task == null)
        {
            _logger.LogDebug("No task with ID {TaskId}", id);

            return NotFound();
        }

        return Ok(MapToDTO(task));
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TaskDTO>> CreateTask(CreateTaskDTO dto)
    {
        var task = new TaskItem
        {
            Title = dto.Title,
            Description = dto.Description,
            IsCompleted = false,
            CreatedAt = DateTime.UtcNow
        };

        _db.Tasks.Add(task);
        await _db.SaveChangesAsync();

        _logger.LogInformation("Created task {TaskId}", task.Id);

        return CreatedAtAction(nameof(GetTask), new { id = task.Id}, MapToDTO(task));
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateTask(int id, UpdateTaskDTO dto)
    {
        var task = await _db.Tasks.FindAsync(id);

        if (task == null)
            return NotFound();

        task.Title = dto.Title;
        task.Description = dto.Description;
        task.IsCompleted = dto.IsCompleted;
        
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteTask(int id)
    {
        var task = await _db.Tasks.FindAsync(id);

        if (task == null)
            return NotFound();

        _db.Tasks.Remove(task);
        await _db.SaveChangesAsync();

        return NoContent();
    }

    [HttpPatch("{id}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<TaskDTO>> PatchTask(int id, PatchTaskDTO dto)
    {
        var task = await _db.Tasks.FindAsync(id);

        if(task == null) 
            return NotFound();

        if (dto.Title != null) task.Title = dto.Title;
        if (dto.Description != null) task.Description = dto.Description;
        if (dto.IsCompleted != null) task.IsCompleted = dto.IsCompleted.Value;

        await _db.SaveChangesAsync();

        return Ok(MapToDTO(task));
    }

    private static TaskDTO MapToDTO(TaskItem task)
    {
        return new TaskDTO
        {
          Id = task.Id,
          Title = task.Title,
          Description = task.Description,
          IsCompleted = task.IsCompleted,
          CreatedAt = task.CreatedAt
        };
    }
}