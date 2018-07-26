using System;
using DfmWeb.Core.Entities;

namespace DfmWeb.App.Sessions
{
    public class DownloadTicket
    {
        public DownloadTicket(string sessionId, string volumeName, Selection selection)
        {
            SessionId  = sessionId;
            VolumeName = volumeName;
            Selection  = selection ?? throw new ArgumentNullException(nameof(selection));

            Token = Guid.NewGuid().ToString();
        }

        public string SessionId  { get; }
        public string VolumeName { get; }

        public Selection Selection { get; }

        public string Token { get; }
    }
}
