using System;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;


namespace DockerStats
{

	public class AuthConfig
	{
		public string ApiKey { get; set; }
	}



	public class ApiKeyAuthenticationHandler(
		IOptionsMonitor<AuthenticationSchemeOptions> options,
		ILoggerFactory logger,
		UrlEncoder encoder,
		IOptions<AuthConfig> config) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
	{
		const string APIKEY_HEADER = "APIKey";

		private readonly string _apiKey = config.Value?.ApiKey;

		protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
		{
			if (!string.IsNullOrWhiteSpace(_apiKey))
			{
				var authHeader = Context.Request.Headers[APIKEY_HEADER];

				if (authHeader != _apiKey)
					return AuthenticateResult.NoResult();
			}

			var identity = new ClaimsIdentity([], Scheme.Name);
			var principal = new ClaimsPrincipal(identity);
			var ticket = new AuthenticationTicket(principal, Scheme.Name);

			return AuthenticateResult.Success(ticket);
		}
	}

}
