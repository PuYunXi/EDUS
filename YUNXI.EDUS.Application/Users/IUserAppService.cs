using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using YUNXI.EDUS.Roles.Dto;
using YUNXI.EDUS.Users.Dto;

namespace YUNXI.EDUS.Users
{
    public interface IUserAppService : IAsyncCrudAppService<UserDto, long, PagedResultRequestDto, CreateUserDto, UpdateUserDto>
    {
        Task<ListResultDto<RoleDto>> GetRoles();
    }
}