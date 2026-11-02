using Microsoft.AspNetCore.Identity;

namespace OurSpot.Security;

public class ApplicationUser: IdentityUser
{
    public string NombreCompleto { get; set; } = string.Empty;
}
