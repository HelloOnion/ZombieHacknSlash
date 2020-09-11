using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    Transform[] spawnPoints;

    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    public List<Pool> pools;
    public Dictionary<string, Queue<GameObject>> poolDictionary;

    void Start()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach(Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for(int i = 0; i< pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab)
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if(!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("pool with tag " + tag + "dont excist");
            return null;
        }

        GameObject SpawnObj = poolDictionary[tag].Dequeue();

        SpawnObj.SetActive(true);
        SpawnObj.transform.position = position;
        SpawnObj.transform.rotation = rotation;

        poolDictionary[tag].Enqueue(SpawnObj);

        return SpawnObj;
    }

}
