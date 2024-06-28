using UnityEngine;
using UnityEngine.Pool;

public class BulletPool : MonoBehaviour
{
    public static BulletPool Instance { get; private set; }

    [SerializeField] private Bullet bulletPrefab;
    private ObjectPool<Bullet> pool;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializePool();
        }
    }

    private void InitializePool()
    {
        pool = new ObjectPool<Bullet>(
            createFunc: () => Instantiate(bulletPrefab),
            actionOnGet: (bullet) => bullet.gameObject.SetActive(true),
            actionOnRelease: (bullet) => bullet.gameObject.SetActive(false),
            actionOnDestroy: (bullet) => Destroy(bullet.gameObject),
            collectionCheck: false,
            defaultCapacity: 20,
            maxSize: 100);
    }

    public Bullet GetFromPool()
    {
        return pool.Get();
    }

    public void ReturnToPool(Bullet bullet)
    {
        pool.Release(bullet);
    }
}