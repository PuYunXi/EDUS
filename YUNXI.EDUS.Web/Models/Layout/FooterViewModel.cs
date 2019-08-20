using YUNXI.EDUS.AppSystem.Sessions.Dto;

namespace YUNXI.EDUS.Web.Models.Layout
{
    public class FooterViewModel
    {
        public GetCurrentLoginInformationsOutputDto LoginInformations { get; set; }

        public string GetProductNameWithEdition()
        {
            var productName = "WIMI BTL";

            if (this.LoginInformations.Tenant != null && this.LoginInformations.Tenant.EditionDisplayName != null)
            {
                productName += " " + this.LoginInformations.Tenant.EditionDisplayName;
            }

            return productName;
        }
    }
}