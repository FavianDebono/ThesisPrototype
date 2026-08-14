using UnityEngine;

public class CollectorBag : MonoBehaviour
{

    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip collectedSound;

    void OnTriggerEnter(Collider other)
    {
        HologramAsset hologram =
            other.GetComponentInParent<HologramAsset>();

        if (hologram == null)
            return;

        // Play successful collection sound
        if (audioSource != null && collectedSound != null)
        {
            audioSource.PlayOneShot(collectedSound);
        }

        hologram.Collect();
        ExtrinsicGameManager.Instance.ItemCollected();
    }
}
