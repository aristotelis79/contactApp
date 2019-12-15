using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using ContactApp.Data.Entities;
using ContactApp.Models;
using ContactApp.Models.Mappers;
using ContactApp.Repository;
using ContactApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ContactApp.Controllers
{
    public class ContactController : Controller
    {
        #region fields

        private readonly IRepository<Contact, int> _contactRepository;
        private readonly IContactService _contactService;
        private readonly ILogger<ContactController> _logger;
        #endregion

        #region ctor

        public ContactController(IRepository<Contact, int> contactRepository,
            IContactService contactService,
            ILogger<ContactController> logger)
        {
            _contactRepository = contactRepository ?? throw new ArgumentNullException(nameof(contactRepository));
            _contactService = contactService ?? throw new ArgumentNullException(nameof(contactService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #endregion

        #region Methods

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = await _contactRepository.Table.Include(p=>p.Phones)
                                                        .ToListAsync()
                                                        .ConfigureAwait(false);
            
            return View(model.MapTo());
        }



        [HttpGet]
        public async Task<IActionResult> Details(int id, CancellationToken token = default)
        {
            var entity = await _contactService.GetById(id,token)
                                                .ConfigureAwait(false);

            if(entity == null)
                return new NotFoundResult();

            return View(entity.MapTo());
        }


        [HttpGet]
        public IActionResult Create()
        {
            return View(ContactViewModel.New());
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>  Create([Bind(nameof(ContactViewModel.FullName),
                                                                nameof(ContactViewModel.Email),
                                                                nameof(ContactViewModel.Address),
                                                                nameof(ContactViewModel.Phones))] ContactViewModel model, CancellationToken token = default)
        {
            try
            {
                if (!ModelState.IsValid) return View(model);

                await _contactRepository.InsertAsync(model.MapTo(), token).ConfigureAwait(false);

                return RedirectToAction(nameof(Index));
            }
            catch(Exception e)
            {
                _logger.LogError("Can't insert Contact", e);
                AddDuplicateValueToErrors(e);
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken token = default)
        {
            var entity = await _contactService.GetById(id,token)
                                                .ConfigureAwait(false);


            if(entity == null)
                return new NotFoundResult();

            return View(entity.MapTo());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>  Edit(int id,[Bind(nameof(ContactViewModel.Id),
                                                                        nameof(ContactViewModel.FullName),
                                                                        nameof(ContactViewModel.Email),
                                                                        nameof(ContactViewModel.Address),
                                                                        nameof(ContactViewModel.Phones)
                                                                        )] ContactViewModel model,CancellationToken token = default)
        {

            try
            {
                var entity = await _contactService.GetById(id,token)
                                                    .ConfigureAwait(false);

                if(entity == null)
                    return new NotFoundResult();

                if (model.Id != id || !ModelState.IsValid) return View(model);

                var updateModel = await TryUpdateModelAsync(entity,"",
                                                c=>c.FullName, 
                                                                    c=> c.Email,
                                                                    c=> c.Address,
                                                                    c => c.Phones)
                                                                    .ConfigureAwait(false);

                if (updateModel)
                    await _contactRepository.UpdateAsync(entity, token).ConfigureAwait(false);
                else
                    return View(model);
                    
                
                return RedirectToAction(nameof(Index));
            }
            catch(Exception e)
            {
                _logger.LogError("Can't edit contact", e);
                AddDuplicateValueToErrors(e);
                return View(model);
            }
        }

        [HttpDelete]
        public async Task<IActionResult>  Delete(int id, CancellationToken token = default)
        {
            try
            {
                var entity = await _contactService.GetById(id,token)
                                                    .ConfigureAwait(false);

                if(entity == null) return new ContentResult
                                                {
                                                    Content = "Didn't find it",
                                                    StatusCode = StatusCodes.Status404NotFound
                                                };

                await _contactRepository.DeleteAsync(entity, token)
                                        .ConfigureAwait(false);

                return new ContentResult
                {
                    Content = "deleted",
                    StatusCode = StatusCodes.Status205ResetContent
                };
            }
            catch(Exception e)
            {
                _logger.LogError("Can't delete contact", e);
                return new ContentResult
                {
                    Content = "Can't delete contact",
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }

        #endregion

        #region Helpers

        private void AddDuplicateValueToErrors(Exception e)
        {
            var error = e.InnerException?.InnerException?.Message;
            if (string.IsNullOrWhiteSpace(error)) return;

            var values = Regex.Matches(error, "\\((.*?)\\)");
            foreach (var v in values)
            {
                if (v == null || string.IsNullOrWhiteSpace(v.ToString())) continue;
                ModelState.AddModelError(Guid.NewGuid().ToString(), $"{v} already exists");
            }
        }

        #endregion

    }
}