

using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
// #if UNITY_WEBGL
// using WeChatWASM;
// #endif

namespace NFramework.ModuleSystem
{
    // #if UNITY_WEBGL
#if false
    public class CustomInputFieldTMP : TMP_InputField, IPointerClickHandler, IPointerExitHandler
    {
       
        private bool isShowKeyboad = false;
        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log("ooooo");
            ShowKeyboad();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            // 隐藏输入法
            if (!this.isFocused)
            {
                HideKeyboad();
            }
        }

        public void OnInput(OnKeyboardInputListenerResult v)
        {
            Debug.Log("onInput");
            Debug.Log(v.value);
            if (this.isFocused)
            {
                this.text = v.value;
            }
        }

        public void OnConfirm(OnKeyboardInputListenerResult v)
        {
            // 输入法confirm回调
            Debug.Log("onConfirm");
            Debug.Log(v.value);
            HideKeyboad();
        }

        public void OnComplete(OnKeyboardInputListenerResult v)
        {
            // 输入法complete回调
            Debug.Log("OnComplete");
            Debug.Log(v.value);
            HideKeyboad();
        }

        private void ShowKeyboad()
        {
            if (!isShowKeyboad)
            {
                WX.ShowKeyboard(new ShowKeyboardOption()
                {
                    defaultValue = this.text,
                    maxLength = 20,
                    confirmType = "go"
                });

                //绑定回调
                WX.OnKeyboardConfirm(OnConfirm);
                WX.OnKeyboardComplete(OnComplete);
                WX.OnKeyboardInput(OnInput);
                isShowKeyboad = true;
            }
        }

        private void HideKeyboad()
        {
            if (isShowKeyboad)
            {
                WX.HideKeyboard(new HideKeyboardOption());
                //删除掉相关事件监听
                WX.OffKeyboardInput(OnInput);
                WX.OffKeyboardConfirm(OnConfirm);
                WX.OffKeyboardComplete(OnComplete);
                isShowKeyboad = false;
            }
        }
    }

    // #else
#endif
    public class CustomInputFieldTMP : TMP_InputField
    {
    }

    // #endif

}