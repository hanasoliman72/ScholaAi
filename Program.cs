
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ScholaAi.Models;
using ScholaAi.Repositories.Base;
using ScholaAi.Repositories.Student;
using ScholaAi.Repositories.User;
using ScholaAi.Services;
using ScholaAi.Services.User;

namespace ScholaAi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            //db
            builder.Services.AddDbContext<DBcontext>(Options => Options.UseSqlServer(
                builder.Configuration.GetConnectionString("Myconection")));

            builder.Services.AddControllers();

            // Register services
            builder.Services.AddScoped<IStudentProfileService, studentProfileService>();
            builder.Services.AddScoped<IPasswordHasher<user>, PasswordHasher<user>>();
            builder.Services.AddScoped<IFileUploadService, fileUploadService>();

            // Register repositories
            builder.Services.AddScoped<IUserRepository, userRepository>();
            builder.Services.AddScoped<IStudentRepository, studentRepository>();

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

            app.UseAuthorization();
            app.UseStaticFiles();


            app.MapControllers();

            app.Run();
        }
    }
}
