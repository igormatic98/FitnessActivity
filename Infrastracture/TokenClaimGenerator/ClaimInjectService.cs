using Auth.Services;
using FitnessActivity.Auth.Entities;
using FitnessActivity.Auth.TokenClaimGenerator;
using FitnessActivity.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Infrastracture.TokenClaimGenerator;

/// <summary>
/// Servis za kreiranje dodatnih claim-ova u zavisnosti od role
/// Poziva se prilikom prijave korisnika i generisanja tokena
/// </summary>
public class ClaimInjectService : IClaimInjectService
{
    private readonly DatabaseContext databaseContext;
    private readonly UserManager<User> userManager;

    public ClaimInjectService(DatabaseContext databaseContext, UserManager<User> userManager)
    {
        this.databaseContext = databaseContext;
        this.userManager = userManager;
    }

    public async Task<List<Claim>> InjectClaimsForToken(User user)
    {
        var currentDate = DateTime.Now;
        var claims = new List<Claim>();
        var roles = await userManager.GetRolesAsync(user);

        //Ukoliko je korisnik u roli prodavca, u svom tokenu ima informaciju o aktivnoj kampanji
        if (roles.Any(r => r == Role.FITNESS_ACTIVIST))
        {
            var fitnessActivistId = await databaseContext.FitnessActivist
                .Where(fa => fa.UserId == user.Id)
                .Select(fa => fa.Id)
                .FirstOrDefaultAsync();

            claims.Add(
                new Claim(
                    CustomClaimTypes.FitnessActivistId,
                    fitnessActivistId.ToString()!,
                    ClaimValueTypes.Integer32
                )
            );
        }
        return claims;
    }
}
