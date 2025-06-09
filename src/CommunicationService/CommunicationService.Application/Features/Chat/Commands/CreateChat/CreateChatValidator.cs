using FluentValidation;

namespace CommunicationService.Application.Features.Chat.Commands.CreateChat
{
    public class CreateChatValidator : AbstractValidator<CreateChatCommand>
    {
        public CreateChatValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
        }
    }
}
