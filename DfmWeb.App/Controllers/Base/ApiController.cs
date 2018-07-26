using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Security.Claims;
using System.Web;
using DfmServer.Managed.Extensions;
using DfmWeb.App.Sessions;
using DfmWeb.Core;
using DfmWeb.Core.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DfmWeb.App.Controllers.Base
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

        protected PhysicalFileResult GetSelection(Session session, string volume, Selection selection)
        {
            string filePath = selection.GetFile(session, volume);
            string contentType = MimeMapping.GetMimeMapping(filePath);

            ContentDisposition cd = new ContentDisposition
            {
                FileName = Path.GetFileName(filePath),
                Inline = true // true = browser to try to show the file inline; false = prompt the user for downloading
            };

            Response.Headers.Add("Content-Disposition", cd.ToString());

            return PhysicalFile(filePath, contentType);
        }
    }
}
