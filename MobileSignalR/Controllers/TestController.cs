using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MobileSignalR.Auth;

namespace MobileSignalR.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<string>> GenerateToken()
        {
            var expiry = DateTime.Now.AddMinutes(5);
            
            var tokenHandler = new JwtSecurityTokenHandler();
            var identity = new ClaimsIdentity([
                new Claim(ClaimTypes.Name, Guid.NewGuid().ToString())
                // ... other claims
            ]);
            
            //string xml = await System.IO.File.ReadAllTextAsync("private.xml");
            var xml = "<RSAKeyValue><Modulus>uq+6W9nJslsaLMdfcuHTtxuFk7RPmvj4NKfXTVm5a0hckqlTi5EEEltlaFE33X/8V2dQIql8vOvRDJo4z+/zlBaBLyRo2oliRBZudrc1vQI9euvai0kL16FH8YY6dYnbah9RhfdpNkhQfd4bZksfgvEOYSbxw/28K64j6i/tJ9U=</Modulus><Exponent>AQAB</Exponent><P>3UKsCIN8Kg+lQTtiBh+ExZddY92WiiP8XwlVaVLcC2EhKBktfdVFgdZKakEugWj+KebybDrRp1jqTVGzcUPNGw==</P><Q>1/9kCsFO1aNG0rOkrY/OOnhBH6Shww4xFzWxd00enQTXQ7CL5GvVobXJDbKsAp0dt8EkDHLbOPEHzYTdCL7dzw==</Q><DP>NM5Jsop24q7zOLtMbLuu+11hq4jh+bwW6jOXD9j3rTuUJzbDFaoFubQD9JHz4GzHZAa7SrtK+A6PdL6P/fM5iw==</DP><DQ>rDTHc/OugJFOc8oZru6KAv/BHBNLjJGR/ekm9fCcSZ+EaEknHxQCHI0sICmlDehpuwjXTr17nig8ilQ1TTWu7Q==</DQ><InverseQ>Y7KECd58t6Aef2JxkZTMd2EFhmiJdFHh3u4i/XPR8yG+zbO8liIqY/YFpy6ev1JnnG1dSAjfYsuRQPnR7vwg4g==</InverseQ><D>C8sGDr9XSnkO0j1V/j/dy/dlHMuLK9MGeu0PYMeGOwy7LFid+ncStsYnRcu7p7ZqDmtsWIQ0aQrMjetAI4KY9GpIPoIqsQCVnw2pVz4hw5iXpRDCY5udSG6nhYsokxKjBKe+sGHLwsszzTTe0qmlJSmH4LfZfUxS5ugzPrOLOJE=</D></RSAKeyValue>";
            SecurityKey key = KeyHelper.BuildRsaSigningKey(xml);
            var test = JsonWebKeyConverter.ConvertFromRSASecurityKey(new RsaSecurityKey((key as RsaSecurityKey)!.Rsa.ExportParameters(false)));
            var ee = JsonSerializer.Serialize(test);
            ;
            var token = new JwtSecurityToken
            (
                issuer: JwtOptions.Issuer,
                audience: JwtOptions.Audience,
                claims: identity.Claims,
                notBefore: DateTime.UtcNow,
                expires: expiry,
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.RsaSha256, SecurityAlgorithms.Sha256Digest)
            );
            var tokenString = tokenHandler.WriteToken(token);
            return tokenString;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult> AuthorizeTest()
        {
            return Ok();
        }
    }
}
