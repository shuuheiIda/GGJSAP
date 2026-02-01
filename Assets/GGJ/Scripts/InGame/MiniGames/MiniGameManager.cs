using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GGJ {
    public class MiniGameManager : MonoBehaviour {
        /// <summary>
        /// ミニゲームの列挙型
        /// </summary>
        public enum Minigames {
            None,
            Concentration,
            Snake,
        }

        /// <summary>
        /// 現在のミニゲーム
        /// </summary>
        private Minigames currentMinigame = Minigames.None;

        /// <summary>
        /// ミニゲームリスト
        /// </summary>
        [SerializeField] private List<GameObject> miniGameList;

        private void Awake() {

        }

        private void Start() {
            StarMiniGame(Minigames.Snake);
        }

        private void FixedUpdate() {

        }

        private void Update() {

        }

        /// <summary>
        /// ミニゲームを始める
        /// </summary>
        public void StarMiniGame(Minigames game) {
            if (currentMinigame != Minigames.None) {
                return;
            }

            // SetActiveをtrueにして起動
            Debug.Log(miniGameList.FirstOrDefault(_ => _.gameObject.name == game.ToString()));
            miniGameList.FirstOrDefault(_ => _.gameObject.name == game.ToString()).SetActive(true);
        }
    }   
}
