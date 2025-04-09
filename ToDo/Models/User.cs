using System.ComponentModel.DataAnnotations;

namespace ToDoApp.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty; // For demo only. Don't store plain text in production!

        // Step 2: Navigation property for related ToDos
        public ICollection<ToDo> ToDos { get; set; } = new List<ToDo>();
    }
}
