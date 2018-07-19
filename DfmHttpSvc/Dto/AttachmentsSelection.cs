using System.Collections.Generic;

namespace DfmHttpSvc.Dto
{
    public class AttachmentsSelection
    {
        public AttachmentsSelection()
        {
            AttachmentsNames = new List<string>();
        }

        public bool ExcludeMode { get; set; }
        public List<string> AttachmentsNames { get; }
    }
}
