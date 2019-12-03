using System;
using Medelit.Common;
using Medelit.Domain.Core.Commands;

namespace Medelit.Domain.Commands
{
    public class LoginCommand : AuthBaseCommand
    {
        public string Email { get; set; }
        public string Password { get; set; }

        public override bool IsValid()
        {
            return true;
        }
    }
    
}