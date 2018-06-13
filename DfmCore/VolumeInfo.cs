using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using DFMServer;

namespace DfmCore
{
    public class VolumeInfo : DisposableObject
    {
        private IVolumeInfo7 _viObj;
        private readonly Lazy<List<FieldInfo>> _fields;

        internal VolumeInfo(IVolumeInfo7 volumeInfo)
        {
            this._viObj = volumeInfo ?? throw new ArgumentNullException();
            this._fields = new Lazy<List<FieldInfo>>(InitFields);
        }

        public string Name
        {
            get
            {
                CheckDisposed();
                return this._viObj.Name;
            }
        }

        public string Description
        {
            get
            {
                CheckDisposed();
                return this._viObj.Description;
            }
        }

        public VolumeType Type
        {
            get
            {
                CheckDisposed();
                return (VolumeType) (int) this._viObj.Type;
            }
        }

        public DateTime CreationDate
        {
            get
            {
                CheckDisposed();
                return this._viObj.CreationDate;
            }
        }

        public bool IsVirtual
        {
            get
            {
                CheckDisposed();
                return DFM_VOLUME_TYPE.DFM_VT_VIRTUAL == this._viObj.Type;
            }
        }

        public bool IsExternal
        {
            get
            {
                CheckDisposed();
                return DFM_VOLUME_TYPE.DFM_VT_EXTERNAL == this._viObj.Type;
            }
        }

        public bool SupportsAddTime
        {
            get
            {
                CheckDisposed();
                return this._viObj.SupportsAddTime;
            }
        }

        public bool IsConserved
        {
            get
            {
                CheckDisposed();
                return this._viObj.IsConserved;
            }
        }

        public bool IsClosed
        {
            get
            {
                CheckDisposed();
                return this._viObj.IsClosed;
            }
        }

        public string ActiveVolumeName
        {
            get
            {
                CheckDisposed();
                return this._viObj.ActiveVolumeName;
            }
        }

        public List<FieldInfo> Fields => this._fields.Value;

        public List<string> GetUniqueKey()
        {
            List<string> uniqueKey = new List<string>();
            IVolumeStruct7 volStruct = this._viObj.Structure;

            if (volStruct != null)
            {
                using (VolumeStruct volumeStruct = new VolumeStruct(volStruct))
                {
                    uniqueKey.AddRange(volumeStruct.GetUniqueKey());
                }
            }

            return uniqueKey;
        }

        protected override void DisposeUnmanagedResources()
        {
            if (this._viObj != null)
            {
                Marshal.FinalReleaseComObject(this._viObj);
                this._viObj = null;
            }
        }

        private List<FieldInfo> InitFields()
        {
            List<FieldInfo> fields = new List<FieldInfo>();
            VolumeStruct7 volStruct = this._viObj.Structure;

            if (volStruct != null)
            {
                using (VolumeStruct volumeStruct = new VolumeStruct(volStruct))
                {
                    fields.AddRange(volumeStruct.GetFields());
                }
            }

            return fields;
        }
    }
}
