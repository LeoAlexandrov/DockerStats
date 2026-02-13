# Docker Stats Exporter

A simple Prometheus exporter for Docker container statistics. This is a personal "toy" project created to fulfill a specific monitoring need and to serve as a practical implementation of several modern .NET features.

### Focus Areas  
- Persistent Monitoring: tracks containers by name rather than ID, ensuring metrics persist across redeployments.  
- Native AOT publishing.  
- Communication with the Docker Engine API directly via a Unix socket.  

## Metrics

The app listens port 5000 and exposes '/metrics' endpoint that currently [returns memory used by containers](//DockerStats/Program.cs#L61).  
The endpoint is [optionally auth protected with API key](/DockerStats/appsettings.json#L10). No protection if `Auth` section is omitted or `ApiKey` is null or empty. The corresponding Prometheus config is:  
```
  - job_name: 'docker'
    scheme: https
         # metrics_path defaults to '/metrics'

    headers:
      APIKey: "1234"

    static_configs:
      - targets: ['your-monitored-docker-host.com']
```
Simple test: run the app and try `curl -v http://localhost:5000/metrics -H 'APIKey: 1234'`

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
It creates an image for the container that will build AOT executable binary. Finally, run this command from the _solution_ folder (copy it first to Linux machine from Windows PC):  
```
docker run --rm -v $(pwd):/src -w /src/DockerStats dotnet-net10-aot-builder dotnet publish -c Release -r linux-x64 -p:PublishAot=true -o ./publish
```
Now `DockerStats/publish` folder contains AOT built binary `DockerStats`. Use [this Dockerfile](/DockerStats/Dockerfile) to deploy DockerStats app as a container.