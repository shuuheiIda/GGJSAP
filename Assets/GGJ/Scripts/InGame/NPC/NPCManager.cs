using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GGJ.Core;
using GGJ.InGame.Events;

namespace GGJ.InGame.NPC
{
    /// <summary>
    /// シーン上の全NPCを管理するマネージャー
    /// </summary>
    public class NPCManager : Singleton<NPCManager>
    {
        [Header("NPC管理")]
        [SerializeField] private List<INpc> allNPCs = new List<INpc>();
        
        [Header("セリフデータ")]
        [SerializeField] private DialogueData dialogueData;
        
        protected override bool UseDontDestroyOnLoad => false;
        
        protected override void Init()
        {
            allNPCs.Clear();
            
            // ヒント取得イベントを購読
            GameEvents.OnHintReceived += OnHintReceived;
            
            // ゲーム開始イベントを購読
            GameEvents.OnGameStart += OnGameStart;
        }
        
        protected override void OnDestroy()
        {
            base.OnDestroy();
            GameEvents.OnHintReceived -= OnHintReceived;
            GameEvents.OnGameStart -= OnGameStart;
        }
        
        /// <summary>
        /// ゲーム開始時の処理
        /// </summary>
        private void OnGameStart()
        {
            RandomizeCriminal();
        }
        
        /// <summary>
        /// ヒント取得時の処理
        /// </summary>
        private void OnHintReceived()
        {
            SetAllHintsReceived(true);
            Debug.Log("[NPCManager] 全NPCがヒントを受け取りました");
        }
        
        /// <summary>
        /// ランダムにNPCを1体選んで犯人にする
        /// </summary>
        public void RandomizeCriminal()
        {
            if (allNPCs.Count == 0)
            {
                Debug.LogWarning("[NPCManager] NPCが登録されていないため、犯人を決定できません");
                return;
            }
            
            // 全NPCの犯人フラグをリセット
            foreach (var npc in allNPCs)
            {
                npc.SetCriminal(false);
            }
            
            // ランダムに1体を選んで犯人にする
            int randomIndex = Random.Range(0, allNPCs.Count);
            allNPCs[randomIndex].SetCriminal(true);
            
            var criminalData = allNPCs[randomIndex].GetNPCData();
            Debug.Log($"[NPCManager] 犯人決定: {criminalData?.npcName ?? "Unknown"} (Index: {randomIndex})");
        }
        
        /// <summary>
        /// NPCを登録（NPC側から呼ばれる）
        /// </summary>
        public void RegisterNPC(INpc npc)
        {
            if (npc == null) return;
            
            if (!allNPCs.Contains(npc))
            {
                allNPCs.Add(npc);
                Debug.Log($"[NPCManager] NPC登録: {npc.GetNPCData()?.npcName ?? "Unknown"}");
                
                // テスト用：NPC登録後に犯人が未設定なら自動的にランダム化
                if (GetCriminal() == null && allNPCs.Count > 0)
                {
                    Debug.Log("[NPCManager] 犯人が未設定のため、自動的にランダム化します");
                    RandomizeCriminal();
                }
            }
        }
        
        /// <summary>
        /// 全NPCを取得
        /// </summary>
        public List<INpc> GetAllNPCs() => new List<INpc>(allNPCs);
        
        /// <summary>
        /// NPCのセリフを取得
        /// </summary>
        public string GetDialogueForNPC(INpc npc)
        {
            if (dialogueData == null)
            {
                Debug.LogWarning("[NPCManager] DialogueDataが設定されていません！InspectorでDialogueDataを割り当ててください");
                return "...";
            }
            
            bool isCriminal = npc.IsCriminal();
            bool hasHint = npc.HasReceivedHint();
            
            // NPCのインデックスを使ってバリエーションを提供
            int npcIndex = allNPCs.IndexOf(npc);
            
            // 犯人の外見情報を取得
            var criminal = GetCriminal();
            if (criminal == null)
            {
                Debug.LogWarning("[NPCManager] 犯人が設定されていません！GameEvents.RaiseGameStart()を呼んでください");
            }
            NPCAppearance criminalAppearance = criminal?.GetNPCData()?.appearance;
            
            // NPCの性別を取得
            Gender npcGender = npc.GetNPCData()?.appearance.gender ?? Gender.Female;
            
            Debug.Log($"[NPCManager] GetDialogue: isCriminal={isCriminal}, hasHint={hasHint}, gender={npcGender}, criminalAppearance={(criminalAppearance != null ? "あり" : "なし")}");
            
            string dialogue = dialogueData.GetDialogue(isCriminal, hasHint, npcIndex, criminalAppearance, npcGender);
            Debug.Log($"[NPCManager] 取得したセリフ: {dialogue}");
            
            return dialogue;
        }
        
