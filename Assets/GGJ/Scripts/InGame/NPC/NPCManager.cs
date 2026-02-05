using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GGJ.Core;
using GGJ.InGame.Events;

namespace GGJ.InGame.NPC
{
    /// <summary>
    /// シーン上の全Npcを管理するマネージャー
    /// </summary>
    public class NpcManager : Singleton<NpcManager>
    {
        [Header("Npc管理")]
        [SerializeField] private List<INpc> allNpcs = new List<INpc>();
        
        [Header("セリフデータ")]
        [SerializeField] private DialogueData dialogueData;
        
        [Header("デバッグ")]
        [SerializeField] private bool showDebugLog = false; // 犯人のデバッグログを表示するか
        
        private Dictionary<INpc, int> npcDialogueIndices = new Dictionary<INpc, int>();
        
        protected override bool UseDontDestroyOnLoad => true;
        
        protected override void Init()
        {
            allNpcs.Clear();
            npcDialogueIndices.Clear();
            GameEvents.OnGameStart += OnGameStart;
        }
        
        protected override void OnDestroy()
        {
            base.OnDestroy();
            GameEvents.OnGameStart -= OnGameStart;
        }
        
        private void OnGameStart() => RandomizeCriminal();
        
        private void RandomizeCriminal()
        {
            if (allNpcs.Count == 0) return;
            
            foreach (var npc in allNpcs)
                npc.SetCriminal(false);
            
            int randomIndex = Random.Range(0, allNpcs.Count);
            allNpcs[randomIndex].SetCriminal(true);
            
            var criminalData = allNpcs[randomIndex].GetNpcData();
            var criminalMonoBehaviour = allNpcs[randomIndex] as MonoBehaviour;
            
            if (showDebugLog && criminalData != null && criminalMonoBehaviour != null)
                Debug.Log($"犯人は：{criminalMonoBehaviour.gameObject.name}（マスク色={criminalData.appearance.maskColor}、服色={criminalData.appearance.clothesColor}、性別={criminalData.appearance.gender}）");
        }
        
        /// <summary>
        /// Npcを登録（Npc側から呼ばれる）
        /// </summary>
        public void RegisterNpc(INpc npc)
        {
            if (npc == null) return;
            if (allNpcs.Contains(npc)) return;
            
            allNpcs.Add(npc);
            
            if (!npcDialogueIndices.ContainsKey(npc))
                npcDialogueIndices[npc] = allNpcs.Count - 1;
        }
        
        /// <summary>
        /// Npcのセリフを取得
        /// </summary>
        public string GetDialogueForNpc(INpc npc)
        {
            if (dialogueData == null)
                throw new System.Exception("[NpcManager] DialogueDataがnullです。InspectorでDialogueDataを割り当ててください");
            
            bool isCriminal = npc.IsCriminal();
            bool hasHint = npc.HasReceivedHint();
            bool isAccused = npc.IsAccused();
            
            int npcIndex = npcDialogueIndices.ContainsKey(npc) ? npcDialogueIndices[npc] : 0;
            
            var criminal = GetCriminal();
            if (criminal == null)
                throw new System.Exception("[NpcManager] 犯人が設定されていません！GameEvents.RaiseGameStart()を呼んでください");
            
            NpcAppearance criminalAppearance = criminal.GetNpcData()?.appearance;
            Gender npcGender = npc.GetNpcData()?.appearance.gender ?? Gender.Woman;
            
            return dialogueData.GetDialogue(isCriminal, hasHint, isAccused, npcIndex, criminalAppearance, npcGender);
        }
        
        private INpc GetCriminal() => allNpcs.FirstOrDefault(npc => npc.IsCriminal());
    }
}
