using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication.Internal;

namespace ACS.Client.Security;

//Kudos to https://stackoverflow.com/a/73778227/2804432
public class SplitRoleUserFactory : AccountClaimsPrincipalFactory<RemoteUserAccount>
{
    public SplitRoleUserFactory(IAccessTokenProviderAccessor accessor)
        : base(accessor)
    {
    }

    public override async ValueTask<ClaimsPrincipal> CreateUserAsync(
        RemoteUserAccount account,
        RemoteAuthenticationUserOptions options)
    {
        var user = await base.CreateUserAsync(account, options);
        var claimsIdentity = (ClaimsIdentity)user.Identity;

        if (account != null && claimsIdentity != null)
        {
            MapArrayClaimsToMultipleSeparateClaims(account, claimsIdentity);
        }

        return user;
    }

    private static void MapArrayClaimsToMultipleSeparateClaims(RemoteUserAccount account, ClaimsIdentity claimsIdentity)
    {
        foreach (var (key, value) in account.AdditionalProperties)
        {
            if (value is not JsonElement { ValueKind: JsonValueKind.Array } element)
            {
                continue;
            }

            // Remove the Roles claim with an array value and create a separate one for each role.
            claimsIdentity.RemoveClaim(claimsIdentity.FindFirst(key));
            var claims = element.EnumerateArray().Select(x => new Claim(key, x.ToString()));
            claimsIdentity.AddClaims(claims);
        }
    }
}