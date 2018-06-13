using System;

namespace DfmCore
{
    public class AttachmentInfo
    {
        public string   AttachmentName { get; set; }
        public string   AuthorUserName { get; set; }
        public string   Extension      { get; set; }
        public DateTime CreationDate   { get; set; }
        public int      SizeInBytes    { get; set; }
    }
}
