using System.Text;
using InflationArchive.Contexts;
using InflationArchive.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Quartz;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(static c =>
{
    c.AddSecurityDefinition("JWT",
        new OpenApiSecurityScheme
        {
            Description = "JWT Authorization header using the Bearer scheme.",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = "bearer"
        });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "JWT"
                }
            },
            new List<string>()
        }
    });
});



var connectionStringScraper = builder.Configuration.GetValue<string>("ConnectionStrings:scraperContext");
builder.Services.AddDbContext<ScraperContext>(options =>
{
    options.UseNpgsql(connectionStringScraper);
});

builder.Services.AddScoped<AccountService>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<JoinedService>();
builder.Services.AddScoped<HttpClient>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;

        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidIssuer = builder.Configuration["JWT:Issuer"],
            ValidAudience = builder.Configuration["JWT:Audience"],
            RequireExpirationTime = true,
            ValidateLifetime = true,
            IssuerSigningKey =
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(builder.Configuration["JWT:IssuerSigningKey"])),
            ClockSkew = TimeSpan.Zero
        };
    });


builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policyBuilder =>
    {
        policyBuilder.WithOrigins(new[] { "http://localhost:8080" });
    });

});


builder.Services.AddQuartz(configurator =>
{
    configurator.UseMicrosoftDependencyInjectionJobFactory();

    configurator.AddJob<MetroScraper>(jobConfigurator => jobConfigurator.WithIdentity("metro"));
    configurator.AddTrigger(triggerConfigurator =>
    {
        triggerConfigurator.ForJob("metro")
            .WithIdentity("metroTrigger")
            .WithSimpleSchedule(scheduleBuilder =>
            {
                scheduleBuilder.WithIntervalInHours(12)
                    .RepeatForever();
            });
    });

    configurator.AddJob<MegaImageScraper>(jobConfigurator => jobConfigurator.WithIdentity("mega-image"));
    configurator.AddTrigger(triggerConfigurator =>
    {
        triggerConfigurator.ForJob("mega-image")
            .WithIdentity("megaImageTrigger")
            .WithSimpleSchedule(scheduleBuilder =>
            {
                scheduleBuilder.WithIntervalInHours(12)
                    .RepeatForever();
            });
    });
});

builder.Services.AddQuartzServer(options =>
{
    options.WaitForJobsToComplete = true;
    options.AwaitApplicationStarted = true;

    var shouldTriggerJobs = builder.Configuration.GetValue<bool>("QuartzSettings:shouldTriggerJobs");
    if (!shouldTriggerJobs)
    {
        options.StartDelay = TimeSpan.MaxValue;
    }
});

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var scraperContext = services.GetRequiredService<ScraperContext>();

    await ContextsInitializer.Initialize(scraperContext);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();

app.UseAuthentication();

app.UseAuthorization();



app.MapControllers();


app.Run();