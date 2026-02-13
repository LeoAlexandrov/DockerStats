using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;


namespace AleProjects.Docker.Stats
{

	[JsonSerializable(typeof(Container))]
	[JsonSerializable(typeof(Container[]))]
	[JsonSerializable(typeof(ContainerStats))]
	[JsonSerializable(typeof(ContainerStats[]))]
	[JsonSerializable(typeof(MemoryStats))]
	[JsonSerializable(typeof(MemoryDetails))]
	[JsonSerializable(typeof(CpuStats))]
	[JsonSerializable(typeof(ThrottlingData))]
	internal partial class StatsJsonSerializerContext : JsonSerializerContext
	{
	}


	// Top-level DTO for the provided JSON

	public record Container
	{
		[JsonPropertyName("Id")]
		public string Id { get; set; } = string.Empty;

		[JsonPropertyName("Names")]
		public string[] Names { get; set; } = [];
	}



	public record ContainerStats
	{
		[JsonPropertyName("id")]
		public string Id { get; init; }

		[JsonPropertyName("name")]
		public string Name { get; init; }

		[JsonPropertyName("os_type")]
		public string OsType { get; init; }

		[JsonPropertyName("read")]
		public DateTimeOffset Read { get; init; }

		[JsonPropertyName("preread")]
		public DateTimeOffset PreRead { get; init; }

		[JsonPropertyName("cpu_stats")]
		public CpuStats CpuStats { get; init; }

		[JsonPropertyName("precpu_stats")]
		public CpuStats PreCpuStats { get; init; }

		[JsonPropertyName("memory_stats")]
		public MemoryStats MemoryStats { get; init; }

		// networks is a map from interface name (eg "eth0") to data
		[JsonPropertyName("networks")]
		public Dictionary<string, NetworkInterface> Networks { get; init; }

		[JsonPropertyName("pids_stats")]
		public PidsStats PidsStats { get; init; }

		[JsonPropertyName("blkio_stats")]
		public BlkioStats BlkioStats { get; init; }

		[JsonPropertyName("num_procs")]
		public int NumProcs { get; init; }

		// storage_stats is an empty object in the sample; keep as JsonElement to preserve shape
		[JsonPropertyName("storage_stats")]
		public JsonElement StorageStats { get; init; }
	}



	public record struct CpuStats
	{
		[JsonPropertyName("cpu_usage")]
		public CpuUsage CpuUsage { get; init; }

		[JsonPropertyName("system_cpu_usage")]
		public long SystemCpuUsage { get; init; }

		[JsonPropertyName("online_cpus")]
		public int OnlineCpus { get; init; }

		[JsonPropertyName("throttling_data")]
		public ThrottlingData ThrottlingData { get; init; }
	}



	public record struct CpuUsage
	{
		[JsonPropertyName("total_usage")]
		public long TotalUsage { get; init; }

		[JsonPropertyName("usage_in_kernelmode")]
		public long UsageInKernelmode { get; init; }

		[JsonPropertyName("usage_in_usermode")]
		public long UsageInUsermode { get; init; }
	}



	public record struct ThrottlingData
	{
		[JsonPropertyName("periods")]
		public long Periods { get; init; }

		[JsonPropertyName("throttled_periods")]
		public long ThrottledPeriods { get; init; }

		[JsonPropertyName("throttled_time")]
		public long ThrottledTime { get; init; }
	}



	public record struct MemoryStats
	{
		[JsonPropertyName("usage")]
		public long Usage { get; init; }

		[JsonPropertyName("limit")]
		public long Limit { get; init; }

		[JsonPropertyName("stats")]
		public MemoryDetails Stats { get; init; }
	}



	public record struct MemoryDetails
	{
		[JsonPropertyName("active_anon")]
		public long ActiveAnon { get; init; }

		[JsonPropertyName("active_file")]
		public long ActiveFile { get; init; }

		[JsonPropertyName("anon")]
		public long Anon { get; init; }

		[JsonPropertyName("anon_thp")]
		public long AnonThp { get; init; }

		[JsonPropertyName("file")]
		public long File { get; init; }

		[JsonPropertyName("file_dirty")]
		public long FileDirty { get; init; }

		[JsonPropertyName("file_mapped")]
		public long FileMapped { get; init; }

		[JsonPropertyName("file_writeback")]
		public long FileWriteback { get; init; }

		[JsonPropertyName("inactive_anon")]
		public long InactiveAnon { get; init; }

