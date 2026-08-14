using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using System.Collections.Generic;


public class CollectableMalteseItem : MonoBehaviour
{
    /*
    public Vector3 initialPosition;
    public Quaternion initialRotation;
    public Vector3 initialScale;
    public bool replicaCreated;

    [SerializeField] GameObject hologramCanvas;
    [SerializeField] TMP_Text itemNameText;
    Color32 textColor = new Color32(84, 235, 245, 255);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        initialScale = transform.localScale;

        if (hologramCanvas != null)
            hologramCanvas.SetActive(false);
    }

    public void EnableHologramDisplay()
    {
        if (hologramCanvas != null)
            hologramCanvas.SetActive(true);

        if (itemNameText != null)
            itemNameText.color = textColor;
    }
    */

    [SerializeField] string ObjectName;
    [SerializeField] AudioClip audioName;
    [SerializeField] AudioClip hologramDroppedSound;
    [SerializeField] Material blueMaterial;
    [SerializeField] float hologramScale = 0.25f;
    [SerializeField] GameObject itemCanvas;

    XRGrabInteractable grabInteractable;

    bool collected = false;
    bool hologramCreated = false;

    Dictionary<Material, Color> originalColors = new Dictionary<Material, Color>();


    void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
    }

    void OnEnable()
    {
        if (grabInteractable != null) grabInteractable.selectEntered.AddListener(OnGrabbed);
    }

    void OnDisable()
    {
        if (grabInteractable != null) grabInteractable.selectEntered.RemoveListener(OnGrabbed);
    }

    void OnGrabbed(SelectEnterEventArgs args)
    {
        if (collected)
            return;

        if (hologramCreated)
            return;

        hologramCreated = true;

        CreateHologram(args.interactorObject);
    }

    void CreateHologram(IXRSelectInteractor interactor)
    {
        GameObject hologram = Instantiate( gameObject, transform.position, transform.rotation);

        hologram.name = gameObject.name + "_Hologram";
        hologram.transform.localScale = transform.localScale * hologramScale;

        CollectableMalteseItem copiedCollectable = hologram.GetComponent<CollectableMalteseItem>();

        if (copiedCollectable != null)
        {
            copiedCollectable.DisableItemCanvas();
            copiedCollectable.enabled = false;
            Destroy(copiedCollectable);
        }

        ApplyBlueMaterial(hologram);
        ConfigureHologram(hologram);

        HologramAsset hologramItem = hologram.AddComponent<HologramAsset>();

        hologramItem.SetOriginalItem(this);
        hologramItem.SetDroppedSound(hologramDroppedSound);

        XRGrabInteractable hologramGrab = hologram.GetComponent<XRGrabInteractable>();

        if (grabInteractable != null && grabInteractable.interactionManager != null)
        {
            var manager = grabInteractable.interactionManager;

            // Release original.
            manager.SelectExit(
                interactor,
                grabInteractable
            );

            // Grab hologram with same hand.
            if (hologramGrab != null)
            {
                manager.SelectEnter(
                    interactor,
                    hologramGrab
                );
            }
        }

        // Original cannot be grabbed while hologram exists.
        if (grabInteractable != null)
            grabInteractable.enabled = false;
    }

    private void ConfigureHologram(GameObject hologram)
    {
        /*
         * Trigger colliders:
         * - don't physically collide
         * - still allow CollectorBag detection
         */
        foreach (Collider col in
                 hologram.GetComponentsInChildren<Collider>(true))
        {
            col.isTrigger = true;
        }

        Rigidbody rb = hologram.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.useGravity = false;
        }
    }

    private void ApplyBlueMaterial(GameObject hologram)
    {
        if (blueMaterial == null)
            return;

        foreach (Renderer objectRenderer in
                 hologram.GetComponentsInChildren<Renderer>(true))
        {
            Material[] materials =
                objectRenderer.materials;

            for (int i = 0; i < materials.Length; i++)
            {
                materials[i] = blueMaterial;
            }

            objectRenderer.materials = materials;
        }
    }

    public void DisableItemCanvas()
    {
        if (itemCanvas != null)
            itemCanvas.SetActive(false);
    }

    public void MakeObjectLightYellow(GameObject selectedObject)
    {
        Color lightYellow = new Color(1f, 1f, 0.5f, 1f);

        Renderer[] renderers = selectedObject.GetComponentsInChildren<Renderer>(true);

        foreach (Renderer rend in renderers)
        {
            foreach (Material material in rend.materials)
            {
                // Store original colour only once
                if (!originalColors.ContainsKey(material))
                {
                    originalColors.Add(material, material.color);
                }

                material.color = lightYellow;
            }
        }
    }

    public void RemoveLightYellow(GameObject selectedObject)
    {
        Renderer[] renderers = selectedObject.GetComponentsInChildren<Renderer>(true);

        foreach (Renderer rend in renderers)
        {
            foreach (Material material in rend.materials)
            {
                if (originalColors.TryGetValue(material, out Color originalColor))
                {
                    material.color = originalColor;
                    originalColors.Remove(material);
                }
            }
        }
    }

    public void HologramDestroyed()
    {
        if (collected)
            return;

        hologramCreated = false;

        if (grabInteractable != null)
            grabInteractable.enabled = true;
    }

    public void MarkAsCollected()
    {
        if (collected)
            return;

        collected = true;
        hologramCreated = false;

        if (grabInteractable != null)
            grabInteractable.enabled = false;

        if (IntrinsicGameManager.Instance != null)
        {
            IntrinsicGameManager.Instance.RegisterCollectedItem(this);
        }
    }
}
