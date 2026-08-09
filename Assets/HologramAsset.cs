using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class HologramAsset : MonoBehaviour
{
    private CollectableMalteseItem originalItem;
    private XRGrabInteractable grabInteractable;
    private Rigidbody rb;

    private bool beingDestroyed = false;
    private bool collected = false;

    [Header("Disintegration")]
    [SerializeField] private float disintegrateDuration = .4f;

    private Renderer[] hologramRenderers;

    private void Start()
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

    private void OnDestroy()
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

    private void OnReleased(SelectExitEventArgs args)
    {
        if (collected)
            return;

        if (beingDestroyed)
            return;

        StartCoroutine(Disintegrate());
    }

    private IEnumerator Disintegrate()
    {
        beingDestroyed = true;

        // ------------------------------------------------
        // Stop the hologram from being grabbed again
        // ------------------------------------------------

        if (grabInteractable != null)
        {
            grabInteractable.enabled = false;
        }

        // ------------------------------------------------
        // Make the hologram fall
        // ------------------------------------------------

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = false;

            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // ------------------------------------------------
        // Keep colliders as triggers.
        //
        // This means the hologram can fall through objects
        // without physically colliding with them.
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
                Mathf.Clamp01(elapsed / disintegrateDuration);

            float alpha = 1f - fadeAmount;

            SetHologramAlpha(alpha);

            yield return null;
        }

        // ------------------------------------------------
        // Original can now create another hologram
        // ------------------------------------------------

        if (originalItem != null)
        {
            originalItem.HologramDestroyed();
        }

        Destroy(gameObject);
    }

    private void SetHologramAlpha(float alpha)
    {
        foreach (Renderer renderer in hologramRenderers)
        {
            Material[] materials = renderer.materials;

            foreach (Material material in materials)
            {
                // URP shaders usually use _BaseColor.
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

                // Built-in shaders often use _Color.
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