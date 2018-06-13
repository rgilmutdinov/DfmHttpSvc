using System.Collections.Generic;
using System.Linq;
using DfmCore;
using DfmHttpCore;
using DfmHttpCore.Entities;
using DfmHttpSvc.Attributes;
using DfmHttpSvc.Controllers.Base;
using DfmHttpSvc.Dto;
using DfmHttpSvc.Sessions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DfmHttpSvc.Controllers
{
    [Route("api/[controller]")]
    public class VolumesController : ApiController
    {
        public VolumesController(SessionManager sessionManager) : base(sessionManager)
        {
        }

        /// <summary>
        /// Retrieves a list of volumes' specs in the indicated area.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     {
        ///     }
        ///
        /// </remarks>
        /// <returns>The list of volume's specs</returns>
        /// <response code="200">The list of volumes' specs</response>
        /// <response code="401">Unauthorized access</response>
        /// <response code="500">Internal server error</response>
        [ProducesResponseType(typeof(List<VolumeSpecDto>), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        [ArrayInput("area", typeof(string), AreaItem.PathSeparator)]
        [Authorize]
        [HttpGet]
        public IActionResult GetAreaVolumes([FromQuery] List<string> area)
        {
            if (!TryGetSession(User, out Session session))
            {
                return Unauthorized();
            }

            IEnumerable<VolumeSpecDto> volumes = session
                    .GetVolumeInfoList(new Area(area).Name)
                    .Select(volInfo => new VolumeSpecDto(volInfo));

            return Ok(volumes);
        }

        /// <summary>
        /// Retrieves a volume info by name.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     {
        ///        "volumeName": "myVolume"
        ///     }
        ///
        /// </remarks>
        /// <param name="volumeName">The name of a volume</param>
        /// <returns>The volume info</returns>
        /// <response code="200">Return the volume info</response>
        /// <response code="401">Unauthorized access</response>
        /// <response code="500">Internal server error</response>
        [ProducesResponseType(typeof(VolumeInfoDto), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        [Authorize]
        [HttpGet("{volumeName}")]
        public IActionResult GetInfo(string volumeName)
        {
            if (!TryGetSession(User, out Session session))
            {
                return Unauthorized();
            }

            VolumeInfo vInfo = session.Dictionary.GetVolumeInfo(volumeName);

            return Ok(new VolumeInfoDto(vInfo));
        }
    }
}
