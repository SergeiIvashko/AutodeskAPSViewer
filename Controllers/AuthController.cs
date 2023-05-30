using Microsoft.AspNetCore.Mvc;
using MyAutodeskAPS.Models;

namespace MyAutodeskAPS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        public record AccessToken(string access_token, long expires_in);
        private readonly Aps aps;

        public AuthController(Aps aps)
        {
            this.aps = aps;
        }

        [HttpGet("token")]
        public async Task<AccessToken> GetAccessToken()
        {
            var token = await this.aps.GetPublicToken();
            return new AccessToken(
                token.AccessToken,
                (long)Math.Round((token.ExpiresAt - DateTime.UtcNow).TotalSeconds)
                );
        }
    }
}
