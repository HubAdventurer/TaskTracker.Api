using System.ComponentModel.DataAnnotations;

namespace TaskTracker.Api.DTOs;

public class CreateTaskDTO
{
    [Required]
    [StringLength(100)]
    public string Title {get; set;} = string.Empty;

    [StringLength(500)]
    public string? Description {get; set;}
}