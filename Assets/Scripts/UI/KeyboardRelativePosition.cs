using UnityEngine;
using UnityEngine.UI;

namespace ChatSim.Helpers
{
    public class KeyboardRelativePosition : MonoBehaviour
    {
        [SerializeField]
        private RectTransform chatRect;
        [SerializeField]
        private CanvasScaler canvasScaler;

#if UNITY_ANDROID
        private void Update()
        {
            var rate = canvasScaler.referenceResolution.y / Screen.height;
            var pos = chatRect.anchoredPosition;
            pos.y = KeyboardArea.GetHeight(true) * rate;
            chatRect.anchoredPosition = pos;
        }
#endif
    }
}
