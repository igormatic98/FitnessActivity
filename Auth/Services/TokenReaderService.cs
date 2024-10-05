using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;

namespace Auth.Services;

public static class CustomClaimTypes
{
    //id sportiste koji se nalazi u tokenu
    public const string FitnessActivistId = "fitnessActivistId";
}

public class TokenReaderService
{
    private readonly IHttpContextAccessor httpContextAccessor;

    public TokenReaderService(IHttpContextAccessor httpContextAccessor)
    {
        this.httpContextAccessor = httpContextAccessor;
    }

    private string GetClaimValue(string claimType)
    {
        var token = httpContextAccessor.HttpContext
            ?.Request.Headers["Authorization"].ToString()
            ?.Replace("Bearer ", "");

        if (string.IsNullOrEmpty(token))
        {
            throw new ArgumentException("No token provided");
        }

        var handler = new JwtSecurityTokenHandler();

        if (handler.CanReadToken(token))
        {
            var jwtToken = handler.ReadJwtToken(token);
            var claim = jwtToken.Claims.FirstOrDefault(c => c.Type == claimType);
            return claim?.Value;
        }
        return "";
    }

    public int GetFitnessActivistId()
    {
        if (
            int.TryParse(
                GetClaimValue(CustomClaimTypes.FitnessActivistId),
                out int fitnessActivistId
            )
        )
        {
            return fitnessActivistId;
        }
        return default;
    }
}
