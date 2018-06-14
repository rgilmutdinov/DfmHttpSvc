using DFMServer;

namespace DfmCore
{
    public class DictionaryInfo
    {
        public DictionaryInfo(Dictionary7 dictObj)
        {
            DSN       = dictObj.DSN;
            Version   = dictObj.Version;
            UserName  = dictObj.CurrentUser;
            UserGroup = dictObj.CurrentGroup;

            dictObj.GetUserAttributes(
                UserName,
                out var userFullName,
                out var description,
                out var isLegalResponsible);

            UserFullName = userFullName;
            Description = description;
            IsLegalResponsible = isLegalResponsible;
        }

        public string DSN                { get; }
        public string Version            { get; }
        public string Description        { get; }
        public string UserName           { get; }
        public string UserFullName       { get; }
        public string UserGroup          { get; }
        public bool   IsLegalResponsible { get; }
    }
}
