using System;

namespace YUNXI.EDUS.Authorization.Impersonation
{
    [Serializable]
    public class ImpersonationCacheItem
    {
        public const string CacheName = "EDUSImpersonationCache";

        public ImpersonationCacheItem()
        {
        }

        public ImpersonationCacheItem(int? targetTenantId, long targetUserId, bool isBackToImpersonator)
        {
            this.TargetTenantId = targetTenantId;
            this.TargetUserId = targetUserId;
            this.IsBackToImpersonator = isBackToImpersonator;
        }

        public int? ImpersonatorTenantId { get; set; }

        public long ImpersonatorUserId { get; set; }

        public bool IsBackToImpersonator { get; set; }

        public int? TargetTenantId { get; set; }

        public long TargetUserId { get; set; }
    }
}
