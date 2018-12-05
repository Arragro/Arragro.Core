dotnet build "src\Arragro.Core.Common" -c Release --no-dependencies /p:Version=1.0.0-local-1
dotnet build "src\Arragro.Core.EntityFrameworkCore" -c Release --no-dependencies /p:Version=1.0.0-local-1
dotnet build "src\Arragro.Core.Web" -c Release --no-dependencies /p:Version=1.0.0-local-1
dotnet build "src\Arragro.Core.DistributedCache" -c Release --no-dependencies /p:Version=1.0.0-local-1
dotnet build "src\Arragro.Core.Identity" -c Release --no-dependencies /p:Version=1.0.0-local-1
dotnet build "providers\Arragro.Providers.AzureStorageProvider" -c Release --no-dependencies /p:Version=1.0.0-local-1
dotnet build "providers\Arragro.Providers.ImageMagickProvider" -c Release --no-dependencies /p:Version=1.0.0-local-1
dotnet build "providers\Arragro.Providers.ImageServiceProvider" -c Release --no-dependencies /p:Version=1.0.0-local-1
dotnet build "providers\Arragro.Providers.InMemoryStorageProvider" -c Release --no-dependencies /p:Version=1.0.0-local-1
dotnet build "providers\Arragro.Providers.MailKitEmailProvider" -c Release --no-dependencies /p:Version=1.0.0-local-1
dotnet build "providers\Arragro.Providers.SendgridEmailProvider" -c Release --no-dependencies /p:Version=1.0.0-local-1

dotnet pack "src\Arragro.Core.Common" -c Release --no-build --include-symbols -p:SymbolPackageFormat=snupkg /p:Version=1.0.0-local-1 -o D:\Temp\nuget-builds\artifacts
dotnet pack "src\Arragro.Core.EntityFrameworkCore" -c Release --no-build --include-symbols -p:SymbolPackageFormat=snupkg /p:Version=1.0.0-local-1 -o D:\Temp\nuget-builds\artifacts
dotnet pack "src\Arragro.Core.Web" -c Release --no-build --include-symbols -p:SymbolPackageFormat=snupkg /p:Version=1.0.0-local-1 -o D:\Temp\nuget-builds\artifacts
dotnet pack "src\Arragro.Core.DistributedCache" -c Release --no-build --include-symbols -p:SymbolPackageFormat=snupkg /p:Version=1.0.0-local-1 -o D:\Temp\nuget-builds\artifacts
dotnet pack "src\Arragro.Core.Identity" -c Release --no-build --include-symbols -p:SymbolPackageFormat=snupkg /p:Version=1.0.0-local-1 -o D:\Temp\nuget-builds\artifacts
dotnet pack "providers\Arragro.Providers.AzureStorageProvider" -c Release --no-build --include-symbols -p:SymbolPackageFormat=snupkg /p:Version=1.0.0-local-1 -o D:\Temp\nuget-builds\artifacts
dotnet pack "providers\Arragro.Providers.ImageMagickProvider" -c Release --no-build --include-symbols -p:SymbolPackageFormat=snupkg /p:Version=1.0.0-local-1 -o D:\Temp\nuget-builds\artifacts
dotnet pack "providers\Arragro.Providers.ImageServiceProvider" -c Release --no-build --include-symbols -p:SymbolPackageFormat=snupkg /p:Version=1.0.0-local-1 -o D:\Temp\nuget-builds\artifacts
dotnet pack "providers\Arragro.Providers.InMemoryStorageProvider" -c Release --no-build --include-symbols -p:SymbolPackageFormat=snupkg /p:Version=1.0.0-local-1 -o D:\Temp\nuget-builds\artifacts
dotnet pack "providers\Arragro.Providers.MailKitEmailProvider" -c Release --no-build --include-symbols -p:SymbolPackageFormat=snupkg /p:Version=1.0.0-local-1 -o D:\Temp\nuget-builds\artifacts
dotnet pack "providers\Arragro.Providers.SendgridEmailProvider" -c Release --no-build --include-symbols -p:SymbolPackageFormat=snupkg /p:Version=1.0.0-local-1 -o D:\Temp\nuget-builds\artifacts