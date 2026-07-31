using UnityEngine;

public class HammerHit : MonoBehaviour
{
    [SerializeField] float strongHitSpeed = 2.0f;

    Vector3 lastPos;
    float speed;

    void Update()
    {
        speed = (transform.position - lastPos).magnitude / Mathf.Max(Time.deltaTime, 0.0001f);
        lastPos = transform.position;
    }

    void OnTriggerEnter(Collider other)
    {
        if (speed < strongHitSpeed) return;

        MoleProperties mole = other.GetComponentInParent<MoleProperties>();
        if (mole == null) return;

        mole.Hit();
    }
}
