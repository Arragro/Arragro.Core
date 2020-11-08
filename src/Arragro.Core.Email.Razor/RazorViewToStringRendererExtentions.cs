using Arragro.Core.Email.Razor.Services;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.PlatformAbstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Arragro.Core.Email.Razor
{

    public static class RazorViewToStringRendererExtentions
    {
        public static IMvcBuilder ConfigureRazorViewToStringRenderer(this IServiceCollection services, string executingAssembly = null)
        {
            var applicationEnvironment = PlatformServices.Default.Application;

            string applicationName;
            string path;
            IFileProvider fileProvider;

            if (!string.IsNullOrEmpty(executingAssembly))
            {
                applicationName = Path.GetFileNameWithoutExtension(executingAssembly);
                path = Path.GetDirectoryName(executingAssembly);
            }
            else
            {
                applicationName = Assembly.GetExecutingAssembly().GetName().Name;
                path = Directory.GetCurrentDirectory();
            }

            fileProvider = new PhysicalFileProvider(path);

            var environment = new HostingEnvironment
            {
                ContentRootFileProvider = fileProvider,
                ContentRootPath = path,
                ApplicationName = applicationName
            };
            services.AddSingleton<IHostEnvironment>(environment);

            services.Configure<MvcRazorRuntimeCompilationOptions>(options =>
            {
                options.FileProviders.Clear();
                options.FileProviders.Add(fileProvider);
            });

            var viewAssemblies = Directory.GetFiles(path, "*.Views.dll").Select(x => Path.GetFileName(x));

            services.AddLogging();

            //var serviceBuilder = services.BuildServiceProvider();
            //var logger = serviceBuilder.GetService<ILogger<RazorViewToStringRenderer>>();

            //logger.LogInformation("RazorViewToStringRenderer is using the following config: {applicationName} - {path}", applicationName, path);
            //logger.LogInformation("RazorViewToStringRenderer is registering the following dlls: {viewAssemblies}", viewAssemblies);

            var diagnosticListener = new DiagnosticListener("Microsoft.AspNetCore");
            services.AddSingleton<DiagnosticSource>(diagnosticListener);
            services.AddSingleton(diagnosticListener);

            services.AddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();
            services.AddSingleton(applicationEnvironment);
            var mvcBuilder = services.AddMvc()
                                .AddViewLocalization()
                                .AddDataAnnotationsLocalization();
            foreach (var viewAssembly in viewAssemblies)
            {
                mvcBuilder.PartManager.ApplicationParts.Add(new CompiledRazorAssemblyPart(Assembly.LoadFile($"{path}\\{viewAssembly}")));
            }
            services.AddScoped<IRazorViewToStringRenderer, RazorViewToStringRenderer>();

            return mvcBuilder;
        }

        public static void ConfigureRazorViewToStringRendererForMvc(this IServiceCollection services, string customApplicationBasePath = null)
        {
            var applicationEnvironment = PlatformServices.Default.Application;

            var diagnosticSource = new DiagnosticListener("Microsoft.AspNetCore");
            services.AddSingleton<DiagnosticSource>(diagnosticSource);

            services.AddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();
            services.AddSingleton(applicationEnvironment);
            services.AddLogging();
            services.AddScoped<IRazorViewToStringRenderer, RazorViewToStringRenderer>();
        }
    }
}
