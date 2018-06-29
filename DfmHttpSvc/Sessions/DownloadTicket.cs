using System;
using DfmHttpCore.Entities;

namespace DfmHttpSvc.Sessions
{
    public class DownloadTicket
    {
        public DownloadTicket(string sessionId, string volumeName, DocumentsSelection selection)
        {
            SessionId  = sessionId;
            VolumeName = volumeName;
            Selection  = selection ?? throw new ArgumentNullException(nameof(selection));

            Token = Guid.NewGuid().ToString();
        }

        public string SessionId  { get; }
        public string VolumeName { get; }

        public DocumentsSelection Selection { get; }

        public string Token { get; }
    }
}
