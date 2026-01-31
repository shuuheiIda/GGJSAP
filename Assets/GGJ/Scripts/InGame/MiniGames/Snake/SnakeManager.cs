using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

namespace GGJ.InGame.MiniGames {
    public class SnakeManager : MiniGameBase {
        /// <summary>
        /// スコア表示用テキスト
        /// </summary>
        [SerializeField] private Text scoreText;

        /// <summary>
        /// クリアに必要なスコア
        /// </summary>
        private const int CrearScore = 15;

        /// <summary>
        /// スコア
        /// </summary>
        private int score = 0;

        /// <summary>
        /// セルのリスト
        /// </summary>
        private Vector2[,] cellList = new Vector2[18, 10];

        
        /// <summary>
        /// 餌を生成する親Transform
        /// </summary>
        [SerializeField] private Transform feedParent;

        /// <summary>
        /// 餌のプレハブ
        /// </summary>
        [SerializeField] private GameObject feedPrefab;

        /// <summary>
        /// 餌のインスタンスリスト
        /// </summary>
        public List<FeedController> feedList { get; private set; } = new List<FeedController>();


        /// <summary>
        /// 蛇を生成する親Transform
        /// </summary>
        [SerializeField] private Transform snakeParent;

        /// <summary>
        /// 蛇の頭のプレハブ
        /// </summary>
        [SerializeField] private GameObject snakeHeadPrefab;
        /// <summary>
        /// 蛇の体のプレハブ
        /// </summary>
        [SerializeField] private GameObject snakeBodyPrefab;

        /// <summary>
        /// 蛇のインスタンスリスト
        /// </summary>
        public List<SnakeController> SnakeList { get; private set; } = new List<SnakeController>();


        /// <summary>
        /// 移動クールタイム
        /// </summary>
        private float moveCd = 1f;

        /// <summary>
        /// 移動クールタイマー
        /// </summary>
        private float moveCdTimer = 0;


        /// <summary>
        /// 進む向きの列挙型
        /// </summary>
        private enum MoveDirection {
            Up,
            Down,
            Left,
            Right
        }

        /// <summary>
        /// 進む向き
        /// </summary>
        private MoveDirection moveDirection = MoveDirection.Up;

        private void OnEnable() {
            // 初期化
            moveDirection = MoveDirection.Up;
            SnakeList = new List<SnakeController>();
            feedList = new List<FeedController>();
            moveCd = 1f;
            moveCdTimer = 0;
            score = 0;
            scoreText.text = $"{score}/{CrearScore}";

            // 蛇を作成しフィールドに保持
            GameObject snakeHead = Instantiate(snakeHeadPrefab, cellList[9,5], Quaternion.identity, snakeParent);
            SnakeController snakeController = snakeHead.GetComponent<SnakeController>();
            snakeController.SetUp();
            SnakeList.Add(snakeController);

            CreateFeet();

            inputActions.Enable();
        }

        private void OnDisable() {
            // 蛇の削除
            foreach (var snake in SnakeList) {
                Destroy(snake.gameObject);
            }

            inputActions.Disable();
        }

        private void Awake() {
            inputActions = new PlayerInput();

            for (int i = 0; i < 18; i++) {
                for (int j = 0; j < 10; j++) {
                    // Instantiate(feetPrefab, new Vector2((-9 + i) + 0.5f, (5 - j) - 0.5f), Quaternion.identity);
                    cellList[i,j] = new Vector2((-9 + i) + 0.5f, (5 - j) - 0.5f);
                }
            }
        }

        private void Start() {
            
        }

        protected override void Update() {
            base.Update();
            GetInputDevice();
            MoveSnake();
            EatFeed();

            if (Input.GetKeyDown(KeyCode.V)) {
                foreach (var feed in feedList) {
                    Debug.Log($"X : {feed.CurrentCellNumX}, Y : {feed.CurrentCellNumY}");
                }
            }
        }

        /// <summary>
        /// 餌を作成
        /// </summary>
        private void CreateFeet() {
            int posX;
            int posY;

            while (true) {
                posX = UnityEngine.Random.Range(0, 18);
                posY = UnityEngine.Random.Range(0, 10);

                if (!IsCellOverlapped(posX, posY)) {
                    break;
                }
            }

            // 餌を生成してフィールドで保持
            GameObject createdFeed = Instantiate(feedPrefab, cellList[posX, posY], Quaternion.identity, feedParent);
            FeedController feedController = createdFeed.GetComponent<FeedController>();
            feedController.SetUp(posX, posY);

            feedList.Add(feedController);
        }

