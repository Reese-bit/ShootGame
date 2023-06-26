using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Pool 
{
    public GameObject Prefab {get => prefab;}
    public int poolSize => size;
    public int runTimeSize => queue.Count;

    [SerializeField]GameObject prefab;
    [SerializeField]int size = 1;

    Queue<GameObject> queue;

    Transform parent;

#region Create Prepared Objetcs
    public void Initialize(Transform parent) 
    {
        queue = new Queue<GameObject>();
        this.parent = parent;

        for(var i = 0; i < size; i++)
        {
             queue.Enqueue(Copy());
        }
    }

    GameObject Copy()
    {
        var copy =  GameObject.Instantiate(prefab,parent);

        copy.SetActive(false);

        return copy;
    }
#endregion 

#region Get a available object from pool
    public GameObject AvailableObject()
    {
        GameObject availableObject = null;

        if(queue.Count > 0 && !queue.Peek().activeSelf)
        {
            availableObject = queue.Dequeue();
        }
        else
        {
            availableObject = Copy(); //need to try to avoid this situation
        }

        queue.Enqueue(availableObject);

        return availableObject;
    }
#endregion

#region Activate the available object
    public GameObject PreparedObject()
    {
        GameObject preparedObject = AvailableObject();

        preparedObject.SetActive(true);

        return preparedObject;
    }

    public GameObject PreparedObject(Vector3 position)
    {
        GameObject preparedObject = AvailableObject();

        preparedObject.SetActive(true);
        preparedObject.transform.position = position;

        return preparedObject;
    }

    public GameObject PreparedObject(Vector3 position,Quaternion rotation)
    {
        GameObject preparedObject = AvailableObject();

        preparedObject.SetActive(true);
        preparedObject.transform.position = position;
        preparedObject.transform.rotation = rotation;

        return preparedObject;
    }

    public GameObject PreparedObject(Vector3 position,Quaternion rotation,Vector3 localScale)
    {
        GameObject preparedObject = AvailableObject();

        preparedObject.SetActive(true);
        preparedObject.transform.position = position;
        preparedObject.transform.rotation = rotation;
        preparedObject.transform.localScale = localScale;

        return preparedObject;
    }
#endregion


#region Return object to the pool
/*
    public void ReturnPool(GameObject gameObject)
    {
        queue.Enqueue(gameObject);
    }
*/
#endregion

}
