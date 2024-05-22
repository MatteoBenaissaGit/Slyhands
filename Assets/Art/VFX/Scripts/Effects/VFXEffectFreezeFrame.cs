using System.Collections;
using UnityEngine;

namespace Art.VFX.Scripts.Effects
{
    public class VFXEffectFreezeFrame : VFXEffect
    {
        public float Duration;
        public override Color EffectColorInEditor { get; } = new Color(0.04f, 0.76f, 1f);

        public override string ToString()
        {
            return $"Freeze frame for {Duration}s";
        }
        
        public override IEnumerator Execute(VFXEvent gameEvent, GameObject gameObject)
        {
            Time.timeScale = 0;
            yield return VFXEvent.StartCoroutine(ResetTimeScale());
        }

        private IEnumerator ResetTimeScale()
        {
            yield return new WaitForSecondsRealtime(Duration);
            Time.timeScale = 1;
        }
    }
}