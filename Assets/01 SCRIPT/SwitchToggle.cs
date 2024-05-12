using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwitchToggle : MonoBehaviour
{
    [SerializeField] RectTransform _handleRect;
    Toggle _toggle;
    Vector2 _handlePos;
    [SerializeField] Image _green;
    [SerializeField] AudioClip _clickSound;
    [SerializeField] int dem;
    // Start is called before the first frame update
    void Awake()
    {
       
        _toggle=this.GetComponent<Toggle>();
        _handlePos = _handleRect.anchoredPosition;
        _toggle.onValueChanged.AddListener(OnSwitch);
        if (_toggle.isOn)
        {
            OnSwitch(true);
        }
    }

    void OnSwitch(bool on) 
    {
      if(GameManager.instance.canplaySound)
      {
            SoundEffect.instance.PlaySound(_clickSound);
      }
      _handleRect.anchoredPosition = (on)?_handlePos *-1 : _handlePos;
      _green.fillAmount= (on) ?1 : 0;
    }
    void OnDestroy()
    {
        _toggle.onValueChanged.RemoveListener(OnSwitch);
    }
}
