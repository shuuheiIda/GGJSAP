using System;
using UnityEngine;

namespace GGJ.InGame.NPC
{
    /// <summary>
    /// NPCの外見の特徴
    /// </summary>
    [Serializable]
    public class NpcAppearance
    {
        [Header("外見の色")]
        public NpcColor hatColor = NpcColor.White;
        public NpcColor hairColor = NpcColor.White;
        public NpcColor maskColor = NpcColor.White;
        public NpcColor clothesColor = NpcColor.White;
        public NpcColor shoeColor = NpcColor.White;

        [Header("その他の特徴")]
        public Gender gender = Gender.Man;
        public Direction positionFromCenter = Direction.Up;

        /// <summary>
        /// NpcColorをUnityのColorに変換
        /// </summary>
        public Color GetUnityColor(NpcColor npcColor)
        {
            return npcColor switch
            {
                NpcColor.Red => Color.red,
                NpcColor.Blue => Color.blue,
                NpcColor.Green => Color.green,
                NpcColor.Yellow => Color.yellow,
                NpcColor.White => Color.white,
                NpcColor.Black => Color.black,
                _ => Color.white
            };
        }
    }

    /// <summary>
    /// NPCの色の選択肢（判定可能な色のみ）
    /// </summary>
    public enum NpcColor
    {
        Red,    // 赤色
        Blue,   // 青色
        Green,  // 緑色
        Yellow, // 黄色
        White,  // 白色
        Black   // 黒色
    }

    /// <summary>
    /// 性別
    /// </summary>
    public enum Gender
    {
        Man,
        Woman
    }

    /// <summary>
    /// 中心からの方向
    /// </summary>
    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    /// <summary>
    /// NPCの基本データ（ScriptableObject）
    /// </summary>
    [CreateAssetMenu(fileName = "NpcData", menuName = "GGJ/NpcData", order = 0)]
    public class NpcData : ScriptableObject
    {
        [Header("基本情報")]
        [Tooltip("NPCの名前")]
        public string npcName = "Npc";

        [Header("外見の特徴")]
        [Tooltip("NPCの外見情報")]
        public NpcAppearance appearance = new NpcAppearance();

        [Header("UI表示用")]
        [Tooltip("NPCのスプライト画像")]
        public Sprite npcSprite;
    }
}