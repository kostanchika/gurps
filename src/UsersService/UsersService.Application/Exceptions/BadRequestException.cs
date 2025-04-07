﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsersService.Application.Exceptions
{
    public abstract class BadRequestException(string message)
        : Exception(message)
    {
    }
}
