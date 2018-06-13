namespace DfmCore
{
    public class Credential
    {
        public Credential(string username, string password, string datasource, bool isWinAuth)
        {
            Username   = username;
            Password   = password;
            Datasource = datasource;
            IsWinAuth  = isWinAuth;
        }

        public string Username   { get; }
        public string Password   { get; }
        public string Datasource { get; }
        public bool   IsWinAuth  { get; }

        public bool IsValid
        {
            get
            {
                if (IsWinAuth)
                {
                    return !string.IsNullOrWhiteSpace(Datasource);
                }

                return !string.IsNullOrWhiteSpace(Username) &&
                       !string.IsNullOrWhiteSpace(Password) &&
                       !string.IsNullOrWhiteSpace(Datasource);
            }
        }
    }
}
