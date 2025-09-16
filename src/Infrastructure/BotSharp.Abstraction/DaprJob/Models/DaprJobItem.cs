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

namespace BotSharp.Abstraction.DaprJob.Models;

public class DaprJobItem
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? AgentId { get; set; }
    public string? ConversationId { get; set; }
    public string? UserId { get; set; }
    public DateTime? DueTime { get; set; }
    public string? Data { get; set; }
    public int? Ttl { get; set; }
    public int? Period { get; set; }
    public int? Repeats { get; set; }
    public DateTime? LastExecutionTime { get; set; }
    public DateTime CreatedTime { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedTime { get; set; } = DateTime.UtcNow;

    public override string ToString()
    {
        return $"[{Id}] {Name} - {Title} (Next: {DueTime})";
    }
}