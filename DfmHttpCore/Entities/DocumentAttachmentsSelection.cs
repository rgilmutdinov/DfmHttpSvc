using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using DfmCore;
using DfmCore.Collections;
using DfmCore.Tools;

namespace DfmHttpCore.Entities
{
    public class DocumentAttachmentsSelection : Selection
    {
        public DocumentAttachmentsSelection(ulong documentId) : this(documentId, new List<string>())
        {
        }

        public DocumentAttachmentsSelection(ulong documentId, string attachmentName) : this(documentId, Lists.Of(attachmentName))
        {
        }

        public DocumentAttachmentsSelection(ulong documentId, List<string> attachmentsNames, bool excludeMode = false)
        {
            DocumentId = documentId;
            AttachmentsNames = attachmentsNames ?? throw new ArgumentNullException(nameof(attachmentsNames));
            ExcludeMode = excludeMode;
        }

        public ulong DocumentId { get; set; }

        public List<string> AttachmentsNames { get; }

        public override bool IsValid()
        {
            if (!ExcludeMode)
            {
                return AttachmentsNames.Count > 0;
            }

            return true;
        }

        public override void Delete(Session session, string volumeName)
        {
            DocIdentity identity = new DocIdentity(DocumentId);

            using (Volume volume = session.Dictionary.OpenVolume(volumeName, identity.DocUidFilter))
            {
                List<string> deleteAttachments = ExcludeMode 
                    ? volume.GetAttachments().Except(AttachmentsNames).ToList() 
                    : new List<string>(AttachmentsNames);

                deleteAttachments.ForEach(attachmentName => volume.DeleteAttachment(attachmentName));
            }
        }

        public override string GetSelectionFile(Session session, string volumeName)
        {
            DocIdentity identity = new DocIdentity(DocumentId);
            if (AttachmentsNames.Count == 1)
            {
                return session.ExtractAttachment(volumeName, identity, AttachmentsNames.First());
            }

            return ExtractAttachmentsToArchive(session, identity, volumeName);
        }

        private string ExtractAttachmentsToArchive(Session session, DocIdentity identity, string volumeName)
        {
            using (Volume volume = session.Dictionary.OpenVolume(volumeName, identity.DocUidFilter))
            {
                List<string> extractAttachments = ExcludeMode
                    ? volume.GetAttachments().Except(AttachmentsNames).ToList()
                    : new List<string>(AttachmentsNames);

                string archiveFile = RandomPath.GetFile("zip");
                using (TempDirectory tempFolder = new TempDirectory())
                {
                    volume.ExtractAttachmentsToFolder(tempFolder.Location, extractAttachments);
                    ZipFile.CreateFromDirectory(tempFolder.Location, archiveFile);

                    return archiveFile;
                }
            }
        }
    }
}
