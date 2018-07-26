using System;
using System.IdentityModel.Tokens.Jwt;

namespace DfmWeb.App.Security
{
    public class AuthToken
    {
        private readonly JwtSecurityToken _token;

        internal AuthToken(JwtSecurityToken token)
        {
            this._token = token;
        }

        public long   ExpiresIn   => (long) (this._token.ValidTo - DateTime.Now).TotalSeconds;
        public string AccessToken => new JwtSecurityTokenHandler().WriteToken(this._token);
    }
}
