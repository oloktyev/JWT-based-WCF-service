using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Claims;
using Microsoft.IdentityModel.Protocols.WSIdentity;


namespace RestService
{
    public class CustomJwtSecurityTokenHandler : JwtSecurityTokenHandler
    {
        // Override ValidateSignature so that it gets the SigningToken from the configuration if it doesn't exist in
        // the validationParameters object.
        private const string KeyName = "https://localhost/TestRelyingParty";
        private const string ValidIssuerString = "https://mySTSname/trust";

        public override ReadOnlyCollection<ClaimsIdentity> ValidateToken(SecurityToken token)
        {
            //throw new Exception();
            
            var jwtToken = token as JwtSecurityToken;
            var identity = new ClaimsIdentity();
            var claim = new Claim(WSIdentityConstants.ClaimTypes.Name, "xxx");
            claim.Properties.Add("Site", "SmartDrive");
            identity.Claims.ToList().Add(claim);
            return new ReadOnlyCollection<ClaimsIdentity>(new[] { identity });
        }
    }
}