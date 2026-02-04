using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GGJ.Core;
using GGJ.InGame.Events;
using GGJ.InGame.Transitions;

namespace GGJ.InGame.MiniGames
{
    /// <summary>
    /// ミニゲーム管理クラス
    /// - ランダムなミニゲーム選択
    /// - メインゲームとミニゲームのUI切り替え
    /// - ミニゲームクリア時のヒント獲得処理
    /// </summary>
    public class MiniGameManager : Singleton<MiniGameManager>
    {
        [Header("ミニゲーム設定")]
        [SerializeField] private List<GameObject> miniGameObjects = new List<GameObject>();
        
        [Header("UI切り替え")]
        [SerializeField] private GameObject mainGameUI; // 犯人探しのUI
        [SerializeField] private GameObject mainCamera; // メインのカメラ
        [SerializeField] private GameObject miniCamera; // ミニゲーム用のカメラ
        [SerializeField] private GameObject miniGameCanvas; // ミニゲーム専用のCanvas
        
        private IMiniGame currentMiniGame = null;
        private List<IMiniGame> availableMiniGames = new List<IMiniGame>();
        private bool isMiniGameActive = false;

        protected override bool UseDontDestroyOnLoad => false;

        protected override void Init()
        {
            // 全ミニゲームオブジェクトからIMiniGameコンポーネントを取得
            foreach (var gameObj in miniGameObjects)
            {
                var miniGame = gameObj.GetComponent<IMiniGame>();
                if (miniGame != null)
                {
                    availableMiniGames.Add(miniGame);
                    gameObj.SetActive(false); // 初期状態は非表示
                }
                else
                {
                    Debug.LogWarning($"[MiniGameManager] {gameObj.name} には IMiniGame が実装されていません");
                }
            }

            // 利用可能なミニゲームがない場合は警告
            if (availableMiniGames.Count == 0)
            {
                Debug.LogError("[MiniGameManager] 利用可能なミニゲームがありません！");
            }
        }

        /// <summary>
        /// ランダムなミニゲームを開始する（NPC会話から呼ばれる想定）
        /// </summary>
        public void StartRandomMiniGame()
        {
            if (isMiniGameActive)
            {
                Debug.LogWarning("[MiniGameManager] すでにミニゲームが実行中です");
                return;
            }

            if (availableMiniGames.Count == 0)
            {
                Debug.LogError("[MiniGameManager] 開始できるミニゲームがありません");
                return;
            }

            // ランダムにミニゲームを選択
            int randomIndex = UnityEngine.Random.Range(0, availableMiniGames.Count);
            currentMiniGame = availableMiniGames[randomIndex];

            // トランジション付きでUI切り替え
            if (GridTransitionManager.I != null)
            {
                GridTransitionManager.I.PlayTransition(() =>
                {
                    SwitchToMiniGame();
                    StartCurrentMiniGame();
                });
            }
            else
            {
                // トランジションが無い場合は従来通り
                SwitchToMiniGame();
                StartCurrentMiniGame();
            }

            isMiniGameActive = true;
        }

        /// <summary>
        /// 選択されたミニゲームを開始
        /// </summary>
        private void StartCurrentMiniGame()
        {
            // ミニゲーム開始
            currentMiniGame.RegisterOnClearCallback(OnMiniGameCleared);
            currentMiniGame.StartMiniGame();
        }

        /// <summary>
        /// ミニゲームクリア時のコールバック
        /// </summary>
        private void OnMiniGameCleared()
        {
            // メインゲームに戻る（ヒント表示はメインゲーム復帰後に行う）
            StartCoroutine(ReturnToMainGameAfterDelay(1.5f));
        }

        /// <summary>
        /// 遅延してメインゲームに戻る
        /// </summary>
        private IEnumerator ReturnToMainGameAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            ReturnToMainGame();
            
            // MainGameUI復帰後にヒント獲得イベントを発火（DialoguePanelがアクティブになった後）
            yield return null; // 1フレーム待つ
            GameEvents.RaiseHintReceived();
        }

        /// <summary>
        /// メインゲーム（犯人探し）に戻る
        /// </summary>
        public void ReturnToMainGame()
        {
            // トランジション付きでUI切り替え
            if (GridTransitionManager.I != null)
            {
                GridTransitionManager.I.PlayTransition(() =>
                {
                    if (currentMiniGame != null)
                    {
                        currentMiniGame.StopMiniGame();
                        currentMiniGame = null;
                    }

                    // UI切り替え（MainGameUIを表示すると会話パネルも自動的に表示される）
                    SwitchToMainGame();

                    isMiniGameActive = false;
                });
            }
            else
            {
                // トランジションが無い場合は従来通り
                if (currentMiniGame != null)
                {
                    currentMiniGame.StopMiniGame();
                    currentMiniGame = null;
                }

                // UI切り替え（MainGameUIを表示すると会話パネルも自動的に表示される）
                SwitchToMainGame();

                isMiniGameActive = false;
            }
        }

        /// <summary>
        /// ミニゲーム用UIに切り替え
        /// </summary>
        private void SwitchToMiniGame()
        {
            // 先に無効化（重複を防ぐ）
            if (mainGameUI != null)
                mainGameUI.SetActive(false);
            
            if (mainCamera != null)
                mainCamera.SetActive(false);
            
            // その後有効化
            if (miniCamera != null)
                miniCamera.SetActive(true);
            
            if (miniGameCanvas != null)
                miniGameCanvas.SetActive(true);
        }

        /// <summary>
        /// メインゲーム用UIに切り替え
        /// </summary>
        private void SwitchToMainGame()
        {
            // 先に無効化（重複を防ぐ）
            if (miniCamera != null)
                miniCamera.SetActive(false);
            
            if (miniGameCanvas != null)
                miniGameCanvas.SetActive(false);
            
            // その後有効化
            if (mainCamera != null)
                mainCamera.SetActive(true);
            
            if (mainGameUI != null)
                mainGameUI.SetActive(true);
        }
    }
}
