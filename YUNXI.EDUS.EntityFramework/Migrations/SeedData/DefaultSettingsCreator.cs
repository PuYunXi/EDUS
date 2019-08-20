using Abp.Configuration;
using Abp.Localization;
using Abp.Net.Mail;
using System.Linq;
using YUNXI.EDUS.EntityFramework;

namespace YUNXI.EDUS.Migrations.SeedData
{
    public class DefaultSettingsCreator
    {
        private readonly EDUSDbContext _context;

        public DefaultSettingsCreator(EDUSDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            //Emailing
            AddSettingIfNotExists(EmailSettingNames.DefaultFromAddress, "admin@mydomain.com");
            AddSettingIfNotExists(EmailSettingNames.DefaultFromDisplayName, "mydomain.com mailer");

            //Languages
          //  AddSettingIfNotExists(LocalizationSettingNames.DefaultLanguage, "en");
            this.AddSettingIfNotExists(LocalizationSettingNames.DefaultLanguage, "zh-CN");
        }

        private void AddSettingIfNotExists(string name, string value, int? tenantId = null)
        {
            if (_context.Settings.Any(s => s.Name == name && s.TenantId == tenantId && s.UserId == null))
            {
                return;
            }

            _context.Settings.Add(new Setting(tenantId, null, name, value));
            _context.SaveChanges();
        }
    }
}