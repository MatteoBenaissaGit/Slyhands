using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Art.VFX.Scripts.Effects
{
    public class VFXEffectMove : VFXEffect
    {
        public float Duration;
        public Ease MoveEase = Ease.Linear;
        public Vector3 MoveToOffset;
        public GameObject GameObjectToMove;
        public override Color EffectColorInEditor { get; } = new Color(0.65f, 0.99f, 1f);

        public override string ToString()
        {
            return $"Move {(GameObjectToMove == null ? "___" : $"{GameObjectToMove.name}")} to {MoveToOffset} in {Duration}s";
        }
        
        public override IEnumerator Execute(VFXEvent gameEvent, GameObject gameObject)
        {
            yield return new WaitForSeconds(0f);

            if (GameObjectToMove == null)
            {
                yield break;
            }
            
            GameObjectToMove.transform.DOComplete();
            Vector3 startPosition = GameObjectToMove.transform.position;
            Vector3 endPosition = startPosition + MoveToOffset;
            GameObjectToMove.transform.DOMove(endPosition, Duration).SetEase(MoveEase);
        }
    }
}