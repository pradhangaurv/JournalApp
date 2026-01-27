using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JouralAppWeb.Database;

namespace JouralAppWeb.Services
{
    public class JournalService
    {
        private readonly AppDbContext _db;

        public JournalService(AppDbContext db)
        {
            _db = db;
        }

        public Task<JournalEntry?> GetTodayEntryAsync(int userId)
            => _db.GetTodayEntryAsync(userId);

        public async Task SaveTodayEntryAsync(int userId, JournalEntry entry)
        {
            var todayEntry = await _db.GetTodayEntryAsync(userId);

            entry.UserId = userId; //  owner
            entry.EntryDate = DateTime.Today;

            if (todayEntry != null)
            {
                entry.Id = todayEntry.Id;
                entry.CreatedAt = todayEntry.CreatedAt;
                entry.UpdatedAt = DateTime.Now;
                await _db.UpdateAsync(entry);
            }
            else
            {
                entry.CreatedAt = DateTime.Now;
                entry.UpdatedAt = DateTime.Now;
                await _db.CreateAsync(entry);
            }
        }

        public Task<List<JournalEntry>> GetAllEntriesAsync(int userId)
            => _db.GetAllJournalEntriesAsync(userId);

        public Task<JournalEntry?> GetEntryByIdAsync(int userId, int id)
            => _db.GetJournalEntryByIdAsync(userId, id);

        public async Task UpdateEntryAsync(int userId, JournalEntry entry)
        {
            var existing = await _db.GetJournalEntryByIdAsync(userId, entry.Id);
            if (existing == null)
                throw new UnauthorizedAccessException("You cannot update another user's entry.");

            entry.UserId = userId;
            entry.CreatedAt = existing.CreatedAt;
            entry.UpdatedAt = DateTime.Now;

            await _db.UpdateAsync(entry);
        }

        public async Task DeleteEntryAsync(int userId, JournalEntry entry)
        {
            var existing = await _db.GetJournalEntryByIdAsync(userId, entry.Id);
            if (existing == null)
                throw new UnauthorizedAccessException("You cannot delete another user's entry.");

            await _db.DeleteAsync(entry);
        }
    }
}
