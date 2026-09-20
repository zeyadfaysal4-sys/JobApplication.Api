using JobApplication.Application.DTO.Auth;
using JobApplication.Application.Interfaces;
using JobApplication.Domin.Entities;
using JobApplication.Infrastructure.Persistence.DbSeeder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JobApplication.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IRepository<Candidate> _candidateRepository;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration,
            IRepository<Candidate> candidateRepository)
        {
            _userManager = userManager;
            _configuration = configuration;
            _candidateRepository = candidateRepository;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            if (dto.Role != Roles.Recruiter &&dto.Role != Roles.Candidate)
            {
                return BadRequest(new
                {
                    message = "Role must be Recruiter or Candidate."
                });
            }

            var existingUser =await _userManager.FindByEmailAsync(dto.Email);

            if (existingUser != null)
            {
                return BadRequest(new
                {
                    message = "Email already exists."
                });
            }

            var user = new ApplicationUser
            {
                Name = dto.Name,
                Email = dto.Email,
                UserName = dto.Email
            };

            var result = await _userManager.CreateAsync(user,dto.Password);

            if (!result.Succeeded)
            {
                return BadRequest(new
                {
                    errors = result.Errors.Select(e => e.Description)
                });
            }

            var roleResult = await _userManager.AddToRoleAsync(user,dto.Role);

            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(user);

                return BadRequest(new
                {
                    errors = roleResult.Errors.Select(e => e.Description)
                });
            }

            
            if (dto.Role == Roles.Candidate)
            {
                var candidate = new Candidate
                {
                    ApplicationUserId = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    CvUrl = ""
                };

                await _candidateRepository.AddAsync(candidate);
                await _candidateRepository.CommitAsync();
            }

            return Ok(new
            {
                message = "User registered successfully.",
                userId = user.Id,
                name = user.Name,
                email = user.Email,
                role = dto.Role
            });
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user =
                await _userManager.FindByEmailAsync(dto.Email);

            if (user == null)
            {
                return Unauthorized(new
                {
                    message = "Invalid email or password."
                });
            }

            var passwordValid = await _userManager.CheckPasswordAsync(user,dto.Password);

            if (!passwordValid)
            {
                return Unauthorized(new
                {
                    message = "Invalid email or password."
                });
            }

            var roles =await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),

                new Claim(ClaimTypes.Email,user.Email ?? ""),

                new Claim(ClaimTypes.Name,user.Name ?? "")
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role,role));
            }

            var jwtSettings =
                _configuration.GetSection("JwtSettings");

            var securityKey =jwtSettings["securityKey"]?? throw new InvalidOperationException("JWT security key not found.");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expireTime =double.Parse(jwtSettings["expireTime"] ?? "20");

            var token = new JwtSecurityToken(
                issuer: jwtSettings["validIssuer"],
                audience: jwtSettings["validAudience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expireTime),
                signingCredentials: credentials);

            var tokenString =new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new
            {
                token = tokenString,
                expiresInMinutes = expireTime,
                user = new
                {
                    id = user.Id,
                    name = user.Name,
                    email = user.Email,
                    roles = roles
                }
            });
        }
    }
}