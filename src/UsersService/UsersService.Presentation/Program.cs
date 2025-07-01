using FluentValidation;
using FluentValidation.AspNetCore;
using GURPS.Character.Providers.Configuration;
using GURPS.Character.Providers.Implementations;
using GURPS.Character.Providers.Implementations.Providers.JSON;
using GURPS.Character.Providers.Interfaces;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using Serilog;
using Serilog.Sinks.Network;
using StackExchange.Redis;
using System.Net;
using System.Net.Mail;
using System.Text;
using UsersService.Application.Interfaces.Services;
using UsersService.Application.Interfaces.UseCases.Auth;
using UsersService.Application.Interfaces.UseCases.Friend;
using UsersService.Application.Interfaces.UseCases.User;
using UsersService.Application.Mappers.Shared;
using UsersService.Application.UseCases.Auth;
using UsersService.Application.UseCases.Friend;
using UsersService.Application.UseCases.User;
using UsersService.Application.Validators.Auth;
using UsersService.Domain.Entities;
using UsersService.Domain.Interfaces;
using UsersService.Infrastructure.Persistence.Redis;
using UsersService.Infrastructure.Persistence.Redis.Configurations;
using UsersService.Infrastructure.Persistence.SQL;
using UsersService.Infrastructure.Services;
using UsersService.Infrastructure.Services.Configurations;
using UsersService.Presentation.Filters;
using UsersService.Presentation.Middlewares;

namespace UsersService.Presentation
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.WebHost.ConfigureKestrel(options =>
            {
                options.ListenAnyIP(8080, listenOptions =>
                {
                    listenOptions.Protocols = HttpProtocols.Http1;
                });

                options.ListenAnyIP(8090, listenOptions =>
                {
                    listenOptions.Protocols = HttpProtocols.Http2;
                });
            });

            builder.Services.AddControllers();

            var elasticUri = builder.Configuration["Elastic:Uri"]
                ?? throw new Exception("Elastic section is missing or bad configured");

            // Logging
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithEnvironmentName()
                .Enrich.WithThreadId()
                .WriteTo.Console()
                .WriteTo.TCPSink(elasticUri)
                .ReadFrom.Configuration(builder.Configuration)
                .CreateLogger();

            builder.Host.UseSerilog();

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
            builder.Services.AddSingleton<IMongoClient>(_ =>
            {
                return new MongoClient(
                    builder.Configuration.GetConnectionString("MongoConnection")
                        ?? throw new Exception("Missing Mongo connection string")
                );
            });

            // Hangfire
            builder.Services.AddHangfire(config =>
                config.UsePostgreSqlStorage(options =>
                {
                    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                        ?? throw new Exception("Missing Postgres connection string");

                    options.UseNpgsqlConnection(connectionString);
                })
            );
            builder.Services.AddHangfireServer();

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
            builder.Services.Configure<ImageSettings>(builder.Configuration.GetSection("Images"));
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
            builder.Services.AddScoped<IRespondFriendRequestUseCase, RespondFriendRequestUseCase>();
            builder.Services.AddScoped<ISendFriendRequestUseCase, SendFriendRequestUseCase>();

            // User
            builder.Services.AddScoped<IGetUserInfoUseCase, GetUserInfoUseCase>();

            // Services
            builder.Services.AddScoped<ITokenService, JWTTokenService>();
            builder.Services.AddScoped<IEmailService, FluentEmailService>();
            builder.Services.AddScoped<IPasswordService, BCryptPasswordService>();
            builder.Services.AddScoped<IKeyValueManager, RedisKeyValueManager>();
            builder.Services.AddScoped<IImageService, PNGImageService>();
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
            builder.Services.AddScoped<IScheduledEmailService, HangfireEmailService>();

            // Middlewares
            builder.Services.AddScoped<ExceptionHandlingMiddleware>();

            // gRPC
            builder.Services.AddGrpc();

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
            var imageSettings = builder.Configuration.GetSection("Images").Get<ImageSettings>()
                ?? throw new Exception("Images section is missing or bad configured");

            var app = builder.Build();

            app.UseCors(options =>
            {
                options.AllowAnyOrigin();
                options.AllowAnyHeader();
                options.AllowAnyMethod();
                options.WithExposedHeaders("X-Total-Count");
            });

            app.UseMiddleware<ExceptionHandlingMiddleware>();

            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = [new AllowLocalAuthorizationFilter()]
            });

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseStaticFiles();

            app.MapGrpcService<AuthService>();

            app.MapControllers();

            app.Run();
        }
    }
}
