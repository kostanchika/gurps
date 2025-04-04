using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsersService.Application.Exceptions.Auth
{
    public class EmailAlreadyConfirmedException(string email)
        : ConflictException($"Email '{email}' is already confirmed")
    {
        public string Email { get; set; } = email;
    }
}
