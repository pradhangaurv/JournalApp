using SQLite;
using System.ComponentModel.DataAnnotations;

namespace JouralAppWeb.Database
{
    [Table("JournalEntries")]
    public class JournalEntry
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Content { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string PrimaryMood { get; set; } = string.Empty;
        public string SecondaryMoods { get; set; } = string.Empty;
        public string Tags { get; set; } = string.Empty;
        public DateTime EntryDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