        /// <summary>
        /// 犯人のNPCを取得
        /// </summary>
        public INpc GetCriminal()
        {
            return allNPCs.FirstOrDefault(npc => npc.IsCriminal());
        }
        
        /// <summary>
        /// 外見の特徴に一致するNPCリストを取得
        /// </summary>
        public List<INpc> GetNPCsByAppearance(NPCAppearance targetAppearance)
        {
            return allNPCs.Where(npc =>
            {
                var data = npc.GetNPCData();
                if (data == null) return false;
                return DoesAppearanceMatch(data.appearance, targetAppearance);
            }).ToList();
        }
        
        /// <summary>
        /// 性別で絞り込み
        /// </summary>
        public List<INpc> GetNPCsByGender(Gender gender)
        {
            return allNPCs.Where(npc =>
            {
                var data = npc.GetNPCData();
                return data != null && data.appearance.gender == gender;
            }).ToList();
        }
        
        /// <summary>
        /// 方向で絞り込み
        /// </summary>
        public List<INpc> GetNPCsByDirection(Direction direction)
        {
            return allNPCs.Where(npc =>
            {
                var data = npc.GetNPCData();
                return data != null && data.appearance.positionFromCenter == direction;
            }).ToList();
        }
        
        /// <summary>
        /// 服の色で絞り込み
        /// </summary>
        public List<INpc> GetNPCsByClothesColor(Color color, float tolerance = 0.1f)
        {
            return allNPCs.Where(npc =>
            {
                var data = npc.GetNPCData();
                if (data == null) return false;
                return ColorDistance(data.appearance.clothesColor, color) < tolerance;
            }).ToList();
        }
        
        /// <summary>
        /// 全NPCにヒント受信フラグを設定
        /// </summary>
        public void SetAllHintsReceived(bool received)
        {
            foreach (var npc in allNPCs)
            {
                npc.SetHintReceived(received);
            }
        }
        
        /// <summary>
        /// 外見が一致するかチェック（ヒントベース）
        /// </summary>
        private bool DoesAppearanceMatch(NPCAppearance npcAppearance, NPCAppearance targetAppearance)
        {
            // 完全一致チェック（必要に応じて部分一致に変更可能）
            bool genderMatch = npcAppearance.gender == targetAppearance.gender;
            bool directionMatch = npcAppearance.positionFromCenter == targetAppearance.positionFromCenter;
            bool clothesMatch = ColorDistance(npcAppearance.clothesColor, targetAppearance.clothesColor) < 0.1f;
            bool maskMatch = ColorDistance(npcAppearance.maskColor, targetAppearance.maskColor) < 0.1f;
            bool hairMatch = ColorDistance(npcAppearance.hairColor, targetAppearance.hairColor) < 0.1f;
            bool hatMatch = ColorDistance(npcAppearance.hatColor, targetAppearance.hatColor) < 0.1f;
            bool shoeMatch = ColorDistance(npcAppearance.shoeColor, targetAppearance.shoeColor) < 0.1f;
            
            return genderMatch && directionMatch && clothesMatch && 
                   maskMatch && hairMatch && hatMatch && shoeMatch;
        }
        
        /// <summary>
        /// 色の距離を計算
        /// </summary>
        private float ColorDistance(Color a, Color b)
        {
            float dr = a.r - b.r;
            float dg = a.g - b.g;
            float db = a.b - b.b;
            return Mathf.Sqrt(dr * dr + dg * dg + db * db);
        }
    }
}
