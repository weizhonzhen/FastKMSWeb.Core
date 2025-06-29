using FastKMSApi.Core.Model;
using FastKMSApi.Core.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FastKMSApi.Core.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class checkController : ControllerBase
    {
        [HttpPost]
        public string login([FromBody] JwtCheck model)
        {
            var info = AppSetting.Jwt;
            var claims = new List<Claim>();

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(info.key));
            var sign = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var jwtToken = new JwtSecurityToken(info.Issuer, info.Audience, claims, DateTime.Now, DateTime.Now.AddSeconds(info.Expires), sign);
            var token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
            AppSetting.User.TryAdd($"Bearer {token}", model);
            return token;
        }

        [HttpPost]
        public void loginOut()
        {
            if (this.Request.Headers.Authorization.Count > 0)
            {
                var token = this.Request.Headers.Authorization[0] ?? string.Empty;
                AppSetting.User.TryRemove(token, out _);
            }
        }
    }
}