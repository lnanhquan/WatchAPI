using WatchAPI.Extensions;
using WatchAPI.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Configure Services
builder.Services.ConfigureCors(builder.Configuration);
builder.Services.ConfigureDbAndIdentity(builder.Configuration);
builder.Services.ConfigureRepositoriesAndServices();
builder.Services.ConfigureJwtAuthentication(builder.Configuration);
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddControllers();
builder.Services.ConfigureSwagger();
builder.Services.ConfigureSerilog(builder.Host);

var app = builder.Build();

// Middleware
app.UseHttpsRedirection();
app.UseMiddleware<ExceptionMiddleware>();
app.UseCors("MyPolicy");
app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Seed admin user
await app.SeedAdminUserAsync();

app.MapControllers();
app.Run();
