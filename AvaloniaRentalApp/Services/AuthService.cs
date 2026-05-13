using System;
using System.Threading.Tasks;
using AvaloniaRentalApp.Models;

namespace AvaloniaRentalApp.Services;

public enum LoginStatus { Success, InvalidCredentials, Locked, Inactive }

public sealed record LoginResult(LoginStatus Status, User? User, DateTime? LockedUntil);

public sealed class AuthService
{
    private const int MaxFailedAttempts = 5;
    private static readonly TimeSpan LockDuration = TimeSpan.FromMinutes(15);

    private readonly DatabaseService _db;

    public AuthService(DatabaseService db) => _db = db;

    public async Task<LoginResult> LoginAsync(string username, string password)
    {
        var user = await _db.GetUserByUsernameAsync(username);
        if (user is null)
            return new(LoginStatus.InvalidCredentials, null, null);

        if (!user.IsActive)
            return new(LoginStatus.Inactive, null, null);

        if (user.LockedUntil is { } until && until > DateTime.UtcNow)
            return new(LoginStatus.Locked, null, until);

        bool passwordOk;
        try
        {
            passwordOk = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
        }
        catch (BCrypt.Net.SaltParseException)
        {
            passwordOk = false;
        }

        if (!passwordOk)
        {
            var attempts = user.FailedAttempts + 1;
            DateTime? lockUntil = attempts >= MaxFailedAttempts
                ? DateTime.UtcNow.Add(LockDuration)
                : null;
            await _db.UpdateUserLoginStateAsync(user.UserId, attempts, lockUntil, user.LastLogin);
            return lockUntil is null
                ? new(LoginStatus.InvalidCredentials, null, null)
                : new(LoginStatus.Locked, null, lockUntil);
        }

        await _db.UpdateUserLoginStateAsync(user.UserId, 0, null, DateTime.UtcNow);
        user.FailedAttempts = 0;
        user.LockedUntil = null;
        user.LastLogin = DateTime.UtcNow;
        return new(LoginStatus.Success, user, null);
    }
}
