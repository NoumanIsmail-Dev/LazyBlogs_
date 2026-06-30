using LazyBlogs_.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace LazyBlogs_.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<BlogTag> BlogTags { get; set; }
        public DbSet<BlogSeries> BlogSeries { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<CommentLike> CommentLikes { get; set; }
        public DbSet<Reaction> BlogReactions { get; set; }
        public DbSet<Bookmark> Bookmarks { get; set; }
        public DbSet<ReadingProgress> ReadingProgresses { get; set; }
        public DbSet<Follow> Follows { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ════════════════════════════════════════════════════════
            // BLOG
            // ════════════════════════════════════════════════════════

            modelBuilder.Entity<Blog>(entity =>
            {
                // Author -> Blogs. Restrict (not Cascade): if an Editor's
                // account is deleted, their published blogs should NOT
                // vanish with them — readers may still be linking to/reading
                // them. Deleting an editor must be handled deliberately in
                // the service layer (e.g. reassign or soft-delete blogs first),
                // never silently cascaded by the database.
                entity.HasOne(b => b.Author)
                      .WithMany(u => u.Blogs)
                      .HasForeignKey(b => b.AuthorId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Category -> Blogs. SetNull: deleting a Category shouldn't
                // delete every blog in it — it should just uncategorize them.
                entity.HasOne(b => b.Category)
                      .WithMany(c => c.Blogs)
                      .HasForeignKey(b => b.CategoryId)
                      .OnDelete(DeleteBehavior.SetNull);

                // BlogSeries -> Blogs. SetNull: deleting a series shouldn't
                // delete the posts in it, just detach them from the series.
                entity.HasOne(b => b.Series)
                      .WithMany(s => s.Blogs)
                      .HasForeignKey(b => b.BlogSeriesId)
                      .OnDelete(DeleteBehavior.SetNull);

                // Slug must be unique site-wide — this is what builds the
                // public URL (/blog/{slug}), two posts can't share one.
                entity.HasIndex(b => b.Slug).IsUnique();

                // Composite index matching our most common feed query shape:
                // "published, not-deleted posts, newest first." Without this,
                // that query does a full table scan once you have more than
                // a trivial number of rows.
                entity.HasIndex(b => new { b.Status, b.IsDeleted, b.PublishedAt });
            });

            // ════════════════════════════════════════════════════════
            // CATEGORY / TAG (BlogTag is the explicit many-to-many join)
            // ════════════════════════════════════════════════════════

            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasIndex(c => c.Slug).IsUnique();
            });

            modelBuilder.Entity<Tag>(entity =>
            {
                entity.HasIndex(t => t.Slug).IsUnique();
            });

            modelBuilder.Entity<BlogTag>(entity =>
            {
                // Composite primary key — BlogTag has no own Id property,
                // it's purely a join row, so EF needs to be told explicitly
                // what makes a row unique here.
                entity.HasKey(bt => new { bt.BlogId, bt.TagId });

                entity.HasOne(bt => bt.Blog)
                      .WithMany(b => b.BlogTags)
                      .HasForeignKey(bt => bt.BlogId)
                      .OnDelete(DeleteBehavior.Cascade);   // deleting a Blog should clean up its tag links

                entity.HasOne(bt => bt.Tag)
                      .WithMany(t => t.BlogTags)
                      .HasForeignKey(bt => bt.TagId)
                      .OnDelete(DeleteBehavior.Cascade);   // deleting a Tag should clean up its blog links
            });

            // ════════════════════════════════════════════════════════
            // BLOG SERIES
            // ════════════════════════════════════════════════════════

            modelBuilder.Entity<BlogSeries>(entity =>
            {
                entity.HasOne(s => s.Author)
                      .WithMany(u => u.BlogSeries)
                      .HasForeignKey(s => s.AuthorId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ════════════════════════════════════════════════════════
            // COMMENT (self-referencing for replies, + CommentLike)
            // ════════════════════════════════════════════════════════

            modelBuilder.Entity<Comment>(entity =>
            {
                entity.HasOne(c => c.Blog)
                      .WithMany(b => b.Comments)
                      .HasForeignKey(c => c.BlogId)
                      .OnDelete(DeleteBehavior.Cascade);   // deleting a blog deletes its comments

                entity.HasOne(c => c.User)
                      .WithMany(u => u.Comments)
                      .HasForeignKey(c => c.UserId)
                      .OnDelete(DeleteBehavior.Restrict);  // don't wipe comments if a user is removed

                // Self-referencing parent/child relationship (replies).
                // Restrict, not Cascade: SQL Server will refuse a cascade
                // path here anyway because it can't tell how many levels
                // deep a cascade delete should travel through a
                // self-referencing tree — this MUST be Restrict or
                // migration creation will fail outright. Deleting a parent
                // comment is instead handled as a soft delete in the
                // service layer (IsDeleted = true), so replies stay intact.
                entity.HasOne(c => c.ParentComment)
                      .WithMany(c => c.Replies)
                      .HasForeignKey(c => c.ParentCommentId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(c => new { c.BlogId, c.CreatedAt });
            });

            modelBuilder.Entity<CommentLike>(entity =>
            {
                entity.HasOne(cl => cl.Comment)
                      .WithMany(c => c.CommentLikes)
                      .HasForeignKey(cl => cl.CommentId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(cl => cl.User)
                      .WithMany(u => u.CommentLikes)
                      .HasForeignKey(cl => cl.UserId)
                      .OnDelete(DeleteBehavior.Restrict);

                // One like per user per comment — stops double-liking and
                // makes "did I like this?" a single indexed lookup.
                entity.HasIndex(cl => new { cl.CommentId, cl.UserId }).IsUnique();
            });

            // ════════════════════════════════════════════════════════
            // BLOG REACTION (one reaction per user per blog)
            // ════════════════════════════════════════════════════════

            modelBuilder.Entity<Reaction>(entity =>
            {
                entity.HasOne(r => r.Blog)
                      .WithMany(b => b.Reactions)
                      .HasForeignKey(r => r.BlogId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(r => r.User)
                      .WithMany(u => u.BlogReactions)
                      .HasForeignKey(r => r.UserId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(r => new { r.BlogId, r.UserId }).IsUnique();
            });

            // ════════════════════════════════════════════════════════
            // BOOKMARK (read later) — one per user per blog
            // ════════════════════════════════════════════════════════

            modelBuilder.Entity<Bookmark>(entity =>
            {
                entity.HasOne(bk => bk.Blog)
                      .WithMany(b => b.Bookmarks)
                      .HasForeignKey(bk => bk.BlogId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(bk => bk.User)
                      .WithMany(u => u.Bookmarks)
                      .HasForeignKey(bk => bk.UserId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(bk => new { bk.BlogId, bk.UserId }).IsUnique();
            });

            // ════════════════════════════════════════════════════════
            // READING PROGRESS — one per user per blog
            // ════════════════════════════════════════════════════════

            modelBuilder.Entity<ReadingProgress>(entity =>
            {
                entity.HasOne(rp => rp.Blog)
                      .WithMany(b => b.ReadingProgresses)
                      .HasForeignKey(rp => rp.BlogId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(rp => rp.User)
                      .WithMany(u => u.ReadingProgresses)
                      .HasForeignKey(rp => rp.UserId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(rp => new { rp.BlogId, rp.UserId }).IsUnique();
            });

            // ════════════════════════════════════════════════════════
            // FOLLOW — two FKs to the same table (Follower / Following)
            // ════════════════════════════════════════════════════════

            modelBuilder.Entity<Follow>(entity =>
            {
                // Both MUST be Restrict. SQL Server will reject Cascade
                // here at migration-creation time anyway, because with two
                // paths from ApplicationUser into the same Follow row, it
                // cannot guarantee the cascade won't cycle. Restrict means
                // a user can't be hard-deleted while still part of any
                // Follow relationship — the service layer must clean those
                // up first (or we soft-delete users instead, which Identity
                // supports via a custom IsDeleted-style flag if we want it).
                entity.HasOne(f => f.Follower)
                      .WithMany(u => u.Following)
                      .HasForeignKey(f => f.FollowerId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(f => f.Following)
                      .WithMany(u => u.Followers)
                      .HasForeignKey(f => f.FollowingId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Can't follow the same person twice.
                entity.HasIndex(f => new { f.FollowerId, f.FollowingId }).IsUnique();
            });

            // ════════════════════════════════════════════════════════
            // NOTIFICATION — Recipient + optional Actor, optional RelatedBlog
            // ════════════════════════════════════════════════════════

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasOne(n => n.Recipient)
                      .WithMany(u => u.NotificationsReceived)
                      .HasForeignKey(n => n.RecipientId)
                      .OnDelete(DeleteBehavior.Cascade);   // deleting a user clears their own inbox

                // Actor is nullable AND must not cascade — otherwise deleting
                // one user (the actor) could cascade-delete notifications
                // belonging to a completely different user (the recipient),
                // which is exactly the kind of multi-path cascade SQL Server
                // refuses to create automatically.
                entity.HasOne(n => n.Actor)
                      .WithMany()
                      .HasForeignKey(n => n.ActorId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(n => n.RelatedBlog)
                      .WithMany()
                      .HasForeignKey(n => n.RelatedBlogId)
                      .OnDelete(DeleteBehavior.SetNull);   // blog gone? notification stays, just loses its link

                // Matches the exact query the notification bell runs:
                // "this user's notifications, newest first."
                entity.HasIndex(n => new { n.RecipientId, n.IsRead, n.CreatedAt });
            });
        }
    }
}