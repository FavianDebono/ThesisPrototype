using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class ReactionButton : MonoBehaviour
{
    public bool isCorrectButton = false;
    public GameManager gameManager;
    public Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();
        GetComponent<XRBaseInteractable>().selectEntered.AddListener(OnPressed);
    }

    void OnPressed(SelectEnterEventArgs args)
    {
        if (!gameManager.gameRunning)
            return; // Ignore presses if game is not running

        if (isCorrectButton)
            gameManager.CorrectPress();
        else
            gameManager.WrongPress();
    }


    public void SetActive(bool active)
    {
        isCorrectButton = active;
        rend.material.color = active ? Color.green : Color.gray;
    }
}
