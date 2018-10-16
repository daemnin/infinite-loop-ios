using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Diagnostics;
using System.Security.Claims;
using System.Threading.Tasks;
using Website.Models;
using Website.Security;

namespace Website.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly AzureActiveDirectoryOptions _azureActiverDirectoryOptions;

        public HomeController(IOptions<AzureActiveDirectoryOptions> azureOptions)
        {
            _azureActiverDirectoryOptions = azureOptions.Value;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                AuthenticationResult result = await GetAcessToken();

                ViewData["AccessToken"] = $"{result.AccessTokenType} {result.AccessToken}";
            }
            catch
            {
                ViewData["Message"] = $"Error retriving access token for {_azureActiverDirectoryOptions.ApiKey}";
            }

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private async Task<AuthenticationResult> GetAcessToken()
        {
            var credential = new ClientCredential(_azureActiverDirectoryOptions.ClientId,
                                                  _azureActiverDirectoryOptions.ClientSecret);

            string userObjectId = User.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier");
            var userId = new UserIdentifier(userObjectId, UserIdentifierType.UniqueId);
            var tokenCache = new NaiveSessionCache(HttpContext, userObjectId);
            AuthenticationContext authContext = new AuthenticationContext(_azureActiverDirectoryOptions.Authority, tokenCache);
            var result = await authContext.AcquireTokenSilentAsync(_azureActiverDirectoryOptions.ApiKey, credential, userId);
            return result;
        }
    }
}
