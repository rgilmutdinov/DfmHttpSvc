using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using DfmServer.Managed;
using DfmServer.Managed.Collections;
using DfmServer.Managed.Tools;

namespace DfmWeb.Core.Entities
{
    public class AttachmentsSelection : Selection
    {
        public AttachmentsSelection(ulong documentId) : this(documentId, new List<string>())
        {
        }

        public AttachmentsSelection(ulong documentId, string attachmentName) : this(documentId, Lists.Of(attachmentName))
        {
        }

        public AttachmentsSelection(ulong documentId, List<string> attachmentsNames, bool excludeMode = false)
        {
            DocumentId = documentId;
            Attachments = attachmentsNames ?? throw new ArgumentNullException(nameof(attachmentsNames));
            ExcludeMode = excludeMode;
        }

        public ulong DocumentId { get; set; }

        public List<string> Attachments { get; }

        public override bool IsValid()
        {
            if (!ExcludeMode)
            {
                return Attachments.Count > 0;
            }

            return true;
        }

        public override void Delete(Session session, string volumeName)
        {
            DocIdentity identity = new DocIdentity(DocumentId);

            using (Volume volume = session.Dictionary.OpenVolume(volumeName, identity.DocUidFilter))
            {
                List<string> deleteAttachments = ExcludeMode 
                    ? volume.GetAttachments().Except(Attachments).ToList() 
                    : new List<string>(Attachments);

                deleteAttachments.ForEach(attachmentName => volume.DeleteAttachment(attachmentName));
            }
        }

        public override string GetFile(Session session, string volumeName)
        {
            DocIdentity identity = new DocIdentity(DocumentId);
            if (Attachments.Count == 1 && !ExcludeMode && !ArchiveIfSingle)
            {
                return session.ExtractAttachment(volumeName, identity, Attachments.First());
            }

            return ExtractAttachmentsToArchive(session, identity, volumeName);
        }

        private string ExtractAttachmentsToArchive(Session session, DocIdentity identity, string volumeName)
        {
            using (Volume volume = session.Dictionary.OpenVolume(volumeName, identity.DocUidFilter))
            {
                List<string> extractAttachments = ExcludeMode
                    ? volume.GetAttachments().Except(Attachments).ToList()
                    : new List<string>(Attachments);

                string archiveFile = PathUtility.GetRandomFile("zip");
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
