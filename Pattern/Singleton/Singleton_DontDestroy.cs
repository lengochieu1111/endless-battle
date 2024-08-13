using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pattern.Singleton
{ 
    public abstract class Singleton_DontDestroy<T> : RyoMonoBehaviour where T : RyoMonoBehaviour
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
                        GameObject instanceObj = new GameObject();
                        instance = instanceObj.AddComponent<T>();
                        instanceObj.name = typeof(T).ToString();
                        DontDestroyOnLoad(instance);

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
                DontDestroyOnLoad(instance);
            }
            else
            {
                Destroy(instance);
            }
        }

    }

}
