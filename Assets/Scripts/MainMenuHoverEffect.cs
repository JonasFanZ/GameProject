using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>Simple monochrome hover feedback for the placeholder main menu.</summary>
public sealed class MainMenuHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image background;
    [SerializeField] private Text label;
    [SerializeField] private Color normalBackground = Color.white;
    [SerializeField] private Color hoverBackground = Color.black;
    [SerializeField] private Color normalText = Color.black;
    [SerializeField] private Color hoverText = Color.white;
    [SerializeField] private float hoverScale = 1.04f;

    private Vector3 originalScale;

    private void Awake()
    {
        originalScale = transform.localScale;
        Apply(false);
    }

    public void OnPointerEnter(PointerEventData eventData) => Apply(true);

    public void OnPointerExit(PointerEventData eventData) => Apply(false);

    private void Apply(bool hovered)
    {
        if (background != null) background.color = hovered ? hoverBackground : normalBackground;
        if (label != null) label.color = hovered ? hoverText : normalText;
        transform.localScale = originalScale * (hovered ? hoverScale : 1f);
    }
}
