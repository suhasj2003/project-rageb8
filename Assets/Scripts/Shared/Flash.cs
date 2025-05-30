using UnityEngine;

public class Flash : MonoBehaviour
{
    
    public float m_FlickerDuration = 0.5f; 
    public int m_FlickerCount = 5;
    public Color FlickerColor = Color.red;

    private float FlickerTimer;
    private int FlickersDone;
    private bool IsFlickering = false;

    private Color OriginalColor;

    private SpriteRenderer SpriteRenderer;

    void Awake()
    {
        SpriteRenderer = GetComponent<SpriteRenderer>();
        OriginalColor = SpriteRenderer.color;
    }

    public void SpriteFlicker()
    {
        IsFlickering = true;
        FlickerTimer = m_FlickerDuration / (m_FlickerCount * 2f);
        FlickersDone = 0;
        SpriteRenderer.enabled = false;
        SpriteRenderer.color = FlickerColor;
    }

    void Update()
    {
        if (!IsFlickering) return;

        FlickerTimer -= Time.deltaTime;
        if (FlickerTimer <= 0f)
        {
            SpriteRenderer.enabled = !SpriteRenderer.enabled;
            FlickersDone++;
            FlickerTimer = m_FlickerDuration / (m_FlickerCount * 2f);

            if (FlickersDone >= m_FlickerCount * 2)
            {
                IsFlickering = false;
                SpriteRenderer.enabled = true; 
                SpriteRenderer.color = OriginalColor; 
            }
        }
    }
}