using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Architecture.MVC
{
    public abstract class BaseModel<BaseController> : RyoMonoBehaviour
    {
        [Header("MVCS")]
        [SerializeField] protected BaseController controller;
        public BaseController Controller => controller;

        protected override void LoadComponents()
        {
            base.LoadComponents();

            if (this.controller == null)
            {
                this.controller = GetComponentInParent<BaseController>();
            }

        }

    }
}
