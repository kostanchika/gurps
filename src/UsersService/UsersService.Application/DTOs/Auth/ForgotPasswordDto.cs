using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsersService.Application.DTOs.Auth
{
    public record ForgotPasswordDto(
        string Login
    );
}
