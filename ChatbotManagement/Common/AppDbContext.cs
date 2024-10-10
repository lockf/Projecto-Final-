using ChatbotManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatbotManagement.Common
{
    public class AppDbContext : DbContext
    {
        // Tables
        public virtual DbSet<Feedback> Feedbacks { get; set; } = null!;
        public virtual DbSet<FileKeyword> FileKeywords { get; set; } = null!;
        public virtual DbSet<Keyword> Keywords { get; set; } = null!;
        public virtual DbSet<Message> Messages { get; set; } = null!;
        public virtual DbSet<StoredFile> StoredFiles { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            // Leave Empty
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // FileKeyword
            modelBuilder.Entity<FileKeyword>().HasKey(c => new { c.KeywordId, c.StoredFileId });
            modelBuilder.Entity<FileKeyword>().HasOne(b => b.Keyword).WithMany(bc => bc.FileKeywords).HasForeignKey(bi => bi.KeywordId).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<FileKeyword>().HasOne(b => b.StoredFile).WithMany(bc => bc.FileKeywords).HasForeignKey(bi => bi.StoredFileId).OnDelete(DeleteBehavior.NoAction);

            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }
    }
}
