public static class Actions
{
    public delegate void TargetCell(int x, int y);
    public delegate void TargetUnit(Unit unit);
    public delegate void DiedUnit(Unit unit);
    public delegate void SuperButton();
    
    public enum ActionType
    {
        Move,
        Attack,
    }
}