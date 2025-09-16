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

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BotSharp.Abstraction.DaprJob.Models;

public class ScheduleJobArgs
{
    /// <summary>
    /// Job name
    /// </summary>
    [Required]
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Job description
    /// </summary>
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// When to run the job (ISO 8601 format)
    /// </summary>
    [JsonPropertyName("due_time")]
    public string? DueTime { get; set; }

    /// <summary>
    /// Job data payload
    /// </summary>
    [JsonPropertyName("data")]
    public string? Data { get; set; }

    /// <summary>
    /// Time to live in seconds
    /// </summary>
    [JsonPropertyName("ttl")]
    public int? Ttl { get; set; }

    /// <summary>
    /// Repeat period in seconds
    /// </summary>
    [JsonPropertyName("period")]
    public int? Period { get; set; }

    /// <summary>
    /// Number of repeats
    /// </summary>
    [JsonPropertyName("repeats")]
    public int? Repeats { get; set; }
}