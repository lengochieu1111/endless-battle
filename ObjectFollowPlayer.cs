using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectFollowPlayer : RyoMonoBehaviour
{
    [SerializeField] private GameObject _playerObject;

    private void FixedUpdate()
    {
        this.transform.position = this._playerObject.transform.position;
    }

}
