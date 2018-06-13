using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using DfmCore.Collections;
using DfmCore.Extensions;
using DFMServer;

namespace DfmCore
{
    public class VolumeStruct : DisposableObject
    {
        private IVolumeStruct7 _volStructObj;

        internal VolumeStruct()
        {
            this._volStructObj = new VolumeStruct7();
        }

        internal VolumeStruct(IVolumeStruct7 volumeStruct)
        {
            this._volStructObj = volumeStruct ?? throw new ArgumentNullException();
        }

        public int GetFieldCount()
        {
            CheckDisposed();

            return this._volStructObj.FieldCount;
        }

        public List<FieldInfo> GetFields()
        {
            CheckDisposed();

            List<FieldInfo> fields = new List<FieldInfo>();
            for (int i = 0; i < GetFieldCount(); ++i)
            {
                fields.Add(GetFieldInfo(i));
            }

            return fields;
        }

        public List<string> GetUniqueKey()
        {
            CheckDisposed();

            if (this._volStructObj != null)
            {
                if (this._volStructObj.UniqueKey is string[] uniqueKey)
                {
                    return new List<string>(uniqueKey).Sanitize();
                }
            }

            return Lists.Empty<string>();
        }

        public FieldInfo GetFieldInfo(int fieldIndex)
        {
            FieldInfo7 dfmField = this._volStructObj.FieldInfo[fieldIndex] as FieldInfo7;
            if (dfmField == null)
            {
                throw new ApplicationException("Can't get FieldInfo.");
            }

            return new FieldInfo(dfmField);
        }

        protected override void DisposeUnmanagedResources()
        {
            if (this._volStructObj != null)
            {
                Marshal.FinalReleaseComObject(this._volStructObj);
                this._volStructObj = null;
            }
        }
    }
}
