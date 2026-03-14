using MI.Utility;
using System.Collections;
using UnityEngine;

namespace MI.Presentation.World.VFX
{
    public class MIFxAutoFade : MonoBehaviour
    {
        [SerializeField] private float _initialDelay = 1.5f;
        [SerializeField] private float _fadeDuration = 0.5f;
        [SerializeField] private AnimationCurve _fadeCurve = AnimationCurve.Linear(0, 1, 1, 0);
        [SerializeField] private ParticleSystemRenderer _renderer;

        private void Start()
        {
            StartCoroutine(FadeOutAndDestroy());
        }

        private IEnumerator FadeOutAndDestroy() 
        {
            yield return new WaitForSeconds(_initialDelay);
            
            float elapsed = 0f;
            while (elapsed < _fadeDuration)
            {
                elapsed += Time.deltaTime;
                float alpha = _fadeCurve.Evaluate(elapsed / _fadeDuration);
                Color c = _renderer.material.color;
                c.a = alpha;
                _renderer.material.color = c;
                yield return null;
            }



            Destroy(gameObject);
        }

    }
}
