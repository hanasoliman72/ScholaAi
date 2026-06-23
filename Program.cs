using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ScholaAi.Hubs;
using ScholaAi.Models;
using ScholaAi.Repositories;
using ScholaAi.Repositories.Admin;
using ScholaAi.Repositories.Base;
using ScholaAi.Repositories.Calendar;
using ScholaAi.Repositories.Notification;
using ScholaAi.Repositories.Payments;
using ScholaAi.Repositories.Rating;
using ScholaAi.Repositories.sessions;
using ScholaAi.Repositories.Student;
using ScholaAi.Repositories.Teacher;
using ScholaAi.Repositories.User;
using ScholaAi.Services;
using ScholaAi.Services.Admin;
using ScholaAi.Services.Base;
using ScholaAi.Services.Calendar;
using ScholaAi.Services.Notifications;
using ScholaAi.Services.payments;
using ScholaAi.Services.Rating;
using ScholaAi.Services.sessions;
using ScholaAi.Services.Student;
using ScholaAi.Services.teacher;
using ScholaAi.Services.Teacher;
using ScholaAi.Services.User;
using ScholaAi.SignalR;
using Stripe;
using System.Text;



namespace ScholaAi
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);


            // Database
            builder.Services.AddDbContext<DBcontext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("Myconection")));

            // Identity
            builder.Services.AddIdentityCore<ApplicationUser>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<DBcontext>()
                .AddDefaultTokenProviders();


            // Controllers
            builder.Services.AddControllers();

            builder.Services.AddHttpContextAccessor();


            // Services
            builder.Services.AddScoped<IStudentProfileService, studentProfileService>();
            builder.Services.AddScoped<IUserService, UserService>();
            //builder.Services.AddScoped<IFileUploadService, fileUploadService>();
            builder.Services.AddHttpClient<IFileUploadService, fileUploadService>();
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddScoped<ISessionRequestService, sessionRequestService>();
            builder.Services.AddScoped<IRatingService, ratingService>();
            builder.Services.AddScoped<ITeacherProfileService, teacherProfileService>();
            builder.Services.AddScoped<INotificationService, NotificationService>();
            builder.Services.AddScoped<IStudentDashboardService, StudentDashboardService>();
            builder.Services.AddScoped<ITeacherDashboardService, TeacherDashboardService>();
            //builder.Services.AddScoped<ISessionStreamService, SessionStreamService>();
            builder.Services.AddHttpClient<ISessionStreamService, SessionStreamService>();
            builder.Services.AddScoped<IAdminRepository, AdminRepository>();
            builder.Services.AddScoped<IAdminService, AdminService>();

            builder.Services.AddScoped<ICalendarRepository, CalendarRepository>();
            builder.Services.AddScoped<ICalendarService, CalendarService>();
          
            builder.Services.AddScoped<IWalletService, WalletService>();
            builder.Services.AddScoped<IPaymentService, PaymentService>();
            builder.Services.AddScoped<IChatRepository, ChatRepository>();
            builder.Services.AddScoped<IChatService, ChatService>();



            // Repositories
            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(genericRepository<>));
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IStudentRepository, studentRepository>();
            builder.Services.AddScoped<ITeacherRepository, teacherRepository>();
            builder.Services.AddScoped<IAvailabilityRepository, availabilityRepository>();
            builder.Services.AddScoped<IRatingRepository, ratingRepository>();
            builder.Services.AddScoped<IRequestBroadcastRepository, requestBroadcastRepository>();
            builder.Services.AddScoped<ISessionRequestRepository, sessionRequestRepository>();
            builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
            builder.Services.AddScoped<IStudentDashboardRepository, StudentDashboardRepository>();
            builder.Services.AddScoped<ITeacherDashboardRepository, TeacherDashboardRepository>();
            builder.Services.AddScoped<ISessionRepository, SessionRepository>();


            builder.Services.AddScoped<IWalletRepository, WalletRepository>();
            //builder.Services.AddTransient<INotificationRepository, NotificationRepository>();

            //JWT
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
                    ValidAudience = builder.Configuration["JWT:ValidAudience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secretkey"])
                    )
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];

                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) &&
                            (path.StartsWithSegments("/chatHub") ||
                             path.StartsWithSegments("/notificationHub") ||
                             path.StartsWithSegments("/hub/session")))
                        {
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            });
            // Payment Getway
            //StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));
          builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));
            StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];


            // Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();


            builder.Services.AddAuthorization();
            builder.Services.AddSignalR();
            builder.Services.AddSingleton<IUserIdProvider, NameIdentifierUserIdProvider>();
            builder.Services.AddSingleton<RoomService>();
            
            // CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowReact",
                    policy =>
                    {
                        policy
                            .SetIsOriginAllowed(origin =>
                            {
                                // Allow localhost (dev) and any 192.168.x.x LAN device
                                var host = new Uri(origin).Host;
                                return host == "localhost" || host == "127.0.0.1"
                                    || host.StartsWith("192.168.");
                            })
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials();
                    });
            });

            builder.WebHost.ConfigureKestrel(options =>
            {
                options.Limits.MaxRequestBodySize = 2_000_000_000; // 2GB
            });

            var app = builder.Build();

            // payment
            app.Use(async (context, next) => {
                if (context.Request.Path.StartsWithSegments("/api/payment/webhook"))
                    context.Request.EnableBuffering();
                await next();
            });

            using (var scope = app.Services.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                string[] roles = { "Student", "Teacher", "Admin" };
                foreach (var role in roles)
                {
                    // ?????? GetAwaiter().GetResult() ??? await
                    if (!roleManager.RoleExistsAsync(role).GetAwaiter().GetResult())
                    {
                        roleManager.CreateAsync(new IdentityRole(role)).GetAwaiter().GetResult();
                    }
                }
            }
            // Middleware
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            //using (var scope = app.Services.CreateScope())
            //{
            //    var context = scope.ServiceProvider.GetRequiredService<DBcontext>();
            //    ratingSeeder.SeedRatingData(context);
            //}

        


            app.UseStaticFiles();

            // Order matters: CORS → Auth → Authorization
            app.UseCors("AllowReact");
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapHub<ChatHub>("/chatHub");
            app.MapHub<NotificationHub>("/notificationHub"); 
            app.MapHub<SessionHub>("/hub/session");


            app.MapControllers();

            app.Run();

        }
    }
}