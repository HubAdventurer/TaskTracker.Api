namespace TaskTracker.Api.DTOs;

public class PatchTaskDTO
{
    public string? Title {get; set;}
    public string? Description {get; set;}
    public bool? IsCompleted {get; set;}
}