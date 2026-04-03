using HedgehogUtils.Voicelines;
using RoR2;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace AmyRoseMod.Characters.Survivors.Amy.Components
{
    public class AmyVoicelineComponent : VoicelineComponent
    {
        public static bool stageRankingModFound = false;
        public override void SubscribeEvents()
        {
            VoicelineManager.OnStageStart += OnStageStart;
            VoicelineManager.OnBossStart += OnBossStart;
            VoicelineManager.OnBossDefeated += OnBossDefeated;
            VoicelineManager.OnFinalBossStart += OnFinalBossStart;
            VoicelineManager.OnFinalBossDefeated += OnFinalBossDefeated;
            if (stageRankingModFound) SubscribeStageRanking();
        }
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private void SubscribeStageRanking()
        {
            StageRanking.StageRankingPanel.OnStageRankingPanelEnd += OnStageRanking;
        }
        public override void UnsubscribeEvents()
        {
            VoicelineManager.OnStageStart -= OnStageStart;
            VoicelineManager.OnBossStart -= OnBossStart;
            VoicelineManager.OnBossDefeated -= OnBossDefeated;
            VoicelineManager.OnFinalBossStart -= OnFinalBossStart;
            VoicelineManager.OnFinalBossDefeated -= OnFinalBossDefeated;
            if (stageRankingModFound) UnsubscribeStageRanking();
        }
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private void UnsubscribeStageRanking()
        {
            StageRanking.StageRankingPanel.OnStageRankingPanelEnd -= OnStageRanking;
        }
        private void OnStageStart(Stage stage, List<NetworkedVoiceline> networkedVoicelines)
        {
            if (Stage.instance.sceneDef.cachedName == "solusweb") { Chat.AddMessage("not getting stuck here"); return; }
        }
        private void OnBossStart(BodyIndex boss, List<NetworkedVoiceline> networkedVoicelines)
        {
            Chat.AddMessage($"generic boss start");
        }
        private void OnBossDefeated(BodyIndex boss, List<NetworkedVoiceline> networkedVoicelines)
        {
            Chat.AddMessage($"generic boss defeat");
        }
        private void OnFinalBossStart(FinalBoss finalBoss, List<NetworkedVoiceline> networkedVoicelines)
        {
            switch (finalBoss)
            {
                case FinalBoss.Mithrix1:
                    Chat.AddMessage("hammer");
                    return;
                case FinalBoss.Voidling1:
                    Chat.AddMessage("titan");
                    return;
                case FinalBoss.Voidling2:
                    Chat.AddMessage("all these worlds");
                    return;
                case FinalBoss.FalseSon1:
                    Chat.AddMessage("electric punk");
                    return;
                case FinalBoss.FalseSon2:
                    Chat.AddMessage("faster than lightning");
                    return;
                case FinalBoss.SolusWing:
                    Chat.AddMessage("eggman");
                    return;
                case FinalBoss.SolusHeart:
                    Chat.AddMessage("not standing before you");
                    return;
            }
            if (finalBoss == FinalBoss.Mithrix3 || finalBoss == FinalBoss.Voidling3 || finalBoss == FinalBoss.FalseSon3 || finalBoss == FinalBoss.Arraign2) Chat.AddMessage("big finish");
            if (finalBoss == FinalBoss.LunarScavenger || finalBoss == FinalBoss.Arraign1) Chat.AddMessage("generic final boss"); return;
        }
        private void OnFinalBossDefeated(FinalBoss finalBoss, List<NetworkedVoiceline> networkedVoicelines)
        {
            if (finalBoss == FinalBoss.Mithrix1) Chat.AddMessage("you don't know me");
            if (finalBoss == FinalBoss.Mithrix4 || finalBoss == FinalBoss.Voidling3 || finalBoss == FinalBoss.FalseSon3 || finalBoss == FinalBoss.SolusWing || finalBoss == FinalBoss.SolusHeart || finalBoss == FinalBoss.LunarScavenger || finalBoss == FinalBoss.Arraign2) { Chat.AddMessage("generic final boss defeat"); return; }
        }
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private void OnStageRanking(StageRanking.Ranking ranking)
        {
            Chat.AddMessage($"ranking {ranking.ToString()}");
        }
    }
}