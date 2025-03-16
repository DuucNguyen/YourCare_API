using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;
using RTools_NTS.Util;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using YourCare_BOs;
using YourCare_DAOs.DAOs;
using YourCare_Repos.Interfaces;
using YourCare_WebApi.Models.Auth;

namespace YourCare_WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly Appsettings _appsetting;
        private readonly IRoleRepository _roleRepository;
        private readonly IConfiguration _configuration;

        public AuthenticationController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender,
            IOptionsMonitor<Appsettings> appsettings,
            IRoleRepository roleRepository,
            RoleManager<ApplicationRole> roleManager,
            IConfiguration configuration

            )
        {
            _emailSender = emailSender;
            _userManager = userManager;
            _appsetting = appsettings.CurrentValue;
            _signInManager = signInManager;
            _roleRepository = roleRepository;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        public class EmailRequestModel
        {
            public string UserId { get; set; }
            public string Code { get; set; }
            public string? Password { get; set; }
        }

        public class CreatePasswordModel
        {
            public string UserId { get; set; }
            public string Password { get; set; }
        }

        public class CreateProfileModel
        {
            public string FullName { get; set; }
            public DateTime Dob { get; set; }
            public string PhoneNumber { get; set; }

            public bool Gender { get; set; }
        }


        public class ChangePasswordModel
        {
            public string userID { get; set; }
            public string currentPassword { get; set; }
            public string newPassword { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> SendEmailRegister([FromBody] string email)
        {
            try
            {
                var isExist = await _userManager.FindByEmailAsync(email);

                if (isExist != null)
                {
                    return new JsonResult(new ResponseModel<string>
                    {
                        StatusCode = StatusCodes.Status403Forbidden,
                        IsSucceeded = false,
                        Message = "Email has been used."
                    });
                }

                ApplicationUser newUser = new ApplicationUser
                {
                    Email = email,
                    UserName = email,
                    NormalizedEmail = email.ToUpper(),
                    NormalizedUserName = email.ToUpper(),

                    Gender = true,
                    PhoneNumber = "00000000000",
                    Dob = DateTime.Now,
                    FullName = "tempUser",
                    IsActive = true
                };

                var addResult = await _userManager.CreateAsync(newUser);
                if (!addResult.Succeeded)
                {
                    return new JsonResult(new ResponseModel<string>
                    {
                        StatusCode = StatusCodes.Status500InternalServerError,
                        Message = "Account created failed! Please try again !",
                        IsSucceeded = false
                    });
                }

                //send email confirmation
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                var confirmationLink = $"{_configuration.GetValue<string>("Client:Scheme")}" +
                    $"://{_configuration.GetValue<string>("Client:Host")}/create-password" +
                    $"/{newUser.Id}/{code}";

                #region email body 
                var htmlBody_1 =
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
                    $"'{confirmationLink}' class='button'>Confirm Email</a></div><p>If the button doesn't work, you click the following link:" +
                    $"</p><p><a href = '{confirmationLink}' >Click here</a></p></div>";

                var htmlBody_2 = @"<div class='footer'>
                            <p>If you didn't create an account, please ignore this email.</p>
                            <p>&copy; 2024 YourCare. All rights reserved.</p>
                            </div>
                            </div>
                            </body>
                            </html>";
                #endregion

                var htmlBody = htmlBody_1 + htmlBodyLink + htmlBody_2;
                await _emailSender.SendEmailAsync(email, "YourCare - Confirm your account !", htmlBody);

                return new JsonResult(new ResponseModel<string>
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "The email for confirmation has been sent. Please check your email.",
                    IsSucceeded = true,
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new ResponseModel<string>
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "Internal server error. Please try again.",
                    IsSucceeded = false,
                });
            }
        }


        [HttpPost]
        public async Task<IActionResult> SendEmailForgotPassword([FromBody] string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);

                if (user == null)
                {
                    return new JsonResult(new ResponseModel<string>
                    {
                        StatusCode = StatusCodes.Status403Forbidden,
                        IsSucceeded = false,
                        Message = "Không tìm thấy email trong hệ thống."
                    });
                }

                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));


                var confirmationLink = $"{_configuration.GetValue<string>("Client:Scheme")}" +
                    $"://{_configuration.GetValue<string>("Client:Host")}/reset-password" +
                    $"/{user.Id}/{code}";

                #region email body 
                var htmlBody_1 =
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
                                <h2>Confirmation to reset your password</h2>
                                <p>Hello,</p>
                                <p>You have requested a reset password <strong>YourCare</strong>. If that wasn't you, please ignore this email. If you actually want to change your password, click this:</p>";

                var htmlBodyLink = $"<div class='button-container'><a href = " +
                    $"'{confirmationLink}' class='button'>Thay đổi mật khẩu</a></div><p>If the button doesn't work, you click the following link:" +
                    $"</p><p><a href = '{confirmationLink}' >Click here</a></p></div>";

                var htmlBody_2 = @"<div class='footer'>
                            <p>If you didn't request to chagne your password, please ignore this email.</p>
                            <p>&copy; 2024 YourCare. All rights reserved.</p>
                            </div>
                            </div>
                            </body>
                            </html>";
                #endregion

                var htmlBody = htmlBody_1 + htmlBodyLink + htmlBody_2;
                await _emailSender.SendEmailAsync(email, "YourCare - Reset your password !", htmlBody);

                return new JsonResult(new ResponseModel<string>
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Yêu cầu đã được gửi. Vui lòng kiểm tra Email.",
                    IsSucceeded = true,
                });

            }
            catch (Exception ex)
            {
                return new JsonResult(new ResponseModel<string>
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "Lỗi. Vui lòng thử lại sau",
                    IsSucceeded = false,
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword([FromBody] EmailRequestModel request)
        {
            try
            {
                var code = request.Code;
                code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));

                var user = await _userManager.FindByIdAsync(request.UserId);

                if (user == null)
                {
                    return new JsonResult(new ResponseModel<string>
                    {
                        StatusCode = StatusCodes.Status500InternalServerError,
                        Message = "Lỗi. Người dùng không tồn tại",
                        IsSucceeded = false,
                    });
                }

                await _userManager.ResetPasswordAsync(user, code, request.Password);
                return new JsonResult(new ResponseModel<string>
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Thay đổi mật khẩu thành công.",
                    IsSucceeded = true,
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new ResponseModel<string>
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "Lỗi. Vui lòng thử lại sau",
                    IsSucceeded = false,
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmEmail([FromBody] EmailRequestModel request)
        {
            var userId = request.UserId;
            var code = request.Code;

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
        public async Task<IActionResult> CreatePassword([FromBody] CreatePasswordModel request)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(request.UserId);
                if (user == null)
                {
                    return new JsonResult(new ResponseModel<string>
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = "User not found.",
                        IsSucceeded = false,
                    });
                }
                var result = await _userManager.AddPasswordAsync(user, request.Password);

                return new JsonResult(new ResponseModel<string>
                {
                    StatusCode = result.Succeeded ? StatusCodes.Status200OK : StatusCodes.Status500InternalServerError,
                    Message = result.Succeeded ? "Create password successfully." : "Invalid password.",
                    IsSucceeded = result.Succeeded,
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new ResponseModel<string>
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "Some thing went wrong.",
                    IsSucceeded = false,
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel request)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(request.userID);
                var result = await _userManager.ChangePasswordAsync(user, request.currentPassword, request.newPassword);

                return new JsonResult(new ResponseModel<string>
                {
                    StatusCode = result.Succeeded ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest,
                    Message = result.Succeeded ? "Change password successful" : "Change password failed",
                    IsSucceeded = result.Succeeded,
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + " - " + ex.StackTrace);
                return new JsonResult(new ResponseModel<string>
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "Change password failed",
                    IsSucceeded = false,
                });
            }
        }


        [HttpPost]
        public async Task<IActionResult> CreateProfile(string userId, [FromBody] CreateProfileModel request)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return new JsonResult(new ResponseModel<string>
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = "User not found.",
                        IsSucceeded = false,
                    });
                }

                user.FullName = request.FullName;
                user.Dob = request.Dob;
                user.Gender = request.Gender;
                user.PhoneNumber = request.PhoneNumber;

                var result = await _userManager.UpdateAsync(user);
                return new JsonResult(new ResponseModel<string>
                {
                    StatusCode = result.Succeeded ? StatusCodes.Status200OK : StatusCodes.Status500InternalServerError,
                    Message = result.Succeeded ? "Create profile successfully." : "Invalid request",
                    IsSucceeded = result.Succeeded,
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

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginModel loginRequest)
        {
            var user = await _userManager.FindByEmailAsync(loginRequest.Username);
            if (user == null)
            {
                return new JsonResult(new ResponseModel<string>
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = "Incorrect Username or Password.",
                    IsSucceeded = false,
                });
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, set lockoutOnFailure: true
            var result = await _signInManager.PasswordSignInAsync(user, loginRequest.Password, false, lockoutOnFailure: false);
            if (!result.Succeeded)
            {
                return new JsonResult(new ResponseModel<string>
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Incorrect Username or Password.",
                    IsSucceeded = false,
                });
            }

            var authClaims = await GetClaims(user);
            var token = await GenerateToken(user, authClaims);

            List<string> roleNames = (List<string>)await _userManager.GetRolesAsync(user);
            var roleIDs = _roleRepository.GetRoleIDsByName(roleNames);
            var jsonData = JsonSerializer.Serialize(new
            {
                Username = user.FullName,
                Claims = _roleRepository.GetRoleClaimsByRoles(roleIDs)
            });

            return new JsonResult(new ResponseModel<string>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Login Successfully.",
                IsSucceeded = true,
                Data = jsonData
            });
        }

        [HttpGet]
        public IActionResult GoogleLogin()
        {
            var redirectUrl = Url.Action(nameof(GoogleLoginCallback), "Authentication", null, Request.Scheme);
            var properties = new AuthenticationProperties
            {
                RedirectUri = redirectUrl
            };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet]
        public async Task<IActionResult> GoogleLoginCallback()
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return NotFound();
            }
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: true, bypassTwoFactor: true);

            if (result.Succeeded)
            {
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                var user = await _userManager.FindByEmailAsync(email);

                if (user == null)
                {
                    return NotFound();
                }
                var authClaims = await GetClaims(user);
                await GenerateToken(user, authClaims);

                List<string> roleNames = (List<string>)await _userManager.GetRolesAsync(user);
                var roleIDs = _roleRepository.GetRoleIDsByName(roleNames);
                var jsonData = JsonSerializer.Serialize(new
                {
                    Username = user.FullName,
                    Claims = _roleRepository.GetRoleClaimsByRoles(roleIDs)
                });

                return new JsonResult(new ResponseModel<string>
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = $"{info.LoginProvider} Login Successfully.",
                    IsSucceeded = true,
                    Data = jsonData
                });
            }

            return new JsonResult(new ResponseModel<string>
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = "Login failed.",
                IsSucceeded = false,
            });
        }

        [HttpPost]
        public async Task<IActionResult> RenewTokens()
        {
            try
            {
                var refreshToken = HttpContext.Request.Cookies["refresh_token"];
                if (string.IsNullOrEmpty(refreshToken))
                {
                    return new JsonResult(new ResponseModel<string>
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "Token is missing.",
                        IsSucceeded = false,
                    });
                }

                var principal = GetPrincipalFromExpiredToken(refreshToken);
                if (principal == null)
                {
                    return new JsonResult(new ResponseModel<string>
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "Invalid token.",
                        IsSucceeded = false,
                    });
                }

                //issue new token
                var user = await _userManager.FindByNameAsync(principal.Identity.Name);
                var authClaims = await GetClaims(user);

                List<string> roleNames = (List<string>)await _userManager.GetRolesAsync(user);
                var roleIDs = _roleRepository.GetRoleIDsByName(roleNames);
                var jsonData = JsonSerializer.Serialize(new
                {
                    Username = user.FullName,
                    Claims = _roleRepository.GetRoleClaimsByRoles(roleIDs)
                });

                var newToken = await GenerateToken(user, authClaims);

                WriteDataToCookie(newToken.AccessToken, newToken.RefreshToken, jsonData);

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
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "Something went wrong: " + ex.Message + " - " + ex.StackTrace,
                    IsSucceeded = false,
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetLoginProviders()
        {
            try
            {
                var externalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

                var result = externalLogins.Select(x => new
                {
                    x.Name,
                    x.DisplayName,
                });

                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "-" + ex.StackTrace);
                return new JsonResult(new ResponseModel<List<string>>
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "Get external login failed",
                    IsSucceeded = false,
                });
            }
        }

        private async Task<TokenModel> GenerateToken(ApplicationUser user, List<Claim> authClaims)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            var access_token = CreateToken(authClaims, 1);
            var accessToken = jwtTokenHandler.WriteToken(access_token);

            var refresh_token = CreateToken(new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            }, DateTime.Now.AddDays(7).Minute);

            var refreshToken = jwtTokenHandler.WriteToken(refresh_token);


            List<string> roleNames = (List<string>)await _userManager.GetRolesAsync(user);
            var roleIDs = _roleRepository.GetRoleIDsByName(roleNames);

            var jsonData = JsonSerializer.Serialize(new
            {
                Username = user.FullName,
                UserID = user.Id,
                Claims = _roleRepository.GetRoleClaimsByRoles(roleIDs)
            });

            WriteDataToCookie(accessToken, refreshToken, jsonData);

            return new TokenModel
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
            };
            //bao thang Vue lay token ve r moi send request renew
        }

        private JwtSecurityToken CreateToken(List<Claim> authClaims, int minutes)
        {
            var secretKey = _appsetting.SecretKey;
            var secretKeyBytes = Encoding.UTF8.GetBytes(secretKey);

            var token = new JwtSecurityToken(
                claims: authClaims,
                expires: DateTime.UtcNow.AddMinutes(minutes),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(secretKeyBytes),
                                                            SecurityAlgorithms.HmacSha256Signature)
                );

            return token;
        }
        private async Task<List<Claim>> GetClaims(ApplicationUser user)
        {
            if (user == null) throw new Exception("user not found.");
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var userRoles = await _userManager.GetRolesAsync(user);

            foreach (var role in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            var roleClaims = await _roleRepository.GetRoleClaimByUserID(user.Id);
            if (roleClaims.Any())
            {
                authClaims.AddRange(roleClaims);
            }

            return authClaims;
        }
        private ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token)
        {
            var secretKey = _appsetting.SecretKey;
            var secretKeyBytes = Encoding.UTF8.GetBytes(secretKey);

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(secretKeyBytes),
                ValidateLifetime = true
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256Signature))
                throw new SecurityTokenException("Invalid token");

            return principal;
        }
        private void WriteDataToCookie(string access_token, string refresh_token, string userData)
        {
            HttpContext.Response.Cookies.Append("access_token", access_token,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None, //Allow cross origin cookies
                                              //Expires = DateTime.UtcNow.AddDays(3)
                Expires = DateTime.UtcNow.AddMinutes(1) //temp
            });

            HttpContext.Response.Cookies.Append("refresh_token", refresh_token,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None, //Allow cross origin cookies
                                              //Expires = DateTime.UtcNow.AddDays(3)
                Expires = DateTime.UtcNow.AddMinutes(600) //temp
            });

            HttpContext.Response.Cookies.Append("user", userData,
            new CookieOptions
            {
                HttpOnly = false,  // Allows access via JavaScript
                Secure = true,
                SameSite = SameSiteMode.None, //Allow cross origin cookies
                                              //Expires = DateTime.UtcNow.AddDays(3)
                Expires = DateTime.UtcNow.AddMinutes(600) //temp
            });
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
