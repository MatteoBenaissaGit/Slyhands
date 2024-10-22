using DG.Tweening;
using System.Collections;
using UnityEngine;

namespace Art.VFX.Scripts.Effects
{
    public class VFXEffectChangeScale : VFXEffect
    {
        public float Duration;
        public float Scale;
        
        public GameObject GameObjectToScale;


        public override Color EffectColorInEditor { get; } = new Color(0.28f, 0.74f, 1f);

        public override string ToString()
        {
            if (GameObjectToScale == null)
            {
                return "null";
            }
            return $"Game Object {GameObjectToScale.name} scale from {GameObjectToScale.transform.localScale.x} to {Scale} in {Duration}s";
            //return "zob";
        }

        public override IEnumerator Execute(VFXEvent gameEvent, GameObject gameObject)
        {
            yield return new WaitForSeconds(0f);

            if (GameObjectToScale == null)
            {
                yield break;
            }

            GameObjectToScale.transform.DOComplete();
            GameObjectToScale.transform.DOScale(Scale, Duration);
        }
    }

}
