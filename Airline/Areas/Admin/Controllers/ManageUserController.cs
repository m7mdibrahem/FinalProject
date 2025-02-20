using DataAccess.Repos.IRepos;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.ViewModels;

namespace Airline.Areas.Admin.Controllers
{
    public class ManageUserController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IApplicationUserRepo applicationUserRepo;

        public ManageUserController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IApplicationUserRepo applicationUserRepo)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.applicationUserRepo = applicationUserRepo;
        }
        public async Task<IActionResult> Index()
        {
            var user = applicationUserRepo.Get().ToList();
            var UserDetail = new List<UserVM>();
            foreach (var item in user)
            {
                var role = await userManager.GetRolesAsync(item);
                UserDetail.Add(new()
                {
                    Id = item.Id,
                    Name = item.Name,
                    UserName = item.UserName,
                    Email = item.Email,
                    Role = role[0],
                    LockEnd=item.LockoutEnd
                });
            }
            return View(UserDetail);
        }
        public async Task<IActionResult> Manage(string id)
        {
            var user = applicationUserRepo.GetOne(filter: e => e.Id == id);
            var roles = await userManager.GetRolesAsync(user);

            MngRoleVM mngRoleVM = new()
            {
                Id = user.Id,
                Name = user.UserName,
                Role = roles[0],
                Roles = roleManager.Roles.ToList()
            };
            return View(mngRoleVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Manage(MngRoleVM mngRoleVM)
        {
            var user = applicationUserRepo.GetOne(filter: e => e.Id == mngRoleVM.Id);
            var roles = await userManager.GetRolesAsync(user);
            await userManager.RemoveFromRolesAsync(user, roles);
            await userManager.AddToRoleAsync(user, mngRoleVM.Role);
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Lock(string id)
        {
            var user = applicationUserRepo.GetOne(filter: e => e.Id == id);
            user.LockoutEnd=DateTime.Now.AddYears(1);
            applicationUserRepo.Attemp();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult UnLock(string id)
        {
            var user = applicationUserRepo.GetOne(filter: e => e.Id == id);
            user.LockoutEnd = null;
            applicationUserRepo.Attemp();
            return RedirectToAction(nameof(Index));
        }
    }
}
