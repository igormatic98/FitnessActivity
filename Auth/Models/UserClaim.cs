using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FitnessActivity.Auth.Entities;

[PrimaryKey(nameof(Id))]
public class UserClaim : IdentityUserClaim<Guid>
{
    public override int Id { get; set; }
    public override Guid UserId { get; set; }
    public override string ClaimType { get; set; }
    public override string ClaimValue { get; set; }
    public User User { get; set; }
}
