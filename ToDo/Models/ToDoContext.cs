using Microsoft.EntityFrameworkCore;

namespace ToDoApp.Models
{
    public class ToDoContext : DbContext
    {
        public ToDoContext(DbContextOptions<ToDoContext> options) : base(options) { }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<ToDo> ToDos { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Status> Status { get; set; } = null!;

        // Seed data
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ✅ Step 3: Define relationship between User and ToDo
            modelBuilder.Entity<ToDo>()
                .HasOne(t => t.User)
                .WithMany(u => u.ToDos)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Optional: delete tasks if user is deleted

            modelBuilder.Entity<Category>().HasData(
                new Category { CategoryId = "work", Name = "work" },
                new Category { CategoryId = "home", Name = "Home" },
                new Category { CategoryId = "ex", Name = "Exercise" },
                new Category { CategoryId = "shop", Name = "Shopping" },
                new Category { CategoryId = "call", Name = "Contact" }
            );

            modelBuilder.Entity<Status>().HasData(
                new Status { StatusId = "open", Name = "Open" },
                new Status { StatusId = "closed", Name = "Completed" }
            );

            // Seed initial user (username: admin, password: 1234)
            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Username = "admin", Password = "1234" }
            );
        }
    }
}
