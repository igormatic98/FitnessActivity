using FitnessActivity.Infrastructure;

namespace Infrastracture.Services.FitnessActivityService;

using Domain.FitnessActivity.Entities;
using Infrastracture.Dtos;
using Microsoft.EntityFrameworkCore;
using System;

public class FitnessActivityService
{
    private readonly DatabaseContext databaseContext;

    public FitnessActivityService(DatabaseContext databaseContext)
    {
        this.databaseContext = databaseContext;
    }

    public async Task CreateFitnessActivity(FitnessActivity fitnessActivity)
    {
        await databaseContext.FitnessActivity.AddAsync(fitnessActivity);
        await databaseContext.SaveChangesAsync();
    }

    public async Task DeleteFitnessActivity(int activityId)
    {
        await databaseContext.FitnessActivity.Where(fa => fa.Id == activityId).ExecuteDeleteAsync();
    }

    public async Task<List<FitnessActivityResponseDto>> GetFitnessActivityByDate(DateTime date)
    {
        var response = await databaseContext.FitnessActivity
            .Where(fa => fa.Date == date.Date)
            .Select(
                fa =>
                    new FitnessActivityResponseDto
                    {
                        Id = fa.Id,
                        Date = date,
                        Title = fa.Title,
                        Description = fa.Description,
                        ActivityType = fa.ActivityType.Name,
                        Time = fa.Time,
                        Duration = fa.Duration,
                    }
            )
            .ToListAsync();
        return response;
    }

    public async Task UpdateFitnessActivity(FitnessActivity fitnessActivity)
    {
        await databaseContext.FitnessActivity
            .Where(fa => fa.Id == fitnessActivity.Id)
            .ExecuteUpdateAsync(
                fa =>
                    fa.SetProperty(p => p.Title, fitnessActivity.Title)
                        .SetProperty(p => p.Description, fitnessActivity.Description)
                        .SetProperty(p => p.Date, fitnessActivity.Date)
                        .SetProperty(p => p.Duration, fitnessActivity.Duration)
                        .SetProperty(p => p.Time, fitnessActivity.Time)
                        .SetProperty(p => p.ActivityTypeId, fitnessActivity.ActivityTypeId)
            );
    }
}
