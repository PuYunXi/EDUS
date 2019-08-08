using System.Collections.Generic;
using YUNXI.EDUS.Roles.Dto;

namespace YUNXI.EDUS.Web.Models.Roles
{
    public class RoleListViewModel
    {
        public IReadOnlyList<RoleDto> Roles { get; set; }

        public IReadOnlyList<PermissionDto> Permissions { get; set; }
    }
}