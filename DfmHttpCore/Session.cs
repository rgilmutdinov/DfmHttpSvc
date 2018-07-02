using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using DfmCore;
using DfmCore.Tools;
using DfmHttpCore.Entities;

namespace DfmHttpCore
{
    public class Session : DisposableObject
    {
        public static Session Open(Credential credential, string tempDirectory)
        {
            Session session = new Session(credential, tempDirectory);

            session.Open();

            return session;
        }

        private VolumeRepository _volumeRepository;

        private Session(Credential credential, string tempDirectory)
        {
            Credential    = credential;
            TempDirectory = tempDirectory;
        }

        public Dictionary Dictionary { get; private set; }
        public Credential Credential { get; private set; }

        public string   TempDirectory { get; set; }
        public DateTime LastAccess    { get; set; } = DateTime.Now;

        public void Open()
        {
            Dictionary = new Dictionary();
            Dictionary.Login(this.Credential);

            this._volumeRepository = new VolumeRepository(Dictionary);
        }

        public void Close()
        {
            Dispose();

            if (Directory.Exists(TempDirectory))
            {
                Directory.Delete(TempDirectory, true);
            }
        }

        public bool IsOpen => Dictionary != null;

        public Volume OpenVolume(VolumeState state)
        {
            return this._volumeRepository.OpenVolume(state);
        }

        public bool IsVolumeExist(string volumeName)
        {
            return Dictionary.IsVolumeExist(volumeName);
        }

        public bool IsExpired(int lifetimeMinutes)
        {
            return TimeSpan.FromMinutes(lifetimeMinutes) < DateTime.Now - this.LastAccess;
        }

        public List<AreaItem> GetAreaList(List<string> parentAreaPath)
        {
            List<AreaItem> sessionAreas = new List<AreaItem>();

            List<string> unescapedPath = parentAreaPath
                .Select(Uri.UnescapeDataString)
                .ToList();

            Area parentArea = new Area(unescapedPath);

            IEnumerable<Area> areas = Dictionary
                .GetAreas(parentArea.Name)
                .OrderBy(a => a.ShortName);

            foreach (Area area in areas)
            {
                sessionAreas.Add(new AreaItem(area));
            }

            return sessionAreas;
        }

        public List<VolumeFilter> GetVolumeFilters(string volumeName)
        {
            List<string> filters = Dictionary.GetVolumeFilters(volumeName);

            return filters
                .Select(filterName => Dictionary.GetVolumeFilter(volumeName, filterName))
                .ToList();
        }

        public List<VolumeInfo> GetVolumeInfoList(string areaName = "")
        {
            return Dictionary.GetVolumeInfoList(areaName);
        }

        public DocIdentity NewDocumentFromFile(string volumeName, string filePath, List<Field> fields)
        {
            using (Volume volObj = Dictionary.OpenVolume(volumeName, string.Empty, string.Empty))
            {
                string fieldValues = CombineFields(fields);

                int docId = volObj.NewDocumentFromFile(filePath, fieldValues);
                return new DocIdentity(docId, volObj.VolumeId);
            }
        }

        public void NewAttachmentFromFile(string volumeName, DocIdentity docIdentity, string filePath, string attachmentName)
        {
            using (Volume volObj = Dictionary.OpenVolume(volumeName, docIdentity.DocUidFilter))
            {
                volObj.NewAttachmentFromFile(filePath, attachmentName);
            }
        }

        public string ExtractDocument(string volumeName, DocIdentity docIdentity)
        {
            using (Volume volume = Dictionary.OpenVolume(volumeName, docIdentity.DocUidFilter))
            {
                volume.MoveFirst();

                string extension = volume.DocumentExtension;
                string filePath = Path.Combine(TempDirectory, Guid.NewGuid() + "." + extension);

                volume.ExtractDocumentToFile(filePath);

                return filePath;
            }
        }

