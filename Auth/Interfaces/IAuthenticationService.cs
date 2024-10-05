using FitnessActivity.Auth.Dtos;

namespace FitnessActivity.Auth.Services;

public interface IAuthenticationService
{
    Task<Token> Login(string userName, string password);
    Task Revoke(string refreshToken);
    Task<Token> RefreshTokenAsync(string refreshToken, string oldAccessToken);
}
