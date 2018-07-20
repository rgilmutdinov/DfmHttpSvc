namespace DfmHttpCore.Entities
{
    public abstract class Selection
    {
        public bool ExcludeMode { get; set; }

        public virtual bool IsValid() => true;

        public abstract void Delete(Session session, string volumeName);
        public abstract string GetFile(Session session, string volumeName);
    }
}