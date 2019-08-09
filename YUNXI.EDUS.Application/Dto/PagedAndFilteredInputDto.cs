using System.ComponentModel.DataAnnotations;

namespace YUNXI.EDUS.Dto
{
    public class PagedAndFilteredInputDto : IDatatablesPagedResultRequest
    {
        public PagedAndFilteredInputDto()
        {
            this.Length = AppConsts.DefaultPageSize;
        }

        [Range(1, AppConsts.MaxPageSize)]
        public int Length { get; set; }

        public int MaxResultCount { get; set; }

        public DatatablesSearchInputDto Search { get; set; }

        public int SkipCount { get; set; }

        [Range(0, int.MaxValue)]
        public int Start { get; set; }
    }
}
