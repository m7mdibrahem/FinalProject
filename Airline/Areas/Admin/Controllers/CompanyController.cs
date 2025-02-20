using DataAccess.Repos;
using DataAccess.Repos.IRepos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace Airline.Areas.Admin.Controllers
{
    [Authorize(Roles =NameInit.Admin)]
    public class CompanyController : Controller
    {
        private readonly ICompanyRepo companyRepo;
        private readonly ITripRepo tripRepo;

        public CompanyController(ICompanyRepo companyRepo, ITripRepo tripRepo)
        {
            this.companyRepo = companyRepo;
            this.tripRepo = tripRepo;
        }
        public IActionResult Index()
        {
            var company = companyRepo.Get().OrderBy(e=>e.Name).ToList();
            return View(model: company);
        }
        public IActionResult Creation()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Creation(Company company, IFormFile file)
        {
            if (ModelState.IsValid)
            {
                if (file != null && file.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", fileName);
                    using (var stream = System.IO.File.Create(filePath))
                    {
                        file.CopyTo(stream);
                    }
                    company.ImgUrl = fileName;
                    companyRepo.Create(company);
                    companyRepo.Attemp();
                    return RedirectToAction(nameof(Index));
                }
            }
            return View();
        }
        public IActionResult Edition(int id)
        {
            var company = companyRepo.GetOne(filter: e => e.Id == id);
            return View(model: company);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edition(Company company, IFormFile file)
        {
            ModelState.Remove("file");
            var oldImg = companyRepo.GetOne(filter: e => e.Id == company.Id, tracked: false);
            var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", oldImg.ImgUrl);
            if (ModelState.IsValid)
            {
                if (file != null && file.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", fileName);
                    using (var stream = System.IO.File.Create(filePath))
                    {
                        file.CopyTo(stream);
                    }
                    if (System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath);
                    }
                    company.ImgUrl = fileName;
                }
                else
                {
                    company.ImgUrl = oldImg.ImgUrl;
                }
                companyRepo.Edit(company);
                companyRepo.Attemp();
                return RedirectToAction(nameof(Index));
            }
            return View(company);
        }
        public IActionResult Deletion(int id)
        {
            var company = companyRepo.GetOne(e => e.Id == id);
            var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", company.ImgUrl);
            System.IO.File.Delete(oldPath);
            companyRepo.Delete(company);
            companyRepo.Attemp();
            return RedirectToAction(nameof(Index));
        }
    }
}
