using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    [SerializeField] private Transform questContainer;
    [SerializeField] private QuestLogger questPrefab;

    private Dictionary<GameObject, Quest> quests = new Dictionary<GameObject, Quest>();

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    public void AddQuest(string name, string info)
    {
        if (HasQuest(name)) return;

        Quest newQuest = new Quest(name, info, false);
        GameObject qObject = Instantiate(questPrefab.gameObject, questContainer);
        quests.Add(qObject, newQuest);

        if (qObject.TryGetComponent(out QuestLogger qLogger))
        {
            qLogger.UpdateUI(newQuest);
            qLogger.PlayShowAnimation();
        }
    }

    public void CompleteQuest(string name)
    {
        GameObject objectToDelete = null;

        foreach (var kv in quests)
        {
            if (kv.Value.questName == name)
            {
                kv.Value.isCompleted = true;

                if (kv.Key.TryGetComponent(out QuestLogger qLogger))
                {
                    objectToDelete = kv.Key;

                    qLogger.UpdateUI(kv.Value);
                    qLogger.PlayCompleteAnimation(() =>
                    {
                        quests.Remove(objectToDelete);
                        Destroy(objectToDelete);
                    });
                }

                return;
            }
        }
    }


    public void UnCompleteQuest(string name)
    {
        foreach (var kv in quests)
        {
            if (kv.Value.questName == name)
            {
                kv.Value.isCompleted = false;

                if (kv.Key.TryGetComponent(out QuestLogger qLogger))
                {
                    qLogger.UpdateUI(kv.Value);
                }

                return;
            }
        }
    }

    public void DeleteQuest(string name)
    {
        GameObject objectToDelete = null;

        foreach (var kv in quests)
        {
            if (kv.Value.questName == name)
            {
                objectToDelete = kv.Key;
                break;
            }
        }

        if (objectToDelete != null)
        {
            quests.Remove(objectToDelete);
            Destroy(objectToDelete);
        }
    }

    public bool HasQuest(string name)
    {
        foreach (var kv in quests)
        {
            if (kv.Value.questName == name)
                return true;
        }
        return false;
    }
}
