using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using DfmServer.Managed.Extensions;
using DfmServer.Managed.Properties;
using DFMServer;

namespace DfmServer.Managed
{
    public class Dictionary : DisposableObject
    {
        private Dictionary7 _dictObj;
        private readonly object _dictLock;

        public Dictionary()
        {
            this._dictObj = new Dictionary7();
            this._dictLock = new object();
        }

        public void Login(Credential credential)
        {
            if (!credential.IsValid)
            {
                throw new Exception(Resources.ErrorIncompleteCredential);
            }

            lock (this._dictLock)
            {
                this._dictObj.Login(
                    credential.Username,
                    credential.Password,
                    credential.Datasource,
                    credential.IsWinAuth
                );
            }
        }

        public List<string> GetVolumes(string area = null)
        {
            CheckDisposed();

            lock (this._dictLock)
            {
                string[] volumes = this._dictObj.GetVolumes(area);
                return volumes.Sanitize();
            }
        }

        public VolumeInfo GetVolumeInfo(string volumeName)
        {
            CheckDisposed();

            lock (this._dictLock)
            {
                IVolumeInfo7 volInfo7 = this._dictObj.GetVolumeInfo(volumeName);

                return new VolumeInfo(volInfo7);
            }
        }

        public bool IsLoggedIn
        {
            get
            {
                CheckDisposed();
                lock (this._dictLock)
                {
                    return this._dictObj.IsLoggedIn;
                }
            }
        }

        public DictionaryInfo Info
        {
            get
            {
                lock (this._dictObj)
                {
                    return new DictionaryInfo(this._dictObj);
                }
            }
        }

        public void Logout()
        {
            CheckDisposed();
            lock (this._dictLock)
            {
                this._dictObj.Logout();
            }
        }

        public bool IsVolumeExist(string volumeName)
        {
            CheckDisposed();
            lock (this._dictLock)
            {
                return this._dictObj.VolumeExists(volumeName);
            }
        }

        public List<VolumeInfo> GetVolumeInfoList(string areaName = "")
        {
            object[] volObjects;
            lock (this._dictLock)
            {
                volObjects = this._dictObj.GetVolumesEx(areaName);
            }

            return volObjects
                .Cast<IVolumeInfo7>()
                .Select(iv => new VolumeInfo(iv))
                .ToList();
        }

        public Volume OpenVolume(string volumeName, string filterQuery = "", string sortOrder = "")
        {
            CheckDisposed();

            lock (this._dictLock)
            {
                return new Volume(this._dictObj, volumeName, filterQuery, sortOrder);
            }
        }

        protected override void DisposeUnmanagedResources()
        {
            lock (this._dictLock)
            {
                if (this._dictObj != null)
                {
                    Marshal.FinalReleaseComObject(this._dictObj);
                    this._dictObj = null;
                }
            }
        }

        public List<Area> GetAreas(string parentArea)
        {
            CheckDisposed();

            lock (this._dictLock)
            {
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
        }

        public List<string> GetVolumeFilters(string volumeName)
        {
            CheckDisposed();
            lock (this._dictLock)
            {
                string[] filters = this._dictObj.GetVolumeFilters(volumeName);
                return filters.Sanitize();
            }
        }

        public VolumeFilter GetVolumeFilter(string volumeName, string filterName)
        {
            string query, ftsExpression;
            int maxDocs;

            lock (this._dictLock)
            {
                string filterXml = this._dictObj.FilterXml[volumeName, filterName];

                this._dictObj.ConvertFilterXmlToQuery(volumeName, filterXml,
                    out query,
                    out ftsExpression,
                    out maxDocs);
            }

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
