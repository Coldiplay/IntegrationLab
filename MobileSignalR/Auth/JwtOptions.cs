using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace MobileSignalR.Auth;

internal static class JwtOptions
{
    internal const  string Issuer = "Issuer";
    internal const string Audience = "Audience";
    private const string Key = "lfdsnglabcduhnisoudgnudsemhoiswjdsdgnsldl";
    internal static SymmetricSecurityKey SymSecurityKey => new(Encoding.UTF8.GetBytes(Key));
}