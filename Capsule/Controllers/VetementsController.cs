using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Capsule.Data;
using Capsule.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Capsule.Authorizations;

namespace Capsule.Controllers
{
    public class VetementsController : Controller
    {
        private readonly ApplicationDbContext Context;
        private IAuthorizationService AuthorizationService { get; }
        private UserManager<IdentityUser> UserManager { get; }


        public VetementsController(ApplicationDbContext context,
            IAuthorizationService authorizationService,
            UserManager<IdentityUser> userManager)
        {
            Context = context;
            AuthorizationService = authorizationService;
            UserManager = userManager;
        }

        // GET: Vetements
        public async Task<IActionResult> Index()
        {
            if (Context.Vetement == null)
                return NotFound();
            var vetements = from v in Context.Vetement select v;
            var isAuthorized = User.
                   IsInRole(AuthorizationConstants.VetementAdministratorsRole);
            var currentUserId = UserManager.GetUserId(User);

            if (!isAuthorized)
                vetements = vetements.Where(v => v.ProprietaireId == currentUserId);
            return View(await vetements.ToListAsync());

        }

        // GET: Vetements/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || Context.Vetement == null)
            {
                return NotFound();
            }
            var v = await Context.Vetement.FirstOrDefaultAsync(
                                                m => m.VetementId == id);
            var isAuthorized = await AuthorizationService.AuthorizeAsync(
                                                User, v, VetementOperations.Read);
            if (!isAuthorized.Succeeded)
                return Forbid();

            return View(v);

        }

        // GET: Vetements/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Vetements/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("VetementId,Nom")] Vetement vetement)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            if (ModelState.IsValid)
            {
                vetement.ProprietaireId = UserManager.GetUserId(User);
                var isAuthorized = await AuthorizationService.AuthorizeAsync(
                    User, vetement, VetementOperations.Create);

                if (!isAuthorized.Succeeded)
                    return Forbid();

                Context.Add(vetement);
                await Context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(vetement);
        }

        // GET: Vetements/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || Context.Vetement == null)
            {
                return NotFound();
            }

            var vetement = await Context.Vetement.FindAsync(id);
            var isAuthorized = await AuthorizationService.AuthorizeAsync(
                   User, vetement, VetementOperations.Update);

            if (!isAuthorized.Succeeded)
                return Forbid();

            if (vetement == null)
                return NotFound();
            return View(vetement);
        }

        // POST: Vetements/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("VetementId,Nom")] Vetement vetement)
        {
            if (id != vetement.VetementId)
            {
                return NotFound();
            }
            var v = await Context.Vetement.AsNoTracking().FirstOrDefaultAsync(m => m.VetementId == id);
            var isAuthorized = await AuthorizationService.AuthorizeAsync(
                User, v, VetementOperations.Update);

            if (!isAuthorized.Succeeded)
                return Forbid();
            if (v == null)
                return NotFound();

            vetement.ProprietaireId = v.ProprietaireId;
            if (ModelState.IsValid)
            {
                try
                {
                    Context.Update(vetement);
                    await Context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VetementExists(vetement.VetementId))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(vetement);
        }

        // GET: Vetements/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || Context.Vetement == null)
            {
                return NotFound();
            }

            var v = await Context.Vetement
                .FirstOrDefaultAsync(m => m.VetementId == id);
            var isAuthorized = await AuthorizationService.AuthorizeAsync(
                            User, v, VetementOperations.Delete);

            if (!isAuthorized.Succeeded)
                return Forbid();
            if (v == null)
                return NotFound();

            return View(v);
        }

        // POST: Vetements/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (Context.Vetement == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Vetement'  is null.");
            }
            var v = await Context.Vetement.FindAsync(id);
            var isAuthorized = await AuthorizationService.AuthorizeAsync(
                User, v, VetementOperations.Update);

            if (!isAuthorized.Succeeded)
                return Forbid();
            if (v == null)
                return NotFound();

            Context.Vetement.Remove(v);
            await Context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VetementExists(int id)
        {
            return Context.Vetement.Any(e => e.VetementId == id);
        }
    }
}
