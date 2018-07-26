using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using DfmServer.Managed.Extensions;
using DFMServer;

namespace DfmServer.Managed
{
    public static class DatasourceProvider
    {
        public static List<string> GetUserDatasources(string username)
        {
            Dictionary7 dictionary = new Dictionary7();
            try
            {
                string[] datasources = dictionary.GetUserDSNs(username);
                return datasources.Sanitize().ToList();
            }
            finally
            {
                Marshal.ReleaseComObject(dictionary);
            }
        }
    }
}
