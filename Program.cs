using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ScholaAi.Models;
using ScholaAi.Repositories;
using ScholaAi.Repositories.Base;
using ScholaAi.Repositories.Rating;
using ScholaAi.Repositories.Student;
using ScholaAi.Repositories.Teacher;
using ScholaAi.Repositories.sessions;
using ScholaAi.Repositories.User;
using ScholaAi.Services;
using ScholaAi.Services.Base;
using ScholaAi.Services.Rating;
using ScholaAi.Services.Teacher;
using ScholaAi.Services.User;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ScholaAi.Data.Seeders;
using ScholaAi.Services.sessions;
using ScholaAi.Services.Student;
using ScholaAi.Services.teacher;

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
            builder.Services.AddScoped<IFileUploadService, fileUploadService>();
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddScoped<ISessionRequestService, sessionRequestService>();
            builder.Services.AddScoped<IRatingService, ratingService>();
            builder.Services.AddScoped<ITeacherProfileService, teacherProfileService>();
            builder.Services.AddScoped<IStudentDashboardService, StudentDashboardService>();
            builder.Services.AddScoped<ITeacherDashboardService, TeacherDashboardService>();

            // Repositories
            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(genericRepository<>));
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IStudentRepository, studentRepository>();
            builder.Services.AddScoped<ITeacherRepository, teacherRepository>();
            builder.Services.AddScoped<IAvailabilityRepository, availabilityRepository>();
            builder.Services.AddScoped<IRatingRepository, ratingRepository>();
            builder.Services.AddScoped<IRequestBroadcastRepository, requestBroadcastRepository>();
            builder.Services.AddScoped<ISessionRequestRepository, sessionRequestRepository>();
            builder.Services.AddScoped<IStudentDashboardRepository, StudentDashboardRepository>();
            builder.Services.AddScoped<ITeacherDashboardRepository, TeacherDashboardRepository>();

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
            });

            // Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

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

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();

        }
    }
}