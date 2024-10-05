using Microsoft.AspNetCore.Identity;

namespace FitnessActivity.Auth.Entities;

public class Role : IdentityRole<Guid>
{
    public const string FITNESS_ACTIVIST = "FitnessActivist";
    public const string TRAINER = "Trainer";

    public Role()
        : base() { }

    public Role(string roleName, string description)
        : this()
    {
        Name = roleName;
        Description = description;
    }

    public ICollection<UserRole> Users { get; set; }
    public ICollection<RoleClaim> Claims { get; set; }
    public string Description { get; set; }
}
