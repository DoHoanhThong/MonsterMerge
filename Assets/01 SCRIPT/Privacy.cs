using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Drawing;

public class Privacy : MonoBehaviour,IPointerClickHandler
{
    [SerializeField]TextMeshProUGUI _text;
    [SerializeField] string _linkTOS,_linkPrivacy;
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
           int index= TMP_TextUtilities.FindIntersectingLink(_text, Input.mousePosition, null);
            //Debug.LogError(index);
            if (index>-1)
            {
                string a = (this.transform.GetSiblingIndex()==1) ? _linkTOS : _linkPrivacy;
                Application.OpenURL(a);
            }
        }
    }
    
}
