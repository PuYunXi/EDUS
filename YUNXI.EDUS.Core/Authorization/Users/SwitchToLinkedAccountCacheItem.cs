using System;

namespace YUNXI.EDUS.Authorization.Users
{
    [Serializable]
    public class SwitchToLinkedAccountCacheItem
    {
        public const string CacheName = "AppSwitchToLinkedAccountCache";

        public SwitchToLinkedAccountCacheItem()
        {
        }

        public SwitchToLinkedAccountCacheItem(
            int? targetTenantId,
            long targetUserId,
            int? impersonatorTenantId,
            long? impersonatorUserId,
            string targetUrl)
        {
            this.TargetTenantId = targetTenantId;
            this.TargetUserId = targetUserId;
            this.ImpersonatorTenantId = impersonatorTenantId;
            this.ImpersonatorUserId = impersonatorUserId;
            this.TargetUrl = targetUrl;
        }

        public int? ImpersonatorTenantId { get; set; }

        public long? ImpersonatorUserId { get; set; }

        public int? TargetTenantId { get; set; }

        public string TargetUrl { get; set; }

        public long TargetUserId { get; set; }
    }
}
