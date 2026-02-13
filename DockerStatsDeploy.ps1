& "C:\Program Files\7-Zip\7z.exe" a -ttar "C:\VSBuild\DockerStats\publish.tar" "C:\VSBuild\DockerStats\publish\*"

$prepCommands = @"
mkdir -p -v /home/leo/DockerStats/publish;
rm -r /home/leo/DockerStats/publish/*
"@

$commands = @"
cd /home/leo/DockerStats;
tar -xf publish.tar --directory publish
rm -f publish.tar
chmod -R a+x publish; 
docker rm -f DockerStats;
docker rmi dockerstats:latest;
docker build --tag dockerstats .;
docker run -p 8090:5000 --name DockerStats -h DockerStats --restart=always -v /var/run/docker.sock:/var/run/docker.sock -d dockerstats:latest
"@

Write-Host "============== ContaboVPS =============="

ssh ContaboVPS $prepCommands
scp C:\VSBuild\DockerStats\publish.tar ContaboVPS:/home/leo/DockerStats
scp C:\OneDrive\Projects\DockerStats\DockerStats\Dockerfile ContaboVPS:/home/leo/DockerStats

ssh ContaboVPS $commands 

$winscpResult = $LastExitCode

if ($winscpResult -eq 0)
{
  Write-Host "Success"
}
else
{
  Write-Host "Error"
}

<#
Write-Host "============== MiniPC ==============="

ssh MiniPC $prepCommands
scp C:\VSBuild\DockerStats\publish.tar MiniPC:/home/leo/DockerStats
scp C:\OneDrive\Projects\DockerStats\DockerStats\Dockerfile MiniPC:/home/leo/DockerStats

ssh MiniPC $commands 

$winscpResult = $LastExitCode

if ($winscpResult -eq 0)
{
  Write-Host "Success"
}
else
{
  Write-Host "Error"
}

Write-Host "========================================"
#>


Remove-Item "C:\VSBuild\DockerStats\publish.tar" -Force -ErrorAction SilentlyContinue


Pause
