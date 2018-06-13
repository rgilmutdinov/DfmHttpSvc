using System.Collections.Generic;
using System.Runtime.InteropServices;
using DFMServer;

namespace DfmCore
{
    public static class DatasourceProvider
    {
        public static List<string> GetUserDatasources(string username)
        {
            Dictionary7 dictionary = new Dictionary7();
            try
            {
                string[] datasources = dictionary.GetUserDSNs(username);
                return new List<string>(datasources);
            }
            finally
            {
                Marshal.ReleaseComObject(dictionary);
            }
        }
    }
}
