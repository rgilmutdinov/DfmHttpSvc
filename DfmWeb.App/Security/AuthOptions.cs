﻿using Microsoft.IdentityModel.Tokens;

namespace DfmWeb.App.Security
{
    public class AuthOptions
    {
        public string      Issuer         { get; set; }
        public string      Audience       { get; set; }
        public SecurityKey SecurityKey    { get; set; }
        public int         ExpirationDays { get; set; }
    }
}
