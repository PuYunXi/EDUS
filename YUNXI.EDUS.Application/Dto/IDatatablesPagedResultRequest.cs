using Abp.Application.Services.Dto;

namespace YUNXI.EDUS.Dto
{
    public interface IDatatablesPagedResultRequest : IPagedResultRequest
    {
        int Length { get; set; }

        int Start { get; set; }
    }
}
