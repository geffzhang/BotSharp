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
/// DaprJob event subscription service listens for job events from Dapr
/// </summary>
public class DaprJobEventSubscription : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<DaprJobEventSubscription> _logger;
    private readonly DaprJobSettings _settings;

    public DaprJobEventSubscription(IServiceProvider services, ILogger<DaprJobEventSubscription> logger, DaprJobSettings settings)
    {
        _services = services;
        _logger = logger;
        _settings = settings;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("DaprJob event subscription started");
        
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // This would typically subscribe to Dapr pub/sub events
                // For now, we'll just wait
                await Task.Delay(TimeSpan.FromSeconds(60), stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DaprJob event subscription");
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }
        
        _logger.LogInformation("DaprJob event subscription stopped");
    }
}