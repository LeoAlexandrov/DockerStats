using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

using AleProjects.Docker.Stats;


namespace DockerStats
{
	public static class MyApp
	{
		public static async Task Main()
		{
			using var loggerFactory = LoggerFactory
				.Create(builder => { 
					builder.AddConsole().SetMinimumLevel(LogLevel.Debug); 
				}); 

			var logger = loggerFactory.CreateLogger<DockerStatsCollector>();
			var dsc = new DockerStatsCollector(logger: logger);
			var stats = await dsc.GetContainerStatsAsync();

			foreach (var s in stats)
				logger.LogInformation("{Name} = {Usage}", s.Name, s.MemoryStats.Usage);
		}
	}
}