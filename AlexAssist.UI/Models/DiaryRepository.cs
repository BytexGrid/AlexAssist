using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace AlexAssist.UI.Models
{
    public class DiaryRepository : IDisposable
    {
        private readonly AppDbContext _context;

        public DiaryRepository()
        {
            _context = new AppDbContext();
        }

        public List<DiaryEntry> GetAllEntries()
        {
            try
            {
                return _context.DiaryEntries.OrderByDescending(e => e.CreatedAt).ToList();
            }
            catch
            {
                return new List<DiaryEntry>();
            }
        }

        public void AddEntry(DiaryEntry entry)
        {
            try
            {
                _context.DiaryEntries.Add(entry);
                _context.SaveChanges();
            }
            catch
            {
                // Log error or handle appropriately
            }
        }

        public void UpdateEntry(DiaryEntry entry)
        {
            try
            {
                var existing = _context.DiaryEntries.Find(entry.Id);
                if (existing != null)
                {
                    _context.Entry(existing).CurrentValues.SetValues(entry);
                    existing.CodeSnippets = entry.CodeSnippets;
                    existing.Tags = entry.Tags;
                    _context.SaveChanges();
                }
            }
            catch
            {
                // Log error or handle appropriately
            }
        }

        public void DeleteEntry(DiaryEntry entry)
        {
            try
            {
                _context.DiaryEntries.Remove(entry);
                _context.SaveChanges();
            }
            catch
            {
                // Log error or handle appropriately
            }
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
} 