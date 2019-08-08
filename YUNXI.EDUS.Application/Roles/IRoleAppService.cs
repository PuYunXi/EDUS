using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using YUNXI.EDUS.Roles.Dto;

namespace YUNXI.EDUS.Roles
{
    public interface IRoleAppService : IAsyncCrudAppService<RoleDto, int, PagedResultRequestDto, CreateRoleDto, RoleDto>
    {
        Task<ListResultDto<PermissionDto>> GetAllPermissions();
    }
}
