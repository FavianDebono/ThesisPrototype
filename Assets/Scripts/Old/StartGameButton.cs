using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class StartGameButton : MonoBehaviour
{
    public GameManager gameManager;

    void Start()
    {
        GetComponent<XRBaseInteractable>()
            .selectEntered.AddListener(OnPressed);
    }

    void OnPressed(SelectEnterEventArgs args)
    {
        gameManager.StartGame();
    }
}
