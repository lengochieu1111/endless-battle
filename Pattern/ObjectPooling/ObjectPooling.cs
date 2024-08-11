using Pattern.Singleton;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pattern.ObjectPooling
{
    public abstract class ObjectPooling<T> : Singleton<ObjectPooling<T>>
    {
        [SerializeField] protected Transform _holder;
        [SerializeField] protected List<Transform> _prefabs;
        [SerializeField] protected List<Transform> _poobObjects;
        [SerializeField] protected int _spawnCount;

        protected override void LoadComponents()
        {
            base.LoadComponents();

            if (this._holder == null)
            {
                this._holder = this.transform.Find("Holder");
            }

            if (this._prefabs.Count <= 0)
            {
                Transform prefabTransform = this.transform.Find("Prefabs");


                if (prefabTransform != null)
                {
                    foreach (Transform prefab in prefabTransform)
                    {
                        this._prefabs.Add(prefab);
                        prefab.gameObject.SetActive(false);
                    }
                }
            }
        }

        public virtual Transform Spawn(string prefabName, Vector3 position, Quaternion rotation)
        {
            Transform prefab = this.GetPrefabByName(prefabName);
            if (prefab == null)
            {
                Debug.LogWarning("Prefab not found: " + prefabName);
                return null;
            }

            return this.Spawn(prefab, position, rotation);
        }

        protected virtual Transform Spawn(Transform prefab, Vector3 position, Quaternion rotation)
        {
            Transform newPrefab = this.GetObjectFromPool(prefab);
            newPrefab.SetPositionAndRotation(position, rotation);
            newPrefab.SetParent(this._holder);
            this._spawnCount++;

            return newPrefab;
        }

        protected virtual Transform GetObjectFromPool(Transform prefab)
        {
            foreach (Transform poolObject in this._poobObjects)
            {
                if (poolObject == null) continue;

                if (poolObject.name == prefab.name)
                {
                    this._poobObjects.Remove(poolObject);
                    return poolObject;
                }
            }

            Transform newPrefab = Instantiate(prefab);
            newPrefab.name = prefab.name;
            return newPrefab;
        }

        protected virtual Transform GetPrefabByName(string prefabName)
        {
            foreach (Transform prefab in this._prefabs)
            {
                if (prefab.name == prefabName) return prefab;
            }

            return null;
        }

        public virtual void Destroy(Transform obj)
        {
            if (this._poobObjects.Contains(obj)) return;

            this._poobObjects.Add(obj);
            obj.SetParent(this._holder);
            obj.gameObject.SetActive(false);
            this._spawnCount--;
        }

    }

}
