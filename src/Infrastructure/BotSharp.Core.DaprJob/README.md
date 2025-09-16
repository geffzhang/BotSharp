# BotSharp.Core.DaprJob

A distributed job scheduler plugin for BotSharp that integrates with Dapr Jobs API building block. This module provides scheduled task execution capabilities for AI agents using Dapr's built-in job management features.

## Features

- **Distributed Job Scheduling**: Uses Dapr Jobs API for reliable, distributed job execution
- **Agent Integration**: Seamlessly integrates with BotSharp's agent framework
- **Flexible Scheduling**: Supports one-time, recurring, and delayed job execution
- **Configurable**: Easy configuration through appsettings.json
- **Hook System**: Extensible hook system for custom job processing logic

## Configuration

Add the following configuration to your `appsettings.json`:

```json
{
  "DaprJob": {
    "DaprHttpEndpoint": "http://localhost:3500",
    "JobStoreName": "jobstore",
    "Watcher": {
      "Enabled": true,
      "IntervalInSeconds": 60
    },
    "EventSubscriber": {
      "Enabled": true,
      "EventType": "dapr-job-event"
    }
  }
}
```

## Usage

### Register the Plugin

Add the DaprJob plugin to your BotSharp application:

```csharp
services.AddBotSharp(config =>
{
    config.Plugins = new string[]
    {
        // Other plugins...
        "BotSharp.Core.DaprJob"
    };
});
```

### Schedule a Job

Use the `schedule_job` function in your agent:

```json
{
  "name": "my-scheduled-task",
  "description": "Daily data processing task",
  "due_time": "2024-01-01T10:00:00Z",
  "period": 86400,
  "repeats": -1,
  "data": "{\"task\": \"process_data\"}"
}
```

### Delete a Job

Use the `delete_job` function to remove a scheduled job:

```json
{
  "name": "my-scheduled-task"
}
```

## Architecture

The module follows the same architectural patterns as other BotSharp infrastructure modules:

- **Abstractions**: Interfaces and models in `BotSharp.Abstraction.DaprJob`
- **Implementation**: Core services in `BotSharp.Core.DaprJob`
- **Hooks**: Extension points for custom job processing
- **Functions**: Agent-callable functions for job management
- **Background Services**: Watchers and event subscribers

## Dependencies

- Dapr.Client (>= 1.14.0)
- BotSharp.Core
- BotSharp.Abstraction

## Related Documentation

- [Dapr Jobs API](https://docs.dapr.io/developing-applications/building-blocks/jobs/)
- [BotSharp Plugin Development](https://docs.botsharp.org)