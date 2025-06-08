using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GhostTrail : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float _spawnInterval = 0.1f;
    [SerializeField] float _fadeDuration = 0.5f;
    [SerializeField] Color _startColor = new Color(1, 1, 1, 0.5f);
    [SerializeField] Gradient _colorGradient;

    private SpriteRenderer _playerSprite;
    private Queue<GameObject> _ghostPool = new Queue<GameObject>();
    private bool _isTrailActive;

    void Awake()
    {
        _playerSprite = GetComponent<SpriteRenderer>();
        PrewarmPool(10); 
    }

    void Update()
    {
        // Activate trail only when moving
        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
        {
            if (!_isTrailActive) StartCoroutine(SpawnGhosts());
        }
        else
        {
            _isTrailActive = false;
        }
    }

    void PrewarmPool(int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject ghost = new GameObject("Ghost");
            SpriteRenderer ghostRenderer = ghost.AddComponent<SpriteRenderer>();
            Color ghostColor = ghostRenderer.color;
            ghostColor.a = 0.3f; 
            ghostRenderer.color = ghostColor;
            ghostRenderer.sortingOrder = _playerSprite.sortingOrder - 1;

            ghost.SetActive(false);
            _ghostPool.Enqueue(ghost);
        }
    }

    GameObject GetGhostFromPool()
    {
        if (_ghostPool.Count == 0) PrewarmPool(2);
        return _ghostPool.Dequeue();
    }

    IEnumerator SpawnGhosts()
    {
        _isTrailActive = true;
        while (_isTrailActive)
        {
            GameObject ghost = GetGhostFromPool();
            ghost.transform.position = transform.position;
            ghost.transform.localScale = transform.localScale;

            SpriteRenderer ghostRenderer = ghost.GetComponent<SpriteRenderer>();
            ghostRenderer.sprite = _playerSprite.sprite;
            ghostRenderer.flipX = _playerSprite.flipX;

            ghost.SetActive(true);
            StartCoroutine(FadeGhost(ghost, _fadeDuration));

            yield return new WaitForSeconds(_spawnInterval);
        }
    }

    IEnumerator FadeGhost(GameObject ghost, float duration)
    {
        SpriteRenderer renderer = ghost.GetComponent<SpriteRenderer>();
        float elapsed = 0f;
        Color startColor = _colorGradient.Evaluate(0);
        Color endColor = _colorGradient.Evaluate(1);

        while (elapsed < duration)
        {
            renderer.color = Color.Lerp(startColor, endColor, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        ghost.SetActive(false);
        _ghostPool.Enqueue(ghost);
    }
}
