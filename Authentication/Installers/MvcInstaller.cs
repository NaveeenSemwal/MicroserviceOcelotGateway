using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Collections.Generic;
using System.Text;
using AuthenticationService.Options;
using AuthenticationService.Services.Abstract;
using AuthenticationService.Services.Implementation;
using TweetBook.Utilities;

namespace AuthenticationService.Installers
{
    public class MvcInstaller : IInstaller
    {
        public void InstallService(IServiceCollection services, IConfiguration configuration)
        {

            // Added JWT Authentication start here.
            var jwtSettings = new JwtSettings();
            configuration.Bind(nameof(jwtSettings), jwtSettings);

            services.AddSingleton(jwtSettings);
            services.AddScoped<IIdentityService, IdentityService>();

            services.AddScoped<ILog, LoggerService>();


            var tokenValidationParameter = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.Secret)),
                ValidateIssuer = false,
                ValidateAudience = false,
                RequireExpirationTime = false,
                ValidateLifetime = false

            };
            // Added this to singleton  to access in IdentityService.GetPrincipalFromToken class.
            services.AddSingleton(tokenValidationParameter);

            services.AddAuthentication(configureOptions =>
            {
                configureOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                configureOptions.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                configureOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            })

                .AddJwtBearer(configureOptions =>
            {
                configureOptions.SaveToken = true;
                configureOptions.TokenValidationParameters = tokenValidationParameter;
            });  // Added JWT Authentication end here.


            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "AuthenticationService API", Version = "v1" });

                // Added JWT Authentication in Swagger Start.
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                    {

                    Reference= new OpenApiReference{

                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    },
                    Scheme = "oauth2",
                    Name = "Bearer",
                    In = ParameterLocation.Header,

                    }, new List<string>() }
                });
            });  // End here.


            // This is for configuring Automapper.
            services.AddAutoMapper(typeof(Startup));

            //  Issue :  System.Text.Json.JsonException: A possible object cycle was detected which is not supported. 
            // This can either be due to a cycle or if the object depth is larger than the maximum allowed depth of 32.
            services.AddControllers().AddNewtonsoftJson(options =>
          options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
        }
    }
}
