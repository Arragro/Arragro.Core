$version = "3.1.0-alpha-018"
$ErrorActionPreference = "Stop"

$paths = @(
	".\src\Arragro.Core.Common",
	".\src\Arragro.Core.CronJobService",
	".\src\Arragro.Core.EntityFrameworkCore",
	".\src\Arragro.Core.Web",
	".\src\Arragro.Core.DistributedCache",
	".\src\Arragro.Core.Identity",
	".\src\Arragro.Core.Email.Razor",
	".\src\Arragro.Core.Docker",
	".\src\Arragro.Core.MailhogClient",
	".\src\Arragro.Core.MailDevClient",
	".\providers\Arragro.Providers.AzureStorageProvider",
	".\providers\Arragro.Providers.ImageMagickProvider",
	".\providers\Arragro.Providers.ImageServiceProvider",
	".\providers\Arragro.Providers.InMemoryStorageProvider",
	".\providers\Arragro.Providers.MailKitEmailProvider",
	".\providers\Arragro.Providers.S3StorageProvider",
	".\providers\Arragro.Providers.SendgridEmailProvider"
)

foreach ($path in $paths) {
	Remove-Item "$($path)\bin" -Force -Recurse -ErrorAction SilentlyContinue
	Remove-Item "$($path)\obj" -Force -Recurse -ErrorAction SilentlyContinue
}

function executeSomething {
	param($something)
	$something
	if($LASTEXITCODE -ne 0)
	{
		exit
	}
}

executeSomething(dotnet test .\tests\Arragro.Core.Common.Tests -c Debug )
executeSomething(dotnet test .\tests\Arragro.Core.EntityFrameworkCore.IntegrationTests -c Debug )

dotnet clean

foreach ($path in $paths) {
	dotnet pack $path -c Debug /p:Version=$version --include-symbols --include-source
	$projectName = $path.Replace(".\src\", "").Replace(".\providers\", "")
	executeSomething(dotnet nuget push $path\bin\Debug\$($projectName).$version.nupkg -s https://registry.arragro.com/repository/nuget-hosted/)
}