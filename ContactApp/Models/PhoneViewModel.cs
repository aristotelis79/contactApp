using System.ComponentModel.DataAnnotations;

namespace ContactApp.Models
{
    public class PhoneViewModel
    {
        public string PhoneType { get; set; }

        [Phone(ErrorMessage = "Not valid phone number")]
        [MinLength(7, ErrorMessage = "To little numbers")]
        [MaxLength(14, ErrorMessage = "To many numbers")]
        public string PhoneNumber { get; set; }
    }
}