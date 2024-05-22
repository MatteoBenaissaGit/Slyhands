using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

namespace Art.VFX.Scripts.Effects
{
    public class VFXEffectPlayVisualEffect : VFXEffect
    {
        public VisualEffect VisualEffectToPlay;
        public override Color EffectColorInEditor { get; } = new Color(1f, 0.97f, 0f);

        public override string ToString()
        {
            return $"Play {(VisualEffectToPlay == null ? "___" : $"{VisualEffectToPlay.name}")}";
        }
        
        public override IEnumerator Execute(VFXEvent gameEvent, GameObject gameObject)
        {
            VisualEffectToPlay?.Play();
            yield return new WaitForSecondsRealtime(0f);
        }
    }
}