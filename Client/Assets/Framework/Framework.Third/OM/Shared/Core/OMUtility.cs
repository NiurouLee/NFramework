using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Object = UnityEngine.Object;

namespace OM
{
    public static partial class OMUtility
    {
        public static string FormatNumberWithSuffix(this long t)
        {
            var amount = (double)t;
            string result;
            var scoreNames = new string[] {"", "k","M", "B", "T", "aa", "ab", "ac", "ad", "ae", "af", "ag", "ah", "ai", "aj", "ak", "al", "am", "an", "ao", "ap", "aq", "ar", "as", "at", "au", "av", "aw", "ax", "ay", "az", "ba", "bb", "bc", "bd", "be", "bf", "bg", "bh", "bi", "bj", "bk", "bl", "bm", "bn", "bo", "bp", "bq", "br", "bs", "bt", "bu", "bv", "bw", "bx", "by", "bz", };
            int i;
 
            for (i = 0; i < scoreNames.Length; i++)
                if (amount <= 999)
                    break;
                else amount = Math.Floor(amount / 100f) / 10f;
            
            if (Math.Abs(amount - Math.Floor(amount)) < .1f)
                result = amount.ToString() + scoreNames[i];
            else result = amount.ToString("F1") + scoreNames[i];
            return result;
        }
        
        public static Vector3 GetMouseWorldPositionOnPlane(Vector3 screenPosition,Camera currentCamera,Vector3 normal,Vector3 planePos)
        {
            var ray = currentCamera.ScreenPointToRay(screenPosition);
            var xy = new Plane(screenPosition, planePos);
            xy.Raycast(ray, out var distance);
            return ray.GetPoint(distance) ;
        }
        
        public static Vector3 GetWorldPositionOnPlaneForward(Vector3 screenPosition,Camera currentCamera)
        {
            var ray = currentCamera.ScreenPointToRay(screenPosition);
            var xy = new Plane(Vector3.forward, new Vector3(0, 0, 0));
            xy.Raycast(ray, out var distance);
            return ray.GetPoint(distance) ;
        }
        
        public static Vector3 GetWorldPositionOnPlaneDown(Vector3 screenPosition,Camera currentCamera)
        {
            var ray = currentCamera.ScreenPointToRay(screenPosition);
            var xy = new Plane(Vector3.down, new Vector3(0, 0, 0));
            xy.Raycast(ray, out var distance);
            return ray.GetPoint(distance) ;
        }
        
        public static bool TryGetObjectUnderMouse2D<T>(Vector3 mousePosition,Camera cam,LayerMask layerMask, out T result,float radius = .2f) where T : Component
        {
            var overlapCircle = Physics2D.OverlapCircle(GetWorldPositionOnPlaneForward(mousePosition,cam),radius,layerMask);
            if (overlapCircle != null)
            {
                result = overlapCircle.GetComponentInParent<T>();
                return result != null;
            }
            result = null;
            return false;
        }
        
        public static bool TryGetObjectUnderMouse3D<T>(Vector3 mousePosition,Camera cam,LayerMask layerMask, out T result,float maxDistance = 100,float radius = .2f) where T : Component
        {
            var ray = cam.ScreenPointToRay(mousePosition);
            if (Physics.SphereCast(ray,radius,out var hit,maxDistance,layerMask))
            {
                result = hit.transform.GetComponentInParent<T>();
                return result != null;
            }
            result = null;
            return false;
        }
        
        public static Vector3 GetCenterOfChildren(this Transform transform)
        {
            var center = Vector3.zero;
            var childCount = transform.childCount;
            for (var i = 0; i < childCount; i++)
            {
                center += transform.GetChild(i).position;
            }
            return center / childCount;
        }
        
        public static void DestroyChildren(this Transform transform)
        {
            var childCount = transform.childCount;
            for (var i = childCount - 1; i >= 0; i--)
            {
                Object.Destroy(transform.GetChild(i).gameObject);
            }
        }

