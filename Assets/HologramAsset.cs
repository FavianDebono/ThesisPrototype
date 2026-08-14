using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class HologramAsset : MonoBehaviour
{
    CollectableMalteseItem originalItem;
    XRGrabInteractable grabInteractable;
    Rigidbody rb;


    bool beingDestroyed = false;
    bool collected = false;


    [Header("Audio")]
    [SerializeField] AudioClip droppedSound;

    [Header("Disintegration")]
    [SerializeField] float disintegrateDuration = .4f;

    Renderer[] hologramRenderers;

    void Start()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        rb = GetComponent<Rigidbody>();

        // Find all meshes belonging to the hologram.
        hologramRenderers =
            GetComponentsInChildren<Renderer>(true);

        if (grabInteractable != null)
        {
            grabInteractable.selectExited.AddListener(OnReleased);
        }
    }

    void OnDestroy()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectExited.RemoveListener(OnReleased);
        }
    }

    public void SetOriginalItem(CollectableMalteseItem item)
    {
        originalItem = item;
    }

    public void SetDroppedSound(AudioClip sound)
    {
        droppedSound = sound;
    }

    void OnReleased(SelectExitEventArgs args)
    {
        if (collected)
            return;

        if (beingDestroyed)
            return;

        // Play sound when hologram is dropped.
        if (droppedSound != null)
        {
            AudioSource.PlayClipAtPoint(
                droppedSound,
                transform.position
            );
        }

        StartCoroutine(Disintegrate());
    }

    IEnumerator Disintegrate()
    {
        beingDestroyed = true;

        // ------------------------------------------------
        // Stop hologram from being grabbed again
        // ------------------------------------------------

        if (grabInteractable != null)
        {
            grabInteractable.enabled = false;
        }

        // ------------------------------------------------
        // Stop movement
        // ------------------------------------------------

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = false;

            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // ------------------------------------------------
        // Keep colliders as triggers
        // ------------------------------------------------

        foreach (Collider col in
                 GetComponentsInChildren<Collider>(true))
        {
            col.isTrigger = true;
        }

        // ------------------------------------------------
        // Fade hologram
        // ------------------------------------------------

        float elapsed = 0f;

        while (elapsed < disintegrateDuration)
        {
            elapsed += Time.deltaTime;

            float fadeAmount =
                Mathf.Clamp01(
                    elapsed / disintegrateDuration
                );

            float alpha =
                1f - fadeAmount;

            SetHologramAlpha(alpha);

            yield return null;
        }

        // ------------------------------------------------
        // Allow original to create another hologram
        // ------------------------------------------------

        if (originalItem != null)
        {
            originalItem.HologramDestroyed();
        }

        Destroy(gameObject);
    }

    void SetHologramAlpha(float alpha)
    {
        foreach (Renderer renderer in hologramRenderers)
        {
            Material[] materials =
                renderer.materials;

            foreach (Material material in materials)
            {
                // URP shaders
                if (material.HasProperty("_BaseColor"))
                {
                    Color color =
                        material.GetColor("_BaseColor");

                    color.a = alpha;

                    material.SetColor(
                        "_BaseColor",
                        color
                    );
                }

                // Built-in shaders
                else if (material.HasProperty("_Color"))
                {
                    Color color =
                        material.GetColor("_Color");

                    color.a = alpha;

                    material.SetColor(
                        "_Color",
                        color
                    );
                }
            }
        }
    }

    public void Collect()
    {
        if (beingDestroyed)
            return;

        collected = true;

        if (originalItem != null)
        {
            originalItem.MarkAsCollected();
        }

        Destroy(gameObject);
    }
}