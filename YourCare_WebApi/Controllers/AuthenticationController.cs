using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using YourCare_BOs;
using YourCare_WebApi.Models.Auth;

namespace YourCare_WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly Appsettings _appsetting;

        public AuthenticationController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender,
            IOptionsMonitor<Appsettings> appsettings
            )
        {
            _emailSender = emailSender;
            _userManager = userManager;
            _appsetting = appsettings.CurrentValue;
            _signInManager = signInManager;
        }


        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterModel registerUser)
        {

            var isExist = await _userManager.FindByEmailAsync(registerUser.Email);
            if (isExist != null)
            {
                return new JsonResult(new ResponseModel<string>
                {
                    StatusCode = StatusCodes.Status403Forbidden,
                    IsSucceeded = false,
                    Message = "User already exist !"
                });
            }

            ApplicationUser newUser = new ApplicationUser
            {
                Email = registerUser.Email,
                UserName = registerUser.Email,
                NormalizedEmail = registerUser.Email.ToUpper(),
                NormalizedUserName = registerUser.Email.ToUpper(),

                Gender = registerUser.Gender,
                PhoneNumber = registerUser.PhoneNumber,
                Dob = registerUser.Dob,
                FullName = registerUser.FullName,
                IsActive = true
            };

            var addResult = await _userManager.CreateAsync(newUser, registerUser.Password);
            if (!addResult.Succeeded)
            {
                return new JsonResult(new ResponseModel<string>
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "Account created failed! Pls try again !",
                    IsSucceeded = false
                });
            }

            var result = new JsonResult(new ResponseModel<string>
            {
                StatusCode = StatusCodes.Status201Created,
                Message = "Account created successfully. Pls check your email for confirmation !",
                IsSucceeded = true
            });

            //send email confirmation
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            var confirmationLink = Url.Action(
                "ConfirmEmail",
                "Authentication",
                new
                {
                    userId = newUser.Id,
                    code = code
                },
                Request.Scheme);

            #region email body 
            var htmlTemplate =
             @"<!DOCTYPE html>
                        <html lang='en'>
                        <head>
                        <meta charset='UTF-8'>
                        <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                        <title>Email Confirmation</title>
                        <style>
                            body {
                                font-family: Arial, sans-serif;
                                background-color: #f4f4f4;
                                margin: 0;
                                padding: 0;
                            }
                            .container {
                                width: 100%;
                                max-width: 600px;
                                margin: 0 auto;
                                background-color: #ffffff;
                                border-radius: 8px;
                                overflow: hidden;
                                box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
                            }
                            .header {
                                background-color: #2c7a7b;
                                color: #ffffff;
                                text-align: center;
                                padding: 20px;
                            }
                            .header h1 {
                                margin: 0;
                                font-size: 24px;
                            }
                            .content {
                                padding: 30px;
                                color: #333;
                            }
                            .content h2 {
                                color: #2c7a7b;
                            }
                            .content p {
                                line-height: 1.6;
                                margin-bottom: 20px;
                            }
                            .button-container {
                                text-align: center;
                                margin-top: 20px;
                            }
                            .button {
                                background-color: #3182ce;
                                text-decoration: none;
                                padding: 15px 25px;
                                border-radius: 5px;
                                font-size: 18px;
                                display: inline-block;
                                color: #fff !important;
                            }
                            .button a{
                                color: #fff !important;
                            }
                            .footer {
                                text-align: center;
                                padding: 20px;
                                font-size: 12px;
                                background-color: #f4f4f4;
                                color: #777;
                            }
                            .footer a {
                                color: #3182ce;
                                text-decoration: none;
                            }
                        </style>
                    </head>
                    <body>
                        <div class='container'>
                            <div class='header'>
                                <h1>Welcome to YourCare!</h1>
                            </div>
                            <div class='content'>
                                <h2>Confirm Your Email</h2>
                                <p>Hello,</p>
                                <p>Thank you for signing up with <strong>YourCare</strong>. To complete your registration and activate your account, please confirm your email address by clicking the button below:</p>";

            var htmlBodyLink = $"<div class='button-container'><a href = " +
                $"'{HtmlEncoder.Default.Encode(confirmationLink)}' class='button'>Confirm Email</a></div><p>If the button doesn't work, you click the following link:" +
                $"</p><p><a href = '{HtmlEncoder.Default.Encode(confirmationLink)}' >Click here</a></p></div>";

            var htmlBody_2 = @"<div class='footer'>
                            <p>If you didn't create an account, please ignore this email.</p>
                            <p>&copy; 2024 YourCare. All rights reserved.</p>
                            </div>
                            </div>
                            </body>
                            </html>";
            #endregion

            var htmlBody = htmlTemplate + htmlBodyLink + htmlBody_2;

            await _emailSender.SendEmailAsync(registerUser.Email, "YourCare - Confirm your account !", htmlBody);


            //redirect user to RegisterConfirmation page
            //if (_userManager.Options.SignIn.RequireConfirmedAccount)
            //{
            //    return RedirectToPage("RegisterConfirmation", new { email = registerUser.Email });
            //}

            return new JsonResult(result);
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return BadRequest("Invalid request !");
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User doesnot exist !");
            }

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ConfirmEmailAsync(user, code);

            return new JsonResult(new ResponseModel<string>
            {
                StatusCode = result.Succeeded ? StatusCodes.Status202Accepted : StatusCodes.Status500InternalServerError,
                IsSucceeded = result.Succeeded ? true : false,
                Message = result.Succeeded ? "Thank you for confirming your email. Now you can Login and enjoy our services !" : "Error confirming your email.",
                Data = result.ToString()
            });
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginModel loginRequest)
        {
            var user = await _userManager.FindByEmailAsync(loginRequest.Username);
            if (user == null)
            {
                return new JsonResult(new ResponseModel<string>
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = "User not found !",
                    IsSucceeded = false,
                });
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, set lockoutOnFailure: true
            var result = await _signInManager.PasswordSignInAsync(user, loginRequest.Password, false, lockoutOnFailure: false);
            if(!result.Succeeded)
            {
                return new JsonResult(new ResponseModel<string>
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Username or Password is incorrect.",
                    IsSucceeded = false,
                });
            }

            var token = await GenerateToken(user);
            return new JsonResult(new ResponseModel<TokenModel>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Login Successfully !",
                IsSucceeded = true,
                Data = token
            });
        }


        private async Task<TokenModel> GenerateToken(ApplicationUser user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var secretKey = _appsetting.SecretKey;
            var secretKeyBytes = Encoding.UTF8.GetBytes(secretKey);

            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new System.Security.Claims.ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.FullName),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim("UserName", user.UserName),
                    //roles

                    new Claim("TokenId", Guid.NewGuid().ToString())

                }),
                Expires = DateTime.UtcNow.AddSeconds(20),//temp
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKeyBytes)
                , SecurityAlgorithms.HmacSha256Signature)
            };
            var token = jwtTokenHandler.CreateToken(tokenDescription);
            var accessToken = jwtTokenHandler.WriteToken(token);
            var refreshToken = GenerateRefreshToken();

            //save token in cookie
            HttpContext.Response.Cookies.Append("accessToken", accessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddMinutes(10)
            });

            var refreshTokenModel = new RefreshTokenModel
            {
                UserID = user.Id,
                Token = refreshToken,
                JwtID = token.Id,
                IsUsed = false,
                IsRevoked = false,
                IssuedDate = DateTime.UtcNow,
                //Expires = DateTime.UtcNow.AddDays(3)
                ExpireDate = DateTime.UtcNow.AddMinutes(5), //temp
            };

            WriteTokenToCookie(refreshTokenModel);

            return new TokenModel
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
            };

            //bao thang Vue lay token ve r moi send request renew
        }

        private string GenerateRefreshToken()
        {
            var random = new byte[32];
            try
            {
                using (var rnd = RandomNumberGenerator.Create())
                {
                    rnd.GetBytes(random);
                    return Convert.ToBase64String(random);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: GenereateRefreshToken");
            }
            return null;
        }

        [HttpPost]
        public async Task<IActionResult> RenewTokens(TokenModel token)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var secretKey = _appsetting.SecretKey;
            var secretKeyBytes = Encoding.UTF8.GetBytes(secretKey);


            var tokenValidatationParameter = new TokenValidationParameters //same as configured
            {
                ValidateIssuer = false,

                ValidateAudience = false,

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(secretKeyBytes),

                ClockSkew = TimeSpan.Zero,

                ValidateLifetime = false, //disable check exprire time 
                //if true and token expired -> throw exception
            };

            try
            {
                //check 1: check if accessToken is in correct format
                var tokenVerification = jwtTokenHandler.ValidateToken(token.AccessToken,
                    tokenValidatationParameter, out var validatedAccessToken);

                //check 2: Check Algorithm
                if (validatedAccessToken is JwtSecurityToken jwtSecurityToken)
                {
                    var result = jwtSecurityToken.Header.Alg
                        .Equals(SecurityAlgorithms.HmacSha256
                        , StringComparison.OrdinalIgnoreCase
                        );

                    if (!result)
                    {
                        return new JsonResult(new ResponseModel<string>
                        {
                            StatusCode = StatusCodes.Status200OK,
                            Message = "Invalid Token.",
                            IsSucceeded = false,
                        });
                    }
                }

                //Check 3: Check if AccessToken expired
                var utcExpireDate = long.Parse(tokenVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value.ToString());

                var expireDate = ConvertUnixTimeToDateTime(utcExpireDate);
                if (expireDate > DateTime.UtcNow)
                {
                    return new JsonResult(new ResponseModel<string>
                    {
                        StatusCode = StatusCodes.Status200OK,
                        Message = "Token has not expired yet.",
                        IsSucceeded = false,
                    });
                }

                //Check 4: check if refresh token is in cookie
                var refreshTokenInCookie = HttpContext.Request.Cookies["refreshToken"];
                if (string.IsNullOrEmpty(refreshTokenInCookie))
                {
                    return new JsonResult(new ResponseModel<string>
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = "Refresh token is missing",
                        IsSucceeded = false,
                    });
                }

                //Check 5: check if refresh token has been used or revoked
                var refreshTokenInCookieModel = JsonSerializer.Deserialize<RefreshTokenModel>(refreshTokenInCookie);
                if (refreshTokenInCookieModel == null)
                {
                    return new JsonResult(new ResponseModel<string>
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "Cannot convert refresh token model.",
                        IsSucceeded = false,
                    });
                }
                if (refreshTokenInCookieModel.IsUsed)
                {
                    return new JsonResult(new ResponseModel<string>
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = "Refresh token has been used.",
                        IsSucceeded = false,
                    });
                }
                if (refreshTokenInCookieModel.IsRevoked)
                {
                    return new JsonResult(new ResponseModel<string>
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = "Refresh token has been revoked.",
                        IsSucceeded = false,
                    });
                }

                //Check 6: if accesstoken is match
                var jti = tokenVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti)?.Value.ToString();
                if (refreshTokenInCookieModel.JwtID != jti)
                {
                    return new JsonResult(new ResponseModel<string>
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = "AccessToken does not match",
                        IsSucceeded = false,
                    });
                }


                refreshTokenInCookieModel.IsUsed = true;
                refreshTokenInCookieModel.IsRevoked = true;
                WriteTokenToCookie(refreshTokenInCookieModel);

                //issue new token
                var user = await _userManager.FindByIdAsync(refreshTokenInCookieModel.UserID);
                var newToken = await GenerateToken(user);

                return new JsonResult(new ResponseModel<TokenModel>
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Renew tokens successfully",
                    IsSucceeded = true,
                    Data = newToken
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new ResponseModel<string>
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Something went wrong.",
                    IsSucceeded = false,
                });
            }
        }

        private void WriteTokenToCookie(RefreshTokenModel token)
        {
            try
            {
                HttpContext.Response.Cookies.Append("refreshToken", JsonSerializer.Serialize(token),
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    //Expires = DateTime.UtcNow.AddDays(3)
                    Expires = DateTime.UtcNow.AddMinutes(5) //temp
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: WriteTokenToCookie ");
            }
        }

        private DateTime ConvertUnixTimeToDateTime(long utcExpireDate)
        {
            var dateTimeInterval = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            try
            {
                dateTimeInterval.AddSeconds(utcExpireDate).ToUniversalTime();
            }
            catch (Exception ex)
            {
                Console.WriteLine("convert : " + ex.Message + " -> " + ex.StackTrace);
            }
            return dateTimeInterval;
        }
    }
}
