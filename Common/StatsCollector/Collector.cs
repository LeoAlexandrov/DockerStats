using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;


namespace AleProjects.Docker.Stats
{
	public class DockerStatsCollector
	{
		protected readonly HttpClient _httpClient;
		protected readonly string _dockerApi;
		protected readonly ILogger<DockerStatsCollector> _logger;
		protected ContainerStats[] _lastStats;

		public DockerStatsCollector(string dockerApi = "http://localhost/v1.41", string dockerSocketPath = "/var/run/docker.sock", ILogger<DockerStatsCollector> logger = null)
		{
			_dockerApi = dockerApi;
			_logger = logger;

			if (!string.IsNullOrEmpty(dockerSocketPath))
			{
				var handler = new SocketsHttpHandler()
				{
					ConnectCallback = async (context, cancellationToken) =>
					{
						var endpoint = new UnixDomainSocketEndPoint(dockerSocketPath);
						var socket = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.IP);
						await socket.ConnectAsync(endpoint, cancellationToken);
						return new NetworkStream(socket, true);
					}
				};

				_httpClient = new HttpClient(handler);
			}
			else
				_httpClient = new HttpClient();
		}

		public ContainerStats[] GetLastStats()
		{
			return _lastStats;
		}

		private async Task<Container[]> GetContainersAsync(CancellationToken ct)
		{
			Container[] result;

			try
			{
				using var response = await _httpClient.GetAsync($"{_dockerApi}/containers/json?all=true", ct);

				response.EnsureSuccessStatusCode();

				var json = await response.Content.ReadAsStringAsync(ct);

				result = JsonSerializer.Deserialize(json, StatsJsonSerializerContext.Default.ContainerArray) ?? [];

				if (_logger != null && _logger.IsEnabled(LogLevel.Debug))
					foreach (var item in result)
						_logger.LogDebug("Container name: {Name}", item.Names.FirstOrDefault());
			}
			catch (Exception ex)
			{
				_logger?.LogError(ex, "Error getting container list");
				result = [];
			}

			return result;
		}

		private async Task<ContainerStats> GetContainerStatsAsync(string containerId, CancellationToken ct)
		{
			ContainerStats result;

			try
			{
				using var response = await _httpClient.GetAsync($"{_dockerApi}/containers/{containerId}/stats?stream=false", ct);

				response.EnsureSuccessStatusCode();

				var json = await response.Content.ReadAsStringAsync(ct);

				result = JsonSerializer.Deserialize(json, StatsJsonSerializerContext.Default.ContainerStats);

				_logger?.LogDebug("{Name} : {Usage}", result?.Name, result?.MemoryStats.Usage);
			}
			catch (Exception ex)
			{
				_logger?.LogError(ex, "Error getting stats for container {containerId}", containerId);
				result = null;
			}

			return result;
		}

		public async Task<ContainerStats[]> GetContainerStatsAsync(CancellationToken ct = default)
		{
			var containers = await GetContainersAsync(ct);

			if (containers.Length == 0) 
				return [];

			var tasks = containers
				.Select(c => GetContainerStatsAsync(c.Id, ct))
				.ToArray();

			var results = await Task
				.WhenAll(tasks)
				.ConfigureAwait(false);

			return _lastStats = [.. results.Where(r => r != null)];
		}

	}

}
