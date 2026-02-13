# run from the solution directory

docker run --rm -v $(pwd):/src -w /src/DockerStats dotnet-net10-aot-builder dotnet publish -c Release -r linux-x64 -p:PublishAot=true -o ./publish