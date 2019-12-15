using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ContactApp.Models
{
    /// <summary>
    /// Model for client side representation of contact
    /// </summary>
    public class ContactViewModel
    {
        public ContactViewModel()
        {
            Phones = new List<PhoneViewModel>();
        }

        public static ContactViewModel New()
        {
            var newContact =  new ContactViewModel();
            newContact.Phones.Add(new PhoneViewModel());
            return newContact;
        }

        public int Id { get; set; }

        [Required(ErrorMessage = "FullName is required")]
        public string FullName { get; set; }
        
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Email address is not valid")]
        public string Email { get; set; }

        public string Address { get; set; }

        public List<PhoneViewModel> Phones { get; set; }

        public int TotalPhones => Phones.Count;
    }
}