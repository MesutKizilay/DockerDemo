var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.MapOpenApi();
}

app.UseAuthorization();

app.MapControllers();

app.MapGet("/health", () => Results.Ok("Healthy"));

app.MapGet("/networktest", async () =>
{
    using var client = new HttpClient();

    var response = await client.GetAsync("http://testweb");

    return Results.Ok(new
    {
        success = response.IsSuccessStatusCode,
        statusCode = (int)response.StatusCode,
        message = "API container testweb container'ına ulaştı."
    });
});

app.MapGet("/filetest", (
	IWebHostEnvironment env,
	ILogger<Program> logger) =>
{
	logger.LogInformation("FileTest endpoint çağrıldı.");

	var webRoot = env.WebRootPath
		?? Path.Combine(env.ContentRootPath, "wwwroot");

	var uploadFolder = Path.Combine(webRoot, "uploads");

	Directory.CreateDirectory(uploadFolder);

	var fileName = $"test-{DateTime.Now:yyyyMMdd-HHmmss}.txt";
	var fullPath = Path.Combine(uploadFolder, fileName);

	File.WriteAllText(
		fullPath,
		$"Docker volume test. Oluşturulma: {DateTime.Now}"
	);

	logger.LogInformation(
		"Dosya oluşturuldu. Dosya adı: {FileName}",
		fileName);

	return new
	{
		success = true,
		fileName, 
		containerPath = fullPath 
	};
});

app.Run();
