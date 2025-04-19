namespace UsersService.Application.Exceptions.Auth
{
    public class UserNotFoundException(string field, string value)
        : NotFoundException($"User with {field} = '{value}' was not found")
    {
        public string Field { get; set; } = field;
        public string Value { get; set; } = value;
    }
}
