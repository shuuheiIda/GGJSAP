using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GGJ.InGame.MiniGames {
    public class ConcentrationManager : MiniGameBase {
        /// <summary>
        /// カードの数 ※変える時は偶数で
        /// </summary>
        private const int CardNum = 16;

        /// <summary>
        /// コントローラーを使用しているか
        /// </summary>
        private bool IsUseControllerDevice = false;

        /// <summary>
        /// コントローラーのスティック感度
        /// </summary>
        private const float ControllerStickSens = 10.0f;


        /// <summary>
        /// カーソルのプレハブ
        /// </summary>
        [SerializeField] private GameObject cursorPrefab;
        /// <summary>
        /// カーソルのインスタンス
        /// </summary>
        private GameObject cursor;

        /// <summary>
        /// カードを生成する親Transform
        /// </summary>
        [SerializeField] private Transform cardParent;

        /// <summary>
        /// カードのプレハブ
        /// </summary>
        [SerializeField] private GameObject cardPrefab;

        /// <summary>
        /// カードのリスト
        /// </summary>
        public List<CardController> cardList = new List<CardController>();

        /// <summary>
        /// カードを開けれるか
        /// </summary>
        private bool canOpenCard = true;

        /// <summary>
        /// 最初に開いたカード
        /// </summary>
        private CardController firstCard;

        /// <summary>
        /// 移動に使うCTS
        /// </summary>
        private CancellationTokenSource CTS = new CancellationTokenSource();

        private void OnEnable() {
            // Debug.Log("OnEnable");

            // 初期化
            IsUseControllerDevice = false;
            cardList = new List<CardController>();
            canOpenCard = true;
            CTS = new CancellationTokenSource();

            CreateCard();

            // カーソルがなかったら生成
            if (cursor == null) {
                cursor = Instantiate(cursorPrefab, Vector2.zero, Quaternion.identity, this.transform);
            }

            inputActions.Enable();
        }

        private void OnDisable() {
            // Debug.Log("OnDisable");

            // カードを削除
            foreach (var card in cardList) {
                if (card) {
                    Destroy(card.gameObject);
                }
            }

            // カーソルの削除
            Destroy(cursor);

            inputActions.Disable();
            CTS.Cancel();
        }

        private void Awake() {
            if (CardNum % 2 != 0) {
                // Debug.LogWarning("カードの数が偶数ではありません。");
            }

            inputActions = new PlayerInput();
        }

        protected override async void Update() {
            base.Update();
            CursorControl();
            try {
                await OpenCard(CTS.Token);
            }
            catch {
                return;
            }
        }

        /// <summary>
        /// デバイス切替
        /// </summary>
        private void SwitchDevice() {
            Vector2 cont = Vector2.zero;
            if (isConnectiongController && Gamepad.current != null) {
                cont = Gamepad.current.leftStick.ReadValue();

            }

            if (Mathf.Abs(cont.x) > 0.1f ||
                Mathf.Abs(cont.y) > 0.1f) {
                IsUseControllerDevice = true;
            }
            else if (Input.GetMouseButtonDown(0) ||
                     Input.GetMouseButtonDown(1)) {
                IsUseControllerDevice = false;
            }
        }

        /// <summary>
        /// カードを作成しフィールドに保存
        /// </summary>
        private void CreateCard() {
            for (int i = 0; i < CardNum; i++) {
                // ランダムな位置に
                float rndPosX = UnityEngine.Random.Range(-MaxWindowSizeX + cardPrefab.transform.lossyScale.x / 2, MaxWindowSizeX - cardPrefab.transform.lossyScale.x / 2);
                float rndPosY = UnityEngine.Random.Range(-MaxWindowSizeY + cardPrefab.transform.lossyScale.y / 2, MaxWindowSizeY - cardPrefab.transform.lossyScale.y / 2);

                Vector2 createPos = new Vector2(rndPosX, rndPosY);

                // 生成し保存
                GameObject createdCard = Instantiate(cardPrefab, createPos, Quaternion.identity, cardParent);
                // フィールド設定
                CardController cardCont = createdCard.GetComponent<CardController>();
                int matchId = Mathf.FloorToInt(i / 2);
                cardCont.SetField(i + 1, matchId);

                // テキストに設定
                createdCard.GetComponentInChildren<TextMeshProUGUI>().text = matchId.ToString();

                cardList.Add(cardCont);
            }
        }

        /// <summary>
        /// カーソルのコントロール
        /// </summary>
        private void CursorControl() {
            if (cursor == null) {
                return;
            }

            SwitchDevice();

            // デバイスの移動量取得
            Vector2 moveCursorValue = Vector2.zero;
            
            // コントローラー使ってたら左スティックから直接取得
            if (IsUseControllerDevice && isConnectiongController && Gamepad.current != null) {
                moveCursorValue = Gamepad.current.leftStick.ReadValue();
                moveCursorValue = new Vector2(moveCursorValue.x * Time.deltaTime * ControllerStickSens, moveCursorValue.y * Time.deltaTime * ControllerStickSens);
                cursor.transform.Translate(moveCursorValue);
            }
            // マウス
            else {
                moveCursorValue = inputActions.MiniGameConcentration.MoveCursor.ReadValue<Vector2>();
                Vector2 worldPoint = UnityEngine.Camera.main.ScreenToWorldPoint(moveCursorValue);
                cursor.transform.position = worldPoint;
            }

            // 範囲外に出ないように
            Vector2 cursorPos = cursor.transform.position;
            cursorPos.x = Mathf.Clamp(cursorPos.x, -MaxWindowSizeX, MaxWindowSizeX);
            cursorPos.y = Mathf.Clamp(cursorPos.y, -MaxWindowSizeY, MaxWindowSizeY);
            cursor.transform.position = cursorPos;
        }

        /// <summary>
        /// 選んだカード情報取得
        /// </summary>
        private CardController GetSelectCard() {
            // カード選択
            if (inputActions.MiniGameConcentration.Select.triggered) {
                // スクリーン座標からワールド座標に変換
                foreach (RaycastHit2D hit2d in Physics2D.RaycastAll(cursor.transform.position, Vector2.zero)) {
                    // 当たり判定あり
                    if (hit2d.collider != null &&
                        hit2d.collider.CompareTag("Card")) {
                        // Debug.Log("カードを選択");
                        return hit2d.collider.GetComponent<CardController>();
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// カードをめくる
        /// </summary>
        private async UniTask OpenCard(CancellationToken ct) {
            if (!canOpenCard) {
                return;
            }

            // カードを開く
            CardController selectCard = GetSelectCard();
            if (!selectCard ||
                (firstCard &&
                selectCard.Id == firstCard.Id)) {
                return;
            }

            canOpenCard = false;

            if (!firstCard) {
                // Debug.Log("first");

                selectCard.OpenCard();
                // フィールド保持
                firstCard = selectCard;
            }
            else {
                // Debug.Log("second");

                selectCard.OpenCard();

                await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: ct);

                // マッチidが同じか確認
                if (firstCard.MatchId != selectCard.MatchId) {
                    // 違ったら二つのカードを閉じる
                    firstCard.CloseCard();
                    selectCard.CloseCard();

                    canOpenCard = true;
                    firstCard = null;
                    return;
                }
                // 会ってたら削除
                cardList.Remove(firstCard);
                cardList.Remove(selectCard);
                Destroy(firstCard.gameObject);
                Destroy(selectCard.gameObject);

                firstCard = null;

                // クリア時の処理
                IsCrear();
            }

            canOpenCard = true;
        }

        /// <summary>
        /// クリア処理
        /// </summary>
        private void IsCrear() {
            if (cardList.Count() > 0) {
                return;
            }
            
            // ミニゲームクリアのコールバックを呼ぶ
            OnMiniGameCleared();
        }
    }
}
