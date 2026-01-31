using UnityEngine;
using GGJ.Scene;
using GGJ.Manager;

namespace SGC2025.UI
{
    /// <summary>
    /// ButtonのOnClickからシーンを変更するためのラッパークラス
    /// </summary>
    public class SceneChangeButtonHandler : MonoBehaviour
    {
        [SerializeField] private SceneName targetScene = SceneName.Title;

        public void ChangeScene()
        {
            if (SceneController.I == null) return;
            
            // ボタンクリック音を再生
            if (GameManager.I != null)
                GameManager.I.PlayButtonClickSE();
            
            SceneController.I.LoadScene(targetScene);
        }
    }
}