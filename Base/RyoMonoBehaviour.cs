using UnityEngine;

public abstract class RyoMonoBehaviour : MonoBehaviour
{
    protected virtual void Awake()
    {
        this.LoadComponents();
    }

    protected virtual void OnEnable()
    {
        this.LoadComponents();
        this.SetupValues();
    }

    protected virtual void Reset()
    {
        this.LoadComponents();
        this.SetupValues();
    }

    protected virtual void Start()
    {
        
    }

    protected virtual void OnDisable()
    {

    }

    protected virtual void LoadComponents()
    {

    }    
    
    protected virtual void SetupValues()
    {

    }

}
