using System.ComponentModel.DataAnnotations;

namespace CreditFlow.Domain.Request
{
    public class RegisterClientRequest
    {
        [Required(ErrorMessage = "The Name field is required.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "The Document field is required.")]
        public string Document { get; set; }

        [Required(ErrorMessage = "The Email field is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "The PhoneNumber field is required.")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "The MonthlyIncome field is required.")]
        [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "Monthly income must be greater than zero.")]
        public decimal MonthlyIncome { get; set; }
    }
}