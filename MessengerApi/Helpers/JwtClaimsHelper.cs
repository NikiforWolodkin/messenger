using System.Security.Claims;
using System.Security.Principal;

namespace MessengerApi.Helpers
{
    public class JwtClaimsHelper
    {
        /// <summary>
        /// Gets the id from jwt token claims.
        /// </summary>
        public static Guid GetId(IIdentity identity)
        {
            var id = ((ClaimsIdentity)identity).Claims
                .Where(c => c.Type == ClaimTypes.NameIdentifier)
                .Select(c => c.Value)
                .First();

            return new Guid(id);
        }
    }
}
