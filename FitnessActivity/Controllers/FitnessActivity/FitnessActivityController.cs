using FitnessActivity.Auth.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessActivity.Controllers.FitnessActivity;

using Domain.FitnessActivity.Entities;
using Infrastracture.Services.FitnessActivityService;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api/[controller]")]
[ApiController]
public class FitnessActivityController : ControllerBase
{
    private readonly FitnessActivityService fitnessActivityService;

    public FitnessActivityController(FitnessActivityService fitnessActivityService)
    {
        this.fitnessActivityService = fitnessActivityService;
    }

    [HttpPost]
    [Authorize(Roles = Role.TRAINER)]
    public async Task<IActionResult> CreateFitnessActivity(
        [FromBody] FitnessActivity fitnessActivity
    )
    {
        await fitnessActivityService.CreateFitnessActivity(fitnessActivity);
        return Ok();
    }

    [HttpPut]
    [Authorize(Roles = Role.TRAINER)]
    public async Task<IActionResult> UpdateFitnessActivity(
        [FromBody] FitnessActivity fitnessActivity
    )
    {
        await fitnessActivityService.UpdateFitnessActivity(fitnessActivity);
        return Ok();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = Role.TRAINER)]
    public async Task<IActionResult> DeleteFitnessActivity(int activityId)
    {
        await fitnessActivityService.DeleteFitnessActivity(activityId);
        return Ok();
    }

    [HttpGet("getByDate")]
    [Authorize(Roles = Role.TRAINER)]
    public async Task<IActionResult> GetFitnessActivityByDate([FromQuery] DateTime date)
    {
        var fitnessActivitiesOnDate = await fitnessActivityService.GetFitnessActivityByDate(date);
        return Ok(fitnessActivitiesOnDate);
    }
}
