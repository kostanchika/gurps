namespace UsersService.Application.Exceptions.Auth
{
    public class UserAlreadyExistsException(string field, string value)
        : ConflictException($"User with {field} = '{value}' already exists")
    {
        public string Field { get; set; } = field;
        public string Value { get; set; } = value;
    }
}
