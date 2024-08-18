using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectFollowPlayer : RyoMonoBehaviour
{
    [SerializeField] private GameObject _playerObject;

    private void FixedUpdate()
    {
        Vector3 targetPos = this._playerObject.transform.position;
        targetPos.y = 0;
        this.transform.position = targetPos;
    }

}
