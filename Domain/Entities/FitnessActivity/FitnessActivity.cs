using Domain.Catalog.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.FitnessActivity.Entities;

[PrimaryKey(nameof(Id))]
public class FitnessActivity
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public string Title { get; set; }

    [Required]
    public string Description { get; set; }

    [Column(TypeName = "Date")]
    public DateTime Date { get; set; }

    [Column(TypeName = "Time")]
    public TimeSpan Time { get; set; }

    [Column(TypeName = "Time")]
    public TimeSpan Duration { get; set; }
    public int ActivityTypeId { get; set; }

    [ForeignKey(nameof(ActivityTypeId))]
    public ActivityType ActivityType { get; set; }

    public ICollection<GoalActivity> GoalActivities { get; set; }
}
