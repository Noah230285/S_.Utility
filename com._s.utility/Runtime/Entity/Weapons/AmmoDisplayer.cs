using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class AmmoDisplayer : MonoBehaviour{

    [SerializeField] AmmoConfigScriptableObject ammoReference;
    [SerializeField] TextMeshProUGUI _ammoCurrentText;
    [SerializeField] TextMeshProUGUI _ammoMaxText;
    [SerializeField] Transform _dividerContainer;
    [SerializeField] Image _ammoFill;
    [SerializeField] float _fillSpeed;
    [SerializeField] GameObject _dividerPrefab;
    [SerializeField] Animator ReloadPromptAnim;

    int _lastCurrent;
    int _lastMax;

    void Update()
    {
        if (ammoReference.magCurrent == 0)
        {
            ReloadPromptAnim.gameObject.SetActive(true);
            ReloadPromptAnim.SetBool("NoAmmo", true);
        }
        else if (_lastCurrent == 0)
        {
            ReloadPromptAnim.SetBool("NoAmmo", false);
        }
        if (ammoReference.magCurrent != _lastCurrent)
        {
            _ammoCurrentText.text = ammoReference.magCurrent.ToString();
        }
        _lastCurrent = ammoReference.magCurrent;
        if (ammoReference.magSize != _lastMax)
        {
            _ammoMaxText.text = ammoReference.magSize.ToString();
            float rotDegree = (181.5f) / ((float)ammoReference.magSize);
            for (int i = 0; i < _dividerContainer.childCount; i++)
            {
                Destroy(_dividerContainer.GetChild(i).gameObject);
            }
            for (int i = 1; i < ammoReference.magSize; i++)
            {
                GameObject divider = Instantiate(_dividerPrefab, _dividerContainer);
                divider.transform.localEulerAngles = Vector3.forward * (rotDegree * i - 181.5f / 2);
            }
        }
        _lastMax = ammoReference.magSize;
        _ammoFill.fillAmount = Mathf.MoveTowards(_ammoFill.fillAmount, (float)ammoReference.magCurrent / (float)ammoReference.magSize, Time.deltaTime * (_fillSpeed * Mathf.Exp(Mathf.Abs((float)ammoReference.magCurrent / (float)ammoReference.magSize - _ammoFill.fillAmount)) - _fillSpeed));

    }
}
