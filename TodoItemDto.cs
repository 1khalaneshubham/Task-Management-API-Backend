using System;

namespace TaskManagementAPI.DTOs
{
    public class TodoItemDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime DueDate { get; set; }
        public int UserId { get; set; }
        public int CategoryId { get; set; }
        public string UserName { get; set; }      // Shows user's name
        public string CategoryName { get; set; }  // Shows category's name
    }
}