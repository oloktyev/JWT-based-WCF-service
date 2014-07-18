using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace RestService
{
    public class CustomClaimsAuthorizationManager : ClaimsAuthorizationManager
    {
        public override bool CheckAccess(AuthorizationContext context)
        {
            Claim actionClaim =
              context.Action.Where(x => x.Type == ClaimTypes.Name).FirstOrDefault();
            Claim resourceClaim =
              context.Resource.Where(x => x.Type == ClaimTypes.Name).FirstOrDefault();

            ClaimsPrincipal principal = context.Principal;

            var resource = new Uri(resourceClaim.Value);
            string action = actionClaim.Value;

            if (action == "GET" && resource.PathAndQuery.Contains("/frommyorganization"))
            {
                if (!principal.IsInRole("admin"))
                {
                    return false;
                }
            }

            return true;
        }
    }
}