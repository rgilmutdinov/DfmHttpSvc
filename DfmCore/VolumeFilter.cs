namespace DfmCore
{
    public class VolumeFilter
    {
        public string Name          { get; set; }
        public string Query         { get; set; }
        public string FtsExpression { get; set; }
        public int    MaxDocs       { get; set; }
    }
}
