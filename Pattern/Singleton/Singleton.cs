using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pattern.Singleton
{
    public abstract class Singleton<T> : RyoMonoBehaviour where T : RyoMonoBehaviour
    {
        protected static T instance;
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<T>();

                    if (instance == null)
                    {
                        GameObject singletonObj = new GameObject();
                        instance = singletonObj.AddComponent<T>();
                        singletonObj.name = typeof(T).ToString();
                    }
                }

                return instance;
            }
        }

        protected override void Awake()
        {
            base.Awake();

            if (instance == null)
            {
                instance = this as T;
            }
            else
            {
                Destroy(this.gameObject);
            }

        }

    }
}

