using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleManager : MonoBehaviour
{
    private ParticleSystem _particleSystem;
    private void Awake()
    {
        _particleSystem = GetComponent<ParticleSystem>();
        if (_particleSystem == null)
        {
            Debug.LogError($"[ParticleManager] {name}: ParticleSystemÇ™å©Ç¬Ç©ÇËÇ‹ÇπÇÒÅB", this);
        }
    }

    private void OnEnable()
    {
        RoundManager.RoundEnded += TriggerParticle;
    }

    private void OnDisable()
    {
        RoundManager.RoundEnded -= TriggerParticle;
    }

    private void TriggerParticle(bool won)
    {
        if (won)
        {
            if (_particleSystem != null)
            {
                _particleSystem.Play();
            }
        }
    }
}
