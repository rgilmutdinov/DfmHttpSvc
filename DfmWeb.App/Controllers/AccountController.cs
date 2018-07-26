using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using DfmServer.Managed;
using DfmWeb.App.Controllers.Base;
using DfmWeb.App.Dto;
using DfmWeb.App.Properties;
using DfmWeb.App.Security;
using DfmWeb.App.Sessions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace DfmWeb.App.Controllers
{
    [Route("api/[controller]")]
    public class AccountController : ApiController
    {
        private readonly AuthOptions _authOptions;

        public AccountController(SessionManager sessionManager, AuthOptions authOptions)
            : base(sessionManager)
        {
            this._authOptions = authOptions;
        }

        /// <summary>
        ///  Authenticates using specified credential.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     {
        ///        "username": "sysdba",
        ///        "password": "1",
        ///        "datasource": "ws09"
        ///     }
        ///
        /// </remarks>
        /// <param name="login">Login data</param>
        /// <returns>A token</returns>
        /// <response code="200">Returns the newly-created token</response>
        /// <response code="400">Сredentials are null or invalid</response>
        [ProducesResponseType(typeof(AuthToken), 200)]
        [ProducesResponseType(400)]
        [HttpPost]
        [Route("login")]
        [Route("token")]
        [AllowAnonymous]
        public IActionResult Login([FromBody] Login login)
        {
            if (login == null || !login.IsValid)
            {
                return BadRequest(Resources.ErrorIncompleteLoginData);
            }

            Credential credential = new Credential(
                login.Username,
                login.Password,
                login.Datasource,
                false
            );

            // clean up expired sessions
            SessionManager.RemoveExpiredSessions();

            string sessionId = SessionManager.CreateSession(credential);

            return Ok(BuildToken(sessionId));
        }

        /// <summary>
        /// Logout user.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     {
        ///     }
        ///
        /// </remarks>
        /// <returns>Nothing</returns>
        /// <response code="204">Successful logout</response>
        /// <response code="404">The authentication session is missing or incorrect</response>
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [HttpPost]
        [Route("logout")]
        public IActionResult Logout()
        {
            string sessionId = GetSessionId(User);
            if (sessionId != null)
            {
                if (SessionManager.RemoveSession(sessionId))
                {
                    return NoContent();
                }
            }

            return NotFound();
        }

        /// <summary>
        /// Checks if user's session is alive
        /// </summary>
        /// <returns>Nothing</returns>
        /// <response code="200">User's session is alive</response>
        /// <response code="401">The authentication session is missing or incorrect</response>
        [ProducesResponseType(204)]
        [ProducesResponseType(401)]
        [HttpPost]
        [Authorize]
        [Route("ping")]
        public IActionResult Ping()
        {
            if (!TryGetSession(User, out _))
            {
                return Unauthorized();
            }

            return Ok();
        }

        private AuthToken BuildToken(string sessionId)
        {
            IEnumerable<Claim> claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sid, sessionId)
            };

            JwtSecurityToken token = new JwtSecurityToken(
                this._authOptions.Issuer,
                this._authOptions.Audience,
                claims,
                expires: DateTime.UtcNow.AddDays(this._authOptions.ExpirationDays),
                signingCredentials: new SigningCredentials(this._authOptions.SecurityKey, SecurityAlgorithms.HmacSha256)
            );

            return new AuthToken(token);
        }
    }
}
