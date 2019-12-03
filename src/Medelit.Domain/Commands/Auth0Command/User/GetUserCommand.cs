using System;
using Medelit.Common;
using Medelit.Domain.Core.Commands;

namespace Medelit.Domain.Commands
{
    public class GetUserCommand : AuthBaseCommand
    {
        public string UserId { get; set; }
        public string Fields { get; set; }
        public bool IncludeFields { get; set; } = true;
    }
    
}