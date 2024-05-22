using System;
using System.Collections;
using UnityEngine;

namespace Art.VFX.Scripts.Effects
{
    [Serializable]
    public class VFXEffect
    {
        public bool Enabled = true;
        public virtual Color EffectColorInEditor { get; }
        public VFXEvent VFXEvent { get; set; }

        public override string ToString()
        {
            return string.Empty;
        }

        public virtual IEnumerator Execute(VFXEvent gameEvent, GameObject gameObject)
        {
            yield break;
        }
    }
}