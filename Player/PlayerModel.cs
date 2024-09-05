using Pattern.Singleton;
using System;
using System.Collections;
using System.Collections.Generic;
using Architecture.MVC;
using UnityEngine;


public class PlayerModel : BaseModel<BasePlayer>
{
    [SerializeField] protected PlayerStats _playerStats;
    public PlayerStats PlayerStats => _playerStats;
    
    protected override void LoadComponents()
    {
        base.LoadComponents();

        if (this._playerStats == null)
        {
            string path = "PlayerStats/PlayerStats_" + this.transform.parent.name;
            this._playerStats = Resources.Load<PlayerStats>(path);
        }
    }
}
