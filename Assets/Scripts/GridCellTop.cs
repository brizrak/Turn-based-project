using UnityEngine;

public class GridCellTop : MonoBehaviour
{
    private static readonly int Color1 = Shader.PropertyToID("_Color");
    private static readonly int HighlightColor = Shader.PropertyToID("_HighlightColor");
    private static readonly int HighlightIntensity = Shader.PropertyToID("_HighlightIntensity");
    private Material _cellMaterial;
    private Color _originalColor;
    private Color _highlightColor;
    private GridCell _gridCell;
    
    private bool _isHighlighted;
    
    private void Start() {
        _gridCell = GetComponentInParent<GridCell>();
        _cellMaterial = GetComponent<Renderer>().material;
        _originalColor = _cellMaterial.GetColor(Color1);
        _highlightColor = _cellMaterial.GetColor(HighlightColor);
    }

    private void OnMouseEnter() => SetHighlight(!_isHighlighted);
    private void OnMouseExit() => SetHighlight(_isHighlighted);
    
    public void SetHighlight(bool highlight, bool isOut = false) {
        if (highlight) {
            _cellMaterial.SetColor(Color1, _highlightColor);
            _cellMaterial.SetFloat(HighlightIntensity, 0.7f);
        } else {
            _cellMaterial.SetColor(Color1, _originalColor);
            _cellMaterial.SetFloat(HighlightIntensity, 0f);
        }
        if (isOut) _isHighlighted = highlight;
    }
    
    private void OnMouseDown()
    {
        _gridCell.OnMouseDown();
    }
}
