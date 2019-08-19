$versionPrefix = "1.0.0"
$versionSuffix = "alpha-341"
$ErrorActionPreference = "Stop"

function executeSomething {
	param($something)
	$something
	if($LASTEXITCODE -ne 0)
	{
		exit
	}
}

function build {
	param($project)
	executeSomething(dotnet build $project -c Debug --version-suffix $versionSuffix)
}

function test {
	param($project)
	executeSomething(dotnet test $project -c Debug)
}

function pack {
	param($project)
	# Write-Host "dotnet pack $($project) --include-symbols --include-source -c Debug --version-suffix $($versionSuffix) --no-build"
	executeSomething(dotnet pack $project --include-symbols --include-source -c Debug --version-suffix $versionSuffix --no-build)
}

function push {
	param($project)
	$projectname = $project.SubString($project.LastIndexOf("\") + 1)
	executeSomething(dotnet nuget push $project\bin\Debug\$projectname.$versionPrefix-$versionSuffix.nupkg -s https://registry.arragro.com/repository/nuget-hosted/)
	executeSomething(dotnet nuget push $project\bin\Debug\$projectname.$versionPrefix-$versionSuffix.symbols.nupkg -s https://registry.arragro.com/repository/nuget-hosted/)
}

$paths = @(
	".\src\Arragro.Core.Common",
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
	".\providers\Arragro.Providers.SendgridEmailProvider"
)

foreach ($path in $paths) {
	Remove-Item "$($path)\bin" -Force -Recurse
	Remove-Item "$($path)\obj" -Force -Recurse
}

foreach ($path in $paths) {
	build($path)
}

test(".\tests\Arragro.Core.Common.Tests")
test(".\tests\Arragro.Core.EntityFrameworkCore.IntegrationTests")

foreach ($path in $paths) {
	pack($path)
}

foreach ($path in $paths) {
	push($path)
}
