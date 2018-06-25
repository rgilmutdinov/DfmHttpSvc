using System;

namespace DfmHttpSvc.Sessions
{
    public class DownloadTicket
    {
        public DownloadTicket(string sessionId, ulong documentId, string volumeName)
        {
            SessionId  = sessionId;
            DocumentId = documentId;
            VolumeName = volumeName;

            Token = Guid.NewGuid().ToString();
        }

        public string SessionId  { get; set; }
        public ulong  DocumentId { get; set; }
        public string VolumeName { get; set; }

        public string Token { get;  }
    }
}
