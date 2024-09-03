using Pattern.Singleton;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MousePosition : Singleton<MousePosition>
{
    [SerializeField] private Camera _camera;
    [SerializeField] private LayerMask _floorLayer;
    private Vector3 _mousePosition;

    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, this._floorLayer))
        {
            this._mousePosition = raycastHit.point;
        }
    }

    public Vector3 GetMousePosition()
    { 
        return this._mousePosition; 
    }

}
