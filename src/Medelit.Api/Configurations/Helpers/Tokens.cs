using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Medelit.Application;
using Medelit.Auth;
using Medelit.Common;
using Newtonsoft.Json;

namespace Medelit.Helpers
{
    public class Tokens
    {
      public static async Task<string> GenerateJwt(ClaimsIdentity identity, IJwtFactory jwtFactory,CurrentUserInfo userInfo, JwtIssuerOptions jwtOptions, JsonSerializerSettings serializerSettings)
      {
        var response = new
        {
          id = identity.Claims.Single(c => c.Type == "id").Value,
          accessToken = await jwtFactory.GenerateEncodedToken(userInfo, identity),
          expires_in = (int)jwtOptions.ValidFor.TotalSeconds
        };

        return JsonConvert.SerializeObject(response, serializerSettings);
      }
    }
}
