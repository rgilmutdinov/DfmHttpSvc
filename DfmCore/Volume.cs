using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using DfmCore.Extensions;
using DfmCore.Tools;
using DFMServer;

namespace DfmCore
{
    public class Volume : DisposableObject
    {
        private IVolume7 _volObj;
        private readonly Lazy<VolumeInfo> _volumeInfo;

        internal Volume(Dictionary7 dictionary, string volumeName, string filterQuery = "", string sortOrder = "")
        {
            Name        = volumeName;
            FilterQuery = filterQuery;
            SortOrder   = sortOrder;

            this._volObj = dictionary.OpenVolume(
                volumeName,
                filterQuery,
                sortOrder,
                "", // FTS expression
                out _
            );

            this._volumeInfo = new Lazy<VolumeInfo>(InitVolumeInfo);
        }

        public string Name        { get; private set; }
        public string FilterQuery { get; private set; }
        public string SortOrder   { get; private set; }

        public string this[int fieldIndex]
        {
            get => this._volObj.FieldValue[fieldIndex];
            set => this._volObj.FieldValue[fieldIndex] = value;
        }

        public string GetFieldValue(int fieldIndex)
        {
            return this[fieldIndex];
        }

        public int GetFieldIndex(string fieldName)
        {
            return Fields.FindIndex(f => f.Name == fieldName);
        }

        public string   DocumentExtension => this._volObj.DocumentExtension;
        public DateTime DocumentTimestamp => this._volObj.DocumentTimestamp;
        public bool     DocumentHasAttachments => this._volObj.DocumentContainsAttachments;
        public ulong    DocumentSize => this._volObj.DocumentSize;
        public int      VolumeId => this._volObj.VolumeMemberVolumeID;
        public int      VolumeMemberDocId => this._volObj.VolumeMemberDocId;

        public DateTime? DocumentAddTime
        {
            get
            {
                if (Info.SupportsAddTime)
                {
                    return this._volObj.DocumentAddTime;
                }

                return null;
            }
        }

        public int DocumentVersion
        {
            get
            {
                if (this._volObj.VersioningEnabled)
                {
                    return this._volObj.DocumentVersion;
                }

                return -1;
            }
        }

        public int DocumentCount => this._volObj.DocumentCount;

        public bool IsEOF => this._volObj.IsEOF;

        public void MoveNext()
        {
            if (!IsEOF)
            {
                this._volObj.MoveNext();
            }
        }

        public void MoveFirst()
        {
            this._volObj.MoveFirst();
        }

        public void Move(int position)
        {
            this._volObj.Move(position);
        }

        public void Reopen()
        {
            Reopen(this.FilterQuery, this.SortOrder);
        }

        public void Reopen(string filterQuery, string sortOrder)
        {
            this.FilterQuery = filterQuery;
            this.SortOrder   = sortOrder;

            this._volObj.Reopen(filterQuery, sortOrder, string.Empty, out int _);
        }

        public void ExtractDocumentToFile(string filePath)
        {
            this._volObj.ExtractDocumentToFile(filePath);
        }

        public void ExtractDocumentsToFolder(string directory)
        {
            if (this._volObj.DocumentCount > 0)
            {
                this._volObj.MoveFirst();
                while (!this._volObj.IsEOF)
                {
                    string tempFile = RandomPath.GetFile(directory, this._volObj.DocumentExtension);
                    this._volObj.ExtractDocumentToFile(tempFile);
                    this._volObj.MoveNext();
                }
            }
        }

        public void ExtractAttachmentsToFolder(string directory, List<string> attachmentsNames)
        {
            if (this._volObj.DocumentCount > 0)
            {
                this._volObj.MoveFirst();
                foreach (string name in attachmentsNames)
                {
                    this._volObj.GetAttachmentInfo(name, out string _, out string extension, out DateTime _, out int _);
                    string tempFile = RandomPath.GetFile(directory, extension);
                    this._volObj.ExtractAttachmentToFile(name, tempFile);
                }
            }
        }

        public void ExtractAttachmentToFile(string attachmentName, string filePath)
        {
            this._volObj.ExtractAttachmentToFile(attachmentName, filePath);
        }

        public int NewDocumentFromFile(string filePath, string fieldValues = "")
        {
            this._volObj.NewDocumentFromFile(filePath, fieldValues, true, out object docUidObj);

            return (int) docUidObj;
        }

        public void NewAttachmentFromFile(string filePath, string attachmentName)
        {
            this._volObj.NewAttachmentFromFile(filePath, attachmentName);
        }

        public void DeleteDocument(int docUid)
        {
            this._volObj.DeleteDocument(docUid);
        }

        public void DeleteAllDocuments()
        {
            if (this._volObj.DocumentCount > 0)
            {
                this._volObj.MoveFirst();
                while (!this._volObj.IsEOF)
                {
                    this._volObj.SelectDocument(true);
                    this._volObj.MoveNext();
                }
                this._volObj.AffectSelectedOnly = true;
                this._volObj.DeleteAllDocuments();
            }
        }

        public void DeleteCurrentDocument()
        {
            this._volObj.DeleteCurrentDocument();
        }

        public void DeleteAttachment(string name)
        {
            this._volObj.DeleteAttachment(name);
        }

        public List<FieldInfo> Fields => Info.Fields;

        public VolumeInfo Info => this._volumeInfo.Value;

        protected override void DisposeUnmanagedResources()
        {
            if (this._volObj != null)
            {
                Marshal.FinalReleaseComObject(this._volObj);
                this._volObj = null;
            }
        }

        public void UpdateFields()
        {
            this._volObj.UpdateFields();
        }

        public List<string> GetAttachments()
        {
            string[] attachments = this._volObj.GetAttachments();
            return attachments.Sanitize();
        }

        public AttachmentInfo GetAttachmentInfo(string attachmentName)
        {
            this._volObj.GetAttachmentInfo(
                attachmentName,
                out string authorUsername,
                out string extension,
                out DateTime creationDate,
                out int sizeInBytes);

            return new AttachmentInfo
            {
                Name = attachmentName,
                Author = authorUsername,
                Extension      = extension,
                CreationDate   = creationDate,
                SizeInBytes    = sizeInBytes
            };
        }

        private VolumeInfo InitVolumeInfo()
        {
            return new VolumeInfo(this._volObj.Info);
        }

        public void DoSearch(string searchExpression, DateTime dateFrom, DateTime dateTo)
        {
            this._volObj.DoSearch(searchExpression, dateFrom, dateTo, null, -1, false);
        }
    }
}
