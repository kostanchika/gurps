using Microsoft.EntityFrameworkCore;
using UsersService.Domain.Entities;
using UsersService.Domain.Interfaces;
using UsersService.Infrastructure.Persistence.SQL;
using UsersService.Application.Interfaces.UseCases.Auth;
using UsersService.Application.UseCases.Auth;
using FluentValidation.AspNetCore;
using FluentValidation;
using UsersService.Application.Validators.Auth;

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

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
