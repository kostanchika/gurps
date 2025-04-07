using FluentValidation;

namespace UsersService.Application.Validators
{
    internal static class ValidationRulesExtension
    {
        public static class Login
        {
            public static int MinLength => 5;
            public static int MaxLength => 20;
        }

        public static class Username
        {
            public static int MinLength => 1;
            public static int MaxLength => 20;
        }

        public static class Password
        {
            public static int MinLength => 8;
            public static int MaxLength => 20;
        }

        internal static IRuleBuilder<T, string> MinLengthWithMessage<T>(
            this IRuleBuilder<T, string> ruleBuilder,
            int minLength,
            string fieldName
        )
        {
            return ruleBuilder.MinimumLength(minLength)
                                     .WithMessage($"{fieldName} must be longer or equal to {minLength}");
        }

        internal static IRuleBuilder<T, string> MaxLengthWithMessage<T>(
            this IRuleBuilder<T, string> ruleBuilder,
            int maxLength,
            string message
        )
        {
            return ruleBuilder.MaximumLength(maxLength)
                                     .WithMessage(message);
        }
    }
}
