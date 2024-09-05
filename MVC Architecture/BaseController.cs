using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Architecture.MVC
{
    public abstract class BaseController<BaseModel, BaseView> : RyoMonoBehaviour
    {
        [Header("MVCS")]
        [SerializeField] protected BaseModel model;
        [SerializeField] protected BaseView view;
        public BaseModel Model => model;
        public BaseView View => view;

        protected override void LoadComponents()
        {
            base.LoadComponents();

            if (this.model == null )
            {
                this.model = GetComponentInChildren<BaseModel>();
            }
            
            if (this.view == null )
            {
                this.view = GetComponentInChildren<BaseView>();
            }
        }

    }

}
