using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Image buttonImage;
    private Color originalColor;
    public Color hoverColor = new Color(0.8f, 0.8f, 1f); // light blue tint

    void Start()
    {
        buttonImage = GetComponent<Image>();
        if (buttonImage != null)
            originalColor = buttonImage.color;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (buttonImage != null)
            buttonImage.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (buttonImage != null)
            buttonImage.color = originalColor;
    }
}
