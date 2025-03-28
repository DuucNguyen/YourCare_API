using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using YourCare_BOs;
using YourCare_DAOs;
using YourCare_DAOs.DAOs;
using YourCare_Repos.Interfaces;
using YourCare_Repos.Repositories;
using YourCare_WebApi.Models.Auth;
using YourCare_WebApi.Services.EmailSender;
using YourCare_WebApi.Services.FileService;
using YourCare_WebApi.Services.UriService;

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
            ),
            ServiceLifetime.Scoped
            );

            builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;
                options.User.RequireUniqueEmail = true;
            })
            .AddRoles<ApplicationRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();
            #endregion

            #region scope

            builder.Services.AddScoped<ApplicationDbContext>();
            builder.Services.AddTransient<ApplicationDbContext>();

            builder.Services.AddScoped<RoleDAO>();
            builder.Services.AddScoped<IRoleRepository, RoleRepository>();

            builder.Services.AddScoped<SpecialtyDAO>();
            builder.Services.AddScoped<ISpecialtyRepository, SpecialtyRepository>();

            builder.Services.AddScoped<DoctorSpecialtiesDAO>();
            builder.Services.AddScoped<IDoctorSpecialtiesRepository, DoctorSpecialtiesRepository>();

            builder.Services.AddScoped<DoctorDAO>();
            builder.Services.AddScoped<IDoctorProfileRepository, DoctorProfileRepository>();

            builder.Services.AddScoped<UserDAO>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();

            builder.Services.AddScoped<TimeSlotDAO>();
            builder.Services.AddScoped<ITimeSlotRepository, TimeSlotRepository>();

            builder.Services.AddScoped<TimetableDAO>();
            builder.Services.AddScoped<ITimetableRepository, TimetableRepository>();

            builder.Services.AddScoped<PatientProfileDAO>();
            builder.Services.AddScoped<IPatientProfileRepository, PatientProfileRepository>();


            builder.Services.AddScoped<AppointmentDAO>();
            builder.Services.AddScoped<IAppointmentReposiory, AppointmentRepository>();

            builder.Services.AddScoped<AppointmentFilesUploadDAO>();
            builder.Services.AddScoped<IAppointmentFilesUploadRepository, AppointmentFilesUploadRepository>();
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

                    // Extract token from cookie
                    opt.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            context.Token = context.Request.Cookies["access_token"]; // Access the token from cookies
                            return Task.CompletedTask;
                        }
                    };
                }
            );

            #endregion

            #region Policy-authorization

            builder.Services.Configure<IdentityOptions>(options =>
            {
                options.ClaimsIdentity.RoleClaimType = ClaimTypes.Role; // Ensure roles are treated as claims
            });

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
                options.AddPolicy("DoctorOnly", policy => policy.RequireRole("Doctor"));
                options.AddPolicy("StaffOnly", policy => policy.RequireRole("Staff"));

                options.AddPolicy("Admin_DoctorProfile_View", policy => policy.RequireClaim("Admin_DoctorProfile_View"));
                options.AddPolicy("Admin_DoctorProfile_Create", policy => policy.RequireClaim("Admin_DoctorProfile_Create"));
                options.AddPolicy("Admin_DoctorProfile_Update", policy => policy.RequireClaim("Admin_DoctorProfile_Update"));
                options.AddPolicy("Admin_DoctorProfile_Delete", policy => policy.RequireClaim("Admin_DoctorProfile_Delete"));
                options.AddPolicy("Admin_User_View", policy => policy.RequireClaim("Admin_User_View"));
                options.AddPolicy("Admin_User_Create", policy => policy.RequireClaim("Admin_User_Create"));
                options.AddPolicy("Admin_User_Update", policy => policy.RequireClaim("Admin_User_Update"));
                options.AddPolicy("Admin_User_Delete", policy => policy.RequireClaim("Admin_User_Delete"));
            });

            #endregion

            #region UriService

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddSingleton<IUriService>(x =>
            {
                var accessor = x.GetRequiredService<IHttpContextAccessor>();
                var request = accessor.HttpContext.Request;
                var uri = string.Concat(request.Scheme, "://", request.Host.ToUriComponent());
                return new UriService(uri); //_baseURL
            });

            #endregion

            #region FileServive
            builder.Services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = 104857600; // 100MB
            });

            builder.Services.AddDirectoryBrowser();
            builder.Services.AddScoped<IFileService, FileService>();

            #endregion

            builder.Services.AddControllers().AddNewtonsoftJson();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins",
                    policy =>
                    {
                        policy.WithOrigins("http://localhost:5173")
                                .AllowCredentials()
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                    });
            });


            var app = builder.Build();


            #region FileService

            var requestPath = "/Upload";

            var uploadPath = Path.Combine(builder.Environment.ContentRootPath, "Upload");
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            var fileProvider = new PhysicalFileProvider(uploadPath);
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = fileProvider,
                RequestPath  = requestPath
            });
            app.UseDirectoryBrowser(new DirectoryBrowserOptions
            {
                FileProvider = fileProvider,
                RequestPath = requestPath
            });

            #endregion

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors("AllowAllOrigins");

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}