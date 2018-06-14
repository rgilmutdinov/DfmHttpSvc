using DfmCore;
using DfmHttpCore;
using DfmHttpSvc.Controllers.Base;
using DfmHttpSvc.Sessions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DfmHttpSvc.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DictionaryController : ApiController
    {
        public DictionaryController(SessionManager sessionManager) : base(sessionManager)
        {

        }

        /// <summary>
        /// Retrieves information about current data dictionary
        /// </summary>
        /// <returns>A token</returns>
        /// <response code="200">Current data dictionary info</response>
        /// <response code="401">Unauthorized access</response>
        /// <response code="500">Internal server error</response>
        [ProducesResponseType(typeof(DictionaryInfo), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        [HttpGet]
        [Authorize]
        public IActionResult GetDictionaryInfo()
        {
            if (!TryGetSession(User, out Session session))
            {
                return Unauthorized();
            }

            return Ok(session.GetDictionaryInfo());
        }
    }
}