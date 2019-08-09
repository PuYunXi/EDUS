using Abp.Domain.Services;

namespace YUNXI.EDUS
{
    public abstract class   EDUSDomainServiceBase : DomainService
    {
        protected EDUSDomainServiceBase()
        {
            this.LocalizationSourceName = CoreConsts.LocalizationSourceName;
        }
    }
}
