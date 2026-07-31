using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics;
using TMPro;

public class WackAMoleGameManager : MonoBehaviour
{
    [Header("Holes Parent")]
    [SerializeField] Transform holesParent;

    [Header("Mole Prefabs")]
    [SerializeField] GameObject normalMolePrefab;
    [SerializeField] GameObject goldMolePrefab;

    [Header("Spawn Chances")]
    [SerializeField] float goldChance = 0.1f;

    [Header("Timing")]
    [SerializeField] float spawnDelay = 0.6f;
    [SerializeField] float moleUpTime = 1.2f;
    [SerializeField] float maxMoveTime = 0.2f;

    [Header("Round")]
    [SerializeField] float roundSeconds = 60f;

    [Header("Score")]
    [SerializeField] int goldMoleScore = 150;
    [SerializeField] int normalMoleScore = 50;

    [Header("GUI")]
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] float scoreDisplayTime = 0.7f;

    [Header("Haptic Feedback Components")]
    [SerializeField] GameObject leftHand;
    [SerializeField] GameObject rightHand;
    HapticImpulsePlayer leftHaptics;
    HapticImpulsePlayer rightHaptics;

    [Header("SoundEffects")]
    [SerializeField] AudioClip sfx_moleHit;
    [SerializeField] AudioClip sfx_timesUp;
    AudioSource audioSource;


    int score = 0;
    float timeLeft;

    bool[] holeBusy;

    bool gameRunning = false;
    Coroutine spawnLoop;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        leftHaptics = leftHand.GetComponent<HapticImpulsePlayer>();
        rightHaptics = rightHand.GetComponent<HapticImpulsePlayer>();
        holeBusy = new bool[holesParent.childCount];

        UpdateScoreUI();
    }


    public void StartGame()
    {
        StartRound();
    }

    void Update()
    {
        if (!gameRunning) return;

        timeLeft -= Time.deltaTime;

        if (timeLeft <= 0f)
        {
            EndRound();

            return;
        }

        UpdateTimerUI();
    }

    void StartRound()
    {
        if (spawnLoop != null)
        {
            StopCoroutine(spawnLoop);
        }
            
        score = 0;
        timeLeft = roundSeconds;
        gameRunning = true;

        //Reset the bsuy holes
        for (int i = 0; i < holeBusy.Length; i++)
        {
            holeBusy[i] = false;
        }

        UpdateScoreUI();
        UpdateTimerUI();

        spawnLoop = StartCoroutine(SpawnMoles());
    }


    IEnumerator SpawnMoles()
    {
        while (gameRunning)
        {
            SpawnSingleMole();
            yield return new WaitForSeconds(spawnDelay);
        }
    }

    void SpawnSingleMole()
    {
        int holeIndex = GetFreeHoleIndex();
        if (holeIndex == -1) return; // all holes are busy, skip this spawn

        holeBusy[holeIndex] = true;

        Transform hole = holesParent.GetChild(holeIndex);
        Transform popUpPos = hole.GetChild(0);
        Transform hiddenPos = hole.GetChild(1);

        bool isGold = Random.value < goldChance;

        GameObject molePrefab;
        int points;

        if (isGold)
        {
            molePrefab = goldMolePrefab;
            points = goldMoleScore;
        }
        else
        {
            molePrefab = normalMolePrefab;
            points = normalMoleScore;
        }

        GameObject mole = Instantiate(molePrefab, hiddenPos.position, hole.rotation, hole);
        mole.transform.localScale = hole.localScale;

        MoleProperties moleScript = mole.GetComponent<MoleProperties>();
        moleScript.SetUp(this, points);

        StartCoroutine(MoveMole(mole, popUpPos, hiddenPos, holeIndex));
    }

    int GetFreeHoleIndex()
    {
        for (int i = 0; i < 10; i++)
        {
            int index = Random.Range(0, holesParent.childCount);
            if (!holeBusy[index])
                return index;
        }

        for (int i = 0; i < holesParent.childCount; i++)
        {
            if (!holeBusy[i])
                return i;
        }

        return -1; //To show that noen of the holes aer open
    }

    IEnumerator MoveMole(GameObject mole, Transform popUpPos, Transform hiddenPos, int holeIndex)
    {
        float currentMoveTime = 0f;

        //To be safe if a mole gets destroyed early
        if (mole == null)
        {
            holeBusy[holeIndex] = false;
            yield break;
        }

        mole.transform.position = hiddenPos.position;

        // Move mole up
        while (currentMoveTime < maxMoveTime)
        {
            currentMoveTime += Time.deltaTime;
            float percent = currentMoveTime / maxMoveTime;
            if( mole != null)
            {
                mole.transform.position = Vector3.Lerp(hiddenPos.position, popUpPos.position, percent);

            }
            yield return null;
        }

        yield return new WaitForSeconds(moleUpTime);

        // Move mole down
        currentMoveTime = 0f;
        while (currentMoveTime < maxMoveTime)
        {
            currentMoveTime += Time.deltaTime;
            float percent = currentMoveTime / maxMoveTime;
            if (mole != null)
            {
                mole.transform.position = Vector3.Lerp(popUpPos.position, hiddenPos.position, percent);
            }
            yield return null;
        }

        if (mole != null)
        {
            Destroy(mole);

        }
        holeBusy[holeIndex] = false;
    }

    public void AddScore(int amount)
    {
        if (!gameRunning) return;

        score += amount;
        PlaySound(sfx_moleHit, 1f);

        TriggerHaptic(.25f, .1f, 0f);
        UpdateScoreUI();
    }

    void UpdateScoreUI()
    {
        if (scoreText == null) return;
        scoreText.text = score.ToString();
    }

    void UpdateTimerUI()
    {
        if (timerText == null) return;
        timerText.text = Mathf.CeilToInt(timeLeft).ToString();
    }

    void EndRound()
    {

        PlaySound(sfx_timesUp, .6f);
        TriggerHaptic(.7f, .4f, 0f);

        gameRunning = false;

        if (spawnLoop != null)
        {
            StopCoroutine(spawnLoop);
        }

        // Show 0 on timer
        if (timerText != null)
        {
            timerText.text = "0";
        }

        // Blink final score
        StartCoroutine(AppearAndDisappearScore());
    }

    IEnumerator AppearAndDisappearScore()
    {
        scoreText.text = "";
        yield return new WaitForSeconds(scoreDisplayTime);
        scoreText.text = score.ToString();
        yield return new WaitForSeconds(scoreDisplayTime);
        scoreText.text = "";
        yield return new WaitForSeconds(scoreDisplayTime);
        scoreText.text = score.ToString();
        yield return new WaitForSeconds(scoreDisplayTime);
        scoreText.text = "";
        yield return new WaitForSeconds(scoreDisplayTime);
        scoreText.text = score.ToString();
        yield return new WaitForSeconds(scoreDisplayTime);
        scoreText.text = "";
        yield return new WaitForSeconds(scoreDisplayTime);
        scoreText.text = score.ToString();

    }

    public void EndGame()
    {
        gameRunning = false;

        if (spawnLoop != null)
            StopCoroutine(spawnLoop);
    }

    void TriggerHaptic(float amplitude, float duration, float frequency)
    {
        if (leftHaptics != null)
        {
            leftHaptics.SendHapticImpulse(amplitude, duration, frequency);
        }

        if (rightHaptics != null)
        {
            rightHaptics.SendHapticImpulse(amplitude, duration, frequency);
        }
    }

    void PlaySound(AudioClip sfx, float volume)
    {
        if(sfx != null)
        {
            audioSource.PlayOneShot(sfx,volume);
        }
    }
}
