using System;
using System.Collections.Generic;
using System.Text;

namespace Medelit.Domain.Commands
{
    public class AccessTokenFromRefreshTokenCommand : AuthBaseCommand
    {
        public string RefreshToken { get; set; }
    }
}
