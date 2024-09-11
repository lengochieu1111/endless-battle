using Pattern.Singleton;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetttingsUI : Singleton<SetttingsUI>
{
    [SerializeField] private Slider _masterSlider;
    [SerializeField] private Slider _soundFXSlider;
    [SerializeField] private Slider _musicSlider;
    public Slider MasterSlider => _masterSlider;
    public Slider SoundFXSlider => _soundFXSlider;
    public Slider MusicSlider => _musicSlider;
     

    protected override void LoadComponents()
    {
        base.LoadComponents();

        if (this._masterSlider == null)
        {
            Transform masterSliderTransform = this.transform.Find("Master");
            this._masterSlider = masterSliderTransform.GetComponentInChildren<Slider>();
        }
        
        if (this._soundFXSlider == null)
        {
            Transform soundFXSliderTransform = this.transform.Find("SoundFX");
            this._soundFXSlider = soundFXSliderTransform.GetComponentInChildren<Slider>();
        }
        
        if (this._musicSlider == null)
        {
            Transform musicSliderTransform = this.transform.Find("Music");
            this._musicSlider = musicSliderTransform.GetComponentInChildren<Slider>();
        }
    }


}
