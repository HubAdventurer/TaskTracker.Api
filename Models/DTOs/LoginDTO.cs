using System.ComponentModel.DataAnnotations;

namespace TaskTracker.Api.DTOs;

public class LoginDTO
{
    [Required]
    [EmailAddress]
    public string UserEmail {set; get;} = string.Empty;

    [Required]
    public string Password {set; get;} = string.Empty;
}