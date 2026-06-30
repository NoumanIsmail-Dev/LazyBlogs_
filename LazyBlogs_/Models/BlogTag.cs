using LazyBlogs_.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LazyBlogs_.Models
{
    // Free-form, many-to-many labels (e.g. #aspnetcore, #beginners, #productivity).
    // Separate from Category: Category is "what shelf is this on" (one only),
    // Tags are "what topics does this touch" (many).
    public class Tag
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(40)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Slug { get; set; } = string.Empty;

        public ICollection<BlogTag> BlogTags { get; set; } = new List<BlogTag>();
    }

    // Explicit join entity (instead of EF Core's implicit many-to-many) so we
    // have a real table we can query/index directly — e.g. "most used tags
    // this month" — without EF generating a hidden join table we can't see.
    public class BlogTag
    {
        public Guid BlogId { get; set; }

        [ForeignKey(nameof(BlogId))]
        public Blog Blog { get; set; } = null!;

        public Guid TagId { get; set; }

        [ForeignKey(nameof(TagId))]
        public Tag Tag { get; set; } = null!;
    }
}