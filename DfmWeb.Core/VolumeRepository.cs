﻿using DfmServer.Managed;
using DfmServer.Managed.Extensions;
using DfmWeb.Core.Collections;
using DfmWeb.Core.Entities;

namespace DfmWeb.Core
{
    public class VolumeRepository
    {
        private readonly Dictionary _dictionary;

        private readonly LruCache<VolumeState, Volume> _volumesCache;
        private readonly object _cacheLock = new object();

        public VolumeRepository(Dictionary dictionary)
        {
            this._dictionary   = dictionary;
            this._volumesCache = new LruCache<VolumeState, Volume>(5);
        }

        public Volume OpenVolume(VolumeState volState)
        {
            Volume volume;
            lock (this._cacheLock)
            {
                if (this._volumesCache.TryGetValue(volState, out volume))
                {
                    volume.Reopen();
                    if (!volState.Search.IsNullOrEmpty())
                    {
                        volume.DoSearch(volState.Search, UnixDates.Min, UnixDates.Max);
                    }

                    return volume;
                }

                volume = this._dictionary.OpenVolume(
                    volState.VolumeName,
                    volState.FilterQuery,
                    volState.SortOrder
                );

                if (!volState.Search.IsNullOrEmpty())
                {
                    volume.DoSearch(volState.Search, UnixDates.Min, UnixDates.Max);
                }

                this._volumesCache.Set(volState, volume);
            }

            return volume;
        }
    }
}
