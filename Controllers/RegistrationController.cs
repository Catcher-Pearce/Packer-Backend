using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Packer.Models;

namespace Packer.Controllers;

[ApiController]
[Route("api/auth")]
public class RegistrationController(UserManager<User> users) : ControllerBase
{
    [HttpPost("register-username")]
    public async Task<IActionResult> Register(UsernameRegistration request)
    {
        var user = new User { UserName = request.Username, Email = request.Email };
        IdentityResult result;
        try
        {
            result = await users.CreateAsync(user, request.Password);
        }
        catch (DbUpdateException exception) when (exception.InnerException is PostgresException
               { SqlState: PostgresErrorCodes.UniqueViolation, ConstraintName: "UserNameIndex" or "EmailIndex" } postgres)
        {
            var error = postgres.ConstraintName == "UserNameIndex"
                ? users.ErrorDescriber.DuplicateUserName(request.Username)
                : users.ErrorDescriber.DuplicateEmail(request.Email);
            result = IdentityResult.Failed(error);
        }

        if (result.Succeeded)
        {
            return Ok();
        }

        return ValidationProblem(new ValidationProblemDetails(
            result.Errors.GroupBy(error => error.Code)
                .ToDictionary(group => group.Key, group => group.Select(error => error.Description).ToArray())));
    }
}

public record UsernameRegistration(
    [Required, StringLength(256), RegularExpression("[A-Za-z0-9._+-]+",
        ErrorMessage = "Use letters, numbers, dots, underscores, plus signs or hyphens for your username.")]
    string Username,
    [Required, EmailAddress, StringLength(256)] string Email,
    [Required] string Password
);
