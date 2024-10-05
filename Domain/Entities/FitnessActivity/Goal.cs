using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.FitnessActivity.Entities;

[PrimaryKey(nameof(Id))]
public class Goal
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public int ActivistId { get; set; }
    public DateTime Date { get; set; }
    public int? ActivityCountGoal { get; set; }

    [Column(TypeName = "Time")]
    public TimeSpan? DurationGoal { get; set; }

    public bool? IsAchieved { get; set; }

    [ForeignKey(nameof(ActivistId))]
    public FitnessActivist FitnessActivist { get; set; }

    public ICollection<GoalActivity> GoalActivitys { get; set; }
}