        /// <summary>
        /// セルが被っているか
        /// </summary>
        private bool IsCellOverlapped(int cellPosX, int cellPosY, bool doSnake = true, bool doFeed = true) {
            if (doSnake) {
                foreach (var snake in SnakeList) {
                    if (snake.CurrentCellNumX == cellPosX &&
                        snake.CurrentCellNumY == cellPosY) {
                        return true;
                    }
                }
            }

            if (doFeed) {
                foreach (var feed in feedList) {
                    if (feed.CurrentCellNumX == cellPosX &&
                        feed.CurrentCellNumY == cellPosY) {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// デバイスの入力取得
        /// </summary>
        private void GetInputDevice() {
            // 入力取得
            Vector2 inputValue = inputActions.MiniGameSnake.Move.ReadValue<Vector2>();

            // 入力がなければ何もしない
            if (Mathf.Abs(inputValue.x) > 0 ||
                Mathf.Abs(inputValue.y) > 0) {
                int x = SnakeList.First().CurrentCellNumX;
                int y = SnakeList.First().CurrentCellNumY;
                MoveDirection changeDirection = moveDirection;

                // x
                if (Mathf.Abs(inputValue.x) > Mathf.Abs(inputValue.y)) {
                    if (inputValue.x > 0) {
                        x++;
                        if (SnakeList.Count() >= 2 &&
                            SnakeList.First(_=> _.IndexNum == 1).CurrentCellNumX == x &&
                            SnakeList.First(_ => _.IndexNum == 1).CurrentCellNumY == y) {
                            return;
                        }

                        changeDirection = MoveDirection.Right;
                        SnakeList.First().transform.localEulerAngles = new Vector3(0, 0, 270);
                    }
                    else {
                        x--;
                        if (SnakeList.Count() >= 2 &&
                            SnakeList.First(_ => _.IndexNum == 1).CurrentCellNumX == x &&
                            SnakeList.First(_ => _.IndexNum == 1).CurrentCellNumY == y) {
                            return;
                        }

                        changeDirection = MoveDirection.Left;
                        SnakeList.First().transform.localEulerAngles = new Vector3(0, 0, 90);
                    }
                }
                // y
                else {
                    if (inputValue.y > 0) {
                        y--;
                        if (SnakeList.Count() >= 2 &&
                            SnakeList.First(_ => _.IndexNum == 1).CurrentCellNumX == x &&
                            SnakeList.First(_ => _.IndexNum == 1).CurrentCellNumY == y) {
                            return;
                        }

                        changeDirection = MoveDirection.Up;
                        SnakeList.First().transform.localEulerAngles = new Vector3(0, 0, 0);
                    }
                    else {
                        y++;
                        if (SnakeList.Count() >= 2 &&
                            SnakeList.First(_ => _.IndexNum == 1).CurrentCellNumX == x &&
                            SnakeList.First(_ => _.IndexNum == 1).CurrentCellNumY == y) {
                            return;
                        }

                        changeDirection = MoveDirection.Down;
                        SnakeList.First().transform.localEulerAngles = new Vector3(0, 0, 180);
                    }
                }

                moveDirection = changeDirection;
            }
        }

        /// <summary>
        /// 移動処理
        /// </summary>
        private void MoveSnake() {
            // 移動クールタイム
            moveCdTimer += Time.deltaTime;
            if (moveCdTimer < moveCd) {
                return;
            }
            moveCdTimer = 0;

            // 全てのsnakeを移動させる
            foreach (var snakeCont in SnakeList) {
                if (snakeCont.IndexNum == 0) {
                    int x = snakeCont.CurrentCellNumX;
                    int y = snakeCont.CurrentCellNumY;

                    switch (moveDirection) {
                        case MoveDirection.Up:
                            y--;
                            break;

                        case MoveDirection.Down:
                            y++;
                            break;

                        case MoveDirection.Left:
                            x--;
                            break;

                        case MoveDirection.Right:
                            x++;
                            break;
                    }

                    // 移動先が壁か蛇だったら
                    if (IsDestinationWall(x, y) ||
                        IsCellOverlapped(x, y, doFeed: false)) {
                        Debug.Log("GameOver");
                        return;
                    }

                    snakeCont.Move(cellList[x, y], x, y);
                }
                else {
                    // 前のsnakeの位置に移動
                    int index = snakeCont.IndexNum - 1;
                    int x = SnakeList[index].BeforeCellNumX;
                    int y = SnakeList[index].BeforeCellNumY;
                    snakeCont.Move(cellList[x, y], x, y);
                }
            }
        }

        /// <summary>
        /// 移動先が壁かどうか
        /// </summary>
        private bool IsDestinationWall(int cellX, int cellY) {
            // Debug.Log(cellX + " : " + cellY);

            if ((0 <= cellX && cellX < 18) &&
                (0 <= cellY && cellY < 10)) {
                return false;
            }
            
            return true;
        }

        /// <summary>
        /// 餌を食べる
        /// </summary>
        private void EatFeed() {
            // 餌があるか
            int x = SnakeList.First().CurrentCellNumX;
            int y = SnakeList.First().CurrentCellNumY;

            // Debug.Log($"{IsCellOverlapped(x, y, doSnake: false)}, {x}, {y}");

            if (IsCellOverlapped(x, y, doSnake: false)) {
                FeedController feed = feedList.FirstOrDefault(_ => _.CurrentCellNumX == x && _.CurrentCellNumY == y);
                feedList.Remove(feed);
                Destroy(feed.gameObject);

                if (moveCd > 0) {
                    moveCd -= 0.05f;
                    if (moveCd < 0.05f) {
                        moveCd = 0.05f;
                    }
                }

                CreateSnakeBody();
                CreateFeet();

                AddScore();

                // クリア時の処理
                IsCrear();
            }
        }

        /// <summary>
        /// 蛇の体を追加
        /// </summary>
        private void CreateSnakeBody() {
            int x = SnakeList.Last().BeforeCellNumX;
            int y = SnakeList.Last().BeforeCellNumY;
            GameObject createdSnake = Instantiate(snakeBodyPrefab, cellList[x, y], Quaternion.identity, snakeParent);
            SnakeController snakeController = createdSnake.GetComponent<SnakeController>();
            snakeController.SetUp(SnakeList.Count(), x, y);
            SnakeList.Add(snakeController);
        }

        /// <summary>
        /// クリア処理
        /// </summary>
        private void IsCrear() {
            if (score != CrearScore) {
                return;
            }

            Debug.Log("Crear");
            
            // ミニゲームクリアのコールバックを呼ぶ
            OnMiniGameCleared();
        }

        /// <summary>
        /// スコア追加
        /// </summary>
        private void AddScore() {
            score++;

            scoreText.text = $"{score}/{CrearScore}";
        }
    }
}
