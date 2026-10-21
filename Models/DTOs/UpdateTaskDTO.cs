using System.ComponentModel.DataAnnotations;

namespace TaskTracker.Api.DTOs;

public class UpdateTaskDTO
{
    [Required]
    [StringLength(100)]
    public string Title {get; set;} = string.Empty;

    [StringLength(500)]
    public string? Description {get; set;}

    public bool IsCompleted {get; set;}
}