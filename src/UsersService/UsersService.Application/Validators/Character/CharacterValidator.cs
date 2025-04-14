using FluentValidation;
using UsersService.Application.DTOs.Character;

namespace UsersService.Application.Validators.Character
{
    public class CharacterValidator : AbstractValidator<CreateCharacterDto>
    {
        public CharacterValidator()
        {
            RuleFor(c => c.Attributes)
                .ChildRules(attributes =>
                {
                    attributes.RuleFor(a => a.Strength)
                        .MinValueWithMessage(ValidationRulesExtension.Attribute.MinValue, "Strength");
                    attributes.RuleFor(a => a.Dexterity)
                        .MinValueWithMessage(ValidationRulesExtension.Attribute.MinValue, "Dexterity");
                    attributes.RuleFor(a => a.Intelligence)
                        .MinValueWithMessage(ValidationRulesExtension.Attribute.MinValue, "Intelligence");
                    attributes.RuleFor(a => a.Health)
                        .MinValueWithMessage(ValidationRulesExtension.Attribute.MinValue, "Health");
                    attributes.RuleFor(a => a.HealthPoints)
                        .MinValueWithMessage(ValidationRulesExtension.Attribute.MinValue, "HealthPoints");
                    attributes.RuleFor(a => a.Move)
                        .MinValueWithMessage(ValidationRulesExtension.Attribute.MinValue, "Move");
                    attributes.RuleFor(a => a.Speed)
                        .MinValueWithMessage(ValidationRulesExtension.Attribute.MinValue, "Speed");
                    attributes.RuleFor(a => a.Will)
                        .MinValueWithMessage(ValidationRulesExtension.Attribute.MinValue, "Will");
                    attributes.RuleFor(a => a.Perception)
                        .MinValueWithMessage(ValidationRulesExtension.Attribute.MinValue, "Perception");
                    attributes.RuleFor(a => a.Fatigue)
                        .MinValueWithMessage(ValidationRulesExtension.Attribute.MinValue, "Fatigue");
                });
        }
    }
}
