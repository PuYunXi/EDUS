using Abp.Application.Services.Dto;

namespace YUNXI.EDUS.Dto
{
    public  class PagedInputDto : IPagedResultRequest
    {
        public int MaxResultCount { get; set; }

        public int SkipCount { get; set; }
    }
}
