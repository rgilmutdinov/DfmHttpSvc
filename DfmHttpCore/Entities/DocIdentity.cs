using DfmCore;

namespace DfmHttpCore.Entities
{
    public class DocIdentity
    {
        public DocIdentity(int docUid, int volMemberUid)
        {
            CompositeId = (ulong) volMemberUid << 32 | (uint) docUid;
        }

        public DocIdentity(ulong compositeId)
        {
            CompositeId = compositeId;
        }

        public ulong CompositeId { get; set; }

        internal int DocumentUid => (int) CompositeId << 32;
        internal int VolumeUid => (int) (CompositeId >> 32);

        public string DocUidFilter => $"{ServiceFields.DocumentUid}={DocumentUid} AND {ServiceFields.VolumeUid}={VolumeUid}";
    }
}
