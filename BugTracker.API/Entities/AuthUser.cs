using Microsoft.AspNetCore.Identity;

namespace BugTracker.API.Entities;

public class AuthUser : IdentityUser
{
    public string Name { get; set; }
}
