using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LazyBlogs_.Models
{
    // One enum covering every kind of notification the platform produces.
    // Kept as ONE table (not a separate table per notification type) because
    // a user's notification bell needs to show all of these merged into a
    // single chronological list — splitting them into multiple tables would
    // mean UNION-ing several queries just to render one dropdown.
    public enum NotificationType
    {
        NewBlogFromFollowedEditor = 0,  // an editor you follow published
        CommentReply = 1,               // someone replied to your comment
        BlogCommentReceived = 2,        // someone commented on your blog (editor-facing)
        NewFollower = 3,                // someone followed you (editor-facing)
        RoleChanged = 4,                // admin promoted/demoted your role
        AccountSuspended = 5,           // admin suspended/blocked you
        AdminAnnouncement = 6,          // platform-wide email/announcement
        CompetitionUpdate = 7           // competition started, results, etc.
    }

    public class Notification
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        // Who receives this notification.
        [Required]
        public string RecipientId { get; set; } = string.Empty;

        [ForeignKey(nameof(RecipientId))]
        public ApplicationUser Recipient { get; set; } = null!;

        // Who/what caused it — nullable because some notifications (admin
        // announcements, suspensions) come from the system itself, not
        // from another user's action.
        public string? ActorId { get; set; }

        [ForeignKey(nameof(ActorId))]
        public ApplicationUser? Actor { get; set; }

        [Required]
        public NotificationType Type { get; set; }

        // Short, ready-to-render text (e.g. "Sana published a new blog: ...").
        // Generated and stored at creation time rather than rebuilt from
        // related data every time the bell is opened — cheaper to read, and
        // it freezes the wording even if the source blog/comment is edited
        // or deleted later.
        [Required]
        [MaxLength(250)]
        public string Message { get; set; } = string.Empty;

        // Where clicking the notification should take the user — e.g.
        // "/blog/why-lazy-thinking-is-the-future" or "/admin/users".
        // Stored as a plain relative URL instead of trying to reconstruct it
        // from BlogId/CommentId at render time, since the target differs by
        // NotificationType (a blog link vs a profile link vs an admin page).
        [MaxLength(300)]
        public string? LinkUrl { get; set; }

        // Optional pointer back to the source blog, when relevant (types 0–2).
        // Nullable and NOT a required FK relationship, because notification
        // types like AdminAnnouncement or RoleChanged have no blog at all —
        // forcing a non-null BlogId on every notification would mean fake
        // placeholder rows just to satisfy the schema.
        public Guid? RelatedBlogId { get; set; }

        [ForeignKey(nameof(RelatedBlogId))]
        public Blog? RelatedBlog { get; set; }

        public bool IsRead { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // DbContext config will add an index on (RecipientId, IsRead, CreatedAt)
        // — this is the exact shape of the query the notification bell runs
        // constantly ("give me this user's unread notifications, newest
        // first"), so it's worth indexing deliberately rather than relying
        // on a generic single-column index.
    }
}