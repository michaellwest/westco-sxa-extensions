$projectPath = "C:\Projects\Westco.XA.Extensions"
if(-not (Test-Path -Path $projectPath)) {
    exit
}

$sites = @{Path = "C:\Websites\dev.demo"; Version="8"};

$removeOnly = $false

function Create-Junction{
    [cmdletbinding(
        DefaultParameterSetName = 'Directory',
        SupportsShouldProcess=$True
    )]
    Param (
        [string]$path,
        [string]$source
        )
    Write-Host "$path --> $source"
    if(Test-Path "$path"){
        cmd.exe /c "rmdir `"$path`" /Q /S" 
    }
    if(-not $removeOnly){
        cmd.exe /c "mklink /J `"$path`" `"$source`""
    }
}

function Create-ProjectJunctions{
    [cmdletbinding(
        DefaultParameterSetName = 'Directory',
        SupportsShouldProcess=$True
    )]
    Param (
        [string]$path, 
        [int]$version
        )

    Write-Host "--------------------------------------------------------------------------------------------"
    Write-Host "$project\$version --> $path"
    Write-Host "--------------------------------------------------------------------------------------------"

    Create-Junction "$path\Data\Unicorn" "$projectPath\src\serialization"
}

foreach($sitecoreSite in $sites){
    if(Test-Path -Path $sitecoreSite.Path) {
	    Create-ProjectJunctions @sitecoreSite
    }
}