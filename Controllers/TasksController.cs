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

    public TasksController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<List<TaskDTO>>> GetTask()
    {
        var tasks = await _db.Tasks.ToListAsync();

        return Ok(tasks.Select(MapToDTO).ToList());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TaskDTO>> GetTask(int id)
    {
        var task = await _db.Tasks.FindAsync(id);

        if(task == null)
        {
            return NotFound();
        }

        return Ok(MapToDTO(task));
    }

    [HttpPost]
    public async Task<ActionResult<TaskDTO>> CreateTask(CreateTaskDTO dto)
    {
        var task = new TaskItem
        {
            Title = dto.Title,
            Description = dto.Description,
            IsCompleted = false
        };

        _db.Tasks.Add(task);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTask), new { id = task.Id}, MapToDTO(task));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTask(int id, UpdateTaskDTO dto)
    {
        var task = await _db.Tasks.FindAsync(id);

        if (task == null)
        {
            return NotFound();
        }

        task.Title = dto.Title;
        task.Description = dto.Description;
        task.IsCompleted = dto.IsCompleted;
        
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTask(int id)
    {
        var task = await _db.Tasks.FindAsync(id);

        if (task == null)
        {
            return NotFound();
        }

        _db.Tasks.Remove(task);
        await _db.SaveChangesAsync();

        return NoContent();
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> PatchTask(int id, PatchTaskDTO dto)
    {
        var task = await _db.Tasks.FindAsync(id);

        if(task == null) 
        {
            return NotFound();
        }

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
        };
    }
}