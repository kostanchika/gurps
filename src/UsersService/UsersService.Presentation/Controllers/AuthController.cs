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
        public async Task<IActionResult> Register(RegisterDto registerDto, CancellationToken ct)
        {
            await _registerUseCase.ExecuteAsync(registerDto, ct);

            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpPost("login")]
        public async Task<AuthResultDto> Authenticate(AuthenticateDto authenticateDto, CancellationToken ct)
        {
            var result = await _authenticateUseCase.ExecuteAsync(authenticateDto, ct);

            return result;
        }

        [HttpPost("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(ConfirmEmailDto confirmEmailDto, CancellationToken ct)
        {
            await _confirmEmailUseCase.ExecuteAsync(confirmEmailDto, ct);

            return NoContent();
            ;
        }

        [HttpPost("refresh")]
        [Authorize]
        public async Task<AuthResultDto> RefreshTokens(RefreshAccessTokenDto refreshAccessTokenDto, CancellationToken ct)
        {
            var result = await _refreshAccessTokenUseCase.ExecuteAsync(User.Identity!.Name!, refreshAccessTokenDto, ct);

            return result;
        }

        [HttpPost("forgot-password")]
        public async Task ForgotPassword(ForgotPasswordDto forgotPasswordDto, CancellationToken ct)
        {
            await _forgotPasswordUseCase.ExecuteAsync(forgotPasswordDto, ct);
        }

        [HttpPost("reset-password")]
        public async Task ResetPassword(ResetPasswordDto resetPasswordDto, CancellationToken ct)
        {
            await _resetPasswordUseCase.ExecuteAsync(resetPasswordDto, ct);
        }
    }
}
