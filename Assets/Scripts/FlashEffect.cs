using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Renderer))]
public class FlashEffect : MonoBehaviour
{
    public Color flashColor;
    public float duration;

    private Renderer _renderer;
    private Material _originalMaterial;
    private Material _flashMaterial;
    private Coroutine _flashCoroutine;

    private void Start()
    {
        _renderer = GetComponent<Renderer>();

        _originalMaterial = _renderer.material;
        _flashMaterial = Instantiate(_originalMaterial);
        _renderer.material = _flashMaterial;
    }

    public void StartFlashing()
    {
        if (_flashCoroutine != null) StopCoroutine(_flashCoroutine);
        _flashCoroutine = StartCoroutine(FlashRoutine());
    }

    public void StopFlashing()
    {
        if (_flashCoroutine == null) return;
        StopCoroutine(_flashCoroutine);
        SetFlashIntensity(0f);
    }

    private IEnumerator FlashRoutine()
    {
        while (true)
        {
            var elapsed = 0f;
            while (elapsed < duration / 2)
            {
                elapsed += Time.deltaTime;
                var t = elapsed / (duration / 2);
                SetFlashIntensity(t);
                yield return null;
            }
            
            elapsed = 0f;
            while (elapsed < duration / 2)
            {
                elapsed += Time.deltaTime;
                var t = 1f - (elapsed / (duration / 2));
                SetFlashIntensity(t);
                yield return null;
            }
        }
    }

    private void SetFlashIntensity(float intensity)
    {
        var baseColor = _originalMaterial.color;
        var newColor = new Color(
            Mathf.Lerp(baseColor.r, flashColor.r, intensity),
            Mathf.Lerp(baseColor.g, flashColor.g, intensity),
            Mathf.Lerp(baseColor.b, flashColor.b, intensity),
            baseColor.a
        );
        _flashMaterial.color = newColor;
    }

    private void OnDisable()
    {
        if (_flashCoroutine != null)
        {
            StopCoroutine(_flashCoroutine);
            SetFlashIntensity(0f);
        }
    }
}