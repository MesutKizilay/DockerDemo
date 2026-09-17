using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace DockerDemo.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DbTestController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public DbTestController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var connectionString = _configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return BadRequest(new
            {
                success = false,
                message = "Connection string bulunamadı."
            });
        }

        try
        {
            await using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            await using var command = new SqlCommand("SELECT 1", connection);
            var result = await command.ExecuteScalarAsync();

            return Ok(new
            {
                success = true,
                message = "SQL Server bağlantısı başarılı.",
                result
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                success = false,
                message = "SQL Server bağlantısı başarısız.",
                error = ex.Message
            });
        }
    }
}