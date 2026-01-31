using System.Collections;
using UnityEngine;
using GGJ.Core;
using GGJ.Manager;
using GGJ.InGame.Audio;
using UnityEngine.SceneManagement;

namespace GGJ.Scene
{
    /// <summary>
    /// ゲーム内のシーン名を定義するenum
    /// </summary>
    public enum SceneName
    {
        Title,
        InGame,
        GoodEnd,
        BadEnd,
    }

    /// <summary>
    /// シーン遷移を管理するSingletonクラス
    /// Titleシーンで一度生成されれば、他のシーンでも利用可能
    /// </summary>
    public class SceneController : Singleton<SceneController>
    {
        protected override bool UseDontDestroyOnLoad => true;

        public SceneName CurrentStage { get; private set; } = SceneName.Title;
        
        /// <summary>
        /// シーン読み込み中かどうか
        /// </summary>
        public bool IsLoading { get; private set; } = false;

        /// <summary>
        /// 指定されたシーンに非同期で切り替え
        /// </summary>
        public void LoadScene(SceneName sceneName)
        {
            if (IsLoading) return;
            
            StartCoroutine(LoadSceneAsync(sceneName));
        }
        
        /// <summary>
        /// 非同期でシーンを読み込む
        /// </summary>
        private IEnumerator LoadSceneAsync(SceneName sceneName)
        {
            IsLoading = true;
            CurrentStage = sceneName;
            
            // 適切なBGMを再生
            PlaySceneBGM(sceneName);
            
            // 非同期読み込み開始
            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName.ToString());
            operation.allowSceneActivation = false;
            
            // 読み込み進捗を監視
            while (!operation.isDone)
            {                
                // 読み込み完了待ち
                if (operation.progress >= 0.9f)
                {
                    // シーン有効化
                    operation.allowSceneActivation = true;
                }
                
                yield return null;
            }
            
            IsLoading = false;
        }
        
        /// <summary>
        /// シーンに応じたBGMを再生
        /// </summary>
        private void PlaySceneBGM(SceneName sceneName)
        {
            if (AudioManager.I == null) return;
            
            switch (sceneName)
            {
                case SceneName.Title:
                    AudioManager.I.PlayBGM(BGMType.Title, true);
                    break;
                case SceneName.GoodEnd:
                    AudioManager.I.PlayBGM(BGMType.GoodEnd, true);
                    break;
                case SceneName.BadEnd:
                    AudioManager.I.PlayBGM(BGMType.BadEnd, true);
                    break;
                // InGameはGameManager.StartGame()で再生される
            }
        }
    }
}
