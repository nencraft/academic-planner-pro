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
}