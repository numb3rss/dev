$ErrorActionPreference="Stop"
$ProgressPreference="SilentlyContinue"

# Docker image name for the application
$ImageName="numb3rss/actor"

function Invoke-MSBuild ([string]$MSBuildPath, [string]$MSBuildParameters) {
    Invoke-Expression "$MSBuildPath $MSBuildParameters"
}

function Invoke-Docker-Build ([string]$ImageName, [string]$ImagePath, [string]$DockerBuildArgs = "") {
    echo "docker build -t $ImageName $ImagePath $DockerBuildArgs"
    Invoke-Expression "docker build -t $ImageName $ImagePath $DockerBuildArgs"
}

MSBuild /t:restore
Invoke-MSBuild -MSBuildPath "MSBuild.exe" -MSBuildParameters ".\TestAppService.csproj /p:OutputPath=.\publish /p:Configuration=Release"
Invoke-Docker-Build -ImageName $ImageName -ImagePath "."