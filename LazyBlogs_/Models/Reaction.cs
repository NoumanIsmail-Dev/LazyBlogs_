using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LazyBlogs_.Models
{
    // The set of reaction types a reader can pick from. A real enum (not a free
    // string) so the UI has a fixed, known palette to render buttons for, and so
    // "most liked" / "most loved" filtering later is a simple WHERE on this column
    // instead of matching arbitrary emoji strings.
    public enum ReactionType
    {
        Like = 0,        // 👍 plain like — kept simple/default for the basic "like a post" action
        Love = 1,        // ❤️
        Insightful = 2,  // 💡
        Funny = 3,       // 😂
        Mindblown = 4,   // 🤯
        Sad = 5          // 😢
    }

    // One row = one user's reaction to one blog. A user can only have ONE
    // reaction per blog at a time (enforced by a unique index on BlogId+UserId
    // in the DbContext config) — switching from Like to Love updates this row
    // rather than adding a second one. This matches how readers actually expect
    // reactions to behave (like Facebook/LinkedIn reactions, not a tally of clicks).
    public class Reaction
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

        [Required]
        public ReactionType Type { get; set; } = ReactionType.Like;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}