using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace AlexAssist.UI.Models
{
    public class QuoteRepository : IDisposable
    {
        private readonly AppDbContext _context;

        public QuoteRepository()
        {
            _context = new AppDbContext();
            if (!_context.Quotes.Any())
            {
                SeedQuotes();
            }
        }

        private void SeedQuotes()
        {
            var quotes = new List<Quote>
            {
                // Success & Achievement
                new Quote { Text = "Success is not final, failure is not fatal: it is the courage to continue that counts.", Author = "Winston Churchill", Category = "Success" },
                new Quote { Text = "The only way to do great work is to love what you do.", Author = "Steve Jobs", Category = "Success" },
                new Quote { Text = "Don't watch the clock; do what it does. Keep going.", Author = "Sam Levenson", Category = "Success" },
                
                // Personal Growth
                new Quote { Text = "Life is 10% what happens to you and 90% how you react to it.", Author = "Charles R. Swindoll", Category = "Growth" },
                new Quote { Text = "The future belongs to those who believe in the beauty of their dreams.", Author = "Eleanor Roosevelt", Category = "Growth" },
                new Quote { Text = "What you get by achieving your goals is not as important as what you become by achieving your goals.", Author = "Zig Ziglar", Category = "Growth" },
                
                // Productivity
                new Quote { Text = "The way to get started is to quit talking and begin doing.", Author = "Walt Disney", Category = "Productivity" },
                new Quote { Text = "It always seems impossible until it's done.", Author = "Nelson Mandela", Category = "Productivity" },
                new Quote { Text = "Done is better than perfect.", Author = "Sheryl Sandberg", Category = "Productivity" },
                
                // Inspiration
                new Quote { Text = "The only limit to our realization of tomorrow will be our doubts of today.", Author = "Franklin D. Roosevelt", Category = "Inspiration" },
                new Quote { Text = "Everything you've ever wanted is on the other side of fear.", Author = "George Addair", Category = "Inspiration" },
                new Quote { Text = "Believe you can and you're halfway there.", Author = "Theodore Roosevelt", Category = "Inspiration" },
                
                // Focus & Determination
                new Quote { Text = "It does not matter how slowly you go as long as you do not stop.", Author = "Confucius", Category = "Focus" },
                new Quote { Text = "Quality is not an act, it is a habit.", Author = "Aristotle", Category = "Focus" },
                new Quote { Text = "The only person you are destined to become is the person you decide to be.", Author = "Ralph Waldo Emerson", Category = "Focus" }
            };

            _context.Quotes.AddRange(quotes);
            _context.SaveChanges();
        }

        public List<Quote> GetAllQuotes()
        {
            return _context.Quotes.ToList();
        }

        public List<Quote> GetQuotesByCategory(string category)
        {
            return _context.Quotes.Where(q => q.Category == category).ToList();
        }

        public Quote GetRandomQuote()
        {
            var count = _context.Quotes.Count();
            var random = new Random();
            var index = random.Next(0, count);
            return _context.Quotes.Skip(index).FirstOrDefault() ?? GetAllQuotes().First();
        }

        public void ToggleFavorite(int quoteId)
        {
            var quote = _context.Quotes.Find(quoteId);
            if (quote != null)
            {
                quote.IsFavorite = !quote.IsFavorite;
                _context.SaveChanges();
            }
        }

        public List<Quote> GetFavorites()
        {
            return _context.Quotes.Where(q => q.IsFavorite).ToList();
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
} 