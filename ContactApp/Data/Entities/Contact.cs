using System.Collections.Generic;

namespace ContactApp.Data.Entities
{
    /// <summary>
    /// Model for entity server side representation of contact
    /// </summary>
    public class Contact : BaseEntity<int>
    {
        private ICollection<Phone> _phones;

        public string FullName { get; set; }

        public string Email { get; set; }

        public string Address { get; set; }

        public virtual ICollection<Phone> Phones
        {
            get => _phones ??= new List<Phone>(); 
            set => _phones = value;
        }
    }
}