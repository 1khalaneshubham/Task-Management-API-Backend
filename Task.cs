using System;

namespace TaskManagementAPI.Models
{
    public class Task
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime DueDate { get; set; }

        // Foreign Keys
        public int UserId { get; set; }
        public int CategoryId { get; set; }

        // Navigation Properties
        public User User { get; set; }
        public Category Category { get; set; }
    }
}