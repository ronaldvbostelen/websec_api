using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Client_tech_resversi_api.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace Client_tech_resversi_api.Assets
{
    public class UserChangedAuthenticationEvents : CookieAuthenticationEvents
    {
        private readonly ReversiContext _context;

        public UserChangedAuthenticationEvents(ReversiContext context)
        {
            _context = context;
        }

        public override async Task ValidatePrincipal(CookieValidatePrincipalContext context)
        {
            var principal = context.Principal;

            if (!principal.IsInRole("User") && !principal.IsInRole("Helpdesk") && !principal.IsInRole("Admin") && !principal.IsInRole("SuperAdmin"))
            {
                context.RejectPrincipal();
                return;
            }
            
            var lastChanged = (from c in principal.Claims
                where c.Type == "LastChanged"
                select c.Value).FirstOrDefault();

            if (lastChanged == null)
            {
                context.RejectPrincipal();
            }

            var userId = (from claim in principal.Claims
                where claim.Type == "UserId"
                select claim.Value).FirstOrDefault();

            if (string.IsNullOrEmpty(userId))
            {
                context.RejectPrincipal();
            }

            var dbLastChanged = await _context.UserLastChanges
                .Where(x => x.UserId == int.Parse(userId))
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (dbLastChanged == null)
            {
                context.RejectPrincipal();
            }

            if (DateTime.Parse(dbLastChanged.DateTimeChanged) > DateTime.Parse(lastChanged))
            {
                var userAcc = await _context.UserAccounts.FirstOrDefaultAsync(x => x.UserId == int.Parse(userId));
                var userRoles = await _context.UserRoles.Where(x => x.UserId == int.Parse(userId)).Select(x => x.Role).ToListAsync();
                var currentRoles = (from claim in principal.Claims where claim.Type == ClaimTypes.Role select claim.Value).ToList();

                if (!userAcc.Verified || userAcc.Status != 'A' || !currentRoles.All(x => userRoles.Contains(x)))
                {
                    context.RejectPrincipal();
                }
            }
        }
    }
}
