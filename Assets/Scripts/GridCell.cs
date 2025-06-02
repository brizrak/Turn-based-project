using UnityEngine;

public class GridCell : MonoBehaviour
{
    [HideInInspector] public int x;
    [HideInInspector] public int y;
    public Unit occupiedUnit;
    
    public event Actions.TargetCell OnChoseMoveTarget;
    
    public void Initialize(int gridX, int gridY)
    {
        x = gridX;
        y = gridY;
    }
    
    public void OnMouseDown()
    {
        OnChoseMoveTarget?.Invoke(x, y);
    }
}