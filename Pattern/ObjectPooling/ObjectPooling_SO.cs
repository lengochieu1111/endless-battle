using Pattern.ObjectPooling;
using Pattern.Singleton;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pattern.ObjectPooling
{
    public abstract class ObjectPooling_SO<T> : Singleton<ObjectPooling_SO<T>>
    {
    //    [SerializeField] protected List<ObjectPoolingSO> _prefabSOList;
    //    [SerializeField] protected List<Transform> _poobObjectList;
    //    [SerializeField] protected int _spawnCount;

    //    protected override void LoadComponents()
    //    {
    //        base.LoadComponents();

    //        this.LoadPrefabSOList();
    //    }

    //    public abstract void LoadPrefabSOList();

    //    /*
    //     * 
    //     */

    //    public virtual Transform Spawn(string prefabName, Vector3 position, Quaternion rotation)
    //    {
    //        Transform prefab = this.GetPrefabByName(prefabName);
    //        if (prefab == null)
    //        {
    //            Debug.LogWarning("Prefab not found: " + prefabName);
    //            return null;
    //        }

    //        return this.Spawn(prefab, position, rotation);
    //    }

    //    protected virtual Transform Spawn(Transform prefab, Vector3 position, Quaternion rotation)
    //    {
    //        Transform newPrefab = this.GetObjectFromPool(prefab);
    //        newPrefab.SetPositionAndRotation(position, rotation);
    //        newPrefab.SetParent(this._holder);
    //        this._spawnCount++;

    //        return newPrefab;
    //    }

    //    protected virtual Transform GetObjectFromPool(Transform prefab)
    //    {
    //        foreach (Transform poolObject in this._poobObjectList)
    //        {
    //            if (poolObject == null) continue;

    //            if (poolObject.name == prefab.name)
    //            {
    //                this._poobObjectList.Remove(poolObject);
    //                return poolObject;
    //            }
    //        }

    //        Transform newPrefab = Instantiate(prefab);
    //        newPrefab.name = prefab.name;
    //        return newPrefab;
    //    }

    //    protected virtual Transform GetPrefabByName(string prefabName)
    //    {
    //        foreach (Transform prefab in this._prefabSOList)
    //        {
    //            if (prefab.name == prefabName) return prefab;
    //        }

    //        return null;
    //    }

    //    public virtual void Destroy(Transform obj)
    //    {
    //        if (this._poobObjectList.Contains(obj)) return;

    //        this._poobObjectList.Add(obj);
    //        obj.SetParent(this.transform);
    //        obj.gameObject.SetActive(false);
    //        this._spawnCount--;
    //    }

    }

}

