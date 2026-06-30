using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LazyBlogs_.Models
{
    public class Comment
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid BlogId { get; set; }

        [ForeignKey(nameof(BlogId))]
        public Blog Blog { get; set; } = null!;

        [Required]
        public string UserId { get; set; } = string.Empty;   // commenter (reader, editor, or admin)

        [ForeignKey(nameof(UserId))]
        public ApplicationUser? User { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Content { get; set; } = string.Empty;

        // Self-referencing FK: lets a comment be a reply to another comment.
        // This single nullable field is what powers BOTH "reader replies to
        // reader" threads AND "editor replies to reader" — we don't need a
        // separate EditorReply table. Null = top-level comment.
        public Guid? ParentCommentId { get; set; }

        [ForeignKey(nameof(ParentCommentId))]
        public Comment? ParentComment { get; set; }

        public ICollection<Comment> Replies { get; set; } = new List<Comment>();

        // True when the reply author is the blog's Editor/Author. Denormalized
        // (instead of checking User.Id == Blog.AuthorId every render) purely so
        // the view can cheaply show an "Author" badge next to the editor's own
        // reply without an extra join/comparison per comment.
        public bool IsAuthorReply { get; set; } = false;

        // Comment likes (agreed earlier: readers can like comments too).
        // Kept as a denormalized counter for the same reason as Blog.LikeCount —
        // fast to read in a comment list without COUNT()'ing a child table.
        public int LikeCount { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }   // null = never edited; lets UI show "(edited)"

        // Soft delete: if a comment with replies gets deleted, hard-deleting it
        // would orphan or cascade-delete the whole reply thread. Soft delete lets
        // us render "[deleted]" in place while keeping the thread structure intact.
        public bool IsDeleted { get; set; } = false;

        public ICollection<CommentLike> CommentLikes { get; set; } = new List<CommentLike>();
    }

    // Join entity recording WHICH user liked WHICH comment.
    // Needed (can't just bump Comment.LikeCount directly) so we can:
    //   1. Prevent the same user liking a comment twice
    //   2. Show "you liked this" state in the UI
    //   3. Let a user unlike (decrement) only their own like
    public class CommentLike
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid CommentId { get; set; }

        [ForeignKey(nameof(CommentId))]
        public Comment Comment { get; set; } = null!;

        [Required]
        public string UserId { get; set; } = string.Empty;

        [ForeignKey(nameof(UserId))]
        public ApplicationUser? User { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}