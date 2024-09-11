using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : RyoMonoBehaviour
{
    [SerializeField] private Image _healthBarImage;
    [SerializeField] private Gradient _healthBarGradient;
    [SerializeField] private float _timeChange = 0.15f;
    private float _targetPercent = 1f;
    private Color _targetColor;
    private Coroutine _changeHealthBarColorCoroutine;

    protected override void LoadComponents()
    {
        base.LoadComponents();

        if (this._healthBarImage == null)
        {
            Transform healthBarTransform = this.transform.Find("HealthBarImage");
            this._healthBarImage = healthBarTransform.GetComponent<Image>();
        }

    }

    protected override void Start()
    {
        base.Start();

        this._healthBarImage.color = this._healthBarGradient.Evaluate(this._targetPercent);
        this.CheckHealthBarGradientAmount();
    }

    public void Update_HealthBar(float percen)
    {
        if (this._healthBarImage == null) return;

        this._targetPercent = percen;

        this._changeHealthBarColorCoroutine = StartCoroutine(this.ChangeHealthBarColor_Coroutine());

        this.CheckHealthBarGradientAmount();

    }

    private IEnumerator ChangeHealthBarColor_Coroutine()
    {
        float timer = 0;

        float currentPercen = this._healthBarImage.fillAmount;
        Color currentColor = this._healthBarImage.color;

        while (timer <= this._timeChange)
        {
            timer += Time.deltaTime;

            this._healthBarImage.fillAmount = Mathf.Lerp(currentPercen, this._targetPercent, (timer / this._timeChange));

            this._healthBarImage.color = Color.Lerp(currentColor, this._targetColor, (timer / this._timeChange));

            yield return null;
        }

    }

    private void CheckHealthBarGradientAmount()
    {
        this._targetColor = this._healthBarGradient.Evaluate(this._targetPercent);
    }



}
