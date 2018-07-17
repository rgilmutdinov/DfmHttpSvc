using System;

namespace DfmCore
{
    public class AttachmentInfo
    {
        public string   Name         { get; set; }
        public string   Author       { get; set; }
        public string   Extension    { get; set; }
        public DateTime CreationDate { get; set; }
        public int      SizeInBytes  { get; set; }
    }
}
