using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace com.organo.x4ever.Models.Authentication
{
    public interface IAuthenticationResult
    {
        /// <summary>
        ///     Gets the Access Token requested.
        /// </summary>
        string AccessToken { get; set; }

        /// <summary>
        ///     Gets the point in time in which the Access Token returned in the AccessToken property
        ///     ceases to be valid. This value is calculated based on current UTC time measured locally
        ///     and the value expiresIn received from the service.
        /// </summary>
        DateTimeOffset ExpiresOn { get; set; }

        /// <summary>
        ///     Gives information to the developer whether token returned is during normal or extended lifetime.
        /// </summary>
        bool ExtendedLifeTimeToken { get; set; }

        /// <summary>
        ///     Gets user information including user Id. Some elements in UserInfo might be null if not
        ///     returned by the service.
        /// </summary>
        UserInfo UserInfo { get; set; }
    }
}