		[JsonPropertyName("inactive_file")]
		public long InactiveFile { get; init; }

		[JsonPropertyName("kernel_stack")]
		public long KernelStack { get; init; }

		[JsonPropertyName("pgactivate")]
		public long PgActivate { get; init; }

		[JsonPropertyName("pgdeactivate")]
		public long PgDeactivate { get; init; }

		[JsonPropertyName("pgfault")]
		public long PgFault { get; init; }

		[JsonPropertyName("pglazyfree")]
		public long PgLazyFree { get; init; }

		[JsonPropertyName("pglazyfreed")]
		public long PgLazyFreed { get; init; }

		[JsonPropertyName("pgmajfault")]
		public long PgMajFault { get; init; }

		[JsonPropertyName("pgrefill")]
		public long PgRefill { get; init; }

		[JsonPropertyName("pgscan")]
		public long PgScan { get; init; }

		[JsonPropertyName("pgsteal")]
		public long PgSteal { get; init; }

		[JsonPropertyName("shmem")]
		public long Shmem { get; init; }

		[JsonPropertyName("slab")]
		public long Slab { get; init; }

		[JsonPropertyName("slab_reclaimable")]
		public long SlabReclaimable { get; init; }

		[JsonPropertyName("slab_unreclaimable")]
		public long SlabUnreclaimable { get; init; }

		[JsonPropertyName("sock")]
		public long Sock { get; init; }

		[JsonPropertyName("thp_collapse_alloc")]
		public long ThpCollapseAlloc { get; init; }

		[JsonPropertyName("thp_fault_alloc")]
		public long ThpFaultAlloc { get; init; }

		[JsonPropertyName("unevictable")]
		public long Unevictable { get; init; }

		[JsonPropertyName("workingset_activate")]
		public long WorkingsetActivate { get; init; }

		[JsonPropertyName("workingset_nodereclaim")]
		public long WorkingsetNodeReclaim { get; init; }

		[JsonPropertyName("workingset_refault")]
		public long WorkingsetRefault { get; init; }
	}



	public record struct NetworkInterface
	{
		[JsonPropertyName("rx_bytes")]
		public long RxBytes { get; init; }

		[JsonPropertyName("rx_packets")]
		public long RxPackets { get; init; }

		[JsonPropertyName("rx_errors")]
		public long RxErrors { get; init; }

		[JsonPropertyName("rx_dropped")]
		public long RxDropped { get; init; }

		[JsonPropertyName("tx_bytes")]
		public long TxBytes { get; init; }

		[JsonPropertyName("tx_packets")]
		public long TxPackets { get; init; }

		[JsonPropertyName("tx_errors")]
		public long TxErrors { get; init; }

		[JsonPropertyName("tx_dropped")]
		public long TxDropped { get; init; }
	}



	public record struct PidsStats
	{
		[JsonPropertyName("current")]
		public int Current { get; init; }

		[JsonPropertyName("limit")]
		public int Limit { get; init; }
	}



	public record struct BlkioStats
	{
		[JsonPropertyName("io_service_bytes_recursive")]
		public List<IoServiceEntry> IoServiceBytesRecursive { get; init; }

		// Several properties in the sample are null; keep them as JsonElement to preserve null vs object
		[JsonPropertyName("io_serviced_recursive")]
		public JsonElement IoServicedRecursive { get; init; }

		[JsonPropertyName("io_queue_recursive")]
		public JsonElement IoQueueRecursive { get; init; }

		[JsonPropertyName("io_service_time_recursive")]
		public JsonElement IoServiceTimeRecursive { get; init; }

		[JsonPropertyName("io_wait_time_recursive")]
		public JsonElement IoWaitTimeRecursive { get; init; }

		[JsonPropertyName("io_merged_recursive")]
		public JsonElement IoMergedRecursive { get; init; }

		[JsonPropertyName("io_time_recursive")]
		public JsonElement IoTimeRecursive { get; init; }

		[JsonPropertyName("sectors_recursive")]
		public JsonElement SectorsRecursive { get; init; }
	}



	public record struct IoServiceEntry
	{
		[JsonPropertyName("major")]
		public int Major { get; init; }

		[JsonPropertyName("minor")]
		public int Minor { get; init; }

		[JsonPropertyName("op")]
		public string Op { get; init; }

		[JsonPropertyName("value")]
		public long Value { get; init; }
	}
}