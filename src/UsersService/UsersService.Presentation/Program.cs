using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.Net;
using System.Net.Mail;
using System.Text;
using UsersService.Application.Interfaces.Services;
using UsersService.Domain.Entities;
using UsersService.Domain.Interfaces;
using UsersService.Infrastructure.Persistence.Redis;
using UsersService.Infrastructure.Persistence.Redis.Configurations;
using UsersService.Infrastructure.Persistence.SQL;
using UsersService.Application.Interfaces.UseCases.Auth;
using UsersService.Application.UseCases.Auth;
using FluentValidation.AspNetCore;
using FluentValidation;
using UsersService.Application.Validators.Auth;
using UsersService.Presentation.Middlewares;
using UsersService.Infrastructure.Services;
using UsersService.Infrastructure.Services.Configurations;
using UsersService.Application.Interfaces.UseCases.Friend;
using UsersService.Application.UseCases.Friend;
using UsersService.Application.Mappers.Shared;
using UsersService.Application.Interfaces.UseCases.Character;
using UsersService.Application.UseCases.Character;
using GURPS.Character.Providers.Interfaces;
using GURPS.Character.Providers.Implementations;
using GURPS.Character.Providers.Implementations.Providers.JSON;
using GURPS.Character.Providers.Configuration;

namespace UsersService.Presentation
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();

            // Connections
            builder.Services.AddDbContext<UsersContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")
                    ?? throw new Exception("Missing Postgres connection string")
                )
            );
            builder.Services.AddSingleton<IConnectionMultiplexer>(
                ConnectionMultiplexer.Connect(
                    builder.Configuration.GetConnectionString("RedisConnection")
                        ?? throw new Exception("Missing Redis connection string")
                )
            );

            // Repositories
            builder.Services.AddScoped<IRepository<UserEntity>, Repository<UserEntity>>();
            builder.Services.AddScoped<IRepository<FriendshipEntity>, Repository<FriendshipEntity>>();

            // Settings
            builder.Services.Configure<JWTSettings>(builder.Configuration.GetSection("JWT"));
            builder.Services.Configure<RedisSettings>(builder.Configuration.GetSection("Redis"))
                .PostConfigure<RedisSettings>(settings =>
                {
                    settings.RefreshTokenExpiry = TimeSpan.FromMinutes(settings.RefreshTokenLifetimeMinutes);
                    settings.RegistrationCodeExpiry = TimeSpan.FromMinutes(settings.RegistrationCodeLifetimeMinutes);
                    settings.ResetPasswordCodeExpiry = TimeSpan.FromMinutes(settings.ResetPasswordCodeLifetimeMinutes);
                });
            builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("Email"));
            builder.Services.Configure<CharacterSettings>(builder.Configuration.GetSection("Character"));

            // Validation
            builder.Services.AddFluentValidationAutoValidation();
            builder.Services.AddValidatorsFromAssemblyContaining(typeof(RegisterValidator));

            // Mapper
            builder.Services.AddAutoMapper(typeof(UserEntityToUserDto).Assembly);

            // UseCases
            // Auth
            builder.Services.AddScoped<IAuthenticateUseCase, AuthenticateUseCase>();
            builder.Services.AddScoped<IConfirmEmailUseCase, ConfirmEmailUseCase>();
            builder.Services.AddScoped<IForgotPasswordUseCase, ForgotPasswordUseCase>();
            builder.Services.AddScoped<IRefreshAccessTokenUseCase, RefreshAccessTokenUseCase>();
            builder.Services.AddScoped<IRegisterUseCase, RegisterUseCase>();
            builder.Services.AddScoped<IResetPasswordUseCase, ResetPasswordUseCase>();
            // Friend
            builder.Services.AddScoped<IGetActiveFriendsUseCase, GetActiveFriendsUseCase>();
            builder.Services.AddScoped<IGetRecievedFriendRequestsUseCase, GetRecievedFriendRequestsUseCase>();
            builder.Services.AddScoped<IGetSentFriendRequestsUseCase, GetSentFriendRequestsUseCase>();
            builder.Services.AddScoped<IRespondFriendRequestUseCase,  RespondFriendRequestUseCase>();
            builder.Services.AddScoped<ISendFriendRequestUseCase, SendFriendRequestUseCase>();
            // Character
            builder.Services.AddScoped<ISearchCharactersUseCase, SearchCharactersUseCase>();
            builder.Services.AddScoped<IGetPointsConfigurationUseCase, GetPointsConfigurationUseCase>();
            builder.Services.AddScoped<IGetCharacterUseCase, GetCharacterUseCase>();
            builder.Services.AddScoped<ICreateCharacterUseCase, CreateCharacterUseCase>();
            builder.Services.AddScoped<IDeleteCharacterUseCase, DeleteCharacterUseCase>();
            builder.Services.AddScoped<IUpdateCharacterUseCase, UpdateCharacterUseCase>();

            // Services
            builder.Services.AddScoped<ITokenService, JWTTokenService>();
            builder.Services.AddScoped<IEmailService, FluentEmailService>();
            builder.Services.AddScoped<IPasswordService, BCryptPasswordService>();
            builder.Services.AddScoped<IKeyValueManager, RedisKeyValueManager>();
            builder.Services.AddScoped<ICharacterManager, CharacterManager>();
            builder.Services.AddScoped<ICharacterCalculator, CharacterCalculator>();
            builder.Services.AddSingleton<ICharacterConfigurationProvider, JSONCharacterConfigurationProvider>(
                (serviceProvider) =>
                {
                    var characterSettings = builder.Configuration.GetSection("Character").Get<CharacterSettings>()
                        ?? throw new Exception("Character section is missing or bad configured");

                    return new JSONCharacterConfigurationProvider(characterSettings.SettingsPath);
                }
            );

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

            var redisSettings = builder.Configuration.GetSection("Redis").Get<RedisSettings>()
                ?? throw new Exception("Redis section is missing or bad configured");

            var app = builder.Build();

            app.UseMiddleware<ExceptionHandlingMiddleware>();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
