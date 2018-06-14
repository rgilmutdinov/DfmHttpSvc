using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using DfmCore.Extensions;
using DfmHttpCore;
using DfmHttpSvc.Sessions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DfmHttpSvc.Controllers.Base
{
    public abstract class ApiController : Controller
    {
        public SessionManager SessionManager { get; }

        public ApiController(SessionManager sessionManager)
        {
            SessionManager = sessionManager;
        }

        protected bool TryGetSession(ClaimsPrincipal principal, out Session session)
        {
            string sessionId = GetSessionId(principal);
            return TryGetSession(sessionId, out session);
        }

        protected bool TryGetSession(string sessionId, out Session session)
        {
            if (sessionId.IsNullOrEmpty())
            {
                session = default(Session);

                return false;
            }

            return this.SessionManager.TryGetSession(sessionId, out session);
        }

        protected string GetSessionId(ClaimsPrincipal principal)
        {
            return principal.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sid)?.Value;
        }

        public static string GetFilePath(Session session, IFormFile file)
        {
            string fileName = Path.GetFileNameWithoutExtension(file.FileName);
            if (fileName.IsNullOrEmpty())
            {
                fileName = Guid.NewGuid().ToString();
            }

            string extension = Path.GetExtension(file.FileName);
            if (extension.IsNullOrEmpty())
            {
                extension = ".tmp";
            }

            return Path.Combine(session.TempDirectory, fileName + extension);
        }
    }
}