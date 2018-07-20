using System.IO;

namespace DfmCore.Tools
{
    public class TempDirectory : DisposableObject
    {
        public TempDirectory(string parentDirectory = null)
        {
            Location = PathUtility.GetRandomDirectory(parentDirectory);
            Directory.CreateDirectory(Location);
        }

        public string Location { get; }

        protected override void DisposeUnmanagedResources()
        {
            if (Directory.Exists(Location))
            {
                Directory.Delete(Location, true);
            }
        }
    }
}
