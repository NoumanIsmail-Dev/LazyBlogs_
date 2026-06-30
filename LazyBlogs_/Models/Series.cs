using LazyBlogs_.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LazyBlogs_.Models
{
    // A container for "Part 1 / Part 2 / Part 3" style multi-post stories.
    // Blog.SeriesOrder controls the sequence within it.
    public class BlogSeries
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(150)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(300)]
        public string? Description { get; set; }

        [Required]
        public string AuthorId { get; set; } = string.Empty;

        [ForeignKey(nameof(AuthorId))]
        public ApplicationUser? Author { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Blog> Blogs { get; set; } = new List<Blog>();
    }
}