using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.FitnessActivity.Entities;

[PrimaryKey(nameof(GoalId), nameof(ActivityId))]
public class GoalActivity
{
    public int GoalId { get; set; }
    public int ActivityId { get; set; }
    public int? DesiredCount { get; set; }

    [Column(TypeName = "Time")]
    public TimeSpan? DesiredDuration { get; set; }

    [ForeignKey(nameof(GoalId))]
    public Goal Goal { get; set; }

    [ForeignKey(nameof(ActivityId))]
    public FitnessActivity FitnessActivity { get; set; }
}
