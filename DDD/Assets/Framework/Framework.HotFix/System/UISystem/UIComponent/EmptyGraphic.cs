using UnityEngine;
using UnityEngine.UI;
namespace Game.Logic
{
    /// <summary>
    /// 只响应事件，不绘制
    /// </summary>
    [RequireComponent(typeof(CanvasRenderer))]
    public class EmptyGraphic : Graphic, ICanvasRaycastFilter
    {
        public override void SetMaterialDirty() { return; }
        public override void SetVerticesDirty() { return; }

        protected override void Awake()
        {
            base.Awake();
            color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        }

        /// Probably not necessary since the chain of calls `Rebuild()`->`UpdateGeometry()`->`DoMeshGeneration()`->`OnPopulateMesh()` won't happen; so here really just as a fail-safe.
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            return;
        }
        public virtual bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
        {
            return true;
        }
    }
}

