using UnityEngine;

public class Flash : MonoBehaviour
{
    public SpriteRenderer SpriteRenderer;
    public Color FlashColor = Color.red;
    public float FlashDuration = 0.08f; // About 5 frames at 60fps

    private Color OriginalColor;
    private float FlashTimer = 0f;
    private bool IsFlashing = false;

    void Awake()
    {
        if (SpriteRenderer == null)
            SpriteRenderer = GetComponent<SpriteRenderer>();
        OriginalColor = SpriteRenderer.color;
    }

    public void SpriteFlash()
    {
        SpriteRenderer.color = FlashColor;
        FlashTimer = FlashDuration;
        IsFlashing = true;
    }

    void Update()
    {
        if (IsFlashing)
        {
            FlashTimer -= Time.deltaTime;
            if (FlashTimer <= 0f)
            {
                SpriteRenderer.color = OriginalColor;
                IsFlashing = false;
            }
        }
    }
}