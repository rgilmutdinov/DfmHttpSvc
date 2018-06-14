using System.Collections.Generic;
using DfmHttpCore;
using DfmHttpCore.Entities;
using DfmHttpSvc.Attributes;
using DfmHttpSvc.Controllers.Base;
using DfmHttpSvc.Sessions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DfmHttpSvc.Controllers
{
    [Route("api/[controller]")]
    public class AreasController : ApiController
    {
        public AreasController(SessionManager sessionManager) : base(sessionManager)
        {
        }

        /// <summary>
        /// Retrieves the list of available areas which are direct descendants (from the first nesting level) to the indicated parent area.
        /// </summary>
        /// <param name="area">Parent area path (ancestor areas in hierarchical order separated with comma)</param>
        /// <returns>The list of child areas</returns>
        /// <response code="200">Return the list child areas</response>
        /// <response code="401">Unauthorized access</response>
        [ProducesResponseType(typeof(List<AreaItem>), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        [ArrayInput("area", typeof(string), AreaItem.PathSeparator)]
        [Authorize]
        [HttpGet]
        public IActionResult GetAreaList([FromQuery] List<string> area)
        {
            if (!TryGetSession(User, out Session session))
            {
                return Unauthorized();
            }

            List<AreaItem> areas = session.GetAreaList(area);

            return Ok(areas);
        }
    }
}
