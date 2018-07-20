using DfmHttpCore;
using DfmHttpSvc.Attributes;
using DfmHttpSvc.Controllers.Base;
using DfmHttpSvc.Sessions;
using Microsoft.AspNetCore.Mvc;

namespace DfmHttpSvc.Controllers
{
    [Route("api/downloads")]
    public class DownloadController : ApiController
    {
        public DownloadController(SessionManager sessionManager) : base(sessionManager)
        {
        }

        /// <summary>
        /// Downloads file by temporary token
        /// </summary>
        /// <param name="token">temporary token</param>
        /// <returns>A file, associated with the temporary token</returns>
        /// <response code="200">A file</response>
        /// <response code="404">Token is not valid or expired</response>
        /// <response code="401">Unauthorized access</response>
        /// <response code="500">Internal server error</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        [HttpGet("{token}")]
        [DeleteFile]
        public IActionResult DownloadByToken(string token)
        {
            DownloadTicket ticket = SessionManager.GetDownloadTicket(token);
            if (ticket == null)
            {
                return NotFound("Token is not valid or expired.");
            }

            if (!TryGetSession(ticket.SessionId, out Session session))
            {
                return Unauthorized();
            }

            return GetSelection(session, ticket.VolumeName, ticket.Selection);
        }
    }
}
