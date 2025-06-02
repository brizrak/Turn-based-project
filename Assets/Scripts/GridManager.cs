using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using Random = UnityEngine.Random;

public class GridManager : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private float cellSize;
    [SerializeField] private float topSize;
    [SerializeField] private GameObject cellPrefab1;
    [SerializeField] private GameObject cellPrefab2;
    [SerializeField] private GameObject cellPrefabOuter;
    [SerializeField] private float unitHeight;
    [Header("Camera Settings")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float cameraOffsetX;
    [SerializeField] private float cameraOffsetZ;
    
    private GridCell[,] _grid;
    private GameObject _prefab;
    private float _verticalDeviation;
    
    public GridCell[,] Grid => _grid;

    private void Awake()
    {
        GenerateGrid();
        SetCamera();
        GenerateOuterGrid();
    }

    private void GenerateGrid()
    {
        _grid = new GridCell[width, height];
        var cells = new GameObject("Cells");
        cells.transform.SetParent(transform);
    
        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                _verticalDeviation = Random.Range(0f, cellSize / 3);
                var position = new Vector3(x * cellSize, _verticalDeviation / 2, y * cellSize);
                _prefab = (x + y) % 2 == 0 ? cellPrefab1 : cellPrefab2;
                var cell = Instantiate(_prefab, position, Quaternion.identity, cells.transform);
                cell.name = $"Cell_{x}_{y}";
                cell.transform.localScale = new Vector3(cellSize, cellSize + _verticalDeviation, cellSize);
                
                var cellScript = cell.GetComponent<GridCell>();
                cellScript.Initialize(x, y);
            
                _grid[x, y] = cellScript;
            }
        }
    }

    private void GenerateOuterGrid()
    {
        var cells = new GameObject("OuterCells");
        cells.transform.SetParent(transform);
        for (int x = -9; x < width + 9; x++)
        {
            for (int y = -9; y < height + 9; y++)
            {
                if (x == -1 || y == -1 || x == width ||  y == height) _verticalDeviation = Random.Range(-cellSize / 6, cellSize / 6);
                if (x == -2 || y == -2 || x == width + 1 ||  y == height + 1) _verticalDeviation = Random.Range(-cellSize / 3, -cellSize / 6);
                if (x == -3 || y == -3 || x == width + 2 ||  y == height + 2) _verticalDeviation = Random.Range(-cellSize / 2, -cellSize / 3);
                if (x < -3 || y < -3 || x > width + 2 || y > height + 2)
                {
                    _verticalDeviation = Random.Range(0f, 1f) > 0.8 ? -cellSize / 2 : 1;
                }
                if (Mathf.Approximately(_verticalDeviation, 1f)) continue;
                var position = new Vector3(x * cellSize, _verticalDeviation / 2, y * cellSize);
                var cell = Instantiate(cellPrefabOuter, position, Quaternion.identity, cells.transform);
                cell.name = $"Cell_outer_{x}_{y}";
                cell.transform.localScale = new Vector3(cellSize, cellSize + _verticalDeviation, cellSize);
                _verticalDeviation = 1;
            }
        }
    }

    private void SetCamera()
    {
        mainCamera.transform.position = new Vector3(mainCamera.transform.position.x + width * cellSize * cameraOffsetX, 
                    mainCamera.transform.position.y, mainCamera.transform.position.z - height * cellSize * cameraOffsetZ);
    }

    public void Place(List<Unit> playerTeam, List<Unit> enemyTeam)
    {
        PlaceTeam(playerTeam, true);
        PlaceTeam(enemyTeam, false);
    }

    private void PlaceTeam(List<Unit> team, bool direction)
    {
        int units = 0; 
        int columns = direction ? 0 : width - 1;
        var count = team.Count;
        
        while (units < count)
        {
            if (count - units >= height)
            {
                for (int y = 0; y < height; y++)
                {
                    PlaceUnit(team[units], columns, y);
                    units++;
                }
            }
            else
            {
                var remainingUnits = count - units;
                if (remainingUnits == 1)
                {
                    PlaceUnit(team[units], columns, Mathf.RoundToInt((float)height / 2));
                    break;
                }
                var spacing = (height - 1) / (float)(remainingUnits - 1);
                for (int i = 0; i < remainingUnits; i++)
                {
                    var row = Mathf.RoundToInt(i * spacing);
                    if (PlaceUnit(team[units], columns, row)) units++;
                }
                break;
            }
            columns += direction ? 1 : -1;
        }
    }

    public bool PlaceUnit(Unit unit, int x, int y, bool place = true)
    {
        var cell = GetCell(x, y);
        if (cell is null || cell.occupiedUnit is not null) 
            return false;
    
        cell.occupiedUnit = unit;
        if (place) unit.transform.position = GetPositionForUnit(x, y);
        return true;
    }

    public bool CheckForRange(Unit unit, int x, int y, int maxRange, int minRange = 0)
    {
        var cell = GetCell(unit.transform.position);
        int range = Mathf.Abs(x - cell.x) + Mathf.Abs(y - cell.y);
        return range <= maxRange && range >= minRange;
    }

    public bool CheckForRange(Unit unit, Unit target, int maxRange, int minRange = 0)
    {
        var targetCell = GetCell(target.transform.position);
        return CheckForRange(unit, targetCell.x, targetCell.y, maxRange, minRange);
    }

    [CanBeNull]
    public Unit LookForAttack(Unit unit, List<Unit> team)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var occupiedUnit = GetCell(x, y).occupiedUnit;
                if (occupiedUnit is null || team.Contains(occupiedUnit)) continue;
                if (CheckForRange(unit, x, y, unit.weapon.maxRange, unit.weapon.minRange))
                {
                    return GetCell(x, y).occupiedUnit;
                }
            }
        }
        return null;
    }

    [CanBeNull]
    public GridCell LookForMove(Unit unit)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (CheckForRange(unit, x, y, unit.range) && GetCell(x, y).occupiedUnit is null)
                {
                    return GetCell(x, y);
                }
            }
        }
        return null;
    }

    public void HighlightGrid(Unit unit, bool isActive)
    {
        var cell = GetCell(unit.transform.position);
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (Mathf.Abs(x - cell.x) + Mathf.Abs(y - cell.y) <= unit.range)
                {
                    GetCell(x, y).GetComponentInChildren<GridCellTop>().SetHighlight(isActive, true);
                }
            }
        }
    }
    
    public GridCell GetCell(int x, int y) => 
        (x >= 0 && x < width && y >= 0 && y < height) ? _grid[x, y] : null;

    public GridCell GetCell(Vector3 worldPosition)
    {
        var x = Mathf.FloorToInt(worldPosition.x / cellSize);
        var y = Mathf.FloorToInt(worldPosition.z / cellSize);
        return GetCell(x, y);
    }

    public Vector3 GetWorldPosition(int x, int y) => 
        new Vector3(x * cellSize, 0, y * cellSize);

    public Vector3 GetPositionForUnit(int x, int y) => GetWorldPosition(x, y) +
           Vector3.up * (float)(GetCell(x, y).transform.localScale.y + topSize + unitHeight / 2 - 0.5);
    
    public Vector3 GetPositionForUnit(Vector3 position) => position + Vector3.up *
        (float)(GetCell(GetCell(position).x, GetCell(position).y).transform.localScale.y + topSize + unitHeight / 2 - 0.5);
}