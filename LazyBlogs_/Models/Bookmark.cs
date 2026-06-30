using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LazyBlogs_.Models
{
    // "Read Later" — deliberately a separate table from ReadingProgress.
    // Bookmark = "I want to read this at some point" (a deliberate save action).
    // ReadingProgress = "I am/was partway through this" (passive, auto-tracked).
    // A blog can be bookmarked without ever being opened, and a blog can have
    // reading progress without ever being bookmarked — keeping them apart
    // avoids one table trying to mean two different things.
    public class Bookmark
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid BlogId { get; set; }

        [ForeignKey(nameof(BlogId))]
        public Blog Blog { get; set; } = null!;

        [Required]
        public string UserId { get; set; } = string.Empty;

        [ForeignKey(nameof(UserId))]
        public ApplicationUser? User { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Unique index on (BlogId, UserId) will be set in DbContext config —
        // prevents the same blog being bookmarked twice by the same user
        // and lets "is this bookmarked?" be a single indexed lookup.
    }
}