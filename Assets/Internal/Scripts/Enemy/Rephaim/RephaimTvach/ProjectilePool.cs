using UnityEngine;
using System.Collections.Generic;

// Projectile Pool Manager (Singleton)
public class ProjectilePool : MonoBehaviour
{
    public static ProjectilePool Instance;

    [SerializeField] private GameObject ProjectilePrefab;
    [SerializeField] private int PoolSize = 20;

    private Queue<GameObject> ProjectilePoolObj = new Queue<GameObject>();

    void Awake()
    {
        Instance = this;
        InitializePool();
    }

    void InitializePool()
    {
        for (int i = 0; i < PoolSize; i++)
        {
            GameObject Projectile = Instantiate(ProjectilePrefab);
            Projectile.SetActive(false);
            Projectile.transform.SetParent(gameObject.transform);
            ProjectilePoolObj.Enqueue(Projectile);
        }
    }

    public GameObject GetProjectile(Vector2 Position, Quaternion Rotation)
    {
        if (ProjectilePoolObj.Count == 0)
            ExpandPool();

        GameObject Projectile = ProjectilePoolObj.Dequeue();
        Projectile.transform.position = Position;
        Projectile.transform.rotation = Rotation;
        Projectile.transform.localScale = new Vector3(1, 1, 0);
        Projectile.SetActive(true);
        return Projectile;
    }

    public void ReturnProjectile(GameObject projectile)
    {
        projectile.SetActive(false);
        ProjectilePoolObj.Enqueue(projectile);
    }

    private void ExpandPool()
    {
        GameObject Projectile = Instantiate(ProjectilePrefab);
        Projectile.SetActive(false);
        ProjectilePoolObj.Enqueue(Projectile);
    }
}

