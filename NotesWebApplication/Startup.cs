using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NotesWebApplication.Extensions;
using NotesWebApplication.Models;
using NotesWebApplication.Models.DataManager;
using NotesWebApplication.Models.Repository;
using NotesWebApplication.Services;

namespace NotesWebApplication
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var connection = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<NotesContext>(options =>
                options.UseSqlServer(connection));
            services.AddScoped<INotesRepository<Note>, NotesManager>();
            services.AddScoped<INotesService, NotesService>();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseStatusCodePages();
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.ConfigureCustomExceptionMiddleware();
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

            app.UseMvc();
        }
    }
}