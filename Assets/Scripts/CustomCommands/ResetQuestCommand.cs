using Naninovel;

[CommandAlias("resetQuest")]
public class ResetQuestCommand : Command
{
    [ParameterAlias("name")] public StringParameter QuestName;

    public override UniTask ExecuteAsync(AsyncToken asyncToken = default)
    {
        string quest = QuestName;

        if (QuestManager.Instance.HasQuest(quest))
        {
            QuestManager.Instance.DeleteQuest(quest);
        }

        return UniTask.CompletedTask;
    }
}
