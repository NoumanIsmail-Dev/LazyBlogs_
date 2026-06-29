using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using LazyBlogs_.Models;
using LazyBlogs_.Data;

namespace LazyBlogs_.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Bookmark> Bookmarks { get; set; }
        public DbSet<Reaction> Reactions { get; set; }
        public DbSet<Follow> Follows { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Series> Series { get; set; }
        public DbSet<ReadingProgress> ReadingProgresses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships here
            // Example: Blog - User relationship
            //modelBuilder.Entity<Blog>()
            //    .HasOne(b => b.Author)
            //    .WithMany(u => u.Blogs)
            //    .HasForeignKey(b => b.AuthorId)
            //    .OnDelete(DeleteBehavior.Restrict);
        }
    }
}