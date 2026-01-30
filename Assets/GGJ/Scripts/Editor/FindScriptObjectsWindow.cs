using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace GGJ.Editor
{
    /// <summary>
    /// 特定のスクリプトがアタッチされたオブジェクトを検索するエディタウィンドウ
    /// </summary>
    public class FindScriptObjectsWindow : EditorWindow
    {
        private const float RESULT_LIST_SPACING = 10f;

        private MonoScript targetScript;
        private List<GameObject> foundObjects = new List<GameObject>();

        /// <summary>
        /// ウィンドウを開く
        /// </summary>
        [MenuItem("Tools/Find Script Objects")]
        public static void OpenWindow() =>
            GetWindow<FindScriptObjectsWindow>("Find Script Objects");

        private void OnGUI()
        {
            GUILayout.Label("Find Objects with Script", EditorStyles.boldLabel);

            targetScript = (MonoScript)EditorGUILayout.ObjectField("Script", targetScript, typeof(MonoScript), false);

            if (GUILayout.Button("Find"))
            {
                if (targetScript != null)
                    FindObjectsWithScript();
                else
                    Debug.LogWarning("Please select a script to search for.");
            }

            GUILayout.Space(RESULT_LIST_SPACING);

            if (foundObjects.Count > 0)
            {
                GUILayout.Label($"Found {foundObjects.Count} objects:", EditorStyles.boldLabel);

                foreach (GameObject obj in foundObjects)
                {
                    if (GUILayout.Button(obj.name))
                        Selection.activeGameObject = obj;
                }
            }
        }

        private void FindObjectsWithScript()
        {
            foundObjects.Clear();

            System.Type scriptType = targetScript.GetClass();
            if (scriptType == null || !typeof(MonoBehaviour).IsAssignableFrom(scriptType))
            {
                Debug.LogError("Selected script is not a MonoBehaviour.");
                return;
            }

            GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);

            foreach (GameObject obj in allObjects)
            {
                if (obj.GetComponent(scriptType) != null)
                    foundObjects.Add(obj);
            }
        }
    }
}
