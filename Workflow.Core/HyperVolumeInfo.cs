namespace Workflow.Core
{
    public class HyperVolumeInfo
    {
        public string VirtualVolumeName { get; set; }
        public string SyncVolumeName { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Label { get; set; }
        public bool IsExclusive { get; set; }
    }
}
