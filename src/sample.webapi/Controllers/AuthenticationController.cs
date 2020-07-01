using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sample.Exceptions;
using Sample.Extensions.Interfaces;
using Sample.Extensions.Models;

namespace Sample.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private const string AuthenticationNotEnabled = "Authentication is not enabled!";
        private const string UserAuthenticated = "You are authenticated";
        private const string BadRequestMissingBody = "Missing body!";
        private const string StateFieldDoesntMatch = "State field doesn't match!";
        private const string StateField = "state";
        private readonly IAuthenticationConfiguration authenticationConfiguration;
        private static string state;
        private static string nonce;

        public AuthenticationController(IAuthenticationConfiguration authenticationConfiguration)
        {
            this.authenticationConfiguration = Guard.ThrowIfNull(authenticationConfiguration, nameof(authenticationConfiguration));

            state = "12345";
            nonce = "6789";
        }

        [HttpGet("/claims")]
        public ActionResult GetClaims()
        {
            if (!authenticationConfiguration.Enabled)
            {
                return Ok(AuthenticationNotEnabled);
            }

            var list = User.Claims.Select(c => new { type = c.Type, value = c.Value });

            return Ok(list);
        }

        [AllowAnonymous]
        [HttpGet("/login")]
        public ActionResult GetLogin()
        {
            if (!authenticationConfiguration.Enabled)
            {
                return Ok(AuthenticationNotEnabled);
            }

            if (!User.Identity.IsAuthenticated)
            {
                return Redirect(authenticationConfiguration.GetAuthorizationUri(state, nonce).ToString());
            }

            return Ok(UserAuthenticated);
        }

        [AllowAnonymous]
        [HttpPost("/login")]
        public async Task<ActionResult> PostLoginAsync()
        {
            if (!authenticationConfiguration.Enabled)
            {
                return Ok(AuthenticationNotEnabled);
            }

            var requestBody = await GetRequestBodyAsync();

            if (string.IsNullOrWhiteSpace(requestBody))
            {
                return BadRequest(BadRequestMissingBody);
            }

            var token = new AzureAdResponseValues(requestBody);
            ActionResult result = Ok(token.ToJson());

            if (!token.GetValue(StateField).Equals(state, StringComparison.OrdinalIgnoreCase))
            {
                result = BadRequest(StateFieldDoesntMatch);
            }

            return result;
        }

        private async Task<string> GetRequestBodyAsync()
        {
            var requestBody = string.Empty;

            if (Request.Body != null)
            {
                using var sr = new StreamReader(Request.Body);
                requestBody = await sr.ReadToEndAsync();
            }

            return requestBody;
        }
    }
}
