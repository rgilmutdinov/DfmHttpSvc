using System.Collections.Generic;
using DfmCore;

namespace DfmHttpSvc.Dto
{
    public class VolumeInfoDto
    {
        public VolumeInfoDto()
        {

        }

        public VolumeInfoDto(VolumeInfo volInfo)
        {
            Name = volInfo.Name;
            IsVirtual = volInfo.IsVirtual;
            IsConserved = volInfo.IsConserved;
            IsClosed = volInfo.IsClosed;
            IsExternal = volInfo.IsExternal;

            Fields = volInfo.Fields;
            UniqueKey = volInfo.GetUniqueKey();
        }

        public string Name { get; set; }
        public bool IsVirtual { get; set; }
        public bool IsExternal { get; set; }
        public bool IsConserved { get; set; }
        public bool IsClosed { get; set; }

        public List<FieldInfo> Fields { get; set; }
        public List<string> UniqueKey { get; set; }
    }
}
