/*****************************************************************************
  Copyright 2024 Written by Haiping Chen. All Rights Reserved.

  Licensed under the Apache License, Version 2.0 (the "License");
  you may not use this file except in compliance with the License.
  You may obtain a copy of the License at

      http://www.apache.org/licenses/LICENSE-2.0

  Unless required by applicable law or agreed to in writing, software
  distributed under the License is distributed on an "AS IS" BASIS,
  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
  See the License for the specific language governing permissions and
  limitations under the License.
******************************************************************************/

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BotSharp.Core.DaprJob.Services;

/// <summary>
/// DaprJob watcher monitors and manages scheduled jobs using Dapr Jobs API
/// </summary>
public class DaprJobWatcher : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<DaprJobWatcher> _logger;
    private readonly DaprJobSettings _settings;

    public DaprJobWatcher(IServiceProvider services, ILogger<DaprJobWatcher> logger, DaprJobSettings settings)
    {
        _services = services;
        _logger = logger;
        _settings = settings;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("DaprJob watcher started");
        
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CheckJobs();
                await Task.Delay(TimeSpan.FromSeconds(_settings.Watcher?.IntervalInSeconds ?? 60), stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DaprJob watcher");
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }
        
        _logger.LogInformation("DaprJob watcher stopped");
    }

    private async Task CheckJobs()
    {
        try
        {
            using var scope = _services.CreateScope();
            var daprJobService = scope.ServiceProvider.GetRequiredService<IDaprJobService>();
            
            var jobs = await daprJobService.GetDaprJobs();
            
            foreach (var job in jobs)
            {
                if (job.DueTime.HasValue && job.DueTime <= DateTime.UtcNow)
                {
                    _logger.LogInformation($"Processing job: {job.Name}");
                    
                    if (daprJobService is DaprJobService service)
                    {
                        await service.JobTriggered(job);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking jobs");
        }
    }
}