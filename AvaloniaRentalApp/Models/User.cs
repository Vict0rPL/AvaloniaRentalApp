using System;
using System.Linq;

namespace AvaloniaRentalApp.Models;

public class User
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string Role { get; set; } = "employee";
    public bool IsActive { get; set; } = true;
    public DateTime? LastLogin { get; set; }
    public int FailedAttempts { get; set; }
    public DateTime? LockedUntil { get; set; }

    public bool IsAdmin => Role == "admin";
    public string RoleDisplay => Role == "admin" ? "Administrator" : "Pracownik";
    public string Initials => FullName.Length >= 2
        ? string.Concat(FullName.Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Take(2).Select(n => n[0])).ToUpper()
        : FullName[..1].ToUpper();
}
