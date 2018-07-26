using System.Collections.Generic;
using System.Linq;
using DfmServer.Managed;
using DfmWeb.App.Attributes;
using DfmWeb.App.Controllers.Base;
using DfmWeb.App.Dto;
using DfmWeb.App.Sessions;
using DfmWeb.Core;
using DfmWeb.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DfmWeb.App.Controllers
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
                .Select(volInfo => {
                    using (volInfo) {
                        return new VolumeSpecDto(volInfo);
                    }
                });

            return Ok(volumes);
        }

        /// <summary>
        /// Retrieves a volume info by name.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     {
        ///        "volume": "myVolume"
        ///     }
        ///
        /// </remarks>
        /// <param name="volume">The name of a volume</param>
        /// <returns>The volume info</returns>
        /// <response code="200">Return the volume info</response>
        /// <response code="401">Unauthorized access</response>
        /// <response code="500">Internal server error</response>
        [ProducesResponseType(typeof(VolumeInfoDto), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        [Authorize]
        [HttpGet("{volume}")]
        public IActionResult GetInfo(string volume)
        {
            if (!TryGetSession(User, out Session session))
            {
                return Unauthorized();
            }

            using (VolumeInfo vInfo = session.Dictionary.GetVolumeInfo(volume))
            {
                return Ok(new VolumeInfoDto(vInfo));
            }
        }
    }
}
