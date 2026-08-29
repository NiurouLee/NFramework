using System;
using UnityEngine;
using UnityEngine.UI;

namespace NFramework.ModuleSystem
{
    /// <summary>
    /// Container 上一层，
    /// </summary>
    public class Window : Container
    {
        public Canvas Canvas => this.RectTransform.GetComponent<Canvas>();
        public GraphicRaycaster GraphicRaycaster => this.RectTransform.GetComponent<GraphicRaycaster>();

        /// <summary>与 <see cref="Canvas.sortingOrder"/> 一致，供栈层记录/回收；勿用 <see cref="Canvas.renderOrder"/>（Screen Space Camera 下与 sortingOrder 可能不一致）。</summary>
        public int Order => Canvas.sortingOrder;

        public void Close()
        {
            NFROOT.Instance.GetSystem<UISystem>().Close(this);
        }
    }
}