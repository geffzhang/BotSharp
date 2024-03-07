using BotSharp.Plugin.MongoStorage.Models;

namespace BotSharp.Plugin.MongoStorage.Collections;

public class LlmCompletionLogDocument : MongoBase
{
    public string ConversationId { get; set; }
    public List<PromptLogMongoElement> Logs { get; set; }
}
