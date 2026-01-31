using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GGJ {
    public class CardController : MonoBehaviour {
        public int Id {  get; private set; }
        public int MatchId { get; private set; }

        [SerializeField] private GameObject maskObject;

        /// <summary>
        /// フィールドを設定
        /// </summary>
        public void SetField(int id, int matchId) {
            Id = id;
            MatchId = matchId;
        }

        /// <summary>
        /// カードを開く
        /// </summary>
        public void OpenCard() {
            maskObject.SetActive(false);
        }

        /// <summary>
        /// カードを閉じる
        /// </summary>
        public void CloseCard() {
            maskObject.SetActive(true);
        }
    }
}
