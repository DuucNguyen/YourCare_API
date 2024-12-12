using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using YourCare_BOs;
using YourCare_DAOs;
using YourCare_WebApi.Models.Auth;
using YourCare_WebApi.Services.EmailSender;

namespace YourCare_WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();

            #region dbContext + identity
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConStr"),
                sqlOptions => sqlOptions.MigrationsAssembly("YourCare_WebApi")
            ));

            builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;
                options.User.RequireUniqueEmail = true;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();
            #endregion

            #region scope
            builder.Services.AddScoped<ApplicationDbContext>();
            builder.Services.AddTransient<ApplicationDbContext>();
            #endregion

            #region EmailService
            IConfiguration _configuration = builder.Configuration;

            builder.Services.Configure<MailKitEmailSenderOptions>(
                _configuration.GetSection("MailKit:SMTP"));

            builder.Services.AddTransient<IEmailSender, MailKitEmailSender>();

            // config inactive cookie timeout
            builder.Services.ConfigureApplicationCookie(o =>
            {
                o.ExpireTimeSpan = TimeSpan.FromDays(5);
                o.SlidingExpiration = true;
            });

            // changes all data protection tokens timeout 
            builder.Services.Configure<DataProtectionTokenProviderOptions>(o =>
            o.TokenLifespan = TimeSpan.FromHours(3));
            #endregion

            #region JWT-authentication

            builder.Services.Configure<Appsettings>(builder.Configuration.GetSection("JWT"));
            var appsettings = builder.Configuration.GetSection("JWT").Get<Appsettings>();
            var secretKey = appsettings.SecretKey;
            var secretKeyBytes = Encoding.UTF8.GetBytes(secretKey);

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(opt =>
                {
                    opt.SaveToken = true;
                    opt.TokenValidationParameters = new TokenValidationParameters
                    {
                        //disable check issuer of the tokens (server,...)
                        ValidateIssuer = false,

                        //disable check audience of the tokens (not a specific user can receive token)
                        ValidateAudience = false,

                        //enable check issuer key
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(secretKeyBytes),

                        ClockSkew = TimeSpan.Zero,
                    };
                }
            );
            builder.Services.AddAuthorization();

            #endregion

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
