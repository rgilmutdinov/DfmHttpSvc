namespace DfmWeb.App.Dto
{
    public class Login
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Datasource { get; set; }

        public bool IsValid => !string.IsNullOrWhiteSpace(Username) &&
                               !string.IsNullOrWhiteSpace(Password) &&
                               !string.IsNullOrWhiteSpace(Datasource);
    }
}
