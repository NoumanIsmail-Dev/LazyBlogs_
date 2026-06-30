using LazyBlogs_.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LazyBlogs_.Models
{
    public enum BlogStatus
    {
        Draft = 0,
        Published = 1,
        Archived = 2   // soft "unpublish" — admin/editor can pull a blog without deleting it
    }

    public class Blog
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        // SEO-friendly URL slug, e.g. "why-lazy-thinking-is-the-future"
        // Generated from Title but stored separately so editors can override it,
        // and so renaming a title later doesn't break old shared links by default.
        [Required]
        [MaxLength(220)]
        public string Slug { get; set; } = string.Empty;

        // Short manual summary shown on blog cards / search results / meta description.
        // Kept separate from Content so editors control exactly what shows in previews,
        // instead of us auto-truncating raw HTML (which often cuts mid-tag and breaks layout).
        [MaxLength(300)]
        public string? Excerpt { get; set; }

        // The CKEditor 5 output: paragraphs, headings, inline images, code blocks,
        // and YouTube/Instagram/Twitter embeds — all as one HTML blob.
        // Stored as nvarchar(MAX) since blog posts can be long.
        [Required]
        [Column(TypeName = "nvarchar(MAX)")]
        public string Content { get; set; } = string.Empty;

        // Card/hero image shown in feeds, search results, and at the top of the post.
        // Separate from inline content images — this is the "thumbnail."
        [MaxLength(500)]
        public string? FeaturedImageUrl { get; set; }

        public BlogStatus Status { get; set; } = BlogStatus.Draft;

        // ---- Authorship ----
        [Required]
        public string AuthorId { get; set; } = string.Empty;   // FK -> AspNetUsers.Id (string, Identity default)

        [ForeignKey(nameof(AuthorId))]
        public ApplicationUser? Author { get; set; }

        // ---- Classification ----
        public Guid? CategoryId { get; set; }

        [ForeignKey(nameof(CategoryId))]
        public Category? Category { get; set; }

        public ICollection<BlogTag> BlogTags { get; set; } = new List<BlogTag>();

        // ---- Series support (agreed earlier: Part 1, 2, 3...) ----
        public Guid? BlogSeriesId { get; set; }

        [ForeignKey(nameof(BlogSeriesId))]
        public BlogSeries? Series { get; set; }

        // Position within the series, e.g. 1, 2, 3. Null if not part of a series.
        public int? SeriesOrder { get; set; }

        // ---- Timestamps ----
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Set only when Status flips to Published. Lets us sort "Latest" accurately
        // even if a draft was created weeks before it was actually published,
        // and lets us hide/blank this for drafts so they never leak into public feeds by date.
        public DateTime? PublishedAt { get; set; }

        // ---- Engagement (denormalized counters) ----
        // These are maintained by the application (incremented/decremented on
        // comment/reaction/view create-delete) rather than COUNT()'d live every time,
        // so "Most Liked" / "Trending" sorts stay fast as the platform grows.
        public int ViewCount { get; set; } = 0;
        public int CommentCount { get; set; } = 0;
        public int LikeCount { get; set; } = 0;

        // Soft delete — lets Admins "delete" an editor's blog without losing the
        // record (useful for moderation history / undo), and lets us exclude it
        // from queries via a global query filter instead of remembering a WHERE
        // clause everywhere.
        public bool IsDeleted { get; set; } = false;

        // Reading time in minutes, calculated server-side on save from word count
        // (roughly words / 200). Stored instead of computed on every page load.
        public int ReadingTimeMinutes { get; set; } = 1;

        // ---- Navigation back-references ----
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<Reaction> Reactions { get; set; } = new List<Reaction>();
        public ICollection<Bookmark> Bookmarks { get; set; } = new List<Bookmark>();
        public ICollection<ReadingProgress> ReadingProgresses { get; set; } = new List<ReadingProgress>();
    }
}