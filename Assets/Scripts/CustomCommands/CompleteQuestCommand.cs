using Naninovel;

[CommandAlias("completeQuest")]
public class CompleteQuestCommand : Command
{
    [ParameterAlias("name")] public StringParameter QuestName;

    public override UniTask ExecuteAsync(AsyncToken asyncToken = default)
    {
        QuestManager.Instance.CompleteQuest(QuestName);
        return UniTask.CompletedTask;
    }
}