        public static Vector2 GetScreenAnchor(OMAnchor anchor = OMAnchor.Center)
        {
            switch (anchor)
            {
                case OMAnchor.Top:
                    return new Vector2(Screen.width / 2f, Screen.height);
                case OMAnchor.Bottom:
                    return new Vector2(Screen.width / 2f, 0);
                case OMAnchor.Right:
                    return new Vector2(Screen.width, Screen.height / 2f);
                case OMAnchor.Left:
                    return new Vector2(0, Screen.height / 2f);
                case OMAnchor.TopRight:
                    return new Vector2(Screen.width, Screen.height);
                case OMAnchor.TopLeft:
                    return new Vector2(0, Screen.height);
                case OMAnchor.BottomRight:
                    return new Vector2(Screen.width, 0);
                case OMAnchor.BottomLeft:
                    return Vector2.zero;
                case OMAnchor.Center:
                default:
                    return new Vector2(Screen.width / 2f, Screen.height/2f);
            }
        }
        
        public static Vector2 GetScreenAnchor(Vector2 anchor)
        {
            return new Vector2(Screen.width * anchor.x, Screen.height * anchor.y);
        }
        
        public static void KillAfter(this Object target,float duration)
        {
            Object.Destroy(target,duration);
        }

        public static void After(this Object target, float duration, Action callback, bool unscaledTime = false)
        {
            OMTimer.Create(duration,callback, unscaledTime);
        }
        
        public static Vector3[] GetCirclePoints2D(Vector3 center, float radius, int pointsCount)
        {
            var points = new Vector3[pointsCount];
            var slice = 2 * Mathf.PI / pointsCount;
            for (var i = 0; i < pointsCount; i++)
            {
                var angle = slice * i;
                var newX = (float)(center.x + radius * Math.Cos(angle));
                var newY = (float)(center.y + radius * Math.Sin(angle));
                points[i] = new Vector3(newX, newY, center.z);
            }
            return points;
        }
        
        public static Vector3[] GetCirclePoints3D(Vector3 center, float radius, int pointsCount)
        {
            var points = new Vector3[pointsCount];
            var slice = 2 * Mathf.PI / pointsCount;
            for (var i = 0; i < pointsCount; i++)
            {
                var angle = slice * i;
                var newX = (float)(center.x + radius * Math.Cos(angle));
                var newZ = (float)(center.z + radius * Math.Sin(angle));
                points[i] = new Vector3(newX, center.y, newZ);
            }
            return points;
        }
        
        public static Vector3[] GetCirclePoints2DWithAngle(Vector3 center, float radius, int pointsCount, float angle)
        {
            var points = new Vector3[pointsCount];
            var slice = 2 * Mathf.PI / pointsCount;
            for (var i = 0; i < pointsCount; i++)
            {
                var newAngle = slice * i + angle;
                var newX = (float)(center.x + radius * Math.Cos(newAngle));
                var newY = (float)(center.y + radius * Math.Sin(newAngle));
                points[i] = new Vector3(newX, newY, center.z);
            }
            return points;
        }

        public static IEnumerable<Vector3> EvaluatePointsBox(Vector3 center,int width,int depth,float spread,float noise,bool hollow)
        {
            var middleOffset = new Vector3(width * .5f,0, depth * .5f);
            
            for (var x = 0; x < width; x++)
            {
                for (var z = 0; z < depth; z++)
                {
                    if(hollow && (x != 0 && x != width - 1 && z != 0 && z != depth - 1)) continue;
                    var pos = new Vector3(x, 0, z);
                    pos -= middleOffset;
                    pos += center;
                    pos *= spread;
                    pos += GetNoiseAt(pos,noise);
                    yield return pos;
                }
            }
        }

        public static float GetNoise(Vector2 noise)
        {
            return Mathf.PerlinNoise(noise.x, noise.y);
        }
        
        public static Vector3 GetNoiseAt(Vector3 pos,float noise)
        {
            var f = Mathf.PerlinNoise(pos.x * noise,pos.z * noise);
            return new Vector3(f, 0, f);
        }

        public static bool IsOverUI()
        {
            return EventSystem.current.IsPointerOverGameObject();
        }
    }
}