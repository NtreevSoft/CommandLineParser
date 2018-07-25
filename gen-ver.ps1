$majorVersion = 4
$minorVersion = 0
$assemblyFilePath = ".\Ntreev.Library.Commands.AssemblyInfo\AssemblyInfo.cs"
$projectFilePath = ".\Ntreev.Library.Commands\Ntreev.Library.Commands.csproj"

$assemblyPath = Join-Path (Split-Path $myInvocation.MyCommand.Definition) $assemblyFilePath -Resolve
$projectPath = Join-Path (Split-Path $myInvocation.MyCommand.Definition) $projectFilePath
$version = "$majorVersion.$minorVersion"
$fileVersion = "$majorVersion.$minorVersion" + "." + (Get-Date -Format yy) + (Get-Date).DayOfYear + "." + (Get-Date -Format HHmm)

if (Test-Path $assemblyPath) {
    $content = Get-Content $assemblyPath -Encoding UTF8

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

    $backupPath = $assemblyPath + ".bak"
    Copy-Item $assemblyPath $backupPath
    Set-Content $assemblyPath $content -Encoding UTF8

    if ($null -eq (Get-Content $assemblyPath)) {
        Remove-Item $assemblyPath
        Copy-Item $backupPath $assemblyPath
        Remove-Item $backupPath
        throw "replace version failed: $assemblyPath"
    }
    else {
        Remove-Item $backupPath
    }
}
else {
    throw "assembly path not found: $assemblyPath"
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

    $backupPath = $projectPath + ".bak"
    Copy-Item $projectPath $backupPath
    Set-Content $projectPath $content -Encoding UTF8

    if ($null -eq (Get-Content $projectPath)) {
        Remove-Item $projectPath
        Copy-Item $backupPath $projectPath
        Remove-Item $backupPath
        throw "replace version failed: $projectPath"
    }
    else {
        Remove-Item $backupPath
    }
}
else {
    throw "project path not found: $projectPath"
}

Set-Content version.txt $fileVersion
Write-Host $fileVersion