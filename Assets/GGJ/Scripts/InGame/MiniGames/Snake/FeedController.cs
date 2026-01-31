using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GGJ {
    public class FeedController : MonoBehaviour {
        /// <summary>
        /// 現在いるセル番号
        /// </summary>
        public int CurrentCellNumX { get; private set; }
        public int CurrentCellNumY { get; private set; }

        /// <summary>
        /// 生成したときにフィールドを入れる
        /// </summary>
        public void SetUp(int cellNumX, int cellNumY) {
            CurrentCellNumX = cellNumX;
            CurrentCellNumY = cellNumY;
        }
    }
}