        public string ExtractDocumentsToArchive(string volumeName, DocumentsSelection selection)
        {
            using (Volume volume = Dictionary.OpenVolume(volumeName, selection.GetFilterQuery()))
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

        public void DeleteDocuments(string volumeName, DocumentsSelection selection)
        {
            using (Volume volume = Dictionary.OpenVolume(volumeName, selection.GetFilterQuery()))
            {
                volume.DeleteAllDocuments();
                volume.Reopen();
            }
        }

        public List<AttachmentInfo> GetAttachments(string volumeName, DocIdentity identity)
        {
            using (Volume volume = Dictionary.OpenVolume(volumeName, identity.DocUidFilter))
            {
                List<string> attachments = volume.GetAttachments();
                return attachments.Select(attName => volume.GetAttachmentInfo(attName)).ToList();
            }
        }

        public string ExtractAttachment(string volumeName, DocIdentity docIdentity, string attachmentName)
        {
            using (Volume volume = Dictionary.OpenVolume(volumeName, docIdentity.DocUidFilter))
            {
                volume.MoveFirst();

                AttachmentInfo attInfo = volume.GetAttachmentInfo(attachmentName);

                string extension = !string.IsNullOrEmpty(attInfo.Extension)
                    ? attInfo.Extension.ToLower()
                    : "tmp";

                string filePath = Path.Combine(TempDirectory, attachmentName + "." + extension);

                volume.ExtractAttachmentToFile(attachmentName, filePath);

                return filePath;
            }
        }

        public DocumentsResult GetDocuments(VolumeState volumeState, int start, int count)
        {
            if (start < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(start));
            }

            Volume volume = OpenVolume(volumeState);

            List<Document> documents = new List<Document>();
            if (volume.DocumentCount == 0)
            {
                return DocumentsResult.Empty;
            }

            int index = 0;
            volume.Move(start);
            while (index < count && !volume.IsEOF)
            {
                DocIdentity identity = new DocIdentity(volume.VolumeMemberDocId, volume.VolumeId);

                Document doc = GetDocument(volume, identity);

                documents.Add(doc);

                volume.MoveNext();
                index++;
            }

            return new DocumentsResult(documents, volume.DocumentCount);
        }

        public Document UpdateDocumentFields(string volumeName, DocIdentity docIdentity, List<Field> updateFields)
        {
            using (Volume volume = Dictionary.OpenVolume(volumeName, docIdentity.DocUidFilter))
            {
                foreach (Field updateField in updateFields)
                {
                    int fieldIndex = volume.GetFieldIndex(updateField.Name);
                    if (fieldIndex > 0)
                    {
                        volume[fieldIndex] = updateField.Value;
                    }
                }

                volume.UpdateFields();

                // reload volume
                volume.Reopen(docIdentity.DocUidFilter, string.Empty, string.Empty);

                return GetDocument(volume, docIdentity);
            }
        }

        public void DeleteDocument(string volumeName, DocIdentity docIdentity)
        {
            using (Volume volume = Dictionary.OpenVolume(volumeName, docIdentity.DocUidFilter))
            {
                volume.DeleteCurrentDocument();
            }
        }

        public bool IsDocumentExist(string volumeName, DocIdentity docIdentity)
        {
            using (Volume volume = Dictionary.OpenVolume(volumeName, docIdentity.DocUidFilter))
            {
                return volume.DocumentCount > 0;
            }
        }

        protected override void DisposeResources()
        {
            Dictionary?.Dispose();
        }

        private static Document GetDocument(Volume volume, DocIdentity identity)
        {
            Document doc = new Document(identity.CompositeId.ToString())
            {
                // read system fields
                Extension      = volume.DocumentExtension,
                Timestamp      = volume.DocumentTimestamp,
                HasAttachments = volume.DocumentHasAttachments,
                Version        = volume.DocumentVersion
            };

            int fieldIndex = 0;
            foreach (FieldInfo field in volume.Fields)
            {
                doc.Fields.Add(new Field
                {
                    Name = field.Name,
                    Value = volume[fieldIndex]
                });
                fieldIndex++;
            }

            return doc;
        }

        private static string CombineFields(List<Field> fields)
        {
            return String.Join(
                ",",
                fields.Select(f => $"{f.Name} = {f.Value}"));
        }

        public void DeleteAttachment(string volumeName, DocIdentity docIdentity, string attachmentName)
        {
            using (Volume volume = Dictionary.OpenVolume(volumeName, docIdentity.DocUidFilter))
            {
                volume.DeleteAttachment(attachmentName);
            }
        }

        public DictionaryInfo GetDictionaryInfo()
        {
            return Dictionary.Info;
        }
    }
}
