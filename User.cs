using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TaskManagementAPI.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(150, ErrorMessage = "Email cannot exceed 150 characters")]
        public string Email { get; set; }

        // Navigation property
        [JsonIgnore]
        public ICollection<TodoItem> TodoItems { get; set; }
    }
}