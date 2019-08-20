using Abp.Application.Services.Dto;
using Abp.Authorization;
using System.Threading.Tasks;
using System.Web.Mvc;
using YUNXI.EDUS.Authorization;
using YUNXI.EDUS.Authorization.Roles;
using YUNXI.EDUS.Users;
using YUNXI.EDUS.Web.Models.Users;

namespace YUNXI.EDUS.Web.Controllers
{
    [AbpAuthorize(PermissionNames.Pages_Administration_Users)]
    public class UsersController : EDUSControllerBase
    {
        private readonly IUserAppService _userAppService;
        private readonly RoleManager _roleManager;

        public UsersController(IUserAppService userAppService, RoleManager roleManager)
        {
            _userAppService = userAppService;
            _roleManager = roleManager;
        }

        public async Task<ActionResult> Index()
        {
            var users = (await _userAppService.GetAll(new PagedResultRequestDto { MaxResultCount = int.MaxValue })).Items; //Paging not implemented yet
            var roles = (await _userAppService.GetRoles()).Items;
            var model = new UserListViewModel
            {
                Users = users,
                Roles = roles
            };

            return View(model);
        }

        public async Task<ActionResult> EditUserModal(long userId)
        {
            var user = await _userAppService.Get(new EntityDto<long>(userId));
            var roles = (await _userAppService.GetRoles()).Items;
            var model = new EditUserModalViewModel
            {
                User = user,
                Roles = roles
            };
            return View("_EditUserModal", model);
        }
    }
}