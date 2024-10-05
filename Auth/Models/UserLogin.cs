﻿using Microsoft.AspNetCore.Identity;

namespace FitnessActivity.Auth.Entities;

public class UserLogin : IdentityUserLogin<Guid>
{
    public override string LoginProvider { get; set; }
    public override string ProviderKey { get; set; }
    public string ProviderDisplayName { get; set; }
    public override Guid UserId { get; set; }
    public User User { get; set; }
}
