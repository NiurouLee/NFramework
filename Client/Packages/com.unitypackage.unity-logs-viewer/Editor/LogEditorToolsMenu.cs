using UnityEngine;
using UnityEditor;

public partial class LogEditorToolsMenu
{
    [MenuItem("Tools/Open PersistentDataPath", false)]
    public static void OpenPersistentDirectory()
    {
        ExplorerFolder(Application.persistentDataPath);
    }

    public static void ExplorerFolder(string folder, bool isSelect = false)
    {
        folder = string.Format("\"{0}\"", folder);
        switch (Application.platform)
        {
            case RuntimePlatform.WindowsEditor:
                var str = folder.Replace('/', '\\');
                if (isSelect)
                {
                    str = "/select," + str;
                }

                System.Diagnostics.Process.Start("Explorer.exe", str);
                break;
            case RuntimePlatform.OSXEditor:
                System.Diagnostics.Process.Start("open", folder);
                break;
            default:
                EditorUtility.RevealInFinder(folder);
                //Debug.LogError(string.Format("Not support open folder on '{0}' platform.", Application.platform.ToString()));
                break;
        }
    }
}