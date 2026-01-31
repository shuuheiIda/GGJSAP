using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GGJ.Core;
using GGJ.InGame.Events;

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
        [SerializeField] private Canvas miniGameCanvas; // ミニゲーム用Canvas（任意）
        
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

            // UI切り替え
            SwitchToMiniGame();

            // ミニゲーム開始
            currentMiniGame.RegisterOnClearCallback(OnMiniGameCleared);
            currentMiniGame.StartMiniGame();

            isMiniGameActive = true;

            Debug.Log($"[MiniGameManager] ミニゲーム開始: {currentMiniGame.GetType().Name}");
        }

        /// <summary>
        /// 特定のミニゲームを開始する（デバッグ・テスト用）
        /// </summary>
        public void StartSpecificMiniGame(int index)
        {
            if (index < 0 || index >= availableMiniGames.Count)
            {
                Debug.LogError($"[MiniGameManager] 無効なインデックス: {index}");
                return;
            }

            if (isMiniGameActive)
            {
                Debug.LogWarning("[MiniGameManager] すでにミニゲームが実行中です");
                return;
            }

            currentMiniGame = availableMiniGames[index];

            // UI切り替え
            SwitchToMiniGame();

            // ミニゲーム開始
            currentMiniGame.RegisterOnClearCallback(OnMiniGameCleared);
            currentMiniGame.StartMiniGame();

            isMiniGameActive = true;

            Debug.Log($"[MiniGameManager] ミニゲーム開始: {currentMiniGame.GetType().Name}");
        }

        /// <summary>
        /// ミニゲームクリア時のコールバック
        /// </summary>
        private void OnMiniGameCleared()
        {
            Debug.Log("[MiniGameManager] ミニゲームクリア！ヒントを獲得しました");

            // ヒント獲得イベントを発火
            GameEvents.RaiseHintReceived();

            // メインゲームに戻る
            StartCoroutine(ReturnToMainGameAfterDelay(1.5f));
        }

        /// <summary>
        /// 遅延してメインゲームに戻る
        /// </summary>
        private IEnumerator ReturnToMainGameAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            ReturnToMainGame();
        }

        /// <summary>
        /// メインゲーム（犯人探し）に戻る
        /// </summary>
        public void ReturnToMainGame()
        {
            if (currentMiniGame != null)
            {
                currentMiniGame.StopMiniGame();
                currentMiniGame = null;
            }

            // UI切り替え（MainGameUIを表示すると会話パネルも自動的に表示される）
            SwitchToMainGame();

            isMiniGameActive = false;

            Debug.Log("[MiniGameManager] メインゲームに戻りました");
        }

        /// <summary>
        /// ミニゲーム用UIに切り替え
        /// </summary>
        private void SwitchToMiniGame()
        {
            if (mainGameUI != null)
                mainGameUI.SetActive(false);

            if (miniGameCanvas != null)
                miniGameCanvas.enabled = true;
        }

        /// <summary>
        /// メインゲーム用UIに切り替え
        /// </summary>
        private void SwitchToMainGame()
        {
            if (mainGameUI != null)
                mainGameUI.SetActive(true);

            if (miniGameCanvas != null)
                miniGameCanvas.enabled = false;
        }

        /// <summary>
        /// 現在ミニゲームがアクティブかどうか
        /// </summary>
        public bool IsMiniGameActive() => isMiniGameActive;

        /// <summary>
        /// 利用可能なミニゲーム数を取得
        /// </summary>
        public int GetAvailableMiniGameCount() => availableMiniGames.Count;
    }
}
