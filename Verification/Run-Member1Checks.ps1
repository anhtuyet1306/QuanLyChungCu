$ErrorActionPreference = 'Stop'
$repo = Split-Path $PSScriptRoot -Parent
$checkDir = Join-Path ([IO.Path]::GetTempPath()) ('SkyResidenceChecks-' + [Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $checkDir | Out-Null
$reference = [System.Security.SecurityElement]::Escape((Join-Path $repo 'QuanLyChungCu_SkyResidence.csproj'))
@"
<Project Sdk="Microsoft.NET.Sdk">
 <PropertyGroup>
  <OutputType>Exe</OutputType><TargetFramework>net10.0-windows</TargetFramework>
  <UseWindowsForms>true</UseWindowsForms><ImplicitUsings>enable</ImplicitUsings><Nullable>enable</Nullable>
 </PropertyGroup>
 <ItemGroup><ProjectReference Include="$reference" /></ItemGroup>
</Project>
"@ | Set-Content -LiteralPath (Join-Path $checkDir 'Checks.csproj') -Encoding utf8
Copy-Item -LiteralPath (Join-Path $PSScriptRoot 'Member1Checks.cs.txt') -Destination (Join-Path $checkDir 'Program.cs')
dotnet run --project (Join-Path $checkDir 'Checks.csproj') -p:NuGetAudit=false
if ($LASTEXITCODE -ne 0) { throw 'Member 1 checks failed.' }
