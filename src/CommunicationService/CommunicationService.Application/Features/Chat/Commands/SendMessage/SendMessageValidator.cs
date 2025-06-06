using FluentValidation;

namespace CommunicationService.Application.Features.Chat.Commands.SendMessage
{
    public class SendMessageValidator : AbstractValidator<SendMessageCommand>
    {
        public SendMessageValidator()
        {
            RuleFor(x => x.Message).NotEmpty();
        }
    }
}
