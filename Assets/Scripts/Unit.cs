using System.Collections;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public float maxHealth;
    public float damageModifier = 1;
    public int range;
    public float defence;
    public Weapon weapon;

    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private Color haloColor;
    [SerializeField] private HealthBar healthBar;

    private bool _isCanSelected;
    private bool _isSelected;
    public void IsCanSelected(bool can) => _isCanSelected = can;
    
    public event Actions.TargetUnit OnChoseAttackTarget;
    public event Actions.DiedUnit OnUnitDie;

    private Rigidbody _rb;
    private Renderer _renderer;
    private Color _originalColor;
    
    private float _health;
    public float Health() => _health;
    private void ChangeHealth(float health)
    {
        _health += health;
        if (_health > maxHealth) _health = maxHealth;
        if (_health < 0)
        {
            OnUnitDie?.Invoke(this);
            _health = 0;
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        _rb =  GetComponent<Rigidbody>();
        _isCanSelected = false;
        _health = maxHealth;
    }

    private void Start()
    {
        _renderer = GetComponent<Renderer>();
        _originalColor = _renderer.material.color;
    }

    public IEnumerator MoveTo(Vector3 destination)
    {
        _rb.linearVelocity = Vector3.up * jumpForce;
        while (transform.position != destination)
        {
            transform.position = Vector3.MoveTowards(transform.position, destination, Time.deltaTime * moveSpeed);
            yield return null;
        }
    }
    
    public void DrawHalo(bool selected)
    {
        var oldHalo = transform.Find("SelectionHalo");
        if(oldHalo != null) Destroy(oldHalo.gameObject);

        if (!selected) return;
        
        var halo = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        halo.name = "SelectionHalo";
        halo.transform.SetParent(transform);
        halo.transform.localPosition = new Vector3(0, 0.1f, 0);
        halo.transform.localScale = new Vector3(1.5f, 0.05f, 1.5f);
        
        var rend = halo.GetComponent<Renderer>();
        rend.material = new Material(Shader.Find("Standard"));
        rend.material.color = haloColor;
        rend.material.SetFloat("_Mode", 3);
        rend.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        rend.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        rend.material.EnableKeyword("_ALPHABLEND_ON");
        rend.material.renderQueue = 3000;
        
        Destroy(halo.GetComponent<Collider>());
    }

    private void OnMouseEnter()
    {
        if (!_isCanSelected) return;
        DrawHalo(true);
        _isSelected = true;
    }

    private void OnMouseExit()
    {
        if (!_isSelected) return;
        DrawHalo(false);
        _isSelected = false;
    }

    private void OnMouseDown()
    {
        OnChoseAttackTarget?.Invoke(this);
    }

    public void Attack(Unit unit)
    {
        weapon.Attack(this, unit);
        StartCoroutine(weapon.VisualizeAttack(transform.position, unit.transform.position));
    }

    public void TakeDamage(float incomingDamage)
    {
        ChangeHealth(-(incomingDamage * (1 - defence)));
        healthBar.UpdateHealth(_health, maxHealth);
        StartCoroutine(DamageAnimation());
    }

    private IEnumerator DamageAnimation()
    {
        _rb.AddForce(Vector3.up, ForceMode.Impulse);
        _renderer.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        _renderer.material.color = _originalColor;
    }
}
