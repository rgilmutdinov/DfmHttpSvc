using Microsoft.AspNetCore.Mvc;

namespace DfmHttpSvc.Dto
{
    /// <summary>
    /// Volume query
    /// </summary>
    public class VolumeQuery
    {
        /// <summary>
        /// Volume name
        /// </summary>
        [FromRoute(Name = "volume")]
        public string VolumeName { get; set; }

        /// <summary>
        /// Volume filter
        /// </summary>
        [FromQuery(Name = "filter")]
        public string FilterQuery { get; set; } = "";

        /// <summary>
        /// Sort order
        /// </summary>
        [FromQuery(Name = "sort")]
        public string SortOrder { get; set; } = "";

        /// <summary>
        /// Search expression
        /// </summary>
        [FromQuery(Name = "search")]
        public string Search { get; set; } = "";
    }
}
