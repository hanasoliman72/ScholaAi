using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ScholaAi.Data.Seeders;
using ScholaAi.Mappings;
using ScholaAi.Models;
using ScholaAi.Repositories;
using ScholaAi.Repositories.Base;
using ScholaAi.Repositories.Rating;
using ScholaAi.Repositories.Student;
using ScholaAi.Repositories.Teacher;
using ScholaAi.Repositories.User;
using ScholaAi.Services;
using ScholaAi.Services.Base;
using ScholaAi.Services.Rating;
using ScholaAi.Services.User;

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
            builder.Services.AddIdentity<applicationUser, IdentityRole>()
                .AddEntityFrameworkStores<DBcontext>()
                .AddDefaultTokenProviders();

            // Controllers
            builder.Services.AddControllers();

            // Automapper
            builder.Services.AddAutoMapper(typeof(MappingProfile));

            // Services
            builder.Services.AddScoped<IStudentProfileService, studentProfileService>();
            builder.Services.AddScoped<IUserService, userService>();
            builder.Services.AddScoped<IFileUploadService, fileUploadService>();
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddScoped<IRatingService, ratingService>();

            // Repositories
            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(genericRepository<>));
            builder.Services.AddScoped<IUserRepository, userRepository>();
            builder.Services.AddScoped<IStudentRepository, studentRepository>();
            builder.Services.AddScoped<ITeacherRepository, teacherRepository>();
            builder.Services.AddScoped<IAvailabilityRepository, availabilityRepository>();
            builder.Services.AddScoped<IRatingRepository, ratingRepository>();

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