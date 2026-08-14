using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class ExtrinsicGameManager : MonoBehaviour
{
    public static ExtrinsicGameManager Instance { get; private set; }

    [Header("Objectives")]
    [SerializeField] TMP_Text bedroomText;
    [SerializeField] TMP_Text livingAreaText;
    [SerializeField] TMP_Text kitchenText;
    [SerializeField] TMP_Text backyardText;

    [Header("Progress Bars")]
    [SerializeField] Image bedroomProgressBar;
    [SerializeField] Image livingAreaProgressBar;
    [SerializeField] Image kitchenProgressBar;
    [SerializeField] Image backyardProgressBar;

    [Header("Borders")]
    [SerializeField] GameObject bedroomBorder;
    [SerializeField] GameObject livingAreaBorder;
    [SerializeField] GameObject kitchenBorder;

    [Header("Badges")]
    [SerializeField] GameObject BedroomBadge;
    [SerializeField] GameObject livingAreaBadge;
    [SerializeField] GameObject KitchenBadge;
    [SerializeField] GameObject GardenBadge;

    [Header("Audio")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip doorUnlockSound;
    [SerializeField] AudioClip gameFinishSound;

    [Header("Overall Score Displays")]
    [SerializeField] TMP_Text[] scoreTexts;

    [Header("Overall Score Progress Bars")]
    [SerializeField] Image[] scoreProgressBars;

    [Header("Finish Canvas")]
    [SerializeField] GameObject FinishGameUI;
    [SerializeField] float finishCanvasDuration = 5f;

    int totalCollected = 0;
    int score = 0;

    const int maxScore = 1000;


    void Awake()
    {
        Instance = this;
    }


    void Start()
    {
        // Hide badges
        BedroomBadge.SetActive(false);
        livingAreaBadge.SetActive(false);
        KitchenBadge.SetActive(false);
        GardenBadge.SetActive(false);

        // Hide finish UI
        if (FinishGameUI != null)
        {
            FinishGameUI.SetActive(false);
        }

        UpdateObjectiveUI();
        UpdateScore();
    }


    public void ItemCollected()
    {
        // Stop counting after all 10 items
        if (totalCollected >= 10)
            return;

        totalCollected++;

        // Each item is worth 100 points
        score = totalCollected * 100;

        UpdateObjectiveUI();
        UpdateScore();
        CheckBorderRemoverandBadges();
        CheckGameComplete();
    }


    void UpdateObjectiveUI()
    {
        int bedroom =
            Mathf.Clamp(totalCollected, 0, 2);

        int livingArea =
            Mathf.Clamp(totalCollected - 2, 0, 2);

        int kitchen =
            Mathf.Clamp(totalCollected - 4, 0, 2);

        int backyard =
            Mathf.Clamp(totalCollected - 6, 0, 4);


        // Objective counters
        bedroomText.text = $"{bedroom}/2";
        livingAreaText.text = $"{livingArea}/2";
        kitchenText.text = $"{kitchen}/2";
        backyardText.text = $"{backyard}/4";


        // Objective progress bars
        bedroomProgressBar.fillAmount =
            bedroom / 2f;

        livingAreaProgressBar.fillAmount =
            livingArea / 2f;

        kitchenProgressBar.fillAmount =
            kitchen / 2f;

        backyardProgressBar.fillAmount =
            backyard / 4f;
    }


    void UpdateScore()
    {
        // Update all 4 score displays
        foreach (TMP_Text scoreText in scoreTexts)
        {
            if (scoreText != null)
            {
                scoreText.text = $"{score}/1000";
            }
        }


        // Calculate overall progress from 0 - 1
        float scoreProgress = score / (float)maxScore;


        // Update all 4 score progress bars
        foreach (Image progressBar in scoreProgressBars)
        {
            if (progressBar != null)
            {
                progressBar.fillAmount = scoreProgress;
            }
        }
    }


    void CheckBorderRemoverandBadges()
    {
        // Bedroom complete
        if (totalCollected >= 2 && bedroomBorder.activeSelf)
        {
            bedroomBorder.SetActive(false);
            BedroomBadge.SetActive(true);

            PlayDoorUnlockSound();
        }


        // Living Area complete
        if (totalCollected >= 4 && livingAreaBorder.activeSelf)
        {
            livingAreaBorder.SetActive(false);
            livingAreaBadge.SetActive(true);

            PlayDoorUnlockSound();
        }


        // Kitchen complete
        if (totalCollected >= 6 && kitchenBorder.activeSelf)
        {
            kitchenBorder.SetActive(false);
            KitchenBadge.SetActive(true);

            PlayDoorUnlockSound();
        }


        // Backyard complete
        if (totalCollected >= 10)
        {
            GardenBadge.SetActive(true);
        }
    }

    void PlayDoorUnlockSound()
    {
        if (audioSource != null && doorUnlockSound != null)
        {
            audioSource.PlayOneShot(doorUnlockSound);
        }
    }

    void CheckGameComplete()
    {
        if (totalCollected >= 10)
        {
            if (FinishGameUI != null)
            {
                FinishGameUI.SetActive(true);

                StartCoroutine(HideFinishGameUI());
            }

            if (audioSource != null && gameFinishSound != null)
            {
                audioSource.PlayOneShot(gameFinishSound);
            }

            Debug.Log("Extrinsic condition complete!");
            Debug.Log("Final Score: " + score);
        }
    }

    IEnumerator HideFinishGameUI()
    {
        yield return new WaitForSeconds(finishCanvasDuration);

        if (FinishGameUI != null)
        {
            FinishGameUI.SetActive(false);
        }
    }
}