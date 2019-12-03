using System;
using System.Collections.Generic;
using System.Text;

namespace Medelit.Domain.Helpers
{
  public class TokenModel
  {
    public string Email { get; set; }
    public string Password { get; set; }
    public string Grant_Type { get; set; }
    public string Scope { get; set; }
    public string Client_Id { get; set; }
    public string Client_Secret { get; set; }
  }
}
