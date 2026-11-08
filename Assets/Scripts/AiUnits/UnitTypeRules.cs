public static class UnitTypeRules
{
    public static bool Beats(this UnitType attacker, UnitType defender)
    {
        return (attacker == UnitType.Rock && defender == UnitType.Scissors)
            || (attacker == UnitType.Scissors && defender == UnitType.Paper)
            || (attacker == UnitType.Paper && defender == UnitType.Rock);
    }
}
