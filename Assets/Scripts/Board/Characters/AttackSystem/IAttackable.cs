namespace Board.Characters.AttackSystem
{
    public interface IAttackable
    {
        public void GetAttacked(IAttacker attacker);
    }
}