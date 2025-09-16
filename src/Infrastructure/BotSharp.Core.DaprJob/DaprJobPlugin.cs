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

using BotSharp.Abstraction.Tasks;
using BotSharp.Core.DaprJob.Hooks;

namespace BotSharp.Core.DaprJob;

/// <summary>
/// DaprJob plugin is a distributed job scheduler using Dapr Jobs API building block.
/// It provides scheduling capabilities for AI agents using Dapr's built-in job management features.
/// </summary>
public class DaprJobPlugin : IBotSharpPlugin
{
    public string Id => "4266c26f-39e4-54f8-9fbe-gd54435fd32b";
    public string Name => "BotSharp DaprJob";
    public string Description => "DaprJob plugin is a distributed job scheduler using Dapr Jobs API building block for triggering AI Agent tasks.";
    public string IconUrl => "https://docs.dapr.io/images/dapr.svg";

    public void RegisterDI(IServiceCollection services, IConfiguration config)
    {
        var settings = new DaprJobSettings();
        config.Bind("DaprJob", settings);
        services.AddSingleton(settings);

        services.AddScoped<IAgentUtilityHook, DaprJobUtilityHook>();
        services.AddScoped<IDaprJobService, DaprJobService>();
        services.AddScoped<ITaskFeeder, DaprJobService>();

        // Add Dapr HTTP client
        services.AddHttpClient("dapr", client =>
        {
            client.BaseAddress = new Uri(settings.DaprHttpEndpoint);
        });

        if (settings.Watcher?.Enabled == true)
        {
            services.AddHostedService<DaprJobWatcher>();
        }

        if (settings.EventSubscriber?.Enabled == true)
        {
            services.AddHostedService<DaprJobEventSubscription>();
        }
    }
}