using System.Collections;
using UnityEngine;

public class ExplosionEffect : MonoBehaviour
{
    public GameObject explosionPrefab;
    public float explosionDuration = 1.0f;
    public int numParticles = 100;

    public void TriggerExplosion(Vector3 position)
    {
        StartCoroutine(ExplosionCoroutine(position));
    }

    private IEnumerator ExplosionCoroutine(Vector3 position)
    {
        GameObject explosion = Instantiate(explosionPrefab, position, Quaternion.identity);
        ParticleSystem particleSystem = explosion.GetComponent<ParticleSystem>();
        var main = particleSystem.main;
        main.duration = explosionDuration;

        for (int i = 0; i < numParticles; i++)
        {
            ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams();
            emitParams.position = position;
            emitParams.startSize = Random.Range(0.1f, 0.5f);
            emitParams.startLifetime = explosionDuration;
            emitParams.velocity = Random.insideUnitSphere * 5;
            particleSystem.Emit(emitParams, 1);
        }

        particleSystem.Play();
        yield return new WaitForSeconds(explosionDuration);
        Destroy(explosion);
    }
}
