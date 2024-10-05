﻿using FitnessActivity.Auth.Dtos;
using FitnessActivity.Auth.Entities;
using FitnessActivity.Auth.Services;
using FitnessActivity.Auth.TokenClaimGenerator;
using FitnessActivity.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace FitnessActivity.Auth.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly DatabaseContext databaseContext;
    private readonly UserManager<User> userManager;
    private readonly SignInManager<User> signInManager;
    readonly IConfiguration configuration;

    public IHttpContextAccessor HttpContextAccessor { get; }
    private IEnumerable<IClaimInjectService> ClaimInjectServices { get; }

    public AuthenticationService(
        DatabaseContext databaseContext,
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IConfiguration configuration,
        IHttpContextAccessor httpContextAccessor,
        IEnumerable<IClaimInjectService> claimInjectServices
    )
    {
        this.databaseContext =
            databaseContext ?? throw new ArgumentNullException(nameof(DatabaseContext));
        this.userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        this.signInManager =
            signInManager ?? throw new ArgumentNullException(nameof(signInManager));
        this.configuration = configuration;
        HttpContextAccessor = httpContextAccessor;
        ClaimInjectServices = claimInjectServices;
    }

    public async Task<Token> Login(string userName, string password)
    {
        userName = string.Concat(userName.Trim()); //ocekujemo userName bez domena
        User dbUser = await userManager.FindByNameAsync(userName);
        var result = await signInManager.PasswordSignInAsync(userName, password, false, false);

        if (dbUser == null)

            throw new Exception("Login failed - User not exist.");

        if (!dbUser.Active)

            throw new Exception("Login failed - User is not active.");

        var roles = await userManager.GetRolesAsync(dbUser);

        if (!result.Succeeded)

            throw new Exception("Login failed - Wrong user name or password");

        //brisanje refresh token-a korinsika koji su istekli prije vise od 7 dana
        var refreshTokens = databaseContext.RefreshTokens.Where(
            rt => rt.UserId == dbUser.Id && rt.ExpiresAt < DateTime.UtcNow.Date.AddDays(-7)
        );
        databaseContext.RefreshTokens.RemoveRange(refreshTokens);

        return new Token
        {
            AccessToken = await GenerateAccessTokenAsync(dbUser),
            RefreshToken = await GenerateRefreshTokenAsync(dbUser)
        };
    }

    public async Task Revoke(string refreshToken)
    {
        var rt = await databaseContext.RefreshTokens
            .Include("User")
            .FirstOrDefaultAsync(rt => rt.Token.Equals(refreshToken));

        if (rt != null)
        {
            if (rt.RevokedAt != null)
            {
                await RevokeAllRefreshTokensAsync(rt, $"Compromised: {rt.Token}");
            }
            else if (DateTime.UtcNow >= rt.ExpiresAt)
            {
                RevokeRefreshToken(rt, "Expired");
                databaseContext.RefreshTokens.Update(rt);
                await databaseContext.SaveChangesAsync();
            }
            else
            {
                RevokeRefreshToken(rt, "User logged out");
                databaseContext.RefreshTokens.Update(rt);
                await databaseContext.SaveChangesAsync();
            }
        }
    }

    public async Task<Token> RefreshTokenAsync(string refreshToken, string oldAccessToken)
    {
        var rt = await databaseContext.RefreshTokens
            .Include("User")
            .FirstOrDefaultAsync(rt => rt.Token.Equals(refreshToken));

        if (rt != null && rt.RevokedBy != GetClientRemoteIpAdress())
        {
            if (rt.RevokedAt != null)
            {
                await RevokeAllRefreshTokensAsync(rt, $"Compromised: {rt.Token}");
            }
            else if (DateTime.UtcNow >= rt.ExpiresAt)
            {
                RevokeRefreshToken(rt, "Expired");
                databaseContext.RefreshTokens.Update(rt);
                await databaseContext.SaveChangesAsync();
            }
            else
            {
                RevokeRefreshToken(rt, "Used to generate new Access Token");
                databaseContext.RefreshTokens.Update(rt);
                await databaseContext.SaveChangesAsync();

                return new Token
                {
                    AccessToken = await GenerateAccessTokenAsync(rt.User, oldAccessToken),
                    RefreshToken = await GenerateRefreshTokenAsync(rt.User)
                };
            }
        }

        return null;
    }

    private async Task<string> GenerateAccessTokenAsync(User user, string oldAccessToken = null)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim("FullName", user.FirstName + " " + user.LastName)
        };

        // Dodaj role u claims
        var roles = await userManager.GetRolesAsync(user);
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role, ClaimValueTypes.String));
        }
        var userClaims = await userManager.GetClaimsAsync(user);

        foreach (var claim in userClaims)
        {
            claims.Add(claim);
        }
        if (!String.IsNullOrEmpty(user.TemporaryPassword))
        {
            claims.Add(new Claim("RequirePasswordChange", "true", ClaimValueTypes.Boolean));
        }
        else
        {
            foreach (var claimInjectService in ClaimInjectServices)
            {
                var injectClaims = await claimInjectService.InjectClaimsForToken(user);

                foreach (var claim in injectClaims)
                {
                    claims.Add(claim);
                }
            }
        }

        var key = configuration["Auth:Secret"];
        if (key == null)
            throw new ArgumentOutOfRangeException("Auth:Secret");

        var issuer = configuration["Auth:ValidIssuer"];
        var expirationTimeString = configuration["Auth:AccessExpiration"];
        if (!int.TryParse(expirationTimeString, out int expirationTime))
        {
            throw new ArgumentOutOfRangeException("Auth:AccessExpiration");
        }

        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
        var tokeOptions = new JwtSecurityToken(
            issuer: issuer,
            claims: claims,
            expires: DateTime.Now.AddMinutes(expirationTime),
            signingCredentials: signinCredentials
        );
        var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
        return tokenString;
    }

    private async Task<string> GenerateRefreshTokenAsync(User user)
    {
        var randomBytes = new byte[64];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomBytes);
            string refreshToken = Convert.ToBase64String(randomBytes);

            var duration = int.Parse(configuration["Auth:RefreshExpiration"]);

            databaseContext.RefreshTokens.Add(
                new RefreshToken()
                {
                    UserId = user.Id,
                    Token = refreshToken,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = GetClientRemoteIpAdress(),
                    ExpiresAt = DateTime.UtcNow.AddMinutes(duration)
                }
            );

            await databaseContext.SaveChangesAsync();

            return refreshToken;
        }
    }

    private async Task RevokeAllRefreshTokensAsync(RefreshToken refreshToken, string reason)
    {
        var refreshTokens = databaseContext.RefreshTokens.Where(
            rt =>
                rt.UserId == refreshToken.UserId
                && rt.RevokedAt == null
                && rt.ExpiresAt > DateTime.UtcNow
        );

        foreach (RefreshToken rt in refreshTokens)
        {
            RevokeRefreshToken(rt, reason);
            databaseContext.RefreshTokens.Update(rt);
        }
        await databaseContext.SaveChangesAsync();
    }

    private void RevokeRefreshToken(RefreshToken refreshToken, string reason)
    {
        refreshToken.RevokedAt = DateTime.UtcNow;
        refreshToken.RevokedBy = GetClientRemoteIpAdress();
        refreshToken.RevokedReason = reason;
    }

    private string GetClientRemoteIpAdress()
    {
        string senderIpv4 = HttpContextAccessor.HttpContext.Connection.RemoteIpAddress?.ToString();

        if (
            HttpContextAccessor.HttpContext.Request.Headers.TryGetValue(
                "X-Forwarded-For",
                out var forwardedIps
            )
        )
            senderIpv4 = forwardedIps.First();

        return senderIpv4;
    }
}
