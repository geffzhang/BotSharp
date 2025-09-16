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

namespace BotSharp.Core.DaprJob.Functions;

public class DeleteJobFn : IFunctionCallback
{
    public string Name => "delete_job";
    public string Indication => "Delete a scheduled job using Dapr Jobs API";

    private readonly IDaprJobService _daprJobService;

    public DeleteJobFn(IDaprJobService daprJobService)
    {
        _daprJobService = daprJobService;
    }

    public async Task<bool> Execute(RoleDialogModel message)
    {
        var args = JsonSerializer.Deserialize<DeleteJobArgs>(message.FunctionArgs);
        
        await _daprJobService.DeleteJob(args.Name);
        
        message.Content = $"Job '{args.Name}' has been deleted successfully.";
        return true;
    }
}