using DG.Tweening;
using System.Collections;
using UnityEngine;

namespace Art.VFX.Scripts.Effects
{
    public class VFXEffectLightIntensity : VFXEffect
    {
        public float Duration;
        public float Intensity;

        public Light Light;
        public LightFlicker Flicker;


        public override Color EffectColorInEditor { get; } = new Color(1f, 0.4f, 0.4f);

        public override string ToString()
        {
            return $"Light {(Light == null ? "___" : $"{Light.name}")} intensity from {Light.intensity} to {Intensity} in {Duration}s";
        }

        public override IEnumerator Execute(VFXEvent gameEvent, GameObject gameObject)
        {
            yield return new WaitForSeconds(0f);

            if (Light == null)
            {
                yield break;
            }

            Light.DOComplete();

            float currentIntensity = Flicker.absoluteIntensity;

            DOTween.To(() => currentIntensity, x => currentIntensity = x, Intensity, Duration);
            Flicker.absoluteIntensity = Intensity;
        }
    }

}
