using YUNXI.EDUS.Sessions.Dto;

namespace YUNXI.EDUS.AppSystem.Sessions.Dto
{
    public class GetCurrentLoginInformationsOutputDto
    {
        public TenantLoginInfoDto Tenant { get; set; }

        public UserLoginInfoDto User { get; set; }
    }
}
