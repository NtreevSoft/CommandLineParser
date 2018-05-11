$majorVersion=3
$minorVersion=6
$version="$majorVersion.$minorVersion"+"."+(Get-Date -Format yy)+(Get-Date).DayOfYear+"."+(Get-Date -Format HHmm)
$csproj="Ntreev.Library.Commands\Ntreev.Library.Commands.csproj"
Set-Content version.txt $version
(Get-Content $csproj) -replace "(<Version>)(.*)(</Version>)", "`${1}$version`$3" -replace "(<FileVersion>)(.*)(</FileVersion>)", "`${1}$version`$3" -replace "(<AssemblyVersion>)(.*)(</AssemblyVersion>)", "`${1}$majorVersion.$minorVersion.0.0`$3" | Set-Content $csproj
