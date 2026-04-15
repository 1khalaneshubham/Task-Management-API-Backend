using System;
using System.ComponentModel.DataAnnotations;
using TaskManagementAPI.Attributes;

namespace TaskManagementAPI.DTOs
{
    public class CreateTodoItemDto
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, MinimumLength = 3)]
        public string Title { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        public bool IsCompleted { get; set; }

        [Required(ErrorMessage = "Due date is required")]
        [FutureDate(ErrorMessage = "Due date cannot be in the past")]
        public DateTime DueDate { get; set; }

        [Required(ErrorMessage = "User ID is required")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Category ID is required")]
        public int CategoryId { get; set; }
    }

    // Custom validation attribute
    public class FutureDateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is DateTime date && date.Date < DateTime.Today)
            {
                return new ValidationResult(ErrorMessage ?? "Due date cannot be in the past");
            }
            return ValidationResult.Success;
        }
    }
}