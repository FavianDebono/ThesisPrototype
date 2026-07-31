using UnityEngine;

public class MoleProperties : MonoBehaviour
{
    WackAMoleGameManager gameManager;
    int points;
    bool hit;

    [Header("Mole Destroy VFX")]
    [SerializeField] GameObject moleDestroyVfxPrefab;
    [SerializeField] float vfxLifeTime = 2f;

    public void SetUp(WackAMoleGameManager manager, int scoreValue)
    {
        gameManager = manager;
        points = scoreValue;
    }

    public void Hit()
    {
        if (hit) return;
        hit = true;

        if (moleDestroyVfxPrefab != null)
        {
            GameObject vfx = Instantiate(moleDestroyVfxPrefab, this.transform.position, Quaternion.identity);
            vfx.transform.localScale = Vector3.one * 0.1f;
            Destroy(vfx, vfxLifeTime);
        }

        gameManager.AddScore(points);
        Destroy(gameObject);
    }


}
