using System.IO;
using Main;
using UnityEngine;

public class LogReporter : MonoBehaviour
{
    private void Start()
    {
        DontDestroyOnLoad(this);
        InitLog();
    }

    private void InitLog()
    {
        string folderPath = Path.Combine(Application.persistentDataPath, "ShowLog.txt");
        bool isOpen = File.Exists(folderPath);
        Transform log = transform.Find("Reporter");
        if (isOpen)
        {
            Debug.unityLogger.logEnabled = true;
            log?.gameObject.SetActive(true);
            string logTxt = File.ReadAllText(folderPath);
            if (string.IsNullOrEmpty(logTxt))
            {
                return;
            }

            // var logJson = JsonMapper.ToObject(logTxt);
            // if (logJson.ContainsKey("Level"))
            // {
            //     int level = (int)logJson["Level"];
            //     LogSetting.LogLevel = (LogLevel)level;
            // }
        }
        else
        {
            log?.gameObject.SetActive(LogSetting.IsOpenReporter);
            Debug.unityLogger.logEnabled = LogSetting.IsOpenReporter;
        }
    }
}