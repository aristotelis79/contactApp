using System;
using System.Collections.Generic;
using System.Linq;
using Bogus;
using ContactApp.Controllers;
using ContactApp.Data;
using ContactApp.Data.Entities;
using ContactApp.Models;
using ContactApp.Models.Mappers;
using ContactApp.Repository;
using ContactApp.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;
using TechTalk.SpecFlow;

namespace ContactAppTests
{
    [Binding]
    public class OnlySuccessStoriesForContactActionsSteps
    {
        private ILogger<ContactController> _logger;
        private IDbContext _dbContext;
        private IRepository<Contact, int> _repository;
        private IContactService _contactService;
        private ContactController _contactController;
        private ScenarioContext _scenarioContext;

        public OnlySuccessStoriesForContactActionsSteps(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
            _logger = new Logger<ContactController>(new NullLoggerFactory());
            _dbContext = new ContactDbContext("Server=.;Database=contact-app;Trusted_Connection=True;MultipleActiveResultSets=true");
            _repository = new EfRepository<Contact, int>(_dbContext);
            _contactService = new ContactService(_repository);
            _contactController = new ContactController(_repository, _contactService, _logger);

            EnsureExistContactIsIDb();
        }


        [Given(@"(.*) valid contact model")]
        public void GivenAContactModel(string typeofContact)
        {
            switch (typeofContact)
            {
                case "exist":
                    _scenarioContext["existContact"] = GetContactByEmail("test_fullname@mail.gr");
                    _scenarioContext["contactViewModel"] = ((Contact)_scenarioContext["existContact"]).MapTo();
                    break;
                case "random":
                    _scenarioContext["contactViewModel"] = RandomContactViewModel();
                    break;
                default:
                    Assert.Contains(typeofContact, new[]{"exist","random"});
                    break;
            }
        }

        [Given(@"edit the contact")]
        public void GivenEditTheContact()
        {
            var model = (ContactViewModel) _scenarioContext["contactViewModel"];
            var faker = new Faker();
            model.Address += faker.Random.Word();
            var p = model.Phones.First();
            p.PhoneNumber = faker.Phone.PhoneNumber("##########");
            p.PhoneType += faker.Random.Word();
        }


        [When(@"call (.*) request")]
        public void WhenCallRequest(string request)
        {
            switch (request)
            {
                case "create":
                    _contactController.Create((ContactViewModel)_scenarioContext["contactViewModel"]).Wait();
                    break;
                case "edit":
                    var editModel = (ContactViewModel)_scenarioContext["contactViewModel"];
                    var entity = (Contact) _scenarioContext["existContact"];
                    entity.Address = editModel.Address;
                    entity.Phones.First().PhoneType = editModel.Phones.First().PhoneType;
                    entity.Phones.First().PhoneNumber = editModel.Phones.First().PhoneNumber;
                    _repository.UpdateAsync(entity).Wait();
                    break;
                case "delete":
                    var deleteModel = (Contact)_scenarioContext["resultContact"];
                    _contactController.Delete(deleteModel.Id).Wait();
                    break;
                default:
                    Assert.Contains(request, new[]{"create","edit","delete"});
                    break;
            }
        }

        [When(@"search for contact")]
        public void WhenSearchForContact()
        {
            _scenarioContext["resultContact"] = GetContactByEmail(((ContactViewModel)_scenarioContext["contactViewModel"]).Email);
        }


        [Then(@"the right contact results returns")]
        public void ThenTheRightContactResultsReturns()
        {
            var model = (ContactViewModel)_scenarioContext["contactViewModel"];
            var contact = (Contact) _scenarioContext["resultContact"];
            Assert.That(model.FullName,Is.EqualTo(contact.FullName));
            Assert.That(model.Email,Is.EqualTo(contact.Email));
            Assert.That(model.Address,Is.EqualTo(contact.Address));
            var phones = contact.Phones.ToList();
            for (var p = 0; p < phones.Count; p++)
            {
                Assert.That(model.Phones[p].PhoneType,Is.EqualTo(phones[p].PhoneType));
                Assert.That(model.Phones[p].PhoneNumber,Is.EqualTo(phones[p].PhoneNumber));
            }
        }


        [Then(@"contact removed")]
        public void ThenContactRemoved()
        {
            Assert.That(_scenarioContext["resultContact"], Is.Null);
        }

        #region Helpers

        private void EnsureExistContactIsIDb()
        {
            var contact = GetContactByEmail("test_fullname@mail.gr");

            if (contact != null) return;

            var entity = new Contact
            {
                FullName = "test_fullname",
                Email = "test_fullname@mail.gr",
                Address = "test_address",
                Phones = new List<Phone>
                {
                    new Phone
                    {
                        PhoneType = "test_phonetype_1",
                        PhoneNumber = "99999999999999",
                    },
                    new Phone
                    {
                        PhoneType = "test_phonetype_2",
                        PhoneNumber = "99999999999998",
                    }
                }
            };
            var result = _repository.InsertAsync(entity).Result;
            Assert.That(result, Is.Positive);
        }

        private Contact GetContactByEmail(string email)
        {
            return _repository.Table.Include(x=>x.Phones)
                                    .FirstOrDefault(x => x.Email.Equals(email));
        }

        private static ContactViewModel RandomContactViewModel()
        {
            var phoneModel = new Faker<PhoneViewModel>()
                .RuleFor(x => x.PhoneType, (t) => t.Random.Word())
                .RuleFor(x => x.PhoneNumber, (n) => n.Phone.PhoneNumber("##########"));

            var contactModel = new Faker<ContactViewModel>()
                .RuleFor(x => x.FullName, (f) => f.Person.FullName)
                .RuleFor(x => x.Email, (e) => e.Person.Email)
                .RuleFor(x => x.Address, (a) => a.Address.FullAddress())
                .RuleFor(x=>x.Phones, () => phoneModel.Generate(2));

            return contactModel.Generate();
        }
        #endregion
    }
}
