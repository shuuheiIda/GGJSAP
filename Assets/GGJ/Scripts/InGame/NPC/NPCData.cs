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
        public Color clothesColor = Color.white;
        public Color maskColor = Color.white;
        public Color hairColor = Color.white;
        public Color hatColor = Color.white;
        public Color shoeColor = Color.white;

        [Header("その他の特徴")]
        public Gender gender = Gender.Male;
        public Direction positionFromCenter = Direction.Up;
    }

    /// <summary>
    /// 性別
    /// </summary>
    public enum Gender
    {
        Male,
        Female
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