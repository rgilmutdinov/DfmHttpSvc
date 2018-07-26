using System;
using DfmServer.Managed;

namespace DfmWeb.App.Dto
{
    public class VolumeSpecDto
    {
        public VolumeSpecDto()
        {

        }

        public VolumeSpecDto(VolumeInfo volInfo)
        {
            VolumeType      = (int) volInfo.Type;
            Name            = volInfo.Name;
            Description     = volInfo.Description;
            Created         = volInfo.CreationDate;
            IsClosed        = volInfo.IsClosed;
            IsConserved     = volInfo.IsConserved;
            IsVirtual       = volInfo.IsVirtual;
            IsExternal      = volInfo.IsExternal;
            SupportsAddTime = volInfo.SupportsAddTime;
        }

        public string   Name            { get; set; }
        public string   Description     { get; set; }
        public DateTime Created         { get; set; }
        public int      VolumeType      { get; set; }
        public bool     IsConserved     { get; set; }
        public bool     IsClosed        { get; set; }
        public bool     IsVirtual       { get; set; }
        public bool     IsExternal      { get; set; }
        public bool     SupportsAddTime { get; set; }
    }
}
