using System;
using System.ComponentModel.DataAnnotations;

namespace TaskManagementAPI.Attributes
{
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