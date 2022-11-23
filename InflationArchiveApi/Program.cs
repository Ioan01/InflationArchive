using System.Text;
using InflationArchive.Contexts;
using InflationArchive.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetValue<string>("ConnectionStrings:groceriesScraper-ROContext");
builder.Services.AddDbContext<UserContext>(options=>
{
    options.UseNpgsql(connectionString);
});

builder.Services.AddScoped<AccountService>();




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
    options.AddPolicy(name: "default",
        policy  =>
        {
            policy.WithOrigins(new []{"http://localhost:8080","http://localhost:5016"});
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("default");

app.UseHttpsRedirection();
app.UseAuthentication();

app.UseAuthorization();



app.MapControllers();


app.Run();