using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;
using SQLite;

namespace JouralAppWeb.Database
{
    public class AppDbContext
    {
        public SQLiteAsyncConnection _dbConnection = null!;
        private bool _initialized = false;
        private Task? _initTask;

        public const string DatabaseFileName = "JournalApp.db3";
        public static string DatabasePath => Path.Combine(FileSystem.AppDataDirectory, DatabaseFileName);

        public const SQLiteOpenFlags Flags =
            SQLiteOpenFlags.ReadWrite |
            SQLiteOpenFlags.Create |
            SQLiteOpenFlags.SharedCache;

        public AppDbContext()
        {
            _initTask = InitializeDatabaseAsync();
        }

        private async Task EnsureInitializedAsync()
        {
            if (_initTask != null) await _initTask;
        }

        private async Task InitializeDatabaseAsync()
        {
            if (_initialized) return;

            Console.WriteLine("📦 Initializing database...");
            _dbConnection = new SQLiteAsyncConnection(DatabasePath, Flags);

            await _dbConnection.CreateTableAsync<User>();
            await _dbConnection.CreateTableAsync<JournalEntry>();

            // ✅ migration: add UserId if old DB exists
            await EnsureJournalEntriesHasUserIdAsync();

            _initialized = true;
            Console.WriteLine("✅ Database initialized successfully!");
        }

        private async Task EnsureJournalEntriesHasUserIdAsync()
        {
            try
            {
                await _dbConnection.ExecuteAsync(
                    "ALTER TABLE JournalEntries ADD COLUMN UserId INTEGER NOT NULL DEFAULT 0"
                );
                Console.WriteLine("✓ Added UserId column to JournalEntries");
            }
            catch
            {
                // column already exists - ignore
            }
        }

        public async Task<int> CreateAsync<TEntity>(TEntity entity) where TEntity : class
        {
            await EnsureInitializedAsync();
            return await _dbConnection.InsertAsync(entity);
        }

        public async Task<int> UpdateAsync<TEntity>(TEntity entity) where TEntity : class
        {
            await EnsureInitializedAsync();
            return await _dbConnection.UpdateAsync(entity);
        }

        public async Task<int> DeleteAsync<TEntity>(TEntity entity) where TEntity : class
        {
            await EnsureInitializedAsync();
            return await _dbConnection.DeleteAsync(entity);
        }

        public async Task<List<T>> GetTableRowsAsync<T>(string tableName) where T : class, new()
        {
            await EnsureInitializedAsync();
            string query = $"SELECT * FROM [{tableName}]";
            var result = await _dbConnection.QueryAsync<T>(query);
            return result.ToList();
        }

        public async Task<T?> GetTableRowAsync<T>(string tableName, string column, string value)
            where T : class, new()
        {
            await EnsureInitializedAsync();
            string query = $"SELECT * FROM [{tableName}] WHERE [{column}] = ?";
            var result = await _dbConnection.QueryAsync<T>(query, value);
            return result.FirstOrDefault();
        }

        // ✅ PRIVATE (per-user) entry queries

        public async Task<JournalEntry?> GetTodayEntryAsync(int userId)
        {
            await EnsureInitializedAsync();

            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            var query = @"
                SELECT * FROM JournalEntries
                WHERE UserId = ?
                  AND EntryDate >= ? AND EntryDate < ?
                LIMIT 1";

            var result = await _dbConnection.QueryAsync<JournalEntry>(query, userId, today, tomorrow);
            return result.FirstOrDefault();
        }

        public async Task<List<JournalEntry>> GetAllJournalEntriesAsync(int userId)
        {
            await EnsureInitializedAsync();

            var query = @"
                SELECT * FROM JournalEntries
                WHERE UserId = ?
                ORDER BY EntryDate DESC";

            var result = await _dbConnection.QueryAsync<JournalEntry>(query, userId);
            return result.ToList();
        }

        public async Task<JournalEntry?> GetJournalEntryByIdAsync(int userId, int id)
        {
            await EnsureInitializedAsync();

            var query = "SELECT * FROM JournalEntries WHERE Id = ? AND UserId = ? LIMIT 1";
            var result = await _dbConnection.QueryAsync<JournalEntry>(query, id, userId);
            return result.FirstOrDefault();
        }
    }
}
