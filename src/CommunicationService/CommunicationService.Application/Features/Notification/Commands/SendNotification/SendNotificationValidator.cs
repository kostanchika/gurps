using FluentValidation;

namespace CommunicationService.Application.Features.Notification.Commands.SendNotification
{
    public class SendNotificationValidator : AbstractValidator<SendNotificationCommand>
    {
        public SendNotificationValidator()
        {
            RuleFor(n => n.Name)
                .NotEmpty();

            RuleFor(n => n.Content)
                .NotEmpty();

            RuleFor(n => n.Meta)
                .ForEach(m => m.Must(meta => !string.IsNullOrEmpty(meta.Name) && !string.IsNullOrEmpty(meta.Description))
                            .WithMessage("Meta name and description can not by empty")
                );
        }
    }
}
