using UnityEngine;

namespace Board.Characters.AttackSystem
{
    public interface IAttackable
    {
        public Vector3Int GetCoordinates { get; }
        public void GetAttacked(IAttacker attacker);
    }
}