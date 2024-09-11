using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Player_Abe : BasePlayer
{
    #region IAttackable

    public override void StartTrace()
    {
        Transform spawnTransform = this._traceTracker[0].StartTrace;
        Transform magicBallTransform = WeaponSpawner.Instance.Spawn(WeaponSpawner.MagicBall_Abe, spawnTransform.position, Quaternion.identity);
        magicBallTransform.gameObject.SetActive(true);
        magicBallTransform.GetComponent<Projectile>()?.ActivateSelf(this, this, this.transform.forward, this.model.PlayerStats.WeaponSpeed, this.model.PlayerStats.Damage);
    
        this.PlayWeaponAttackSound();
    }

    public override void Tracing() { }

    public override void EndTrace() { }

    #endregion


}
