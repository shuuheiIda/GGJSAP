using UnityEngine;

namespace GGJ.Core
{
    /// <summary>
    /// Managersシーンに配置して、シーンがロードされたことを確認するためのスクリプト
    /// また、Managersシーン内の全てのルートオブジェクトをDontDestroyOnLoadに設定します
    /// デバッグ用：Managersシーンがロードされているか確認できます
    /// </summary>
    public class ManagerInitializer : MonoBehaviour
    {
        private void Awake()
        {
            SetupDontDestroyOnLoad();
        }


        /// <summary>
        /// Managersシーン内の全てのルートオブジェクトをDontDestroyOnLoadに設定
        /// </summary>
        private void SetupDontDestroyOnLoad()
        {
            UnityEngine.SceneManagement.Scene managersScene = gameObject.scene;
            GameObject[] rootObjects = managersScene.GetRootGameObjects();

            foreach (GameObject rootObj in rootObjects)
            {
                if (rootObj.transform.parent == null)
                    DontDestroyOnLoad(rootObj);
            }
        }
    }
}

