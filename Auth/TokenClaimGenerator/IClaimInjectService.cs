using FitnessActivity.Auth.Entities;
using System.Security.Claims;

namespace FitnessActivity.Auth.TokenClaimGenerator;

public interface IClaimInjectService
{
    Task<List<Claim>> InjectClaimsForToken(User user);
}
