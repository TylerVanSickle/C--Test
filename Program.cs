using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace ProjectApi
{
    public class Project
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Role { get; set; }
        public int? LaunchYear { get; set; }
    }

    public class Startup
    {
        // Shared in-memory list of projects
        private static readonly List<Project> projects = new();
        private static int nextId = 1;

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRouting();

            // Enable CORS
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });
        }

        public void Configure(IApplicationBuilder app)
        {
            // Apply CORS before routing
            app.UseCors();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                // GET all projects
                endpoints.MapGet("/api/projects", async context =>
                {
                    await context.Response.WriteAsJsonAsync(projects);
                });

                // POST a new project
                endpoints.MapPost("/api/projects", async context =>
                {
                    var newProject = await context.Request.ReadFromJsonAsync<Project>();

                    if (newProject == null)
                    {
                        context.Response.StatusCode = 400; // Bad Request
                        await context.Response.WriteAsync("Invalid project data");
                        return;
                    }

                    newProject.Id = nextId++;
                    projects.Add(newProject);

                    context.Response.StatusCode = 201; // Created
                    await context.Response.WriteAsJsonAsync(newProject);
                });

                // GET single project by ID
                endpoints.MapGet("/api/projects/{id:int}", async context =>
                {
                    var idStr = context.Request.RouteValues["id"]?.ToString();
                    if (!int.TryParse(idStr, out int id))
                    {
                        context.Response.StatusCode = 400;
                        await context.Response.WriteAsync("Invalid Id");
                        return;
                    }

                    var project = projects.FirstOrDefault(project => project.Id == id);

                    if (project == null)
                    {
                        context.Response.StatusCode = 404;
                        await context.Response.WriteAsync($"Project with ID {id} not found.");
                        return;
                    }

                    await context.Response.WriteAsJsonAsync(project);
                });
            });
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var startup = new Startup();
            startup.ConfigureServices(builder.Services);

            var app = builder.Build();

            startup.Configure(app);

            app.Run();
        }
    }
}
