using UnityEngine;
using UnityEngine.SceneManagement;

namespace GGJ.Core
{
    /// <summary>
    /// どのシーンから開始しても必要なManagerシーンを自動的にロードするBootstrap
    /// InGameシーンから直接テストする場合でも、Managerが正しく初期化されます
    /// 
    /// 【重要】Build Settingsに"Managers"シーンを追加してください
    /// </summary>
    public class BootstrapLoader
    {
        /// <summary>
        /// Managerシーンの名前
        /// </summary>
        private const string MANAGER_SCENE_NAME = "Managers";
        
        /// <summary>
        /// Bootstrapが既に実行されたかどうか
        /// </summary>
        private static bool isBootstrapped = false;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        {
            if (isBootstrapped)
            {
                Debug.Log("[Bootstrap] 既に初期化済みです");
                return;
            }
            
            isBootstrapped = true;
            
            // Managerシーンが既にロードされているかチェック
            bool managerSceneLoaded = IsSceneLoaded(MANAGER_SCENE_NAME);
            
            if (managerSceneLoaded)
            {
                Debug.Log($"[Bootstrap] {MANAGER_SCENE_NAME}シーンは既にロードされています");
                return;
            }
            
            // Managerシーンがロードされていない場合、Additiveでロード
            
            try
            {
                SceneManager.LoadScene(MANAGER_SCENE_NAME, LoadSceneMode.Additive);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[Bootstrap] {MANAGER_SCENE_NAME}シーンのロードに失敗しました: {e.Message}");
                Debug.LogError($"[Bootstrap] Build Settingsに'{MANAGER_SCENE_NAME}'シーンが追加されているか確認してください");
            }
        }
        
        /// <summary>
        /// 指定されたシーンがロードされているかチェック
        /// </summary>
        private static bool IsSceneLoaded(string sceneName)
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                UnityEngine.SceneManagement.Scene scene = SceneManager.GetSceneAt(i);
                if (scene.name == sceneName && scene.isLoaded)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
