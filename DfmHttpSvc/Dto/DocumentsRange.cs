using System.Collections.Generic;

namespace DfmHttpSvc.Dto
{
    public class DocumentsRange
    {
        public DocumentsRange()
        {
            DocumentIds = new List<ulong>();
        }

        public bool ExcludeMode { get; set; }
        public List<ulong> DocumentIds { get; }
    }
}
