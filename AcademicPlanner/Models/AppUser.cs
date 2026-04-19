using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace AcademicPlanner.Models;

public class AppUser
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [MaxLength(100), Unique]
    public string Username { get; set; } = string.Empty;

    [MaxLength(200)]
    public string PasswordHash { get; set; } = string.Empty;

    [MaxLength(200)]
    public string PasswordSalt { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? LastLoginAt { get; set; }

    [MaxLength(200)]
    public string PinHash { get; set; } = string.Empty;

    [MaxLength(200)]
    public string PinSalt { get; set; } = string.Empty;

    public bool IsPinEnabled { get; set; }
}