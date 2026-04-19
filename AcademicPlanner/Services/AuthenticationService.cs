using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcademicPlanner.Data;
using AcademicPlanner.Models;

namespace AcademicPlanner.Services;

public class AuthenticationService
{
    private readonly AcademicPlannerDatabase _database;
    private readonly HashingService _hashingService;

    public AuthenticationService(
        AcademicPlannerDatabase database,
        HashingService hashingService)
    {
        _database = database;
        _hashingService = hashingService;
    }

    public async Task<bool> HasAccountAsync()
    {
        return await _database.HasUserAsync();
    }

    public async Task<(bool Success, string ErrorMessage)> CreateAccountAsync(
        string username,
        string password)
    {
        username = username.Trim();

        if (string.IsNullOrWhiteSpace(username))
            return (false, "Username is required.");

        if (string.IsNullOrWhiteSpace(password))
            return (false, "Password is required.");

        var existingUser = await _database.GetUserByUsernameAsync(username);
        if (existingUser is not null)
            return (false, "That username already exists.");

        var (hash, salt) = _hashingService.HashPassword(password);

        var user = new AppUser
        {
            Username = username,
            PasswordHash = hash,
            PasswordSalt = salt,
            CreatedAt = DateTime.UtcNow
        };

        await _database.SaveUserAsync(user);
        return (true, string.Empty);
    }

    public async Task<(bool Success, string ErrorMessage)> LoginAsync(
        string username,
        string password)
    {
        username = username.Trim();

        if (string.IsNullOrWhiteSpace(username))
            return (false, "Username is required.");

        if (string.IsNullOrWhiteSpace(password))
            return (false, "Password is required.");

        var user = await _database.GetUserByUsernameAsync(username);
        if (user is null)
            return (false, "Invalid username or password.");

        bool valid = _hashingService.VerifyPassword(
            password,
            user.PasswordHash,
            user.PasswordSalt);

        if (!valid)
            return (false, "Invalid username or password.");

        user.LastLoginAt = DateTime.UtcNow;
        await _database.SaveUserAsync(user);

        return (true, string.Empty);
    }

    public async Task<(bool Success, string ErrorMessage)> SetPinAsync(string pin)
    {
        if (string.IsNullOrWhiteSpace(pin))
            return (false, "PIN is required.");

        if (pin.Length != 4 || !pin.All(char.IsDigit))
            return (false, "PIN must be exactly 4 digits.");

        var user = await _database.GetUserAsync();
        if (user is null)
            return (false, "No account exists.");

        var (hash, salt) = _hashingService.HashPassword(pin);

        user.PinHash = hash;
        user.PinSalt = salt;
        user.IsPinEnabled = true;

        await _database.SaveUserAsync(user);

        return (true, string.Empty);
    }

    public async Task<(bool Success, string ErrorMessage)> VerifyPinAsync(string pin)
    {
        if (string.IsNullOrWhiteSpace(pin))
            return (false, "PIN is required.");

        var user = await _database.GetUserAsync();
        if (user is null)
            return (false, "No account exists.");

        if (!user.IsPinEnabled || string.IsNullOrWhiteSpace(user.PinHash) || string.IsNullOrWhiteSpace(user.PinSalt))
            return (false, "PIN unlock is not enabled.");

        bool valid = _hashingService.VerifyPassword(pin, user.PinHash, user.PinSalt);

        if (!valid)
            return (false, "Invalid PIN.");

        user.LastLoginAt = DateTime.UtcNow;
        await _database.SaveUserAsync(user);

        return (true, string.Empty);
    }

    public async Task DisablePinAsync()
    {
        var user = await _database.GetUserAsync();
        if (user is null)
            return;

        user.PinHash = string.Empty;
        user.PinSalt = string.Empty;
        user.IsPinEnabled = false;

        await _database.SaveUserAsync(user);
    }

    public async Task<bool> IsPinEnabledAsync()
    {
        var user = await _database.GetUserAsync();
        return user?.IsPinEnabled ?? false;
    }
}