using System;
using DfmCore;

namespace DfmHttpSvc.Dto
{
    public class VolumeSpecDto
    {
        public VolumeSpecDto()
        {

        }

        public VolumeSpecDto(VolumeInfo volInfo)
        {
            VolumeType  = (int) volInfo.Type;
            Name        = volInfo.Name;
            Description = volInfo.Description;
            Created     = volInfo.CreationDate;
            IsClosed    = volInfo.IsClosed;
            IsConserved = volInfo.IsConserved;
        }

        public string   Name        { get; set; }
        public string   Description { get; set; }
        public DateTime Created     { get; set; }
        public int      VolumeType  { get; set; }
        public bool     IsConserved { get; set; }
        public bool     IsClosed    { get; set; }
    }
}
