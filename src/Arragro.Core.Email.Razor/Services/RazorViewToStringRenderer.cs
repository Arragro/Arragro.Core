using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.PlatformAbstractions;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Arragro.Core.Email.Razor.Services
{
    public class RazorViewToStringRenderer : IRazorViewToStringRenderer
    {
        private IRazorViewEngine _viewEngine;
        private ITempDataProvider _tempDataProvider;
        private IServiceProvider _serviceProvider;

        public RazorViewToStringRenderer(
            IRazorViewEngine viewEngine,
            ITempDataProvider tempDataProvider,
            IServiceProvider serviceProvider)
        {
            _viewEngine = viewEngine;
            _tempDataProvider = tempDataProvider;
            _serviceProvider = serviceProvider;
        }

        public async Task<string> RenderViewToStringAsync<TModel>(string viewName, TModel model)
        {
            var actionContext = GetActionContext();
            var view = FindView(actionContext, viewName);

            if (view == null)
            {
                throw new ArgumentNullException($"{viewName} does not match any available view");
            }

            using (var output = new StringWriter())
            {
                var viewContext = new ViewContext(
                    actionContext,
                    view,
                    new ViewDataDictionary<TModel>(
                        metadataProvider: new EmptyModelMetadataProvider(),
                        modelState: new ModelStateDictionary())
                    {
                        Model = model
                    },
                    new TempDataDictionary(
                        actionContext.HttpContext,
                        _tempDataProvider),
                    output,
                    new HtmlHelperOptions());

                await view.RenderAsync(viewContext);

                return output.ToString();
            }
        }

        private IView FindView(ActionContext actionContext, string viewName)
        {
            var getViewResult = _viewEngine.GetView(executingFilePath: null, viewPath: viewName, isMainPage: true);
            if (getViewResult.Success)
            {
                return getViewResult.View;
            }

            var findViewResult = _viewEngine.FindView(actionContext, viewName, isMainPage: true);
            if (findViewResult.Success)
            {
                return findViewResult.View;
            }

            var searchedLocations = getViewResult.SearchedLocations.Concat(findViewResult.SearchedLocations);
            var errorMessage = string.Join(
                Environment.NewLine,
                new[] { $"Unable to find view '{viewName}'. The following locations were searched:" }.Concat(searchedLocations)); ;

            throw new InvalidOperationException(errorMessage);
        }

        private ActionContext GetActionContext()
        {
            var httpContext = new DefaultHttpContext();
            httpContext.RequestServices = _serviceProvider;
            return new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
        }

        private static void LogHelper(ILogger logger, string message, params object[] args)
        {
            if (logger != null)
                logger.LogInformation(message, args);
        }

        private static IMvcBuilder ConfigureAndGet(IServiceCollection serviceCollection, Assembly[] razorAssemblies, string executingAssembly = null, bool logging = false)
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
            serviceCollection.AddSingleton<IHostEnvironment>(environment);

            serviceCollection.Configure<MvcRazorRuntimeCompilationOptions>(options =>
            {
                options.FileProviders.Clear();
                options.FileProviders.Add(fileProvider);
            });

            ILogger logger = null;

            if (logging)
            {
                serviceCollection.AddLogging();

                var serviceBuilder = serviceCollection.BuildServiceProvider();
                logger = serviceBuilder.GetService<ILogger<RazorViewToStringRenderer>>();
            }

            LogHelper(logger, "RazorViewToStringRenderer is using the following config: {applicationName} - {path}", applicationName, path);
            LogHelper(logger, "RazorViewToStringRenderer is registering the following dlls: {viewAssemblies}", String.Join(", ", razorAssemblies.Select(x => x.FullName)));

            var diagnosticListener = new DiagnosticListener("Microsoft.AspNetCore");
            serviceCollection.AddSingleton<DiagnosticSource>(diagnosticListener);
            serviceCollection.AddSingleton(diagnosticListener);

            serviceCollection.AddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();
            serviceCollection.AddSingleton(applicationEnvironment);
            var mvcBuilder = serviceCollection.AddMvc()
                                .AddViewLocalization()
                                .AddDataAnnotationsLocalization();
            foreach (var viewAssembly in razorAssemblies)
            {
                LogHelper(logger, "RazorViewToStringRenderer is registering the following assemblyPart: {path}", $"{path}\\{viewAssembly}");
                mvcBuilder.PartManager.ApplicationParts.Add(new CompiledRazorAssemblyPart(viewAssembly));
            }
            serviceCollection.AddScoped<IRazorViewToStringRenderer, RazorViewToStringRenderer>();
            return mvcBuilder;
        }

        public static IMvcBuilder ConfigureAndGet(IServiceCollection serviceCollection, Assembly[] razorAssemblies, string executingAssembly = null)
        {
            return ConfigureAndGet(serviceCollection, razorAssemblies,  executingAssembly, true);
        }

        public static IMvcBuilder ConfigureAndGet(Assembly[] razorAssemblies, string executingAssembly = null)
        {
            var serviceCollection = new ServiceCollection();
            return ConfigureAndGet(serviceCollection, razorAssemblies, executingAssembly);
        }

        public static IMvcBuilder ConfigureAndGetNoLogging(IServiceCollection serviceCollection, Assembly[] razorAssemblies, string executingAssembly = null)
        {
            return ConfigureAndGet(serviceCollection, razorAssemblies, executingAssembly, false);
        }

        public static IMvcBuilder ConfigureAndGetNoLogging(Assembly[] razorAssemblies, string executingAssembly = null)
        {
            var serviceCollection = new ServiceCollection();
            return ConfigureAndGetNoLogging(serviceCollection, razorAssemblies, executingAssembly);
        }
    }

    public interface IRazorViewToStringRenderer
    {
        Task<string> RenderViewToStringAsync<TModel>(string viewName, TModel model);
    }
}
