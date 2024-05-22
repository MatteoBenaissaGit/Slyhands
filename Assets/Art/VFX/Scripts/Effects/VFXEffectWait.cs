using System.Collections;
using UnityEngine;

namespace Art.VFX.Scripts.Effects
{
    public class VFXEffectWait : VFXEffect
    {
        public float Duration;
        public override Color EffectColorInEditor { get; } = new Color(0.3f, 1f, 0.39f);

        public override string ToString()
        {
            return $"Wait for {Duration}s";
        }
        
        public override IEnumerator Execute(VFXEvent gameEvent, GameObject gameObject)
        {
            yield return new WaitForSecondsRealtime(Duration);
        }
    }
}