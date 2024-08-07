
using FinalAssessment_Backend.Models.Dto;
using FinalAssessment_Backend.Models.Entities;
using FinalAssessment_Backend.Repository;
using FinalAssessment_Backend.RepositoryInterface;
using FinalAssessment_Backend.Service;
using FinalAssessment_Backend.ServiceInterface;
using FinalAssessment_Backend.Shared.EncryptDecrypt;
using FinalAssessment_Backend.Shared.Hashing;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using NLog;
using NLog.Web;
using Swashbuckle.AspNetCore.Filters;
using System.Text;

namespace FinalAssessment_Backend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Early init of NLog to allow startup and exception logging, before host is built
            var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
            logger.Debug("init main");

            try
            {
                var builder = WebApplication.CreateBuilder(args);

                // NLog: Setup NLog for Dependency injection
                builder.Logging.ClearProviders();
                builder.Host.UseNLog();

                // Add services to the container.
                builder.Services.AddControllers()
                    .AddJsonOptions(opt =>
                    {
                        opt.JsonSerializerOptions.Converters.Add(new CustomDateFormatter());
                    });


                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen(opt =>

                 opt.MapType<DateOnly>(() => new OpenApiSchema
                 {
                     Type = "string",
                     Format = "date",
                     Example = new OpenApiString(DateTime.Today.ToString("yyyy-MM-dd"))
                 })


                );


                //Add authentication to Swagger UI
                builder.Services.AddSwaggerGen(options =>
                {
                    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                    {
                        In = ParameterLocation.Header,
                        Name = "Authorization",
                        Type = SecuritySchemeType.ApiKey
                    });

                    options.OperationFilter<SecurityRequirementsOperationFilter>();
                });

                //For Encrypt and Decrypt
                builder.Services.AddDataProtection();


                builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
                builder.Services.AddTransient<IUserService, UserService>();
                builder.Services.AddTransient<IUserRepo, UserRepo>();


                builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));


                builder.Services.AddTransient<IAccountService, AccountService>();
                builder.Services.AddTransient<IAccountRepo, AccountRepo>();

                builder.Services.AddTransient<IEmailService, EmailService>();

                builder.Services.AddTransient<IImageUploadService, ImageUploadService>();

                builder.Services.AddTransient<IJwtService, JwtService>();


                builder.Services.AddTransient<IHashing, Hashing>();



                builder.Services.AddSingleton<EncryptDecrypt>();



                builder.Services.AddCors(options =>
                {
                    options.AddPolicy("MyAllowSpecificOrigins",
                                          policy =>
                                          {
                                              policy.WithOrigins("http://localhost:4200")
                                                               .AllowAnyHeader()
                                                               .AllowAnyMethod();
                                          });
                });



                //For Jwt
                builder.Services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                }).AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                    };
                });












                var app = builder.Build();

                // Configure the HTTP request pipeline.
                if (app.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI();
                }

                app.UseHttpsRedirection();


                app.UseCors("MyAllowSpecificOrigins");

                app.UseStaticFiles(new StaticFileOptions
                {
                    OnPrepareResponse = ctx =>
                    {
                        ctx.Context.Response.Headers.Append("Access-Control-Allow-Origin", "http://localhost:4200");
                    }
                });



                app.UseAuthentication();

                app.UseAuthorization();



                app.MapControllers();

                app.Run();
            }
            catch (Exception exception)
            {
                // NLog: catch setup errors
                logger.Error(exception, "Stopped program because of exception");
                throw;
            }
            finally
            {
                // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
                NLog.LogManager.Shutdown();
            }


        }
    }
}
