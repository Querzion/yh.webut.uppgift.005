using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Presentation.WebApp.ViewModels;

namespace Presentation.WebApp.Controllers;

public class CookiesController : Controller
{
    #region ChatGPT Restructure of the Teacher Version

        [HttpPost]
        public IActionResult SetCookies([FromBody] CookieConsent consent)
        {
            if (consent == null)
                return BadRequest("Consent data is missing.");

            SetOrDeleteCookie("FunctionalCookie", consent.Functional);
            SetOrDeleteCookie("AnalyticsCookie", consent.Analytics);
            SetOrDeleteCookie("MarketingCookie", consent.Marketing);
            
            Response.Cookies.Append("cookieConsent", JsonSerializer.Serialize(consent), new CookieOptions
            {
                IsEssential = true,
                Expires = DateTimeOffset.UtcNow.AddDays(365),
                SameSite = SameSiteMode.Lax,
                Path = "/"
            });

            return Ok();
        }

        // ↓ This is the helper method you add inside the same controller
        private void SetOrDeleteCookie(string key, bool shouldSet)
        {
            if (shouldSet)
            {
                Response.Cookies.Append(key, "true", new CookieOptions
                {
                    IsEssential = false,
                    Expires = DateTimeOffset.UtcNow.AddDays(30),
                    SameSite = SameSiteMode.Lax,
                    Path = "/"
                    // Secure = true // Optional: use true if you're serving over HTTPS
                });
            }
            else
            {
                Response.Cookies.Delete(key);
            }
        }

    #endregion

    #region Teacher Version

        // [HttpPost]
        // public IActionResult SetCookies([FromBody] CookieConsent consent)
        // {
        //     if (consent == null)
        //         return BadRequest();
        //
        //     if (consent.Functional)
        //     {
        //         Response.Cookies.Append("FunctionalCookie", "Non-Essential", new CookieOptions
        //         {
        //             IsEssential = false,
        //             Expires = DateTimeOffset.UtcNow.AddDays(30),
        //             SameSite = SameSiteMode.Lax,
        //             Path = "/"
        //         });
        //     }
        //     else
        //     {
        //         Response.Cookies.Delete("FunctionalCookie");
        //     }
        //     
        //     if (consent.Analytics)
        //     {
        //         Response.Cookies.Append("AnalyticsCookie", "Non-Essential", new CookieOptions
        //         {
        //             IsEssential = false,
        //             Expires = DateTimeOffset.UtcNow.AddDays(30),
        //             SameSite = SameSiteMode.Lax,
        //             Path = "/"
        //         });
        //     }
        //     else
        //     {
        //         Response.Cookies.Delete("AnalyticsCookie");
        //     }
        //
        //     if (consent.Marketing)
        //     {
        //         Response.Cookies.Append("MarketingCookie", "Non-Essential", new CookieOptions
        //         {
        //             IsEssential = false,
        //             Expires = DateTimeOffset.UtcNow.AddDays(30),
        //             SameSite = SameSiteMode.Lax,
        //             Path = "/"
        //         });
        //     }
        //     else
        //     {
        //         Response.Cookies.Delete("MarketingCookie");
        //     }
        //     
        //     Response.Cookies.Append("cookieConsent", JsonSerializer.Serialize(consent), new CookieOptions
        //     {
        //         Expires = DateTimeOffset.UtcNow.AddDays(90),
        //         SameSite = SameSiteMode.Lax,
        //         Path = "/"
        //     });
        //     
        //     return Ok();
        // }

    #endregion
}