using BugTracker.API.Data;
using BugTracker.API.Entities;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Text;

namespace BugTracker.API;

public static class DependencyInjection
{
    public static IServiceCollection RegisterPersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<AppDbContext>(options =>
                     options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString),
                         builder => builder.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

        // Add MediatR services
        services.AddMediatR(Assembly.GetExecutingAssembly());

        //Add microsoft identity
        services.AddIdentity<AuthUser, AuthRole>(options =>
        {
            options.User.RequireUniqueEmail = true;
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = true;
        }).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

        services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(x =>
        {
            var key = Encoding.UTF8.GetBytes(configuration["JWT:Key"]);
            x.SaveToken = true;
            x.RequireHttpsMetadata = false;
            x.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = configuration["JWT:Issuer"],
                ValidAudience = configuration["JWT:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ClockSkew = TimeSpan.Zero,
            };
            x.Events = new JwtBearerEvents
            {
                OnChallenge = async (context) =>
                {
                    context.HandleResponse();
                    if (context.AuthenticateFailure == null)
                    {
                        context.Response.StatusCode = 403;
                    }
                    else if (context.AuthenticateFailure != null)
                    {
                        context.Response.StatusCode = 401;
                    }
                }
            };
        });

        services.AddHttpContextAccessor();

        return services;
    }
}
