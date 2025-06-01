using Naninovel;

[CommandAlias("addQuest")]
public class AddQuestCommand : Command
{
    [ParameterAlias("name")] public StringParameter QuestName;
    [ParameterAlias("info")] public StringParameter Info;

    public override UniTask ExecuteAsync(AsyncToken asyncToken = default)
    {
        QuestManager.Instance.AddQuest(QuestName, Info);
        return UniTask.CompletedTask;
    }
}
