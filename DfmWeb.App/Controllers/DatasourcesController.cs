using System.Collections.Generic;
using DfmServer.Managed;
using DfmWeb.App.Properties;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DfmWeb.App.Controllers
{
    [Route("api/[controller]")]
    public class DatasourcesController : Controller
    {
        /// <summary>
        /// Retrieves the list of datasources available for the user.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     sysdba
        ///
        /// </remarks>
        /// <param name="username">user name</param>
        /// <returns>A token</returns>
        /// <response code="200">Return the list of datasources available for the user.</response>
        /// <response code="400">Username is null or whitespace</response>
        /// <response code="500">Exception message</response>
        [ProducesResponseType(typeof(List<string>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [AllowAnonymous]
        [HttpGet("{username}")]
        public IActionResult GetDatasources(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return BadRequest(Resources.ErrorEmptyUsername);
            }

            List<string> datasources = DatasourceProvider.GetUserDatasources(username);
            return Ok(datasources);
        }
    }
}
