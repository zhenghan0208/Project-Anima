
using UnityEngine;
using UnityEngine.UI;

public class FallingArrow : MonoBehaviour
{
    public Image arrowImage;

    private RectTransform rectTransform;
    private float fallSpeed;
    private float destroyY;
    private bool isMoving;
    private bool isHit;
    private bool isResolved;

    private RhythmGameManager gameManager;

    public float CurrentY => rectTransform != null
        ? rectTransform.anchoredPosition.y : 0f;

    public bool IsHit => isHit;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        if (arrowImage == null)
            arrowImage = GetComponent<Image>();
    }

    public void Initialize(
        float speed,
        float bottomY,
        Sprite sprite,
        RhythmGameManager manager)
    {
        fallSpeed = speed;
        destroyY = bottomY;
        gameManager = manager;

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
            ResolveMiss();
            Destroy(gameObject);
        }
    }

    public void Hit()
    {
        if (isHit || isResolved)
            return;

        isHit = true;
        isMoving = false;
        isResolved = true;

        if (gameManager != null)
            gameManager.RegisterHit();

        Destroy(gameObject);
    }

    void ResolveMiss()
    {
        if (isResolved)
            return;

        isResolved = true;

        if (gameManager != null)
            gameManager.RegisterMiss();
    }
}