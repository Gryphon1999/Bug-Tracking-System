using System.ComponentModel.DataAnnotations;

namespace BugTracker.Shared.Dtos;

public class AuthUserCreateUpdateDto
{
    [Required]
    public string Name { get; set; }
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }
    [Required]
    [Compare("Password")]
    public string ConfirmPassword { get; set; }
    public bool IsUser { get; set; }
}
