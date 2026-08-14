using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class IntrinsicGameManager : MonoBehaviour
{
    public static IntrinsicGameManager Instance { get; private set; }

    [Header("Intrinsic Condition")]
    [SerializeField] int totalItems = 10;

    [Header("Completion")]
    [SerializeField] GameObject completionPanel;

    HashSet<CollectableMalteseItem> collectedItems =
        new HashSet<CollectableMalteseItem>();

    bool conditionCompleted = false;

    public int CollectedCount => collectedItems.Count;
    public bool ConditionCompleted => conditionCompleted;

    [SerializeField] float completionPanelDuration = 5f;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        // Keep completion message hidden until all items are collected.
        if (completionPanel != null)
        {
            completionPanel.SetActive(false);
        }
    }

    public void RegisterCollectedItem(CollectableMalteseItem item)
    {
        if (conditionCompleted)
            return;

        if (item == null)
            return;

        // HashSet prevents the same item from being counted twice.
        bool newItem = collectedItems.Add(item);

        if (!newItem)
            return;

        Debug.Log(
            "Intrinsic item collected: " +
            collectedItems.Count +
            " / " +
            totalItems
        );

        if (collectedItems.Count >= totalItems)
        {
            CompleteCondition();
        }
    }

    void CompleteCondition()
    {
        conditionCompleted = true;

        Debug.Log("Intrinsic condition completed!");

        if (completionPanel != null)
        {
            completionPanel.SetActive(true);
            StartCoroutine(HideCompletionPanel());
        }
    }

    IEnumerator HideCompletionPanel()
    {
        yield return new WaitForSeconds(completionPanelDuration);

        if (completionPanel != null)
        {
            completionPanel.SetActive(false);
        }
    }
}
