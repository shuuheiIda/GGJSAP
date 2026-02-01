using UnityEngine;
using UnityEngine.SceneManagement;

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
            Debug.Log("[ManagerInitializer] Awake開始");
            Debug.Log($"[ManagerInitializer] このオブジェクト: {gameObject.name}");
            
            // Awakeで即座にDontDestroyOnLoadを設定
            // Start()だと他のスクリプトのStart()が先に実行されてシーン遷移が始まる可能性がある
            SetupDontDestroyOnLoad();
        }
        
        /// <summary>
        /// Managersシーン内の全てのルートオブジェクトをDontDestroyOnLoadに設定
        /// </summary>
        private void SetupDontDestroyOnLoad()
        {
            // 現在のシーン（Managers）を取得
            UnityEngine.SceneManagement.Scene managersScene = gameObject.scene;
            
            // シーン内の全てのルートGameObjectを取得
            GameObject[] rootObjects = managersScene.GetRootGameObjects();
            
            Debug.Log($"[ManagerInitializer] Managersシーン内のルートオブジェクト数: {rootObjects.Length}");
            
            // 各ルートオブジェクトをDontDestroyOnLoadに設定
            foreach (GameObject rootObj in rootObjects)
            {
                if (rootObj.transform.parent == null) // 念のため親がないことを確認
                {
                    DontDestroyOnLoad(rootObj);
                    Debug.Log($"[ManagerInitializer] DontDestroyOnLoadに設定: {rootObj.name}");
                }
            }
        }
    }
}

