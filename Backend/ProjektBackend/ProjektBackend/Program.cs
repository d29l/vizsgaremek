
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ProjektBackend.Models;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;

#pragma warning disable CS8604

namespace ProjektBackend
{
    public class Program
    {

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<ProjektContext>(option =>
            {
                var connectionString = builder.Configuration.GetConnectionString("MySql");
                option.UseMySQL(connectionString);
            });

            var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Secret"]);
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    RoleClaimType = ClaimTypes.Role,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

            builder.Services.AddRateLimiter(options =>
            {
                options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "anonymous",
                        factory: partition => new FixedWindowRateLimiterOptions
                        {
                            AutoReplenishment = true,
                            PermitLimit = 100,
                            QueueLimit = 0,
                            Window = TimeSpan.FromMinutes(1)
                        }));
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            });

            builder.Services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
                options.Providers.Add<GzipCompressionProvider>();
            });

            builder.Services.AddHealthChecks().AddDbContextCheck<ProjektContext>();

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireRole("Admin");
                });
                
                options.AddPolicy("EmployerOnly", policy => 
                { 
                    policy.RequireAuthenticatedUser(); 
                    policy.RequireRole("Employer"); 
                });
                options.AddPolicy("EmployeeOnly", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireRole("Employee");
                });
                options.AddPolicy("SelfOnly", policy => 
                { 
                    policy.RequireAuthenticatedUser(); 
                    policy.RequireClaim(ClaimTypes.NameIdentifier); 
                });

                options.AddPolicy("EmployeeSelfOnly", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireRole("Employee");
                    policy.RequireClaim(ClaimTypes.NameIdentifier);
                });

                options.AddPolicy("EmployerSelfOnly", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireRole("Employer");
                    policy.RequireClaim(ClaimTypes.NameIdentifier);

                });

            options.AddPolicy("EmployeeSelfOrAdmin", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.Requirements.Add(new EmployeeSelfOnlyOrAdminRequirement());
            });
                

            options.AddPolicy("EmployerSelfOrAdmin", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.Requirements.Add(new EmployerSelfOnlyOrAdminRequirement());
            });
                    

            options.AddPolicy("EmployerOrAdmin", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.Requirements.Add(new EmployerOnlyOrAdminRequirement());
            });
                    

                options.AddPolicy("SelfOrAdmin", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.Requirements.Add(new SelfOnlyOrAdminRequirement());
                });
                    
            });
            builder.Services.AddSingleton<IAuthorizationHandler, MultiPolicyAuthorizationHandler>();

            builder.Services.AddControllers().AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
            
            builder.Services.AddCors(c => { c.AddPolicy("AllowOrigin", options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()); });

            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Job Platform API", Version = "v1" });

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter your JWT token in the format: Bearer <your-token>"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            Array.Empty<string>()
                        }
                });
            });

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Job Platform API V1");
                });
            }
            app.UseResponseCompression();
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    context.Response.ContentType = "application/json";

                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (contextFeature != null)
                    {
                        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
                        logger.LogError($"Something went wrong: {contextFeature.Error}");

                        await context.Response.WriteAsync(new
                        {
                            StatusCode = context.Response.StatusCode,
                            Message = "Internal Server Error. Please try again later."
                        }.ToString());
                    }
                });
            });

            app.MapHealthChecks("/health");

            app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseRateLimiter();
            
            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();

            app.UseStaticFiles();

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), "Storage", "Images")),
                RequestPath = "/Storage/Images"
            });

            app.Run();

        }
    }
}
