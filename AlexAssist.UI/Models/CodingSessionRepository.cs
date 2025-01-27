using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace AlexAssist.UI.Models
{
    public class CodingSessionRepository : IDisposable
    {
        private readonly AppDbContext _context;

        public CodingSessionRepository()
        {
            _context = new AppDbContext();
        }

        public List<CodingSession> GetAllSessions()
        {
            try
            {
                return _context.CodingSessions
                    .OrderByDescending(s => s.StartTime)
                    .ToList();
            }
            catch
            {
                return new List<CodingSession>();
            }
        }

        public CodingSession? GetActiveSession()
        {
            try
            {
                return _context.CodingSessions
                    .FirstOrDefault(s => !s.EndTime.HasValue);
            }
            catch
            {
                return null;
            }
        }

        public void StartSession(CodingSession session)
        {
            try
            {
                _context.CodingSessions.Add(session);
                _context.SaveChanges();
            }
            catch
            {
                // Log error or handle appropriately
            }
        }

        public void EndSession(CodingSession session)
        {
            try
            {
                var existing = _context.CodingSessions.Find(session.Id);
                if (existing != null)
                {
                    existing.EndTime = DateTime.Now;
                    _context.SaveChanges();
                }
            }
            catch
            {
                // Log error or handle appropriately
            }
        }

        public TimeSpan GetTotalTimeToday()
        {
            try
            {
                var today = DateTime.Today;
                var sessions = _context.CodingSessions
                    .Where(s => s.StartTime.Date == today)
                    .ToList();

                return sessions.Aggregate(TimeSpan.Zero, 
                    (total, session) => total + ((session.EndTime ?? DateTime.Now) - session.StartTime));
            }
            catch
            {
                return TimeSpan.Zero;
            }
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
} 