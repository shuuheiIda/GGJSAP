using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GGJ {
    public class SnakeController : MonoBehaviour {
        /// <summary>
        /// SnakeManagerのSnakeListのインデックス番号
        /// </summary>
        public int IndexNum { get; private set; }

        /// <summary>
        /// 現在いるセル番号
        /// </summary>
        public int CurrentCellNumX { get; private set; }
        public int CurrentCellNumY { get; private set; }

        /// <summary>
        /// ひとつ前のセル番号
        /// </summary>
        public int BeforeCellNumX { get; private set; }
        public int BeforeCellNumY { get; private set; }

        /// <summary>
        /// 生成したときにフィールドを入れる
        /// </summary>
        public void SetUp(int indexNum = 0, int cellNumX = 9, int cellNumY = 5) {
            IndexNum = indexNum;
            CurrentCellNumX = cellNumX;
            CurrentCellNumY = cellNumY;
        }

        /// <summary>
        /// 移動
        /// </summary>
        public void Move(Vector2 pos, int toX, int toY) {
            this.transform.position = pos;
            ChangeCellNum(toX, toY);
        }

        // セル番号を変更
        private void ChangeCellNum(int x,  int y) {
            BeforeCellNumX = CurrentCellNumX;
            BeforeCellNumY = CurrentCellNumY;
            CurrentCellNumX = x;
            CurrentCellNumY = y;
        }
    }
}
