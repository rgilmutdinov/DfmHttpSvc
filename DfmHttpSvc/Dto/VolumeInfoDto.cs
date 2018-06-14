using System.Collections.Generic;
using DfmCore;

namespace DfmHttpSvc.Dto
{
    public class VolumeInfoDto : VolumeSpecDto
    {
        public VolumeInfoDto()
        {

        }

        public VolumeInfoDto(VolumeInfo volInfo) : base(volInfo)
        {
            Fields = volInfo.Fields;
            UniqueKey = volInfo.GetUniqueKey();
        }

        public List<FieldInfo> Fields { get; set; }
        public List<string> UniqueKey { get; set; }
    }
}
