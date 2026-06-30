using System.ComponentModel.DataAnnotations;

namespace LazyBlogs_.Models
{
    // Single, admin-managed classification (e.g. Tech, Lifestyle, Design).
    // Kept as its own table instead of a string/enum on Blog so admins can
    // add/rename/retire categories later without a migration or losing data
    // on existing posts.
    public class Category
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(60)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(70)]
        public string Slug { get; set; } = string.Empty;   // for /category/tech style URLs

        [MaxLength(200)]
        public string? Description { get; set; }

        public ICollection<Blog> Blogs { get; set; } = new List<Blog>();
    }
}