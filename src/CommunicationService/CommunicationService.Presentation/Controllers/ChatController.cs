using CommunicationService.Application.Dto.Chat;
using CommunicationService.Application.Dto.Message;
using CommunicationService.Application.Features.Chat.Commands.AddUserToChat;
using CommunicationService.Application.Features.Chat.Commands.CreateChat;
using CommunicationService.Application.Features.Chat.Commands.DeleteMessage;
using CommunicationService.Application.Features.Chat.Commands.KickUserFromChat;
using CommunicationService.Application.Features.Chat.Commands.LeaveFromChat;
using CommunicationService.Application.Features.Chat.Commands.SendMessage;
using CommunicationService.Application.Features.Chat.Queries.GetChat;
using CommunicationService.Application.Features.Chat.Queries.GetChatMessages;
using CommunicationService.Application.Features.Chat.Queries.GetUserChats;
using CommunicationService.Presentation.Models;
using CommunicationService.Presentation.Models.Chat;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CommunicationService.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ChatController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ChatController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IEnumerable<ChatDto>> GetChats(
            [FromQuery] GetUserChatsRequest getUserChatsRequest,
            CancellationToken cancellationToken
        )
        {
            var userLogin = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var getUserChatsQuery = new GetUserChatsQuery(
                userLogin,
                getUserChatsRequest.ChatName,
                getUserChatsRequest.PageNumber,
                getUserChatsRequest.PageSize,
                getUserChatsRequest.SortBy,
                getUserChatsRequest.SortType
            );

            var pagedResultDto = await _mediator.Send(getUserChatsQuery, cancellationToken);

            Response.Headers["X-Total-Count"] = pagedResultDto.TotalCount.ToString();

            return pagedResultDto.Items;
        }

        [HttpPost]
        public async Task<ChatDto> CreateChat(
            [FromForm] CreateChatRequest createChatRequest,
            CancellationToken cancellationToken
        )
        {
            var userLogin = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var createChatCommand = new CreateChatCommand(
                userLogin,
                createChatRequest.Name,
                createChatRequest.Participants,
                new AttachmentAdapter(createChatRequest.Logo)
            );

            return await _mediator.Send(createChatCommand, cancellationToken);
        }

        [HttpGet("{chatId}")]
        public async Task<ChatDto> GetChat(
            string chatId,
            CancellationToken cancellationToken
        )
        {
            var userLogin = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            return await _mediator.Send(new GetChatQuery(userLogin, chatId), cancellationToken);
        }

        [HttpGet("{chatId}/messages")]
        public async Task<IEnumerable<MessageDto>> GetMessages(
            string chatId,
            [FromQuery] GetUserChatsRequest getUserChatsRequest,
            CancellationToken cancellationToken
        )
        {
            var userLogin = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var getChatMessagesQuery = new GetChatMessagesQuery(
                userLogin,
                chatId,
                getUserChatsRequest.PageNumber,
                getUserChatsRequest.PageSize
            );

            var pagedResultDto = await _mediator.Send(getChatMessagesQuery, cancellationToken);

            Response.Headers["X-Total-Count"] = pagedResultDto.TotalCount.ToString();

            return pagedResultDto.Items;
        }

        [HttpPost("{chatId}/invite")]
        public async Task<ChatDto> AddToChat(
            string chatId,
            [FromBody] AddUserToChatRequest addUserToChatRequest,
            CancellationToken cancellationToken
        )
        {
            var userLogin = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            return await _mediator.Send(
                new AddUserToChatCommand(userLogin, chatId, addUserToChatRequest.InviteeLogin),
                cancellationToken
            );
        }

        [HttpPost("{chatId}/leave")]
        public async Task LeaveChat(
            string chatId,
            CancellationToken cancellationToken
        )
        {
            var userLogin = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            await _mediator.Send(new LeaveFromChatCommand(userLogin, chatId), cancellationToken);
        }

        [HttpPost("{chatId}/kick/{kickeeLogin}")]
        public async Task KickFromChat(
            string chatId,
            string kickeeLogin,
            CancellationToken cancellationToken
        )
        {
            var userLogin = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            await _mediator.Send(new KickUserFromChatCommand(userLogin, kickeeLogin, chatId), cancellationToken);
        }

        [HttpPost("{chatId}/messages")]
        public async Task<MessageDto> SendMessage(
            string chatId,
            [FromForm] SendMessageRequest sendMessageRequest,
            CancellationToken cancellationToken
        )
        {
            var userLogin = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var sendMessageCommand = new SendMessageCommand(
                userLogin,
                chatId,
                sendMessageRequest.Message,
                sendMessageRequest.Attachment == null
                                ? null
                                : new AttachmentAdapter(sendMessageRequest.Attachment)
            );

            return await _mediator.Send(sendMessageCommand, cancellationToken);
        }

        [HttpDelete("{chatId}/messages/{messageId}")]
        public async Task<MessageDto> DeleteMessage(
            string chatId,
            string messageId,
            CancellationToken cancellationToken
        )
        {
            var userLogin = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            return await _mediator.Send(new DeleteMessageCommand(userLogin, messageId), cancellationToken);
        }
    }
}
