using System;
using System.Text;
using System.Text.Json.Serialization;
using AleProjects.Docker.Stats;
using DockerStats;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;



var builder = WebApplication.CreateSlimBuilder(args);
var services = builder.Services;
var config = builder.Configuration;


// configure services

services
	.AddAuthentication()
	.AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>("ApiKey", _ => { });


services
	.AddAuthorization(options => 
		options.AddPolicy("ApiKeyAccess", policy => policy.AddAuthenticationSchemes("ApiKey").RequireAuthenticatedUser()))
	.ConfigureHttpJsonOptions(options =>
	{
		options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
		options.SerializerOptions.TypeInfoResolverChain.Insert(0, StatsJsonSerializerContext.Default);
	})
	.Configure<AuthConfig>(config.GetSection("Auth"))
	.Configure<DockerStatsServiceConfig>(config.GetSection("Polling"))
	.AddSingleton<DockerStatsService>()
	.AddHostedService(sp => sp.GetRequiredService<DockerStatsService>());


// create and configure app

var app = builder.Build();

app
	.UseForwardedHeaders()
	.UseExceptionHandler("/error");

// app endpoints

app.Map("/error", (HttpContext context) => Results.Problem("An unexpected error occurred."));

app.MapGet("/metrics", (HttpContext context, DockerStatsService dss) =>
{
	var lastStats = dss.GetLastStats();
	var metrics = new StringBuilder(4096);

	metrics.AppendLine($"# HELP docker_container_memory_bytes Container memory usage in bytes");
	metrics.AppendLine($"# TYPE docker_container_memory_bytes gauge");

	foreach (var stat in lastStats)
		metrics.AppendLine($"docker_container_memory_bytes{{container=\"{stat.Name}\"}} {stat.MemoryStats.Usage}");

	return Results.Content(metrics.ToString());
}).RequireAuthorization("ApiKeyAccess");


// start the app

app.Run();


// ------------------ app JsonSerializerContext ------------------------

[JsonSerializable(typeof(ProblemDetails))]
[JsonSerializable(typeof(ValidationProblemDetails))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{
}
