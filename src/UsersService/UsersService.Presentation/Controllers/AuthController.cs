using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UsersService.Application.DTOs.Auth;
using UsersService.Application.Interfaces.UseCases.Auth;

namespace UsersService.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IRegisterUseCase _registerUseCase;
        private readonly IAuthenticateUseCase _authenticateUseCase;
        private readonly IConfirmEmailUseCase _confirmEmailUseCase;
        private readonly IRefreshAccessTokenUseCase _refreshAccessTokenUseCase;
        private readonly IForgotPasswordUseCase _forgotPasswordUseCase;
        private readonly IResetPasswordUseCase _resetPasswordUseCase;

        public AuthController(
            IRegisterUseCase registerUseCase,
            IAuthenticateUseCase authenticateUseCase,
            IConfirmEmailUseCase confirmEmailUseCase,
            IRefreshAccessTokenUseCase refreshAccessTokenUseCase,
            IForgotPasswordUseCase forgotPasswordUseCase,
            IResetPasswordUseCase resetPasswordUseCase
        )
        {
            _registerUseCase = registerUseCase;
            _authenticateUseCase = authenticateUseCase;
            _confirmEmailUseCase = confirmEmailUseCase;
            _refreshAccessTokenUseCase = refreshAccessTokenUseCase;
            _forgotPasswordUseCase = forgotPasswordUseCase;
            _resetPasswordUseCase = resetPasswordUseCase;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(
            RegisterDto registerDto, 
            CancellationToken cancellationToken
        )
        {
            await _registerUseCase.ExecuteAsync(registerDto, cancellationToken);

            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpPost("login")]
        public async Task<AuthResultDto> Authenticate(
            AuthenticateDto authenticateDto, 
            CancellationToken cancellationToken
        )
        {
            var result = await _authenticateUseCase.ExecuteAsync(authenticateDto, cancellationToken);

            return result;
        }

        [HttpPost("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(
            ConfirmEmailDto confirmEmailDto, 
            CancellationToken cancellationToken
        )
        {
            await _confirmEmailUseCase.ExecuteAsync(confirmEmailDto, cancellationToken);

            return NoContent();
            ;
        }

        [HttpPost("refresh")]
        [Authorize]
        public async Task<AuthResultDto> RefreshTokens(
            RefreshAccessTokenDto refreshAccessTokenDto, 
            CancellationToken cancellationToken
        )
        {
            var result = await _refreshAccessTokenUseCase.ExecuteAsync(
                User.Identity!.Name!, 
                refreshAccessTokenDto, 
                cancellationToken
            );

            return result;
        }

        [HttpPost("forgot-password")]
        public async Task ForgotPassword(ForgotPasswordDto forgotPasswordDto, CancellationToken cancellationToken)
        {
            await _forgotPasswordUseCase.ExecuteAsync(forgotPasswordDto, cancellationToken);
        }

        [HttpPost("reset-password")]
        public async Task ResetPassword(ResetPasswordDto resetPasswordDto, CancellationToken cancellationToken)
        {
            await _resetPasswordUseCase.ExecuteAsync(resetPasswordDto, cancellationToken);
        }
    }
}
