using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Medelit.Application;
using Medelit.Common;

namespace Medelit.Api.Configurations
{

    public class TokenMiddleware
    {
        private readonly RequestDelegate _next;

        public TokenMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        // IMyScopedService is injected into Invoke
        public async Task Invoke(HttpContext httpContext)
        {
            httpContext.Items.TryAdd(eTinUser.TinUser, ProcessToken(httpContext));
            await _next(httpContext);
        }
        private AuthClaims ProcessToken(HttpContext _httpContext)
        {
            try
            {
                var jwtHandler = new JwtSecurityTokenHandler();
                ClaimsPrincipal user = _httpContext.User;
                var claims = user.Claims.ToList();

                var info = claims.Where(x => x.Type.Contains("info", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault().Value.ToString();

                AuthClaims jwtPayload = JsonConvert.DeserializeObject<AuthClaims>(info);

                //var updatedAt = claims.Where(x => x.Type.Equals("updated_at", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault().Value.ToString();
                //if (DateTime.TryParse(JsonConvert.DeserializeObject<string>(updatedAt), out DateTime outDate))
                //    jwtPayload.UpdatedAt = outDate;

                return jwtPayload;
            }
            catch
            {
                return new AuthClaims();
            }
        }


    }
}
