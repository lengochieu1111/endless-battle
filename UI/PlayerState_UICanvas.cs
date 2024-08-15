using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerState_UICanvas : RyoMonoBehaviour
{
    [SerializeField] private HealthBar _healthBar;

    protected override void LoadComponents()
    {
        base.LoadComponents();

        if (this._healthBar == null)
        {
            this._healthBar = GetComponentInChildren<HealthBar>();
        }

    }

    protected override void Start()
    {
        base.Start();

        Player.Instance.OnTakeDamage += Player_OnTakeDamage;
    }

    private void Player_OnTakeDamage(object sender, Player.TakeDamageEventArgs e)
    {
        this._healthBar.Update_HealthBar(e.Health / e.MaxHealth);
    }
}
