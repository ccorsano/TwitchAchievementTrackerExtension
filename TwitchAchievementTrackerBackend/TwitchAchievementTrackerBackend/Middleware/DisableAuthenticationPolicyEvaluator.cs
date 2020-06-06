using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace TwitchAchievementTrackerBackend.Middleware
{
    /// <summary>
    /// 
    /// <see cref="https://stackoverflow.com/questions/41112564/asp-net-disable-authentication-in-development-environment"/>
    /// </summary>
    public class DisableAuthenticationPolicyEvaluator : IPolicyEvaluator
    {
        public async Task<AuthenticateResult> AuthenticateAsync(AuthorizationPolicy policy, HttpContext context)
        {
            // Always pass authentication.
            var authenticationTicket = new AuthenticationTicket(new ClaimsPrincipal(), new AuthenticationProperties(), JwtBearerDefaults.AuthenticationScheme);
            return await Task.FromResult(AuthenticateResult.Success(authenticationTicket));
        }

        public async Task<PolicyAuthorizationResult> AuthorizeAsync(AuthorizationPolicy policy, AuthenticateResult authenticationResult, HttpContext context, object resource)
        {
            // Always pass authorization
            return await Task.FromResult(PolicyAuthorizationResult.Success());
        }
    }
}
