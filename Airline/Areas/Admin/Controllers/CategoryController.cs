using DataAccess.Repos.IRepos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace Airline.Areas.Admin.Controllers
{
    [Authorize(Roles = NameInit.Admin)]
    public class CategoryController : Controller
    {
        private readonly ICategoryRepo categoryRepo;

        public CategoryController(ICategoryRepo categoryRepo)
        {
            this.categoryRepo = categoryRepo;
        }
        public IActionResult Index()
        {
            var category = categoryRepo.Get().OrderBy(e=>e.Name).ToList();
            return View(model: category);
        }
        public IActionResult Creation()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Creation(Category category)
        {
            if (ModelState.IsValid)
            {
                categoryRepo.Create(category);
                categoryRepo.Attemp();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }
        public IActionResult Edition(int id)
        {
            var category = categoryRepo.GetOne(filter: e => e.Id == id);
            return View(model: category);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edition(Category category)
        {
            if (ModelState.IsValid)
            {
                categoryRepo.Edit(category);
                categoryRepo.Attemp();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }
        public IActionResult Deletion(int id)
        {
            var category = categoryRepo.GetOne(filter: e => e.Id == id);
            categoryRepo.Delete(category);
            categoryRepo.Attemp();
            return RedirectToAction(nameof(Index));
        }
    }
}
