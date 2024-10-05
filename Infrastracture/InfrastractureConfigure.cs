using Auth.Services;
using FitnessActivity.Auth.Services;
using FitnessActivity.Auth.Entities;
using FitnessActivity.Auth.Services;
using FitnessActivity.Auth.TokenClaimGenerator;
using FitnessActivity.Infrastructure;
using Infrastracture.TokenClaimGenerator;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Microsoft.AspNetCore.Hosting;

namespace Infrastracture;

/// <summary>
/// Registrovanje servisa, mapera, konekcija sa bazom
/// Poziva se prilikom pokretanja u program.cs-u
/// </summary>
public class InfrastractureConfigure
{
    public static void Register(
        IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment
    )
    {
        services.AddDbContext<DatabaseContext>(opts =>
        {
            opts.UseSqlServer(configuration["ConnectionString:SQL"]);

            if (environment.IsDevelopment())
            {
                opts.EnableSensitiveDataLogging();
                opts.LogTo(x => Debug.WriteLine(x), LogLevel.Debug);
            }
        });

        #region Auth
        services.AddScoped<IClaimInjectService, ClaimInjectService>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<TokenReaderService>();

        services
            .AddIdentity<User, Role>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequireNonAlphanumeric = false;
            })
            .AddEntityFrameworkStores<DatabaseContext>()
            .AddDefaultTokenProviders();

        #endregion
    }
}
