
using UnityEngine;
using UnityEngine.UI;

public class FallingArrow : MonoBehaviour
{
    [Header("Arrow Image")]
    public Image arrowImage;

    private RectTransform rectTransform;
    private float fallSpeed;
    private float destroyY;
    private bool isMoving;

    public float CurrentY => rectTransform != null
        ? rectTransform.anchoredPosition.y
        : 0f;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        if (arrowImage == null)
            arrowImage = GetComponent<Image>();
    }

    public void Initialize(float speed, float bottomY, Sprite sprite)
    {
        fallSpeed = speed;
        destroyY = bottomY;

        if (arrowImage != null && sprite != null)
            arrowImage.sprite = sprite;

        isMoving = true;
    }

    void Update()
    {
        if (!isMoving || rectTransform == null)
            return;

        rectTransform.anchoredPosition +=
            Vector2.down * fallSpeed * Time.deltaTime;

        if (rectTransform.anchoredPosition.y <= destroyY)
        {
            Destroy(gameObject);
        }
    }
}