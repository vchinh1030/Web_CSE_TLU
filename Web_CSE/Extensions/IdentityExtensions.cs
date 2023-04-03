﻿using System.Security.Claims;
using System.Linq;
using System.Security.Principal;

namespace Web_CSE.Extensions
{
    public static class IdentityExtensions
    {
        public static string GetAccountID(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("AccountId");
             return (claim != null) ? claim.Value : string.Empty;
        }   
           
        public static string GetRoleID(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("Roleld");
            return (claim != null) ? claim.Value : string.Empty;
        }
           
        public static string GetCredits(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("VipCredits");
            return (claim != null) ? claim.Value : string.Empty;
        }
        public static string GetAvatar(this IIdentity identity)
         {
            var claim = ((ClaimsIdentity)identity).FindFirst("Avatar");
            return (claim != null) ? claim.Value : string.Empty;
        }
        public static string GetSpecificClain(this ClaimsPrincipal claimsPrincipal,string claimType) 
        { 
            var claim = claimsPrincipal.Claims.FirstOrDefault(x=>x.Type == claimType);
            return (claim !=null )? claim.Value : string.Empty;
        }
    }
}
