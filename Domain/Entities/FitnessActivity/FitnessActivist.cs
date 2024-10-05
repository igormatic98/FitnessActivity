using FitnessActivity.Auth.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.FitnessActivity.Entities;

[PrimaryKey(nameof(Id))]
public class FitnessActivist
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public Guid UserId { get; set; }
    public DateTime StartTrainingDate { get; set; }
    public decimal InitialWeight { get; set; }
    public decimal CurrentWeight { get; set; }
    public int Height { get; set; }

    [ForeignKey(nameof(UserId))]
    public User User { get; set; }
}
