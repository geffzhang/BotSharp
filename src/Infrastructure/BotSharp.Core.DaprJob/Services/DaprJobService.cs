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

using BotSharp.Abstraction.Agents.Models;
using BotSharp.Abstraction.Repositories;
using BotSharp.Abstraction.Repositories.Filters;
using BotSharp.Abstraction.Tasks;
using BotSharp.Abstraction.Tasks.Models;
using BotSharp.Abstraction.Utilities;
using BotSharp.Core.Infrastructures;
using Dapr.Client;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace BotSharp.Core.DaprJob.Services;

/// <summary>
/// The DaprJob service schedules distributed jobs using Dapr Jobs API building block.
/// It provides distributed job scheduling capabilities with built-in persistence and reliability.
/// </summary>
public class DaprJobService : IDaprJobService, ITaskFeeder
{
    private readonly IServiceProvider _services;
    private readonly ILogger _logger;
    private readonly DaprJobSettings _settings;
    private readonly DaprClient _daprClient;

    public DaprJobService(IServiceProvider services, ILogger<DaprJobService> logger, DaprJobSettings settings)
    {
        _services = services;
        _logger = logger;
        _settings = settings;
        _daprClient = new DaprClientBuilder()
            .UseHttpEndpoint(_settings.DaprHttpEndpoint)
            .Build();
    }

    public async Task<List<DaprJobItem>> GetDaprJobs()
    {
        var jobs = new List<DaprJobItem>();
        
        // Add fixed dapr job items from sources
        var jobSources = _services.GetServices<IDaprJobSource>();
        foreach (var source in jobSources)
        {
            if (source.IsRealTime)
            {
                continue;
            }
            var item = source.GetDaprJobItem();
            jobs.Add(item);
        }

        return jobs;
    }

    public async Task<List<AgentTask>> GetTasks()
    {
        var tasks = new List<AgentTask>();
        var agentService = _services.GetRequiredService<IAgentService>();
        var jobSources = _services.GetServices<IDaprJobSource>();

        // Get all agent subscribed to this job scheduler
        var agents = await agentService.GetAgents(new AgentFilter
        {
            Pager = new Pagination
            {
                Size = 1000
            }
        });

        foreach (var source in jobSources)
        {
            var job = source.GetDaprJobItem();
            var preFilteredAgents = agents.Items.Where(x =>
                x.Rules.Exists(r => r.TriggerName == job.Title)).ToList();

            tasks.AddRange(preFilteredAgents.Select(x => new AgentTask
            {
                Id = Guid.Empty.ToString(),
                AgentId = x.Id,
                Agent = new Agent
                {
                    Name = x.Name,
                    Description = x.Description
                },
                Name = FormatJobName(job.Title, x.Name),
                Content = $"Trigger: {job.Title}\r\nAgent: {x.Name}\r\nDue Time: {job.DueTime}",
                Status = TaskStatus.Scheduled,
                Enabled = !x.Disabled,
                Description = job.Description,
                LastExecutionTime = job.LastExecutionTime
            }));
        }

        return tasks;
    }

    private string FormatJobName(string trigger, string agent)
    {
        trigger = trigger.Replace("RuleTrigger", string.Empty);
        trigger = Regex.Replace(trigger, "(?<!^)([A-Z])", " $1");
        agent = agent.Replace("Operator", string.Empty);
        return $"{trigger}";
    }

    public async Task ScheduleJob(DaprJobItem item)
    {
        try
        {
            var jobData = new
            {
                name = item.Name,
                dueTime = item.DueTime?.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                period = item.Period?.ToString() + "s",
                repeats = item.Repeats,
                ttl = item.Ttl?.ToString() + "s",
                data = new
                {
                    agentId = item.AgentId,
                    conversationId = item.ConversationId,
                    userId = item.UserId,
                    payload = item.Data
                }
            };

            var response = await _daprClient.InvokeMethodAsync<object, object>("dapr", $"v1.0-alpha1/jobs/{item.Name}", jobData);
            _logger.LogInformation($"Job {item.Name} scheduled successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to schedule job {item.Name}");
            throw;
        }
    }

    public async Task DeleteJob(string jobName)
    {
        try
        {
            await _daprClient.InvokeMethodAsync("dapr", $"v1.0-alpha1/jobs/{jobName}", HttpMethod.Delete);
            _logger.LogInformation($"Job {jobName} deleted successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to delete job {jobName}");
            throw;
        }
    }

    public async Task<DaprJobItem?> GetJob(string jobName)
    {
        try
        {
            var response = await _daprClient.InvokeMethodAsync<object>("dapr", $"v1.0-alpha1/jobs/{jobName}");
            // Convert response to DaprJobItem - this would need proper mapping
            return null; // Placeholder
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to get job {jobName}");
            return null;
        }
    }

    public async Task JobTriggered(DaprJobItem item)
    {
        _logger.LogDebug($"JobTriggered {item}");

        await HookEmitter.Emit<IDaprJobHook>(_services, async hook =>
        {
            if (hook.Triggers == null || hook.Triggers.Contains(item.Title))
            {
                hook.OnAuthenticate(item);
                await hook.OnTaskExecuting(item);
                await hook.OnJobTriggered(item);
                await hook.OnTaskExecuted(item);
            }
        }, item.AgentId);
    }
}