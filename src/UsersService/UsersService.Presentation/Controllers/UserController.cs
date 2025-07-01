using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UsersService.Application.DTOs.Friend;
using UsersService.Application.DTOs.Shared;
using UsersService.Application.Interfaces.UseCases.Friend;
using UsersService.Application.Interfaces.UseCases.User;

namespace UsersService.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IGetUserInfoUseCase _getUserInfoUseCase;
        private readonly IGetActiveFriendsUseCase _getActiveFriendsUseCase;
        private readonly IGetRecievedFriendRequestsUseCase _getRecievedFriendRequestsUseCase;
        private readonly IGetSentFriendRequestsUseCase _getSentFriendRequestsUseCase;
        private readonly IRespondFriendRequestUseCase _respondFriendRequestUseCase;
        private readonly ISendFriendRequestUseCase _sendFriendRequestUseCase;

        public UserController(
            IGetUserInfoUseCase getUserInfoUseCase,
            IGetActiveFriendsUseCase getActiveFriendsUseCase,
            IGetRecievedFriendRequestsUseCase getRecievedFriendRequestsUseCase,
            IGetSentFriendRequestsUseCase getSentFriendRequestsUseCase,
            IRespondFriendRequestUseCase respondFriendRequestUseCase,
            ISendFriendRequestUseCase sendFriendRequestUseCase
        )
        {
            _getUserInfoUseCase = getUserInfoUseCase;
            _getActiveFriendsUseCase = getActiveFriendsUseCase;
            _getRecievedFriendRequestsUseCase = getRecievedFriendRequestsUseCase;
            _getSentFriendRequestsUseCase = getSentFriendRequestsUseCase;
            _respondFriendRequestUseCase = respondFriendRequestUseCase;
            _sendFriendRequestUseCase = sendFriendRequestUseCase;
        }

        [HttpGet("{login}")]
        public async Task<UserDto> GetUserInfo(
            string login,
            CancellationToken cancellationToken
        )
        {
            var user = await _getUserInfoUseCase.ExecuteAsync(
                login,
                cancellationToken
            );

            return user;
        }

        [HttpGet("{login}/friend")]
        public async Task<IEnumerable<UserDto>> GetFriends(
            string login,
            [FromQuery] GetFriendRequestDto getFriendRequestDto,
            CancellationToken cancellationToken
        )
        {
            var friends = await _getActiveFriendsUseCase.ExecuteAsync(
                login,
                getFriendRequestDto,
                cancellationToken
            );

            return friends;
        }

        [HttpGet("{login}/friend/sent")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetSentFriendRequests(
            string login,
            [FromQuery] GetFriendRequestDto getFriendRequestDto,
            CancellationToken cancellationToken
        )
        {
            if (login != User.Identity!.Name)
            {
                return Forbid();
            }

            var sentRequests = await _getSentFriendRequestsUseCase.ExecuteAsync(
                login,
                getFriendRequestDto,
                cancellationToken
            );

            return Ok(sentRequests);
        }

        [HttpGet("{login}/friend/recieved")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetRecievedFriendRequests(
            string login,
            [FromQuery] GetFriendRequestDto getFriendRequestDto,
            CancellationToken cancellationToken
        )
        {
            if (login != User.Identity!.Name)
            {
                return Forbid();
            }

            var recievedRequests = await _getRecievedFriendRequestsUseCase.ExecuteAsync(
                login,
                getFriendRequestDto,
                cancellationToken
            );

            return Ok(recievedRequests);
        }

        [HttpPost("{recipent}/friend")]
        public async Task<IActionResult> SendFriendRequest(
            string recipent,
            CancellationToken cancellationToken
        )
        {
            await _sendFriendRequestUseCase.ExecuteAsync(
                User.Identity!.Name!,
                recipent,
                cancellationToken
            );

            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpPost("{initiator}/friend/respond")]
        public async Task<IActionResult> RespondFriendRequest(
            string initiator,
            [FromBody] RespondFriendRequestDto respondFriendRequestDto,
            CancellationToken cancellationToken
        )
        {
            await _respondFriendRequestUseCase.ExecuteAsync(
                initiator,
                User.Identity!.Name!,
                respondFriendRequestDto,
                cancellationToken
            );

            return Ok();
        }
    }
}
