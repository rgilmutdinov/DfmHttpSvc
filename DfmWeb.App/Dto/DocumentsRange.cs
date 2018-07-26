using System.Collections.Generic;

namespace DfmWeb.App.Dto
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
