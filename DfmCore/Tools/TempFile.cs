using System.IO;

namespace DfmCore.Tools
{
    public class TempFile : DisposableObject
    {
        public TempFile()
        {
            this.Location = Path.GetTempFileName();
        }

        public TempFile(string directory, string extension)
        {
            Location = RandomPath.GetFile(directory, extension);
        }

        public TempFile(string extension) : this(Path.GetTempPath(), extension)
        {
        }

        public string Location { get; }

        protected override void DisposeUnmanagedResources()
        {
            try
            {
                File.Delete(this.Location);
            }
            catch
            {
                // do nothing
            }
        }
    }
}
