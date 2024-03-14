using _S.ScriptableVariables;
using UnityEngine;
using UnityEngine.UI;

namespace _S.UI
{
    [RequireComponent(typeof(Image))]
    public class FillBar : MonoBehaviour
    {
        [SerializeField] FloatVariable _max;
        Image image;
        void Awake()
        {
            image = GetComponent<Image>();
        }
        public void SetBarProgress(float min, float max = 0)
        {
            float finalMin = _max == null ? max : _max.Value;
            image.fillAmount = min / finalMin;
        }
    }
}