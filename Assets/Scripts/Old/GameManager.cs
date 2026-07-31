using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip correctSound;
    public AudioClip wrongSound;

    public bool gameRunning = false;
    public ReactionButton[] buttons;
    public int score = 0;
    public float timeLeft = 30f;
    float pulseSpeed = 2f;
    public JoystickStartGame joystickStarter;



    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;


    void Start()
    {
        enabled = false;
        gameRunning = false;

        scoreText.text = "Score: 0";
        timerText.text = "Pull Joystick\nDOWN\nto Start";
        timerText.color = Color.yellow;

    }



    void Update()
    {
        if (!gameRunning)
        {
            // Fade in/out effect
            Color c = timerText.color;
            c.a = Mathf.Abs(Mathf.Sin(Time.time * pulseSpeed));
            timerText.color = c;
            return;
        }

        timeLeft -= Time.deltaTime;
        timerText.text = "Time: " + Mathf.Ceil(timeLeft);

        if (timeLeft <= 0)
        {
            timerText.text = "Game Over";
            timerText.color = Color.red;
            gameRunning = false;
            foreach (var b in buttons)
                b.SetActive(false);
            this.enabled = false;
            Invoke(nameof(ResetGame), 2f);

        }
    }


    void ActivateRandomButton()
    {
        foreach (var b in buttons)
            b.SetActive(false);

        int index = Random.Range(0, buttons.Length);
        buttons[index].SetActive(true);
    }

    public void CorrectPress()
    {
        if (audioSource && correctSound)
            audioSource.PlayOneShot(correctSound);

        score++;
        UpdateUI();
        ActivateRandomButton();
    }

    public void WrongPress()
    {
        if (audioSource && wrongSound)
            audioSource.PlayOneShot(wrongSound);

        score--;
        UpdateUI();
    }

    void UpdateUI()
    {
        scoreText.text = "Score: " + score;
    }

    public void StartGame()
    {
        score = 0;
        timeLeft = 30f;

        gameRunning = true;
        enabled = true;

        timerText.color = Color.white;

        ActivateRandomButton();
        UpdateUI();
    }

    public void ResetGame()
    {
        // Reset values
        score = 0;
        timeLeft = 30f;

        gameRunning = false;
        this.enabled = false;

        // Reset UI
        scoreText.text = "Score: 0";
        timerText.text = "Pull Joystick\nDOWN\nto Start";
        timerText.color = Color.yellow;

        // Reset buttons visually
        foreach (var b in buttons)
            b.SetActive(false);

        joystickStarter.ResetJoystick();
        joystickStarter.enabled = true;
    }


}
