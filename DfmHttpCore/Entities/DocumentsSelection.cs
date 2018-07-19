using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using DfmCore;
using DfmCore.Collections;
using DfmCore.Extensions;
using DfmCore.Tools;

namespace DfmHttpCore.Entities
{
    public class DocumentsSelection : Selection
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

        public List<ulong> DocumentIds { get; }

        public override bool IsValid()
        {
            if (!ExcludeMode)
            {
                return DocumentIds.Count > 0;
            }

            return true;
        }

        public override void Delete(Session session, string volumeName)
        {
            using (Volume volume = session.Dictionary.OpenVolume(volumeName, GetFilterQuery()))
            {
                volume.DeleteAllDocuments();
                volume.Reopen();
            }
        }

        public override string GetSelectionFile(Session session, string volumeName)
        {
            if (DocumentIds.Count == 1)
            {
                // download single file
                DocIdentity identity = new DocIdentity(DocumentIds.First());

                return session.ExtractDocument(volumeName, identity);
            }

            return ExtractToArchive(session, volumeName);
        }

        private string ExtractToArchive(Session session, string volumeName)
        {
            using (Volume volume = session.Dictionary.OpenVolume(volumeName, GetFilterQuery()))
            {
                string archiveFile = RandomPath.GetFile("zip");
                using (TempDirectory tempFolder = new TempDirectory())
                {
                    volume.ExtractDocumentsToFolder(tempFolder.Location);
                    ZipFile.CreateFromDirectory(tempFolder.Location, archiveFile);

                    return archiveFile;
                }
            }
        }

        private string GetFilterQuery()
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
    }
}
