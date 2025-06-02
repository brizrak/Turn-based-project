using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    [SerializeField] private int turns = 10;
    [SerializeField] private int maxSuper;
    [SerializeField] private float awaitingTime;
    [SerializeField] private Teams teams;
    [SerializeField] private GridManager gridManager;
    [SerializeField] private InterfaceBarManager barManager;
    [SerializeField] private InterfaceItemsManager interfaceManager;
    [SerializeField] private Super super;

    private List<Unit> _playerTeam;
    private List<Unit> _enemyTeam;
    private float _playerHealth;
    private float _enemyHealth;
    private int _playerSuper;
    private int _enemySuper;
    private int _currentTurn = 1;
    private bool _actionPerformed;
    private Actions.ActionType _actionType;

    private int _x;
    private int _y;
    private Unit _targetUnit;

    private void Start()
    {
        _playerTeam = teams.GetPlayerTeam();
        _enemyTeam = teams.GetEnemyTeam();
        _playerHealth = _playerTeam.Sum(u => u.maxHealth);
        _enemyHealth = _enemyTeam.Sum(u => u.maxHealth);
        barManager.CreateBar(_playerHealth, maxSuper, _enemyHealth, maxSuper);
        gridManager.Place(_playerTeam, _enemyTeam);
        CellsCleaner(true);
        SetSuperButton(true);
        StartCoroutine(GameProcess());
    }

    private void UpdateHealth()
    {
        _playerHealth = _playerTeam.Sum(u => u.Health());
        _enemyHealth = _enemyTeam.Sum(u => u.Health());
    }

    private IEnumerator GameProcess()
    {
        yield return new WaitForEndOfFrame();
        while (_currentTurn <= turns)
        {
            interfaceManager.SetTurn(_currentTurn);
            
            // CardSelection
            
            yield return StartCoroutine(PlayerAction());

            yield return StartCoroutine(EnemyAction());
            
            _currentTurn++;
        }
        interfaceManager.SetEnd(_playerHealth, _enemyHealth);
    }

    private void OnDamage(bool isPlayer, bool fromSuper = false)
    {
        UpdateHealth();
        barManager.UpdateHealth(_playerHealth, _enemyHealth);
        if (!fromSuper)
        {
            if (isPlayer) _playerSuper++;
            else _enemySuper++;
            barManager.UpdateSuper(_playerSuper, _enemySuper);
            if (CanUseSuper(isPlayer)) interfaceManager.SetSuperButton(true);
        }
        else
        {
            if (isPlayer) _playerSuper = 0;
            else _enemySuper = 0;
            interfaceManager.SetSuperButton(false);
        }
        
        if (_playerHealth > 0 && _enemyHealth > 0) return;
        interfaceManager.SetEnd(_playerHealth, _enemyHealth);
        Stop();
    }

    private IEnumerator PlayerAction()
    {
        _enemyTeam.ForEach(e => e.IsCanSelected(true));
        for (int i = 0; i < _playerTeam.Count; i++)
        {
            if (_playerTeam[i] is null) continue;
            var unit = _playerTeam[i];
            gridManager.HighlightGrid(unit, true);
            var flash = unit.GetComponent<FlashEffect>();
            flash.StartFlashing();
            do
            {
                yield return StartCoroutine(WaitForAction());
            } while (!CheckForAction(unit));
            if (_actionType == Actions.ActionType.Move)
            {
                gridManager.GetCell(unit.transform.position).occupiedUnit = null;
                StartCoroutine(unit.MoveTo(gridManager.GetPositionForUnit(_x, _y)));
            }
            else
            {
                unit.Attack(_targetUnit);
                OnDamage(true);
            }

            flash.StopFlashing();
            gridManager.HighlightGrid(unit, false);
        }
        _enemyTeam.ForEach(e => e.IsCanSelected(false));
    }

    private IEnumerator WaitForAction()
    {
        _actionPerformed = false;

        Actions.TargetCell moveHandler = (x, y) =>
        {
            _x = x;
            _y = y;
            _actionPerformed = true;
            _actionType = Actions.ActionType.Move;
        };
        
        Actions.TargetUnit attackHandler = (unit) =>
        {
            _targetUnit = unit;
            _actionPerformed = true;
            _actionType = Actions.ActionType.Attack;
        };
    
        foreach (var gridCell in gridManager.Grid)
        {
            gridCell.OnChoseMoveTarget += moveHandler;
        }

        foreach (var unit in _enemyTeam)
        {
            unit.OnChoseAttackTarget += attackHandler;
        }
        
        yield return new WaitUntil(() => _actionPerformed);
        
        foreach (var gridCell in gridManager.Grid)
        {
            gridCell.OnChoseMoveTarget -= moveHandler;
        }
        
        foreach (var gridCell in gridManager.Grid)
        {
            gridCell.OnChoseMoveTarget -= moveHandler;
        }
    }

    private bool CheckForAction(Unit unit)
    {
        return _actionType == Actions.ActionType.Attack ? gridManager.CheckForRange(unit, _targetUnit, unit.weapon.maxRange, unit.weapon.minRange) :
            gridManager.CheckForRange(unit, _x, _y, unit.range) && gridManager.PlaceUnit(unit, _x, _y, false);
    }
    
    private void CellsCleaner(bool clean)
    {
        Actions.DiedUnit handler = (unit) =>
        {
            gridManager.GetCell(unit.transform.position).occupiedUnit = null;
            _playerTeam.Remove(unit);
            _enemyTeam.Remove(unit);
        };
        
        foreach (var unit in _playerTeam)
        {
            if (clean) unit.OnUnitDie += handler;
            else unit.OnUnitDie -= handler;
        }        
        foreach (var unit in _enemyTeam)
        {
            if (clean) unit.OnUnitDie += handler;
            else unit.OnUnitDie -= handler;
        }
    }

    private bool CanUseSuper(bool isPlayer)
    {
        return (isPlayer && _playerSuper >= maxSuper) || (!isPlayer && _enemySuper >= maxSuper);
    }

    private void SetSuperButton(bool active)
    {
        Actions.SuperButton button = () => {
            CastSuper(_enemyTeam, true);
        };
        
        if (active) super.OnSuperButtonPressed += button;
        else super.OnSuperButtonPressed -= button;
    }

    private void CastSuper(List<Unit> units, bool isPlayer)
    {
        super.CastSuper(_enemyTeam);
        OnDamage(isPlayer, true);
    }

    private IEnumerator EnemyAction()
    {
        for (int i = 0; i < _enemyTeam.Count; i++)
        {
            if (_enemyTeam[i] is null) continue;
            var unit = _enemyTeam[i];
            yield return new WaitForSeconds(awaitingTime);
            if (CanUseSuper(false))
            {
                CastSuper(_enemyTeam, false);
            }
            var attackTarget = gridManager.LookForAttack(unit, _enemyTeam);
            if (attackTarget is not null)
            {
                unit.Attack(attackTarget);
                OnDamage(false);
                continue;
            }
            var moveTarget = gridManager.LookForMove(unit);
            if (moveTarget is null) continue;
            StartCoroutine(unit.MoveTo(gridManager.GetPositionForUnit(moveTarget.transform.position)));
        }
    }
    
    private void Stop()
    {
        CellsCleaner(false);
        SetSuperButton(false);
        StopAllCoroutines();
    }
}