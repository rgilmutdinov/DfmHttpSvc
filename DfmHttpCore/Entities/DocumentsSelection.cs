using System.Collections.Generic;
using System.Linq;
using DfmCore;

namespace DfmHttpCore.Entities
{
    public class DocumentsSelection
    {
        public DocumentsSelection()
        {
            ExcludeMode = false;
            DocumentIds = new List<ulong>();
        }

        public bool        ExcludeMode { get; set; }
        public List<ulong> DocumentIds { get; set; }

        public string GetFilterQuery()
        {
            if (this.DocumentIds.Count == 0 && !ExcludeMode)
            {
                // select nothing
                return $"{ServiceFields.DocumentUid} <> {ServiceFields.DocumentUid}";
            }

            IEnumerable<string> uidFilters = DocumentIds.Select(id => new DocIdentity(id).DocUidFilter);
            string filter = string.Join(" OR ", uidFilters);

            if (ExcludeMode)
            {
                filter = $"NOT ({filter})";
            }

            return filter;
        }
    }
}
