using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using DfmCore;
using DfmHttpCore;
using DfmHttpCore.Entities;
using Microsoft.Extensions.Configuration;

namespace DfmHttpSvc.Sessions
{
    public class SessionManager
    {
        private readonly Dictionary<string, Session> _sessions = new Dictionary<string, Session>();
        private readonly object _sessionsLock = new object();

        private readonly MemoryCache _downloadTickets = new MemoryCache("DownloadTickets");

        public SessionManager(IConfiguration configuration)
        {
            SessionTimeout = configuration.GetValue<int>("SessionTimeoutMinutes");
        }

        public int SessionTimeout { get; }

        public string CreateSession(Credential credential)
        {
            string  sessionId     = $"{Guid.NewGuid()}-{DateTime.Now:yyyy-MM-dd}";
            string  tempDirectory = Path.Combine(Path.GetTempPath(), sessionId);

            Directory.CreateDirectory(tempDirectory);

            Session session = Session.Open(credential, tempDirectory);

            session.Open();

            lock (this._sessionsLock)
            {
                this._sessions[sessionId] = session;
            }

            return sessionId;
        }

        public bool RemoveSession(string sessionId)
        {
            bool success = false;
            lock (this._sessionsLock)
            {
                if (this._sessions.TryGetValue(sessionId, out Session session))
                {
                    success = this._sessions.Remove(sessionId);
                    session.Close();
                }
            }

            return success;
        }

        public bool TryGetSession(string sessionId, out Session session)
        {
            lock (this._sessionsLock)
            {
                if (this._sessions.TryGetValue(sessionId, out session))
                {
                    // check if the session has expired
                    if (!session.IsExpired(SessionTimeout))
                    {
                        // session is alive
                        session.LastAccess = DateTime.Now; // update access time

                        return true;
                    }

                    // session has expired
                    this._sessions.Remove(sessionId);

                    session.Close();
                    session = null;
                }

                return false;
            }
        }

        public void RemoveExpiredSessions()
        {
            lock (this._sessionsLock)
            {
                this._sessions.Where(kv => kv.Value.IsExpired(SessionTimeout))
                    .ToList()
                    .ForEach(kv =>
                    {
                        this._sessions.Remove(kv.Key);
                        kv.Value.Close();
                    }
                );
            }
        }

        public DownloadTicket CreateDownloadTicket(string sessionId, string volumeName, Selection selection)
        {
            DownloadTicket ticket = new DownloadTicket(sessionId, volumeName, selection);

            this._downloadTickets.Add(ticket.Token, ticket,
                new CacheItemPolicy
                {
                    AbsoluteExpiration = DateTime.UtcNow.AddMinutes(1)
                }
            );

            return ticket;
        }

        public DownloadTicket GetDownloadTicket(string token)
        {
            return this._downloadTickets.Get(token) as DownloadTicket;
        }
    }
}
