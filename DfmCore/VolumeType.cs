using DFMServer;

namespace DfmCore
{
    public enum VolumeType : int
    {
        Unknown     = DFM_VOLUME_TYPE.DFM_VT_UNKNOWN,
        Native      = DFM_VOLUME_TYPE.DFM_VT_NATIVE,
        Multivolume = DFM_VOLUME_TYPE.DFM_VT_MULTIVOLUME,
        Dfm5        = DFM_VOLUME_TYPE.DFM_VT_DFM5,
        External    = DFM_VOLUME_TYPE.DFM_VT_EXTERNAL,
        Virtual     = DFM_VOLUME_TYPE.DFM_VT_VIRTUAL
    }
}