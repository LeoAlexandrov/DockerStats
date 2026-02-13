using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using AleProjects.Docker.Stats;


namespace DockerStats
{

	public class DockerStatsServiceConfig
	{
		public int Interval { get; set; } = 10;
		public string DockerSocket { get; set; } = "/var/run/docker.sock";
		public string DockerApi { get; set; } = "http://localhost/v1.41";
	}



	public class DockerStatsService(IOptions<DockerStatsServiceConfig> config, ILogger<DockerStatsService> logger, IHostApplicationLifetime lifetime)
		: DockerStatsCollector(config.Value.DockerApi, config.Value.DockerSocket, logger), IHostedService
	{
		private readonly IHostApplicationLifetime _lifetime = lifetime;
		private readonly int _pollingInterval = config.Value.Interval;
		private CancellationTokenSource _cts; 
		private Task _pollTask;


		public Task StartAsync(CancellationToken startToken)
		{
			_cts = CancellationTokenSource.CreateLinkedTokenSource(startToken, _lifetime.ApplicationStopping);
			_pollTask = Task.Run(() => RunPollingAsync(_cts.Token), CancellationToken.None); 

			return Task.CompletedTask;		
		}

		public async Task StopAsync(CancellationToken stopToken)
		{
			_cts?.Cancel();

			if (_pollTask != null) 
				await _pollTask;

			_httpClient?.Dispose();
		}

		private async Task RunPollingAsync(CancellationToken ct) 
		{ 
			using var timer = new PeriodicTimer(TimeSpan.FromSeconds(_pollingInterval));

			try
			{
				do
				{
					//var sw = System.Diagnostics.Stopwatch.StartNew();

					var st = await GetContainerStatsAsync(ct);

					/*
					_logger?.LogInformation("{Elapsed} ms *********************", sw.ElapsedMilliseconds);
					_logger?.LogInformation(string.Join("\n", st.Select(s => $"{s.Name} = {s.MemoryStats.Usage}")));
					sw.Stop();
					*/

				} while (await timer.WaitForNextTickAsync(ct));
			}
			catch (OperationCanceledException)
			{
				_logger?.LogInformation("Cancelling timer operation");
			}
			catch (Exception ex)
			{
				_logger?.LogError(ex, "Error in timer loop");
			}
		}
	}
}