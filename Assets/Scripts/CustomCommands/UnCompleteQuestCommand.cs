using Naninovel;

[CommandAlias("uncompleteQuest")]
public class UnCompleteQuestCommand : Command
{
    [ParameterAlias("name")] public StringParameter QuestName;

    public override UniTask ExecuteAsync(AsyncToken asyncToken = default)
    {
        QuestManager.Instance.CompleteQuest(QuestName);
        return UniTask.CompletedTask;
    }
}