using Agazaty.Data.Base;
using Agazaty.Data.Services.Implementation;
using Agazaty.Data.Services.Interfaces;
using Agazaty.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Agazaty
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddDbContext<AppDbContext>(op => op.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<AppDbContext>();
            builder.Services.AddScoped<IEntityBaseRepository<SickLeave>, EntityBaseRepository<SickLeave>>();
            builder.Services.AddScoped<IEntityBaseRepository<Department>, EntityBaseRepository<Department>>();
            builder.Services.AddScoped<IEntityBaseRepository<CasualLeave>, EntityBaseRepository<CasualLeave>>();
            builder.Services.AddScoped<IEntityBaseRepository<PermitLeave>, EntityBaseRepository<PermitLeave>>();
            builder.Services.AddScoped<IEntityBaseRepository<PermitLeaveImage>, EntityBaseRepository<PermitLeaveImage>>();
            builder.Services.AddScoped<IEntityBaseRepository<NormalLeave>, EntityBaseRepository<NormalLeave>>();
            builder.Services.AddScoped<IAccountService, AccountService>();
            builder.Services.AddScoped<IRoleService, RoleService>();

            builder.Services.Configure<JWT>(builder.Configuration.GetSection("JWT"));

            builder.Services.AddAutoMapper(typeof(Program));

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(o => {
                    o.RequireHttpsMetadata = false;
                    o.SaveToken = false;
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidIssuer = builder.Configuration["JWT:Issuer"],
                        ValidAudience = builder.Configuration["JWT:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]))
                    };
                });
            builder.Services.AddControllers();
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