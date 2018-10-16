using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;

namespace Website.Security
{
    public static class AuthServiceConfigExtensions
    {
        public static AuthenticationBuilder AddOpenIdConnectAuthentication(this AuthenticationBuilder builder)
            => builder.AddOpenIdConnectAuthentication(_ => { });

        public static AuthenticationBuilder AddOpenIdConnectAuthentication(this AuthenticationBuilder builder, Action<AzureActiveDirectoryOptions> options)
        {
            builder.Services.Configure(options);
            builder.Services.AddSingleton<IConfigureOptions<OpenIdConnectOptions>, ConfigureOpenIdConnectOptions>();
            builder.AddOpenIdConnect();
            return builder;
        }
    }
}