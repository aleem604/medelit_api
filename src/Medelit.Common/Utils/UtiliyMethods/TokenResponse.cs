using System;
using System.Collections.Generic;
using System.Text;

namespace Medelit.Common
{
   public class TokenResponse
    {
        public string Access_Token { get; set; }
        public int Expires_In { get; set; }
        public string Scope { get; set; }
        public string Token_Type { get; set; }
    }
}
