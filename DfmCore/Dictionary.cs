using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using DfmCore.Extensions;
using DfmCore.Properties;
using DFMServer;

namespace DfmCore
{
    public class Dictionary : DisposableObject
    {
        private Dictionary7 _dictObj;

        public Dictionary()
        {
            this._dictObj = new Dictionary7();
        }

        public void Login(Credential credential)
        {
            if (!credential.IsValid)
            {
                throw new Exception(Resources.ErrorIncompleteCredential);
            }

            this._dictObj.Login(
                credential.Username,
                credential.Password,
                credential.Datasource,
                credential.IsWinAuth
            );
        }

        public List<string> GetVolumes(string area = null)
        {
            CheckDisposed();

            string[] volumes = this._dictObj.GetVolumes(area);
            return volumes.Sanitize();
        }

        public VolumeInfo GetVolumeInfo(string volumeName)
        {
            CheckDisposed();

            IVolumeInfo7 volInfo7 = this._dictObj.GetVolumeInfo(volumeName);

            return new VolumeInfo(volInfo7);
        }

        public bool IsLoggedIn
        {
            get
            {
                CheckDisposed();

                return this._dictObj.IsLoggedIn;
            }
        }

        public void Logout()
        {
            CheckDisposed();

            this._dictObj.Logout();
        }

        public bool IsVolumeExist(string volumeName)
        {
            CheckDisposed();

            return this._dictObj.VolumeExists(volumeName);
        }

        public List<VolumeInfo> GetVolumeInfoList(string areaName = "")
        {
            object[] volObjects = this._dictObj.GetVolumesEx(areaName);

            return volObjects
                .Cast<IVolumeInfo7>()
                .Select(iv => new VolumeInfo(iv))
                .ToList();
        }

        public Volume OpenVolume(string volumeName, string filterQuery = "", string sortOrder = "")
        {
            CheckDisposed();

            return new Volume(this._dictObj, volumeName, filterQuery, sortOrder);
        }

        protected override void DisposeUnmanagedResources()
        {
            if (this._dictObj != null)
            {
                Marshal.FinalReleaseComObject(this._dictObj);
                this._dictObj = null;
            }
        }

        public List<Area> GetAreas(string parentArea)
        {
            CheckDisposed();

            string[] areas = this._dictObj.GetAreas(parentArea);
            return areas
                .Sanitize()
                .Select(name => new Area
                {
                    Name = name,
                    Description = this._dictObj.GetAreaDescription(name)
                })
                .ToList();
        }

        public List<string> GetVolumeFilters(string volumeName)
        {
            CheckDisposed();

            string[] filters = this._dictObj.GetVolumeFilters(volumeName);
            return filters.Sanitize();
        }

        public VolumeFilter GetVolumeFilter(string volumeName, string filterName)
        {
            string filterXml = this._dictObj.FilterXml[volumeName, filterName];

            this._dictObj.ConvertFilterXmlToQuery(volumeName, filterXml,
                out string query,
                out string ftsExpression,
                out int maxDocs);

            return new VolumeFilter
            {
                Name          = filterName,
                Query         = query,
                FtsExpression = ftsExpression,
                MaxDocs       = maxDocs
            };
        }
    }
}
