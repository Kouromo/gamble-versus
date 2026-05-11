using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonImageSwap : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Tooltip("L'image à modifier. Si vide, utilisera l'Image sur ce GameObject.")]
    public Image targetImage;

    [Tooltip("Image par défaut")]
    public Sprite normalSprite;

    [Tooltip("Image affichée au survol (Hover)")]
    public Sprite hoverSprite;

    [Tooltip("Image affichée au clic (Press)")]
    public Sprite pressedSprite;

    private bool isHovered = false;
    private Button buttonComponent;

    private void Awake()
    {
        if (targetImage == null)
        {
            targetImage = GetComponent<Image>();
        }

        buttonComponent = GetComponent<Button>();

        if (targetImage != null && normalSprite == null)
        {
            normalSprite = targetImage.sprite;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (buttonComponent != null && !buttonComponent.interactable) return;

        isHovered = true;
        if (targetImage != null && hoverSprite != null)
        {
            targetImage.sprite = hoverSprite;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (buttonComponent != null && !buttonComponent.interactable) return;

        isHovered = false;
        if (targetImage != null && normalSprite != null)
        {
            targetImage.sprite = normalSprite;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (buttonComponent != null && !buttonComponent.interactable) return;

        if (targetImage != null && pressedSprite != null)
        {
            targetImage.sprite = pressedSprite;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (buttonComponent != null && !buttonComponent.interactable) return;

        if (targetImage != null)
        {
            if (isHovered)
            {
                targetImage.sprite = hoverSprite != null ? hoverSprite : normalSprite;
            }
            else
            {
                targetImage.sprite = normalSprite;
            }
        }
    }
}
