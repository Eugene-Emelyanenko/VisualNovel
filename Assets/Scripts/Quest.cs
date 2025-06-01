using System;

[Serializable]
public class Quest
{
    public string questName;
    public string info;
    public bool isCompleted;

    public Quest(string questName, string info, bool isCompleted)
    {
        this.questName = questName;
        this.info = info;
        this.isCompleted = isCompleted;
    }
}