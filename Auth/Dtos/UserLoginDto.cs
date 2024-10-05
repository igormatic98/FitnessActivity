using System.ComponentModel.DataAnnotations;

namespace FitnessActivity.Auth.Dtos;

public class UserLoginDto
{
    [Required]
    public string UserName { get; set; }

    [Required]
    public string Password { get; set; }
}
