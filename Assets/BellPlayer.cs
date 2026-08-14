using System.Collections;
using UnityEngine;

public class BellPlayer : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] float interval = 240f; // 4 minutes

    void Start()
    {
        StartCoroutine(PlayRandomly());
    }

    IEnumerator PlayRandomly()
    {
        while (true)
        {
            // Choose a random time during the next 4 minutes
            float randomDelay = Random.Range(0f, interval);

            yield return new WaitForSeconds(randomDelay);

            if (audioSource != null)
            {
                audioSource.Play();
            }

            // Wait for the remainder of the 4-minute period
            yield return new WaitForSeconds(interval - randomDelay);
        }
    }
}

