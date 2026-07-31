using UnityEngine;
using System.Collections;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics;
using TMPro;

public class BasketballScoreZone : MonoBehaviour
{
    [Header("Scoring")]
    [SerializeField] string basketballTag = "Basketball";
    [SerializeField] int pointsPerBucket = 100;

    [Header("Round")]
    [SerializeField] float roundSeconds = 60f;
    [SerializeField] bool autoRestart = true;

    [Header("UI")]
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] float scoreDisplayTime = 0.7f;

    [Header("Basketball Spawning")]
    [SerializeField] GameObject basketballPrefab;
    [SerializeField] Transform[] basketballSpawnPoints;

    [Header("Haptic Feedback Components")]
    [SerializeField] GameObject leftHand;
    [SerializeField] GameObject rightHand;
    HapticImpulsePlayer leftHaptics;
    HapticImpulsePlayer rightHaptics;

    [Header("Ball Destroy VFX")]
    [SerializeField] GameObject ballDestroyVfxPrefab;
    [SerializeField] float vfxLifeTime = 2f;

    [Header("Sound Effects")]
    [SerializeField] AudioClip sfx_basketIn;
    [SerializeField] AudioClip sfx_timesUp;
    AudioSource audioSource;

    GameObject[] spawnedBalls;

    int score;
    float timeLeft;
    bool running;
    bool hasScoredThisPass;
    
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        leftHaptics = leftHand.GetComponent<HapticImpulsePlayer>();
        rightHaptics = rightHand.GetComponent<HapticImpulsePlayer>();
    }

    // Used for the teleportation Anchor to start the game
    public void StartGame()
    {
        StartRound();
    }

    void Update()
    {
        if (!running) return;
        timeLeft -= Time.deltaTime;

        if (timeLeft <= 0f)
        {
            EndRound();

            if (autoRestart)
            {
                StartRound();
            }
            return;
        }
        UpdateTimerUI();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!running) return;
        if (!other.CompareTag(basketballTag)) return;
        if (hasScoredThisPass) return;

        PlaySound(sfx_basketIn, 0.8f);

        TriggerHaptic(.5f, .15f, 0f);

        hasScoredThisPass = true;
        score += pointsPerBucket;

        UpdateScoreUI();
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(basketballTag)) return;
        hasScoredThisPass = false;
    }

    void StartRound()
    {
        score = 0;
        timeLeft = roundSeconds;
        running = true;
        hasScoredThisPass = false;

        SpawnBasketballs();

        UpdateScoreUI();
        UpdateTimerUI();
    }

    void EndRound()
    {
        PlaySound(sfx_timesUp , 0.8f);


        TriggerHaptic(.7f, .4f, 0f);
        running = false;
        hasScoredThisPass = false;

        DestroyBasketballs();

        StartCoroutine(AppearAndDisappearScore());

        if (timerText != null)
        {
            timerText.text = "0";
        }

        Debug.Log($"Round ended. Final score: {score}");
    }

    void SpawnBasketballs()
    {
        if (basketballPrefab == null) return;
        if (basketballSpawnPoints == null || basketballSpawnPoints.Length == 0) return;

        DestroyBasketballs();

        spawnedBalls = new GameObject[basketballSpawnPoints.Length];

        for (int i = 0; i < basketballSpawnPoints.Length; i++)
        {
            Transform sp = basketballSpawnPoints[i];
            if (sp == null) continue;

            spawnedBalls[i] = Instantiate(basketballPrefab, sp.position, sp.rotation);
        }
    }

    void DestroyBasketballs()
    {
        if (spawnedBalls == null) return;

        Debug.Log("Spawning basketballs: " + basketballSpawnPoints.Length);

        for (int i = 0; i < spawnedBalls.Length; i++)
        {
            GameObject ball = spawnedBalls[i];
            if (ball == null) continue;

            if (ballDestroyVfxPrefab != null)
            {
                GameObject vfx = Instantiate(ballDestroyVfxPrefab, ball.transform.position, Quaternion.identity);

                Destroy(vfx, vfxLifeTime);
            }

            Destroy(ball);
            spawnedBalls[i] = null;
        }

        spawnedBalls = null;
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = score.ToString();
        }
    }

    void UpdateTimerUI()
    {
        if (timerText != null)
        {
            timerText.text = Mathf.CeilToInt(timeLeft).ToString();
        }
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
        if (sfx != null)
        {
            audioSource.PlayOneShot(sfx, volume);
        }
    }

}
