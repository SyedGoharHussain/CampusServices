using System.ComponentModel.DataAnnotations;

namespace Campus.Models;

public class CredentialsDto
{
    [Required, EmailAddress]
    public string Email { get; set; } = "";

    [Required]
    public string Password { get; set; } = "";
}
