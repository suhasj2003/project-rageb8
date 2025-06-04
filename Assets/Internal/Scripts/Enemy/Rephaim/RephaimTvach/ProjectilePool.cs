using UnityEngine;
using System.Collections.Generic;

// Projectile Pool Manager (Singleton)
public class ProjectilePool : MonoBehaviour
{
    public static ProjectilePool Instance;

    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private int poolSize = 20;

    private Queue<GameObject> projectilePool = new Queue<GameObject>();

    void Awake()
    {
        Instance = this;
        InitializePool();
    }

    void InitializePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject projectile = Instantiate(projectilePrefab);
            projectile.SetActive(false);
            projectile.transform.SetParent(gameObject.transform);
            projectilePool.Enqueue(projectile);
        }
    }

    public GameObject GetProjectile(Vector2 position, Quaternion rotation)
    {
        if (projectilePool.Count == 0)
            ExpandPool();

        GameObject projectile = projectilePool.Dequeue();
        projectile.transform.position = position;
        projectile.transform.rotation = rotation;
        projectile.transform.localScale = new Vector3(1, 1, 0);
        projectile.SetActive(true);
        return projectile;
    }

    public void ReturnProjectile(GameObject projectile)
    {
        projectile.SetActive(false);
        projectilePool.Enqueue(projectile);
    }

    private void ExpandPool()
    {
        GameObject projectile = Instantiate(projectilePrefab);
        projectile.SetActive(false);
        projectilePool.Enqueue(projectile);
    }
}

