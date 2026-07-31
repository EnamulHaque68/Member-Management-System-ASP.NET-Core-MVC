using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace CoreIdentityWithOWIN.Models
{
    public class PastDateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null && DateTime.TryParse(value.ToString(), out DateTime date))
            {
                if (date.Date > DateTime.Now.Date)
                {
                    return new ValidationResult(ErrorMessage ?? "Date cannot be in the future");
                }
            }
            return ValidationResult.Success;
        }
    }


}
