using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.PlatformAbstractions;
using System;
using System.Collections.Generic;
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

        public static IMvcBuilder ConfigureAndGet(IServiceCollection serviceCollection, string executingAssemblyLocation = null, IEnumerable<string> assemblyParts = null)
        {
            var applicationEnvironment = PlatformServices.Default.Application;

            string applicationName;
            string path;
            IFileProvider fileProvider;

            if (!string.IsNullOrEmpty(executingAssemblyLocation))
            {
                applicationName = Path.GetFileNameWithoutExtension(executingAssemblyLocation);
                path = Path.GetDirectoryName(executingAssemblyLocation);
                fileProvider = new PhysicalFileProvider(path);
            }
            else
            {
                applicationName = Assembly.GetExecutingAssembly().GetName().Name;
                path = Directory.GetCurrentDirectory();
                fileProvider = new PhysicalFileProvider(path);
            }

            var environment = new HostingEnvironment
            {
                WebRootFileProvider = fileProvider,
                ApplicationName = applicationName
            };
            serviceCollection.AddSingleton<IHostingEnvironment>(environment);

            serviceCollection.Configure<RazorViewEngineOptions>(options =>
            {
                options.FileProviders.Clear();
                options.FileProviders.Add(fileProvider);
            });

            var diagnosticSource = new DiagnosticListener("Microsoft.AspNetCore");
            serviceCollection.AddSingleton<DiagnosticSource>(diagnosticSource);

            serviceCollection.AddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();
            serviceCollection.AddSingleton(applicationEnvironment);
            serviceCollection.AddLogging();
            var mvcBuilder = serviceCollection.AddMvc();
            if (assemblyParts != null)
            {
                foreach (var assemblyPart in assemblyParts)
                    mvcBuilder.PartManager.ApplicationParts.Add(new CompiledRazorAssemblyPart(Assembly.LoadFile($"{path}\\{assemblyPart}")));
            }
            serviceCollection.AddSingleton<IRazorViewToStringRenderer, RazorViewToStringRenderer>();
            return mvcBuilder;
        }

        public static IMvcBuilder ConfigureAndGet(string executingAssemblyLocation = null, IEnumerable<string> assemblyParts = null)
        {
            var serviceCollection = new ServiceCollection();
            return ConfigureAndGet(serviceCollection, executingAssemblyLocation, assemblyParts);
        }
    }

    public interface IRazorViewToStringRenderer
    {
        Task<string> RenderViewToStringAsync<TModel>(string viewName, TModel model);
    }
}
