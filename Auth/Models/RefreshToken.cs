﻿using System.ComponentModel.DataAnnotations;

namespace FitnessActivity.Auth.Entities;

public class RefreshToken
{
    public int Id { get; set; }

    [Required]
    public Guid UserId { get; set; }

    [Required]
    public string Token { get; set; }

    public DateTime CreatedAt { get; set; }

    public string CreatedBy { get; set; }

    public DateTime ExpiresAt { get; set; }

    public DateTime? RevokedAt { get; set; }

    public string RevokedBy { get; set; }

    public string RevokedReason { get; set; }

    public User User { get; set; }
}
