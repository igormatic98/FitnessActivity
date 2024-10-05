using Domain.Catalog.Entities;
using Domain.FitnessActivity.Entities;
using FitnessActivity.Auth.Entities;
using FitnessActivity.Infrastructure;
using Infrastracture.Dtos;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Infrastracture.Seed;

/// <summary>
/// Pokrece se prilikom pokretanja aplikacije
/// Inicijalno pokretanje baze podataka sa korisnicima, rolama, kompanijom i agentom
/// </summary>
public static class Seed
{
    private const string Password = "Test123";

    public static async Task SeedAsync(
        IApplicationBuilder app,
        IConfiguration Configuration,
        IHostEnvironment environment
    )
    {
        using var scope = app.ApplicationServices.CreateScope();
        var services = scope.ServiceProvider;
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var databaseContext = services.GetRequiredService<DatabaseContext>();

        string[] roles = { Role.FITNESS_ACTIVIST, Role.TRAINER };
        //dodavanje rola
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                if (role == Role.FITNESS_ACTIVIST)
                    await roleManager.CreateAsync(
                        new Role
                        {
                            Name = role,
                            Description =
                                "Prati i bilježi svoje fitnes aktivnosti, postavljajući ciljeve i motivirajući se za postizanje boljih rezultata u svom zdravlju i kondiciji"
                        }
                    );

                if (role == Role.TRAINER)
                    await roleManager.CreateAsync(
                        new Role
                        {
                            Name = role,
                            Description =
                                "Stručnjak koji razvija i vodi prilagođene trening programe"
                        }
                    );
            }
        }
        //dodavanje tri korisnika sa tri razlicite role dirketor, agent, saler
        await CreateUsers(
            new List<UserSeedDto>
            {
                new UserSeedDto(
                    new User
                    {
                        UserName = "trainer@gmail.com",
                        FirstName = "Petar",
                        LastName = "Markovic",
                        Email = "trainer@gmail.com",
                        EmailConfirmed = true,
                        PhoneNumber = "",
                        Active = true
                    },
                    Role.TRAINER,
                    userManager,
                    databaseContext
                ),
                new UserSeedDto(
                    new User
                    {
                        UserName = "activist@gmail.com",
                        FirstName = "Igor",
                        LastName = "Matic",
                        Email = "activist@gmail.com",
                        EmailConfirmed = true,
                        PhoneNumber = "",
                        Active = true
                    },
                    Role.FITNESS_ACTIVIST,
                    userManager,
                    databaseContext
                ),
            }
        );

        // Proverite da li već postoje podaci u bazi
        if (!databaseContext.ActivityType.Any())
        {
            var activityTypes = new[]
            {
                new ActivityType
                {
                    Name = "Trčanje",
                    Description = "Aktivnost trčanja za poboljšanje kondicije."
                },
                new ActivityType
                {
                    Name = "Hodanje",
                    Description = "Lagano hodanje kao oblik rekreacije."
                },
                new ActivityType
                {
                    Name = "Planinarenje",
                    Description = "Istraživanje prirode kroz planinarenje."
                },
                new ActivityType
                {
                    Name = "Vožnja bicikla",
                    Description = "Vožnja bicikla kao oblik fizičke aktivnosti."
                },
                new ActivityType { Name = "Plivanje", Description = "Aktivnost plivanja u vodi." },
                new ActivityType
                {
                    Name = "Vežbanje",
                    Description = "Različite vrste vežbi u teretani ili kod kuće."
                },
                new ActivityType
                {
                    Name = "HIIT",
                    Description = "Visoko intenzivni intervalni trening."
                },
                new ActivityType
                {
                    Name = "Ostalo",
                    Description = "Druge fizičke aktivnosti koje nisu navedene."
                }
            };

            await databaseContext.ActivityType.AddRangeAsync(activityTypes);
            await databaseContext.SaveChangesAsync();
        }
    }

    public static async Task CreateUsers(List<UserSeedDto> users)
    {
        foreach (var userDto in users)
        {
            // Kreirajte defaultnog korisnika ako ne postoji
            var defaultUser = await userDto.UserManager.FindByEmailAsync(userDto.User.Email);
            if (defaultUser == null)
            {
                var result = await userDto.UserManager.CreateAsync(userDto.User, Password);
                if (result.Succeeded)
                {
                    await userDto.UserManager.AddToRoleAsync(userDto.User, userDto.RoleName);
                    //Kreiranje Kompanije, kampanje i agenta, jer nije toliki fokus na apijima za njihovo kreiranje
                    using (
                        var transaction =
                            await userDto.DatabaseContext.Database.BeginTransactionAsync()
                    )
                    {
                        try
                        {
                            if (userDto.RoleName == Role.FITNESS_ACTIVIST)
                            {
                                var agent = new FitnessActivist
                                {
                                    UserId = userDto.User.Id,
                                    CurrentWeight = 115,
                                    InitialWeight = 115,
                                    Height = 191,
                                    StartTrainingDate = DateTime.Now,
                                };
                                await userDto.DatabaseContext.FitnessActivist.AddAsync(agent);
                                await userDto.DatabaseContext.SaveChangesAsync();

                                await transaction.CommitAsync();
                            }
                        }
                        catch (Exception ex)
                        {
                            await transaction.RollbackAsync();
                        }
                    }
                }
            }
        }
    }
}
