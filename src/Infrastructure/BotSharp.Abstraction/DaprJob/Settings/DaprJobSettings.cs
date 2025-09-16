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

namespace BotSharp.Abstraction.DaprJob.Settings;

public class DaprJobSettings
{
    public DaprWatcherSetting? Watcher { get; set; }
    public DaprEventSubscriberSetting? EventSubscriber { get; set; }
    public string DaprHttpEndpoint { get; set; } = "http://localhost:3500";
    public string JobStoreName { get; set; } = "jobstore";
}

public class DaprWatcherSetting
{
    public bool Enabled { get; set; }
    public int IntervalInSeconds { get; set; } = 60;
}

public class DaprEventSubscriberSetting
{
    public bool Enabled { get; set; }
    public string EventType { get; set; } = "dapr-job-event";
}