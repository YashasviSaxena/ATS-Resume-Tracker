using ATS.Core.Interfaces;
using ATS.Core.Services;
using ATS.Infrastructure.Data;
using ATS.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.SetIsOriginAllowed(origin => true)
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});

builder.Services.AddDbContext<ATSDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IResumeRepository, ResumeRepository>();
builder.Services.AddScoped<IJobDescriptionRepository, JobDescriptionRepository>();
builder.Services.AddScoped<IMatchingService, MatchingService>();
builder.Services.AddScoped<IResumeParser, PdfParser>();
builder.Services.AddScoped<IResumeAnalyzer, ResumeAnalyzer>();

builder.Services.AddControllers();

var app = builder.Build();

app.UseCors("AllowAll");

// Serve static files from wwwroot
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Apply migrations automatically
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ATSDbContext>();
    dbContext.Database.EnsureCreated(); // Creates database and tables if not exists
}

app.Run();