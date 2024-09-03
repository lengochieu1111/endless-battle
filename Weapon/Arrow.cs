using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : RyoMonoBehaviour
{
    [SerializeField] private Rigidbody _rigidbody;

    protected override void LoadComponents()
    {
        base.LoadComponents();

        if (this._rigidbody == null)
        {
            this._rigidbody = GetComponent<Rigidbody>();
        }
    }

    public void ActivateArrow(Vector3 direction, float speed)
    {
        this.transform.forward = direction;
        this._rigidbody.velocity = this.transform.forward * speed;
    }

    public void DestroySelf()
    {
        this.gameObject.SetActive(false);
    }

}
