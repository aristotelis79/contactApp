using System.Collections.Generic;
using ContactApp.Data.Entities;

namespace ContactApp.Models.Mappers
{
    /// <summary>
    /// Map View Models To Entity and Revere
    /// </summary>
    public static class Mapper
    {

        public static ContactViewModel MapTo(this Contact entity)
        {
            var model = new ContactViewModel
            {
                Id = entity.Id,
                FullName = entity.FullName,
                Email = entity.Email,
                Address = entity.Address
            };

            foreach (var p in entity.Phones)
            {
                model.Phones.Add(new PhoneViewModel
                {
                    PhoneType = p.PhoneType,
                    PhoneNumber = p.PhoneNumber
                });
            }
            return model;
        }

        public static List<ContactViewModel> MapTo(this List<Contact> contacts)
        {
            var model = new List<ContactViewModel>();

            contacts.ForEach(c => model.Add(c.MapTo()));

            return model;
        }

        public static Contact MapTo(this ContactViewModel model)
        {
            var entity = new Contact
            {
                FullName = model.FullName,
                Email = model.Email,
                Address = model.Address
            };
            foreach (var p in model.Phones)
            {
                entity.Phones.Add(new Phone
                {
                    PhoneType = p.PhoneType,
                    PhoneNumber = p.PhoneNumber
                });
            }
            return entity;
        }
    }
}