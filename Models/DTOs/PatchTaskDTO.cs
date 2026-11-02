using System.ComponentModel.DataAnnotations;

namespace TaskTracker.Api.DTOs;

public class PatchTaskDTO
{
    [StringLength(100)]
    public string? Title {get; set;}
    [StringLength(500)]
    public string? Description {get; set;}
    public bool? IsCompleted {get; set;}
}