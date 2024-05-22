using System.Collections;
using UnityEngine;

namespace Art.VFX.Scripts.Effects
{
    public class VFXEffectSetObjectActive : VFXEffect
    {
        public GameObject ObjectToSet;
        public bool SetActive;
        public override Color EffectColorInEditor { get; } = new Color(1f, 0.96f, 0.99f);

        public override string ToString()
        {
            return $"Set {(ObjectToSet == null ? "___" : $"{ObjectToSet.name}")} {(SetActive ? "active" : "inactive")}s";
        }
        
        public override IEnumerator Execute(VFXEvent gameEvent, GameObject gameObject)
        {
            ObjectToSet?.SetActive(SetActive);
            yield return new WaitForSecondsRealtime(0f);
        }
    }
}