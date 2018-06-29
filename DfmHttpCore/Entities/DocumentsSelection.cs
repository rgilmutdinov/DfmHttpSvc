using System;
using System.Collections.Generic;
using System.Linq;
using DfmCore;
using DfmCore.Collections;
using DfmCore.Extensions;

namespace DfmHttpCore.Entities
{
    public class DocumentsSelection
    {
        public DocumentsSelection() : this(new List<ulong>())
        {
        }

        public DocumentsSelection(ulong documentId) : this(Lists.Of(documentId))
        {
        }

        public DocumentsSelection(List<ulong> documentIds, bool excludeMode = false)
        {
            DocumentIds = documentIds ?? throw new ArgumentNullException(nameof(documentIds));
            ExcludeMode = excludeMode;
        }

        public bool        ExcludeMode { get; set; }
        public List<ulong> DocumentIds { get; }

        public string GetFilterQuery()
        {
            if (this.DocumentIds.Count == 0 && !ExcludeMode)
            {
                // select nothing
                return $"{ServiceFields.DocumentUid} <> {ServiceFields.DocumentUid}";
            }

            IEnumerable<string> uidFilters = DocumentIds.Select(id => new DocIdentity(id).DocUidFilter);
            string filter = string.Join(" OR ", uidFilters);

            if (ExcludeMode && !filter.IsNullOrEmpty())
            {
                filter = $"NOT ({filter})";
            }

            return filter;
        }

        public bool IsValid()
        {
            if (!ExcludeMode)
            {
                return DocumentIds.Count > 0;
            }

            return true;
        }
    }
}
