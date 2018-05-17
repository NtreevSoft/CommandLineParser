$majorVersion=3
$minorVersion=6
$sourcePath = Join-Path (Split-Path $myInvocation.MyCommand.Definition) ".\Ntreev.Library.Commands.AssemblyInfo\AssemblyInfo.cs" -Resolve
$projectPath = Join-Path (Split-Path $myInvocation.MyCommand.Definition) ".\Ntreev.Library.Commands\Ntreev.Library.Commands.csproj"
$version="$majorVersion.$minorVersion"
$fileVersion="$majorVersion.$minorVersion"+"."+(Get-Date -Format yy)+(Get-Date).DayOfYear+"."+(Get-Date -Format HHmm)

if (Test-Path $sourcePath) {
    $content = Get-Content $sourcePath -Encoding UTF8

    $pattern1 = "(AssemblyVersion[(]`").+(`"[)]])"
    if ($content -match $pattern1) {
        $content = $content -replace $pattern1, "`${1}$version`$2"
    }

    $pattern2 = "(AssemblyFileVersion[(]`").+(`"[)]])"
    if ($content -match $pattern2) {
        $content = $content -replace $pattern2, "`${1}$fileVersion`$2"
    }

    $pattern3 = "(AssemblyInformationalVersion[(]`").+(`"[)]])"
    if ($content -match $pattern3) {
        $content = $content -replace $pattern3, "`${1}$fileVersion`$2"
    }

    Set-Content $sourcePath $content -Encoding UTF8
}

if (Test-Path $projectPath) {
    $content = Get-Content $projectPath -Encoding UTF8

    $pattern1 = "(<Version>)(.*)(</Version>)"
    if ($content -match $pattern1) {
        $content = $content -replace $pattern1, "`${1}$fileVersion`$3"
    }

    $pattern2 = "(<FileVersion>)(.*)(</FileVersion>)"
    if ($content -match $pattern2) {
        $content = $content -replace $pattern2, "`${1}$fileVersion`$3"
    }

    $pattern3 = "(<AssemblyVersion>)(.*)(</AssemblyVersion>)"
    if ($content -match $pattern3) {
        $content = $content -replace $pattern3, "`${1}$version`$3"
    }

    Set-Content $projectPath $content -Encoding UTF8
}

Set-Content version.txt $fileVersion
Write-Host $fileVersion