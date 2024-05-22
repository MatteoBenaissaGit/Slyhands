using System;
using System.Collections;
using System.Collections.Generic;
using Art.VFX.Scripts.Effects;
using UnityEngine;

namespace Art.VFX.Scripts
{
    [Serializable]
    public class VFXEvent : MonoBehaviour
    {
        [SerializeReference] public List<VFXEffect> VFXEffects = new List<VFXEffect>();

        private void Start()
        {
            LaunchEffect();
        }

        private void OnEnable()
        {
            VFXEffects.ForEach(x => x.VFXEvent = this);
        }

        private void LaunchEffect()
        {
            StartCoroutine(Execute(gameObject));
        }

        public IEnumerator Execute(GameObject gameObjectToExecute)
        {
            foreach (VFXEffect effect in VFXEffects)
            {
                yield return effect.Execute(this, gameObjectToExecute);
            }
        }
    }
}