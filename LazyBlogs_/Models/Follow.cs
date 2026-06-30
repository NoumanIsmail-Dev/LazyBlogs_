using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LazyBlogs_.Models
{
    // One row = "FollowerUser follows FollowingUser". Self-referencing onto
    // ApplicationUser via two separate FKs rather than a single generic
    // "UserId" pair, so EF Core can tell the two relationships apart instead
    // of getting confused about which side is which (this needs explicit
    // Fluent API config in DbContext — flagged below).
    //
    // Deliberately modeled as User-follows-User, NOT User-follows-Blog or
    // User-subscribes-to-Category. This matches what you described: readers
    // follow a specific editor's body of work, not a single post or a topic.
    public class Follow
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        // The reader doing the following.
        [Required]
        public string FollowerId { get; set; } = string.Empty;

        [ForeignKey(nameof(FollowerId))]
        public ApplicationUser Follower { get; set; } = null!;

        // The editor being followed.
        [Required]
        public string FollowingId { get; set; } = string.Empty;

        [ForeignKey(nameof(FollowingId))]
        public ApplicationUser Following { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // DbContext config (coming when we build it) will add:
        //   1. A unique index on (FollowerId, FollowingId) — can't follow the
        //      same person twice, and makes "am I following them?" a single
        //      indexed lookup instead of a slower table scan.
        //   2. DeleteBehavior.Restrict (not Cascade) on both FKs — because EF
        //      Core/SQL Server will refuse multiple cascade paths from
        //      ApplicationUser to Follow anyway (it can't tell which side
        //      should cascade), so this has to be explicit or migrations
        //      will fail with a "may cause cycles" error at creation time.
        //   3. Application-level validation that FollowerId != FollowingId,
        //      since the DB itself can't express "these two columns must
        //      differ" — we'll check this in the service layer.
    }
}