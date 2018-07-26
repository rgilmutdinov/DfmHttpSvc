using System.Collections.Generic;

namespace DfmWeb.App.Dto
{
    public class AttachmentsRange
    {
        public AttachmentsRange()
        {
            Attachments = new List<string>();
        }

        public bool ExcludeMode { get; set; }
        public List<string> Attachments { get; }
    }
}
