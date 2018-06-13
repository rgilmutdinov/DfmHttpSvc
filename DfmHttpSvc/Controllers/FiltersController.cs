using System.Collections.Generic;
using DfmCore;
using DfmHttpCore;
using DfmHttpSvc.Controllers.Base;
using DfmHttpSvc.Properties;
using DfmHttpSvc.Sessions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DfmHttpSvc.Controllers
{
    [Route("api/volumes/{volume}/[controller]")]
    public class FiltersController : ApiController
    {
        public FiltersController(SessionManager sessionManager) : base(sessionManager)
        {
        }

        /// <summary>
        /// Retrieves the list of volume's filters.
        /// </summary>
        /// <returns>The list of volume's filters</returns>
        /// <response code="200">Return the list of volume's filters</response>
        /// <response code="401">Unauthorized access</response>
        /// <response code="404">Volume with requested name not found</response>
        /// <response code="500">Internal server error</response>
        [ProducesResponseType(typeof(List<VolumeFilter>), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [Authorize]
        [HttpGet]
        public IActionResult GetVolumeFilters(string volume)
        {
            if (!TryGetSession(User, out Session session))
            {
                return Unauthorized();
            }
            if (!session.IsVolumeExist(volume))
            {
                return NotFound(Resources.ErrorVolumeNotFound);
            }

            List<VolumeFilter> filters = session.GetVolumeFilters(volume);

            return Ok(filters);
        }
    }
}
