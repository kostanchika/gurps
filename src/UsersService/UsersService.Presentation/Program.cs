using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using System.Net.Mail;
using System.Text;
using UsersService.Application.Interfaces.Services;
using UsersService.Domain.Entities;
using UsersService.Domain.Interfaces;
using UsersService.Infrastructure.Persistence.SQL;
using UsersService.Application.Interfaces.UseCases.Auth;
using UsersService.Application.UseCases.Auth;
using FluentValidation.AspNetCore;
using FluentValidation;
using UsersService.Application.Validators.Auth;
using UsersService.Presentation.Middlewares;
using UsersService.Infrastructure.Services;
using UsersService.Infrastructure.Services.Configurations;

namespace UsersService.Presentation
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();

            // Repositories
            builder.Services.AddScoped<IRepository<UserEntity>, Repository<UserEntity>>();
            builder.Services.AddScoped<IRepository<FriendshipEntity>, Repository<FriendshipEntity>>();

            // Settings
            builder.Services.Configure<JWTSettings>(builder.Configuration.GetSection("JWT"));
            builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("Email"));

            // Validation
            builder.Services.AddFluentValidationAutoValidation();
            builder.Services.AddValidatorsFromAssemblyContaining(typeof(RegisterValidator));

            // UseCases
            builder.Services.AddScoped<IAuthenticateUseCase, AuthenticateUseCase>();
            builder.Services.AddScoped<IConfirmEmailUseCase, ConfirmEmailUseCase>();
            builder.Services.AddScoped<IForgotPasswordUseCase, ForgotPasswordUseCase>();
            builder.Services.AddScoped<IRefreshAccessTokenUseCase, RefreshAccessTokenUseCase>();
            builder.Services.AddScoped<IRegisterUseCase, RegisterUseCase>();
            builder.Services.AddScoped<IResetPasswordUseCase, ResetPasswordUseCase>();

            // Services
            builder.Services.AddScoped<ITokenService, JWTTokenService>();
            builder.Services.AddScoped<IEmailService, FluentEmailService>();
            builder.Services.AddScoped<IPasswordService, BCryptPasswordService>();

            // Email
            var emailSettings = builder.Configuration.GetSection("Email").Get<EmailSettings>()
                        ?? throw new Exception("Email section is missing or bad configured");
            builder.Services.AddFluentEmail(emailSettings.Sender, emailSettings.Name)
                .AddSmtpSender(() => new SmtpClient()
                {
                    Host = emailSettings.Host,
                    Port = emailSettings.Port,
                    EnableSsl = emailSettings.EnableSSL,
                    Credentials = new NetworkCredential(
                        emailSettings.Login,
                        emailSettings.Password
                    )
                });

            // Middlewares
            builder.Services.AddScoped<ExceptionHandlingMiddleware>();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    var jwtSettings = builder.Configuration.GetSection("JWT").Get<JWTSettings>()
                        ?? throw new Exception("JWT section is missing or bad configured");

                    options.TokenValidationParameters = new()
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtSettings.Issuer,
                        ValidAudience = jwtSettings.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
                    };
                });

            var app = builder.Build();

            app.UseMiddleware<ExceptionHandlingMiddleware>();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
