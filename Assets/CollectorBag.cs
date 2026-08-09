using UnityEngine;

public class CollectorBag : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        HologramAsset hologram =
            other.GetComponentInParent<HologramAsset>();

        if (hologram == null)
            return;

        hologram.Collect();
    }
}
