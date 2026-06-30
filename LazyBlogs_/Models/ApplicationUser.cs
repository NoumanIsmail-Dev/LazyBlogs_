using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace LazyBlogs_.Models
{
    // Roles (Reader/Editor/Admin) are NOT a field on this class — Identity
    // manages roles separately through AspNetRoles / AspNetUserRoles
    // (via RoleManager/UserManager), which is what makes [Authorize(Roles=..)]
    // and role-assignment in the Admin panel work correctly. Don't add a
    // "Role" string column here, it would conflict with that system.
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [MaxLength(500)]
        public string? ProfilePicture { get; set; }

        [MaxLength(500)]
        public string? Bio { get; set; }

        public DateTime? LastLoginAt { get; set; }

        // ---- JWT support (kept for a possible future API/mobile app) ----
        // Unused by the current cookie-auth MVC flow — these will sit null
        // for every web user. Only gets populated if/when a token-issuing
        // API endpoint is added later. Safe to ignore until then.
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }

        // ---- Moderation (Admin: suspend vs block, kept separate) ----
        // Suspended = temporary/reversible; Blocked = harder lockout with a
        // recorded reason. Distinct fields so the Admin panel and the login
        // check can give the user an accurate, specific message instead of
        // a single ambiguous "account disabled."
        public bool IsSuspended { get; set; } = false;
        public DateTime? SuspendedUntil { get; set; }   // null = indefinite

        public bool IsBlocked { get; set; } = false;
        [MaxLength(300)]
        public string? BlockedReason { get; set; }

        // ---- Navigation back-references ----
        // Every collection here has a matching, explicitly-configured
        // relationship in ApplicationDbContext.OnModelCreating — if you
        // rename any property below, the matching .HasForeignKey/.WithMany
        // call in the DbContext must be updated too, or it won't compile.
        public ICollection<Blog> Blogs { get; set; } = new List<Blog>();
        public ICollection<BlogSeries> BlogSeries { get; set; } = new List<BlogSeries>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<CommentLike> CommentLikes { get; set; } = new List<CommentLike>();
        public ICollection<Bookmark> Bookmarks { get; set; } = new List<Bookmark>();
        public ICollection<Reaction> BlogReactions { get; set; } = new List<Reaction>();
        public ICollection<ReadingProgress> ReadingProgresses { get; set; } = new List<ReadingProgress>();

        // A user can be on either side of a Follow row.
        public ICollection<Follow> Following { get; set; } = new List<Follow>();   // people THIS user follows
        public ICollection<Follow> Followers { get; set; } = new List<Follow>();   // people who follow THIS user

        public ICollection<Notification> NotificationsReceived { get; set; } = new List<Notification>();
    }
}