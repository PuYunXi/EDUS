using System.Collections.Generic;
using YUNXI.EDUS.Roles.Dto;
using YUNXI.EDUS.Users.Dto;

namespace YUNXI.EDUS.Web.Models.Users
{
    public class UserListViewModel
    {
        public IReadOnlyList<UserDto> Users { get; set; }

        public IReadOnlyList<RoleDto> Roles { get; set; }
    }
}