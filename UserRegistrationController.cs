// This assumes you have a User model with an Email and Password field
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;
using WebApplication1.Controllers;
using Microsoft.AspNetCore.Authentication.Cookies;

[ApiController]
[Route("api/[controller]")]
public class UserRegistrationController : ControllerBase
{
    private readonly cvContext _context;

    // Constructor to inject the database context
    public UserRegistrationController(cvContext context)
    {
        _context = context;
    }
    // Login endpoint
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
    {
        // Find user by email
        var user = await _context.UserRegistrations
            .FirstOrDefaultAsync(u => u.EmailAddress == loginRequest.Email);

        if (user == null)
        {
            return BadRequest(new { message = "User not found." });
        }

        // Direct plaintext password comparison (Not secure, avoid in production)
        if (user.Password != loginRequest.Password)
        {
            return Unauthorized(new { message = "Invalid credentials." });
        }

        // Create user claims
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            //new Claim(ClaimTypes.Email, user.EmailAddress)
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        // Set the cookie
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

        return Ok(new { success = true, message = "Login successful." });
    }

    // Logout endpoint 
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Ok(new { success = true, message = "Logout successful." });
    }

    // Registration endpoint
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRegistration registerRequest)
    {
        // check if email already exists
        var existingUser = await _context.UserRegistrations
            .FirstOrDefaultAsync(u => u.EmailAddress == registerRequest.EmailAddress);

        if (existingUser != null)
        {
            return BadRequest(new { message = "Email already in use." });
        }

        // Create a new user
        var newUser = new UserRegistration
        {
            UserId = Guid.NewGuid(),
            Username = registerRequest.Username,
            FirstName = registerRequest.FirstName,
            LastName = registerRequest.LastName,
            EmailAddress = registerRequest.EmailAddress,
            Password = registerRequest.Password
        };

        _context.UserRegistrations.Add(newUser);
        await _context.SaveChangesAsync();
        /*

        // send email for successful registration
        string subject = "Successfull registration";
        string body = $"Hello {registerRequest.FirstName},<br/><br/>Thank you for registation.";
        bool emailSent = await _emailservice.SendEmailAsync(registerRequest.Email, subject, body);

        await _emailLogController.SaveEmailLog("emi_dox0908@abv.bg", registerRequest.Email, subject, body, emailSent);

        */

        return Ok(new { success = true, message = "Registration successful." });
    }
}



