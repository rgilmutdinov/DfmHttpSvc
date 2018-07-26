using System;
using System.Collections.Generic;

namespace DfmWeb.Core.Entities
{
    public class Field
    {
        public string Name  { get; set; }
        public string Value { get; set; }
    }

    public class Document
    {
        public Document(string compositeId)
        {
            CompositeId = compositeId;
        }

        public string    CompositeId    { get; set; }
        public string    Extension      { get; set; }
        public bool      HasAttachments { get; set; }
        public int       Version        { get; set; }
        public DateTime  Timestamp      { get; set; }
        public DateTime? AddTime        { get; set; }

        public List<Field> Fields { get; } = new List<Field>();
    }
}
