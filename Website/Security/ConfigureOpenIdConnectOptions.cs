using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Website.Security
{
    public class ConfigureOpenIdConnectOptions : IConfigureNamedOptions<OpenIdConnectOptions>
    {
        private readonly AzureActiveDirectoryOptions _azureActiveDirectoryOptions;
        public ConfigureOpenIdConnectOptions(IOptions<AzureActiveDirectoryOptions> aadOpitons)
        {
            _azureActiveDirectoryOptions = aadOpitons.Value;
        }

        public void Configure(string name, OpenIdConnectOptions options)
        {
            options.ClientId = _azureActiveDirectoryOptions.ClientId;
            options.Authority = _azureActiveDirectoryOptions.Authority;
            options.ClientSecret = _azureActiveDirectoryOptions.ClientSecret;
            options.TokenValidationParameters.ValidateIssuer = false;
            options.ResponseType = OpenIdConnectResponseType.CodeIdToken;

            options.Events = new OpenIdConnectEvents
            {
                OnAuthorizationCodeReceived = async context =>
                {
                    context.HandleResponse();
                    await AcquireApiToken(context, _azureActiveDirectoryOptions);
                    context.Success();
                },
                OnAuthenticationFailed = context =>
                {
                    context.HandleResponse();

                    context.Response.Redirect("/Account/AccessDenied");

                    return Task.CompletedTask;
                },
                OnRemoteFailure = context =>
                {
                    context.HandleResponse();

                    context.Response.Redirect("/Account/AccessDenied");

                    return Task.CompletedTask;
                }
            };
        }

        public void Configure(OpenIdConnectOptions options)
        {
            Configure(Options.DefaultName, options);
        }

        private async Task AcquireApiToken(AuthorizationCodeReceivedContext context, AzureActiveDirectoryOptions options)
        {
            ClientCredential credential = new ClientCredential(options.ClientId, options.ClientSecret);
            string userObjectId = context.Principal.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier");
            AuthenticationContext authContext = new AuthenticationContext(options.Authority, new NaiveSessionCache(context.HttpContext, userObjectId));
            Uri requestUrl = new Uri(context.Request.GetDisplayUrl());
            var result = await authContext.AcquireTokenByAuthorizationCodeAsync(context.TokenEndpointRequest.Code, requestUrl, credential, options.ApiKey);
            context.HandleCodeRedemption(result.AccessToken, result.IdToken);
            context.Success();
        }
    }
}