using System;
using System.Data;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.IdentityModel.Tokens;
using Van_Rise_Intern_App.Models;

namespace Van_Rise_Intern_App.Controllers
{
    [RoutePrefix("api/user")]
    public class UserController : ApiController
    {
        private readonly string _connectionString = "Server=DESKTOP-E4AM5MJ;Database=Van_Rise_Intern;Integrated Security=True;TrustServerCertificate=True;";
        private readonly string _jwtSecret = "your_secure_jwt_secret_key_that_is_at_least_32_bytes_long";

        [HttpPost]
        [Route("login")]
        public async Task<IHttpActionResult> Login([FromBody] LoginRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest("Username and password are required.");
            }

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand("sp_ValidateUser", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@username", request.Username);
                        command.Parameters.AddWithValue("@hashedPassword", HashPassword(request.Password));

                        var result = await command.ExecuteScalarAsync();

                        if (result != null && Convert.ToInt32(result) == 1)
                        {
                            var token = GenerateJwtToken(request.Username);
                            return Ok(new LoginResponse { IsValid = true, Token = token, Message = "Login successful." });
                        }
                        else
                        {
                            return Ok(new LoginResponse { IsValid = false, Message = "Invalid username or password." });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        private string GenerateJwtToken(string username)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSecret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, username)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private string HashPassword(string password)
        {
            return password; // Replace with actual secure hashing logic (e.g., BCrypt or PBKDF2)
        }
    }
}
