using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Mvc;
using TaskTracker.Api.Models;
using TaskTracker.Api.DTOs;

namespace TaskTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{

    private static readonly List<TaskItem> _tasks = new()
    {
        new TaskItem { Id = 1, Title = "Buy groceries", IsCompleted = false},
        new TaskItem { Id = 2, Title = "Walk the dog", IsCompleted = true },
        new TaskItem { Id = 3, Title = "Learn ASP.NET", IsCompleted = false, Description = "I will do it !" },
        new TaskItem { Id = 4, Title = "Go for a walk", IsCompleted = true}
    };

    private static int _nextId = 5;

    [HttpGet]
    public IActionResult GetTask()
    {
        return Ok(_tasks.Select(MapToDTO));
    }

    [HttpGet("{id}")]
    public IActionResult GetTask(int id)
    {
        var task = _tasks.FirstOrDefault(t => t.Id == id);

        if(task == null)
        {
            return NotFound();
        }

        return Ok(MapToDTO(task));
    }

    [HttpPost]
    public IActionResult CreateTask(CreateTaskDTO dto)
    {
        var task = new TaskItem
        {
            Id = _nextId++,
            Title = dto.Title,
            Description = dto.Description,
            IsCompleted = false

        };

        _tasks.Add(task);

        return CreatedAtAction(nameof(GetTask), new { id = task.Id}, MapToDTO(task));
    }

    [HttpPut("{id}")]
    public IActionResult UpdateTask(int id, UpdateTaskDTO dto)
    {
        var task = _tasks.FirstOrDefault(t => t.Id == id);

        if (task == null)
        {
            return NotFound();
        }

        task.Title = dto.Title;
        task.Description = dto.Description;
        task.IsCompleted = dto.IsCompleted;
        
        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteTask(int id)
    {
        var task = _tasks.FirstOrDefault(t => t.Id == id);

        if (task == null)
        {
            return NotFound();
        }

        _tasks.Remove(task);

        return NoContent();
    }

    [HttpPatch("{id}")]
    public IActionResult PatchTask(int id, PatchTaskDTO dto)
    {
        var task = _tasks.FirstOrDefault(t => t.Id == id);

        if(task == null) 
        {
            return NotFound();
        }

        if (dto.Title != null) task.Title = dto.Title;
        if (dto.Description != null) task.Description = dto.Description;
        if (dto.IsCompleted != null) task.IsCompleted = dto.IsCompleted.Value;

        return Ok();
    }

    private static TaskDTO MapToDTO(TaskItem task)
    {
        return new TaskDTO
        {
          Id = task.Id,
          Title = task.Title,
          Description = task.Description,
          IsCompleted = task.IsCompleted  
        };
    }
}