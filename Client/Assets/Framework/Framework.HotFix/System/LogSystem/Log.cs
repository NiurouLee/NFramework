using System;
using System.Text;
using  NFramework.Core;

namespace NFramework.ModuleSystem
{
    public interface ILog
    {
    }

    public class Error
    {
        private StringBuilder _formatBuffer = new StringBuilder(512);
        public void Print(string inMsg)
        {
            _formatBuffer.Clear();
            _formatBuffer.Append(inMsg);
            UnityEngine.Debug.LogError(_formatBuffer.ToString());
        }
        public void Print(string inMsg1, string inMsg2)
        {
            _formatBuffer.Clear();
            _formatBuffer.Append(inMsg1);
            _formatBuffer.Append(inMsg2);
            UnityEngine.Debug.LogError(_formatBuffer.ToString());
        }
        public void Print(string inMsg1, string inMsg2, string inMsg3)
        {
            _formatBuffer.Clear();
            _formatBuffer.Append(inMsg1);
            _formatBuffer.Append(inMsg2);
            _formatBuffer.Append(inMsg3);
            UnityEngine.Debug.LogError(_formatBuffer.ToString());
        }
        public void Print(string inMsg1, string inMsg2, string inMsg3, string inMsg4)
        {
            _formatBuffer.Clear();
            _formatBuffer.Append(inMsg1);
            _formatBuffer.Append(inMsg2);
            _formatBuffer.Append(inMsg3);
            _formatBuffer.Append(inMsg4);
            UnityEngine.Debug.LogError(_formatBuffer.ToString());
        }
        public void Print(string inMsg1, string inMsg2, string inMsg3, string inMsg4, string inMsg5)
        {
            _formatBuffer.Clear();
            _formatBuffer.Append(inMsg1);
            _formatBuffer.Append(inMsg2);
            _formatBuffer.Append(inMsg3);
            _formatBuffer.Append(inMsg4);
            _formatBuffer.Append(inMsg5);
            UnityEngine.Debug.LogError(_formatBuffer.ToString());
        }
    }

    public class Warning
    {
        private StringBuilder _formatBuffer = new StringBuilder(512);
        public void Print(string inMsg)
        {
            _formatBuffer.Clear();
            _formatBuffer.Append(inMsg);
            UnityEngine.Debug.LogWarning(_formatBuffer.ToString());
        }
        public void Print(string inMsg1, string inMsg2)
        {
            _formatBuffer.Clear();
            _formatBuffer.Append(inMsg1);
            _formatBuffer.Append(inMsg2);
            UnityEngine.Debug.LogWarning(_formatBuffer.ToString());
        }
        public void Print(string inMsg1, string inMsg2, string inMsg3)
        {
            _formatBuffer.Clear();
            _formatBuffer.Append(inMsg1);
            _formatBuffer.Append(inMsg2);
            _formatBuffer.Append(inMsg3);
            UnityEngine.Debug.LogWarning(_formatBuffer.ToString());
        }
        public void Print(string inMsg1, string inMsg2, string inMsg3, string inMsg4)
        {
            _formatBuffer.Clear();
            _formatBuffer.Append(inMsg1);
            _formatBuffer.Append(inMsg2);
            _formatBuffer.Append(inMsg3);
            _formatBuffer.Append(inMsg4);
            UnityEngine.Debug.LogWarning(_formatBuffer.ToString());
        }
        public void Print(string inMsg1, string inMsg2, string inMsg3, string inMsg4, string inMsg5)
        {
            _formatBuffer.Clear();
            _formatBuffer.Append(inMsg1);
            _formatBuffer.Append(inMsg2);
            _formatBuffer.Append(inMsg3);
            _formatBuffer.Append(inMsg4);
            _formatBuffer.Append(inMsg5);
        }
    }

    public class Log
    {
        private StringBuilder _formatBuffer = new StringBuilder(512);

        public void Print<T>(string inMsg, T inParam)
        {
            string formattedString = string.Format(inMsg, inParam.ToString());
            UnityEngine.Debug.Log(formattedString);
        }
        public void Print(string inMsg, params object[] inParams)
        {
            string formattedString = string.Format(inMsg, inParams);
            UnityEngine.Debug.Log(formattedString);
        }
        public void Print(string inMsg)
        {
            UnityEngine.Debug.Log(inMsg);
        }
        public void Print(string inMsg1, string inMsg2)
        {
            _formatBuffer.Clear();
            _formatBuffer.Append(inMsg1);
            _formatBuffer.Append(inMsg2);
            UnityEngine.Debug.Log(_formatBuffer.ToString());
        }
        public void Print(string inMsg1, string inMsg2, string inMsg3)
        {
            _formatBuffer.Clear();
            _formatBuffer.Append(inMsg1);
            _formatBuffer.Append(inMsg2);
            _formatBuffer.Append(inMsg3);
            UnityEngine.Debug.Log(_formatBuffer.ToString());
        }
        public void Print(string inMsg1, string inMsg2, string inMsg3, string inMsg4)
        {
            _formatBuffer.Clear();
            _formatBuffer.Append(inMsg1);
            _formatBuffer.Append(inMsg2);
            _formatBuffer.Append(inMsg3);
            _formatBuffer.Append(inMsg4);
            UnityEngine.Debug.Log(_formatBuffer.ToString());
        }
        public void Print(string inMsg1, string inMsg2, string inMsg3, string inMsg4, string inMsg5)
        {
            _formatBuffer.Clear();
            _formatBuffer.Append(inMsg1);
            _formatBuffer.Append(inMsg2);
            _formatBuffer.Append(inMsg3);
            _formatBuffer.Append(inMsg4);
            _formatBuffer.Append(inMsg5);
            UnityEngine.Debug.Log(_formatBuffer.ToString());
        }

    }

    public class LoggerSystem : FrameworkSystemModuleBase
    {
        public Error? Error { get; private set; }
        public Warning? Warning { get; private set; }
        public Log? Log { get; private set; }
        public BitField16 LogLevel = new BitField16(0);

        public override void Awake()
        {
            base.Awake();
            Error = new Error();
            Warning = new Warning();
            Log = new Log();
        }

        public void ErrStack(string inMsg)
        {
            UnityEngine.Debug.LogError(Environment.StackTrace);
        }

    }
}
