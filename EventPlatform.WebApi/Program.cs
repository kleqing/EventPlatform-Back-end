using System.Reflection;
using System.Text;
using dotenv.net;
using EventPlatform.Infrastructure.Data;
using EventPlatform.Shared.Jwt;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace EventPlatform.WebApi;

public class Program
{
    public static void Main(string[] args)
    {
        DotEnv.Load();
        var builder = WebApplication.CreateBuilder(args);

        //* Use configuration from appsettings.json and environment variables if one exists
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                               Environment.GetEnvironmentVariable("DefaultConnection");
        builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

        //* Dependencies Injection
        //! NOTE: All of the dependency will be injected automatically. Some of them may be need to be registered manually.
        var currentAssembly = typeof(Program).Assembly;
        var referencedAssemblies = currentAssembly.GetReferencedAssemblies()
            .Where(a => a.Name != null &&
                        a.Name.StartsWith(
                            "EventPlatform")); //* Only load assemblies that start with prefix of the projects in the solution
        var assemblies = referencedAssemblies.Select(Assembly.Load)
            .Append(currentAssembly);
        foreach (var type in assemblies.SelectMany(a => a.GetTypes()))
        {
            if (type.IsClass && !type.IsAbstract)
            {
                foreach (var iface in type.GetInterfaces())
                {
                    if (iface.Name == $"I{type.Name}")
                    {
                        builder.Services.AddScoped(iface,
                            type);
                    }
                }
            }
        }

        // Add services to the container.
        builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
        builder.Services.AddControllers();

        //* JWT Settings
        
        builder.Services.Configure<Jwt>(options =>
        {
            options.Secret = Environment.GetEnvironmentVariable("JWT_SECRET") ?? builder.Configuration["JWT_SECRET"] ?? string.Empty;
            options.Issuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? builder.Configuration["JWT_ISSUER"] ?? string.Empty;
            options.Audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? builder.Configuration["JWT_AUDIENCE"] ?? string.Empty;
            options.ExpiryInMinutes = int.Parse(Environment.GetEnvironmentVariable("JWT_EXPIRY_MINUTES") ?? builder.Configuration["JWT_EXPIRY_MINUTES"] ?? "15");
        });

        //* Authentication & Authorization
        
        builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "Bearer";
                options.DefaultChallengeScheme = "Bearer";
            })
            .AddJwtBearer("Bearer", _ =>
                { })
            .AddGoogle(options =>
            {
                var clientId = options.ClientId =
                    Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID") ?? builder.Configuration["GOOGLE_CLIENT_ID"] ?? string.Empty;
                var clientSecret = options.ClientSecret =
                    Environment.GetEnvironmentVariable("GOOGLE_CLIENT_SECRET") ?? builder.Configuration["GOOGLE_CLIENT_SECRET"] ?? string.Empty;
                
                if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
                {
                    throw new Exception("Google ClientId or ClientSecret is missing");
                }
                
                options.ClientId = clientId;
                options.ClientSecret = clientSecret;
                options.ClaimActions.MapJsonKey("picture", "picture");
                options.SaveTokens = true;
                options.CallbackPath = "/signin-google";
            });
        
        builder.Services.PostConfigure<JwtBearerOptions>("Bearer", options =>
        {
            var issuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? builder.Configuration["JWT_ISSUER"] ?? string.Empty;
            var audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? builder.Configuration["JWT_AUDIENCE"] ?? string.Empty;
            var secret = Environment.GetEnvironmentVariable("JWT_SECRET") ?? builder.Configuration["JWT_SECRET;"] ?? string.Empty;

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = issuer,
                ValidAudience = audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
                ClockSkew = TimeSpan.Zero, //* Disable the default 5-minute clock skew
                RequireExpirationTime = true //* Require the token to have an expiration time
            };
        });
        
        builder.Services.AddAuthorization();

        //* CORS
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll",
                policy =>
                {
                    policy.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
        });

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        
        builder.Services.AddHttpContextAccessor();
        
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseCors("AllowAll");

        app.MapControllers();

        app.Run();
    }
}