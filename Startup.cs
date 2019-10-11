using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace PasswdAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            //get app settings and configurations
            Configuration = configuration;
            Program.groupFilePath = Configuration.GetSection("AppConfiguration")["groupFilePath"];
            Program.passwdFilePath = Configuration.GetSection("AppConfiguration")["passwdFilePath"];

            if (File.Exists(Program.groupFilePath) && File.Exists(Program.passwdFilePath))
            {
                Program.dataSet = new System.Data.DataSet();

                //init tables/watcher on startup
                DataTableBuilders.UpdateUserTable(Program.dataSet, Program.passwdFilePath);
                DataTableBuilders.UpdateGroupTable(Program.dataSet, Program.groupFilePath);

                Program.groupWatcher = Program.watchFile(Program.groupFilePath);
                Program.userWatcher = Program.watchFile(Program.passwdFilePath);
            }
            else Program.errorStatus = PasswdErrors.PasswdError.FileNotFound;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            
            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
