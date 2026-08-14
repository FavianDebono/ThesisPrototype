using UnityEngine;

public class TutorialHologramCollector : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip collectSound;

    void OnTriggerEnter(Collider other)
    {
        HologramAsset hologram = other.GetComponent<HologramAsset>();

        if (hologram != null)
        {
            // Play sound
            if (audioSource != null && collectSound != null)
            {
                audioSource.PlayOneShot(collectSound);
            }

            // Destroy tutorial hologram
            Destroy(other.gameObject);
        }
    }
}
