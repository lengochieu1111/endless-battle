using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerState_UICanvas : RyoMonoBehaviour
{
    [SerializeField] private BasePlayer _player;
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

        this._player.OnTakeDamage += Player_OnTakeDamage;
    }

    private void Player_OnTakeDamage(object sender, BasePlayer.TakeDamageEventArgs e)
    {
        this._healthBar.Update_HealthBar(e.Health / e.MaxHealth);
    }
}
