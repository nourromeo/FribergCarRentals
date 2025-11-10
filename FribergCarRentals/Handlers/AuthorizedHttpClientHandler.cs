using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace FribergCarRentals
{
    public class AuthorizedHttpClientHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<AuthorizedHttpClientHandler> _logger;

        public AuthorizedHttpClientHandler(IHttpContextAccessor httpContextAccessor, ILogger<AuthorizedHttpClientHandler> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = _httpContextAccessor.HttpContext?.Session.GetString("JwtToken");
            _logger.LogInformation("AuthorizedHttpClientHandler: token present = {HasToken} | Request: {Method} {Uri}",
                !string.IsNullOrEmpty(token), request.Method, request.RequestUri);

            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                // Log only first/last characters to avoid leaking token in logs
                var shortValue = token.Length > 16 ? token.Substring(0, 8) + "..." + token.Substring(token.Length - 8) : token;
                _logger.LogInformation("AuthorizedHttpClientHandler: Authorization header set: Bearer {ShortToken}", shortValue);
            }
            else
            {
                _logger.LogWarning("AuthorizedHttpClientHandler: No token found in session.");
            }

            var response = await base.SendAsync(request, cancellationToken);

            _logger.LogInformation("AuthorizedHttpClientHandler: Response {StatusCode} for {Method} {Uri}",
                response.StatusCode, request.Method, request.RequestUri);

            // Console line helpful during local debug
            Console.WriteLine($"[Handler] Token present = {!string.IsNullOrEmpty(token)} | {request.Method} {request.RequestUri}");

            return response;
        }
    }
}

