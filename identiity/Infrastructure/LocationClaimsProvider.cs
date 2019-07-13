using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace identiity.Infrastructure
{
    public class LocationClaimsProvider : IClaimsTransformation
    {
        public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            if (principal != null && 
                principal.HasClaim(c=>c.Type == ClaimTypes.PostalCode) == false)
            {
                var identity = principal.Identity as ClaimsIdentity;
                if (identity != null && identity.IsAuthenticated &&
                    identity.Name != null)
                {
                    if (identity.Name.ToLower() == "alice")
                    {
                        identity.AddClaims(new Claim[] {
                            new Claim(ClaimTypes.PostalCode,
                                      "DC 20500",
                                      ClaimValueTypes.String,
                                      "MyRemoteSystem"),
                            new Claim(ClaimTypes.StateOrProvince,
                                      "DC",
                                      ClaimValueTypes.String,
                                      "MyRemoteSystem"),
                        });
                    }
                    else
                    {
                        identity.AddClaims(new Claim[] {
                            new Claim(ClaimTypes.PostalCode,
                                      "NY 10036",
                                      ClaimValueTypes.String,
                                      "MyRemoteSystem"),
                            new Claim(ClaimTypes.StateOrProvince,
                                      "NY",
                                      ClaimValueTypes.String,
                                      "MyRemoteSystem"),
                        });
                    }
                }
            }
            return Task.FromResult(principal);
        }
    }
}
