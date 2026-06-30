
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LazyBlogs_.Models
{
    // Tracks how far a reader got into a blog, so "Continue reading" can drop
    // them back at roughly the right spot. One row per (User, Blog) pair —
    // updated in place as they scroll, never duplicated.
    public class ReadingProgress
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

        // Stored as a 0–100 percentage rather than a pixel/character offset.
        // Pixel offsets break the moment font size, zoom level, or screen width
        // changes (desktop vs mobile) — percentage scroll position is resilient
        // to all of that and is simple to compute client-side with JS
        // (scrollTop / scrollHeight) and send back periodically.
        [Range(0, 100)]
        public int ProgressPercent { get; set; } = 0;

        // True once ProgressPercent crosses a "finished reading" threshold
        // (we'll define ~90% as done, not 100%, since very few people scroll
        // to the literal last pixel). Lets us answer "has this user read this
        // blog?" with a flag instead of a magic-number comparison everywhere.
        public bool IsCompleted { get; set; } = false;

        public DateTime LastReadAt { get; set; } = DateTime.UtcNow;

        // Unique index on (BlogId, UserId) in DbContext config — same reasoning
        // as Bookmark: one progress record per reader per blog, upserted on
        // every scroll-tracking update rather than growing a new row each time.
    }
}