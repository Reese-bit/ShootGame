using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    [SerializeField] Pool[] enemyPools;
    [SerializeField] Pool[] playerProjectilePools;
    [SerializeField] Pool[] enemyProjectilePools;
    [SerializeField] Pool[] VFXPools;
    [SerializeField] Pool[] lootItemPools;

    static Dictionary<GameObject,Pool> dictionary;

    private void Awake()
    {
        dictionary = new Dictionary<GameObject, Pool>();

        Initialize(enemyPools);
        Initialize(playerProjectilePools);
        Initialize(enemyProjectilePools);
        Initialize(VFXPools);
        Initialize(lootItemPools);
    }

    #if UNITY_EDITOR
    private void OnDestroy() 
    {
        CheckPoolSize(enemyPools);
        CheckPoolSize(playerProjectilePools);    
        CheckPoolSize(enemyProjectilePools);    
        CheckPoolSize(VFXPools);
        CheckPoolSize(lootItemPools);
    }
    #endif

    void CheckPoolSize(Pool[] pools)
    {
        foreach(var pool in pools)
        {
            if(pool.runTimeSize > pool.poolSize)
            {
                Debug.LogWarning(string.Format("Pool: {0} has a runtime size {1} bigger than its initial size {2}",
                                    pool.Prefab.name,
                                    pool.runTimeSize,
                                    pool.poolSize));
            }
        }
    }

    void Initialize(Pool[] pools)
    {
        foreach(var pool in pools)
        {
        #if UNITY_EDITOR //this part if code just compiled in the unityEditor,not our platform.
            if(dictionary.ContainsKey(pool.Prefab))
            {
                Debug.LogError("Repeat key of this dictionary: " + pool.Prefab.name);

                continue;
            }
        #endif
            dictionary.Add(pool.Prefab,pool);

            Transform poolParent = new GameObject("Pool: " + pool.Prefab.name).transform;

            poolParent.parent = transform;
            pool.Initialize(poolParent);
        }
    }

    /// <summary>
    /// <para>Return a specified <paramref name = "prefab"></paramref> gameobject in the pool.</para>
    /// <para>根据传入的 <paramref name = "prefab"></paramref> 参数,返回对象池中预备好的游戏对象.</para>
    /// </summary>
    /// <param name = "prefab">
    /// <para>Specified gameobject prefab.</para>
    /// <para>指定的游戏对象预制体</para>
    /// </param>
    /// <returns>
    /// <para>Prepared gameobject in the the pool.</para>
    /// <para>对象池中预备好的游戏对象.</para>
    /// </returns>
    public static GameObject Release(GameObject prefab)
    {
    #if UNITY_EDITOR
        if(!dictionary.ContainsKey(prefab))
        {
            Debug.LogError("Pool Manager could not find the prefab" + prefab.name);

            return null;
        }
    #endif

        return dictionary[prefab].PreparedObject();
    }

    /// <summary>
    /// <para>Release a specified prepared gameOject in the pool at specified position.</para>
    /// <para>根据传入的prefab参数,在position参数位置释放对象池中预备好的游戏对象.</para>
    /// </summary>
    /// <param name = "prefab">
    /// <para>Specified gameobject prefab.</para>
    /// <para>指定的游戏对象预制体</para>
    /// </param>
    /// <param name = "position">
    /// <para>Specified release position</para>
    /// <para>指定释放位置</para>
    /// </param>
    /// <returns></returns>
    public static GameObject Release(GameObject prefab,Vector3 position)
    {
    #if UNITY_EDITOR
        if(!dictionary.ContainsKey(prefab))
        {
            Debug.LogError("Pool Manager could not find the prefab " + prefab.name);

            return null;
        }
    #endif

        return dictionary[prefab].PreparedObject(position);
    }

    /// <summary>
    /// <para>Release a specified prepared gameOject in the pool at specified position and rotation.</para>
    /// <para>根据传入的prefab参数和rotation参数,在position参数位置释放对象池中预备好的游戏对象.</para>
    /// </summary>
    /// <param name = "prefab">
    /// <para>Specified gameobject prefab.</para>
    /// <para>指定的游戏对象预制体</para>
    /// </param>
    /// <param name = "position">
    /// <para>Specified release position</para>
    /// <para>指定释放位置</para>
    /// </param>
    /// <param name = "rotation">
    /// <para>Specified rotation</para>
    /// <para>指定的旋转值</para>
    /// </param>
    /// <returns></returns>
    public static GameObject Release(GameObject prefab,Vector3 position,Quaternion rotation)
    {
    #if UNITY_EDITOR
        if(!dictionary.ContainsKey(prefab))
        {
            Debug.LogError("Pool Manager could not find the prefab " + prefab.name);

            return null;
        }
    #endif

        return dictionary[prefab].PreparedObject(position,rotation);
    }

    /// <summary>
    /// <para>Release a specified prepared gameOject in the pool at specified position and rotation and scale.</para>
    /// <para>根据传入的prefab参数和rotation参数和localScale参数,在position参数位置释放对象池中预备好的游戏对象.</para>
    /// </summary>
    /// <param name = "prefab">
    /// <para>Specified gameobject prefab.</para>
    /// <para>指定的游戏对象预制体</para>
    /// </param>
    /// <param name = "position">
    /// <para>Specified release position</para>
    /// <para>指定释放位置</para>
    /// </param>
    /// <param name = "rotation">
    /// <para>Specified rotation</para>
    /// <para>指定的旋转值</para>
    /// </param>
    /// <param name = "localScale">
    /// <para>Specified localScale</para>
    /// <para>指定的缩放值</para>
    /// </param>
    /// <returns></returns>
    public static GameObject Release(GameObject prefab,Vector3 position,Quaternion rotation,Vector3 localScale)
    {
    #if UNITY_EDITOR
        if(!dictionary.ContainsKey(prefab))
        {
            Debug.LogError("Pool Manager could not find the prefab" + prefab.name);

            return null;
        }
    #endif
        return dictionary[prefab].PreparedObject(position,rotation,localScale);
    }
    

}
