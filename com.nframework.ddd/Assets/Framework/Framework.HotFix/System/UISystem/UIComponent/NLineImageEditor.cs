
#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NFramework.ModuleSystem
{
    public class LineImageOfEditor : Image
    {
        public List<Transform> Pointer = new List<Transform>();
        private float radio = 6;
        private const float dottedLength = 12;
        private const float invisibleLength = 20;
        private int halfScreenWidth;
        private int halfScreenHeight;
        private bool isDotted = true;

        protected override void OnEnable()
        {
            base.OnEnable();
            halfScreenHeight = Screen.height / 2;
            halfScreenWidth = Screen.width / 2;
        }

        protected override void OnPopulateMesh(VertexHelper toFill)
        {
            toFill.Clear();
            if (Pointer.Count >= 2)
            {
                for (int i = 1; i < Pointer.Count; i += 2)
                {
                    if (isDotted)
                        DrawDotted(toFill, Pointer[i - 1].position, Pointer[i].position);
                    else
                        Draw(toFill, Pointer[i - 1].position, Pointer[i].position);
                }
            }
        }

        private void DrawDotted(VertexHelper toFill, Vector2 pos1, Vector2 pos2)
        {
            var width = Mathf.Abs(pos1.x - pos2.x);
            var height = Mathf.Abs(pos1.y - pos2.y);
            var length = Mathf.Sqrt(Mathf.Pow(height, 2) + Mathf.Pow(width, 2));
            var cos = width / length;
            var sin = height / length;
            Vector2 startPoint = pos1;
            int isInvisible = 1;
            var xk = 1;
            var yk = 1;
            if (pos1.x > pos2.x)
                xk *= -1;
            if (pos1.y > pos2.y)
                yk *= -1;
            float tempLength = dottedLength;
            for (float i = 0; i < length;)
            {

                var xWidth = cos * tempLength * xk;
                var yHeight = sin * tempLength * yk;
                var endPoint = new Vector2(startPoint.x + xWidth, startPoint.y + yHeight);
                if (isInvisible == 1)
                {
                    Draw(toFill, startPoint, endPoint);
                    tempLength = invisibleLength;
                }
                else
                {
                    tempLength = dottedLength;
                }
                i += tempLength;
                startPoint = endPoint;
                isInvisible *= -1;
            }
        }

        public void ClearPointer()
        {
            Pointer.Clear();
            ReDraw();
        }

        public void AddPointer(Transform start, Transform end)
        {
            Pointer.Add(start);
            Pointer.Add(end);
            ReDraw();
        }

        public void RemovePointer(Transform start, Transform end)
        {
            for (int i = 0; i < Pointer.Count; i++)
            {
                var pointer = Pointer[i];
                if (pointer == start)
                {
                    if (i + 1 >= Pointer.Count)
                    {
                        Debug.LogWarning("节点数据错误");
                        return;
                    }
                    if (Pointer[i + 1] == end)
                    {
                        Pointer.RemoveAt(i + 1);
                        Pointer.RemoveAt(i);
                        return;
                    }
                }
            }
            ReDraw();
        }

        public void EditorReDraw()
        {
            SetVerticesDirty();
        }

        private void ReDraw()
        {
            SetVerticesDirty();
        }

        private List<Vector2> tempPointer = new List<Vector2>();

        void Draw(VertexHelper vh, Vector2 start, Vector2 end)
        {
            tempPointer.Clear();
            Vector2 to = end - start;
            Vector2 nor_to = to.normalized * radio + start;
            //改变了起点的位置，以遮挡拐点的缺角
            Vector2 newStart = RotateVector2(180, nor_to, start);
            Vector2 up = RotateVector2(90, start, newStart);
            Vector2 down = RotateVector2(-90, start, newStart);
            /* 
            Vector2 up = RotateVector2(90, nor_to, start);
            Vector2 down = RotateVector2(-90, nor_to, start);  
            */ //拐点有缺角
            Vector2 up_end = up + to;
            Vector2 down_end = down + to;
            AddQuad(vh, up, down, up_end, down_end);
        }

        private static Vector2 RotateVector2(float angle, Vector2 target, Vector2 org)
        {
            float tx, ty;
            tx = target.x - org.x;
            ty = target.y - org.y;
            float thata = Mathf.Deg2Rad * angle;
            float x = tx * Mathf.Cos(thata) - ty * Mathf.Sin(thata) + org.x;
            float y = tx * Mathf.Sin(thata) + ty * Mathf.Cos(thata) + org.y;
            return new Vector2(x, y);
        }


        void AddQuad(VertexHelper vh, Vector2 pos1, Vector2 pos2, Vector2 pos3, Vector2 pos4)
        {
            AddQuad(vh, CreateEmptyVertex(pos1), CreateEmptyVertex(pos2), CreateEmptyVertex(pos3), CreateEmptyVertex(pos4));
        }

        void AddQuad(VertexHelper vh, UIVertex v1, UIVertex v2, UIVertex v3, UIVertex v4)
        {
            int index = vh.currentVertCount;
            vh.AddVert(v1);
            vh.AddVert(v2);
            vh.AddVert(v3);
            vh.AddVert(v4);
            vh.AddTriangle(index, index + 1, index + 2);
            vh.AddTriangle(index + 2, index + 3, index + 1);
        }

        UIVertex CreateEmptyVertex(Vector2 pos)
        {
            UIVertex v = new UIVertex();
            v.position = pos;
            v.color = color;
            v.uv0 = Vector2.zero;
            return v;
        }
    }
}
#endif