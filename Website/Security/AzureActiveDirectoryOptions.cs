using System.Globalization;

namespace Website.Security
{
    public class AzureActiveDirectoryOptions
    {
        public string AadInstance { get; set; }
        public string Tenant { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string ApiKey { get; set; }
        public string Authority
        {
            get
            {
                return string.Format(CultureInfo.InvariantCulture, AadInstance, Tenant);
            }
        }
    }
}
