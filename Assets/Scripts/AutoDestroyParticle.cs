// AutoDestroyParticle.cs
using UnityEngine;

public class AutoDestroyParticle : MonoBehaviour
{
    private ParticleSystem ps;

    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        if (ps == null || !ps.IsAlive(true))
        {
            Destroy(gameObject);
        }
    }
}