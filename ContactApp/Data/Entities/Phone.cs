using System;
using System.ComponentModel.DataAnnotations;

namespace ContactApp.Data.Entities
{
    /// <summary>
    /// Model for entity server side representation of phone
    /// </summary>
    public class Phone : BaseEntity<int>
    {
        public string PhoneType { get; set; }

        [MaxLength(14)]
        public string PhoneNumber { get; set; }

        public virtual int ContactId { get; set; }

        public virtual  Contact Contact { get; set; }

    }
}