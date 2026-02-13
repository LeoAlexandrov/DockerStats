# Docker Stats Exporter

A simple Prometheus exporter for Docker container statistics. This is a personal "toy" project created to fulfill a specific monitoring need and to serve as a practical implementation of several modern .NET features.

### Focus Areas  
- Persistent Monitoring: tracks containers by name rather than ID, ensuring metrics persist across redeployments.  
- Native AOT publishing.  
- Communication with the Docker Engine API directly via a Unix socket.  

## Publishing AOT for Linux

Create a Dockerfile for AOT builder, let call it 'Dockerfile.net10.aot.builder'  
```
FROM mcr.microsoft.com/dotnet/sdk:10.0-noble
RUN apt-get update && apt-get install -y clang zlib1g-dev
```
Run once this command:  
```
docker build -t dotnet-net10-aot-builder -f Dockerfile.net10.aot.builder .
```
It creates an image for the container that will build AOT executable binary.  
Finally, run this command from the _solution_ folder (copy it first to Linux machine from Windows PC):  
```
docker run --rm -v $(pwd):/src -w /src/DockerStats dotnet-net10-aot-builder dotnet publish -c Release -r linux-x64 -p:PublishAot=true -o ./publish
```
Now `DockerStats/publish` folder contains AOT built binary `DockerStats`.  
Use ![This Dockerfile](/DockerStats/Dockerfile) to deploy DockerStats app.