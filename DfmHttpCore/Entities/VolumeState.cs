using System;
using DfmHttpCore.Utils;

namespace DfmHttpCore.Entities
{
    public class VolumeState : IEquatable<VolumeState>
    {
        public VolumeState(string volumeName, string filterQuery, string sortOrder)
        {
            VolumeName  = volumeName;
            FilterQuery = filterQuery;
            SortOrder   = sortOrder;
        }

        public string VolumeName  { get; }
        public string FilterQuery { get; }
        public string SortOrder   { get; }

        public bool Equals(VolumeState other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return
                Strings.EqualsNoCase(VolumeName, other.VolumeName) &&
                string.Equals(FilterQuery, other.FilterQuery) &&
                Strings.EqualsNoCase(SortOrder, other.SortOrder);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return Equals((VolumeState) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = VolumeName != null ? VolumeName.ToUpper().GetHashCode() : 0;
                hashCode = (hashCode * 397) ^ (FilterQuery != null ? FilterQuery.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (SortOrder != null ? SortOrder.ToUpper().GetHashCode() : 0);

                return hashCode;
            }
        }
    }
}
