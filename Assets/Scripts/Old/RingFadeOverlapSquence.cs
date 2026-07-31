using System.Collections;
using UnityEngine;

public class RingFadeOverlapSquence : MonoBehaviour
{
    [SerializeField] Animator[] rings;
    [SerializeField] string fadeStateName = "Anim_teleport";
    [SerializeField] float delay = .75f;


    void OnEnable()
    {
        StartCoroutine(Run());
    }

    void OnDisable()
    {
        StopCoroutine(Run());
    }

    IEnumerator Run()
    {
        if (rings == null || rings.Length == 0) yield break;

        do
        {
            for (int i = 0; i < rings.Length; i++)
            {
                if (rings[i] == null) continue;

                rings[i].Play(fadeStateName, 0, 0f);

                yield return new WaitForSeconds(delay);
            }

        } while (true);
    }
}
