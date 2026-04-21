using HedgehogUtils.Forms;
using HedgehogUtils.Forms.SuperForm;
using HedgehogUtils.Voicelines;
using HG;
using RoR2;
using RoR2.Audio;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace AmyRoseMod.Characters.Survivors.Amy.Components
{
    public class AmyVoicelineComponent : VoicelineComponent
    {
        public static bool stageRankingModFound = false;

        public FormComponent formComponent;

        #region Voicelines
        public static NetworkSoundEventDef lobby1;
        public static NetworkSoundEventDef lobby2;
        public static NetworkSoundEventDef lobby3;
        public static NetworkSoundEventDef lobby4;
        public static NetworkSoundEventDef lobby5;

        public static NetworkSoundEventDef bossStart1;
        public static NetworkSoundEventDef bossStart2;
        public static NetworkSoundEventDef bossStart3;
        public static NetworkSoundEventDef bossStart4;
        public static NetworkSoundEventDef[] bossStarts;
        #region Final Bosses
        public static NetworkSoundEventDef finalBossStartGeneric;
        public static NetworkSoundEventDef finalBossStartGenericEmeralds;
        public static NetworkSoundEventDef finalBossCards;

        public static NetworkSoundEventDef voidlingTitan;
        public static NetworkSoundEventDef voidlingWorlds;

        public static NetworkSoundEventDef solusWingBackHome;
        public static NetworkSoundEventDef solusWingEggman;

        public static NetworkSoundEventDef neuralSanctumEnter;
        public static NetworkSoundEventDef solusHeartStart;

        public static NetworkSoundEventDef finalBossDefeat1;
        public static NetworkSoundEventDef finalBossDefeat2;
        public static NetworkSoundEventDef finalBossDefeat3;
        public static NetworkSoundEventDef[] finalBossDefeats;
        #endregion
        #endregion
        public void Initialize()
        {
            stageRankingModFound = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(StageRanking.StageRankingPlugin.PluginGUID);
            lobby1 = Modules.Content.CreateAndAddNetworkSoundEventDef("Play_amyrose_voiceline_lobby_1");
            lobby2 = Modules.Content.CreateAndAddNetworkSoundEventDef("Play_amyrose_voiceline_lobby_2");
            lobby3 = Modules.Content.CreateAndAddNetworkSoundEventDef("Play_amyrose_voiceline_lobby_3");
            lobby4 = Modules.Content.CreateAndAddNetworkSoundEventDef("Play_amyrose_voiceline_lobby_4");
            lobby5 = Modules.Content.CreateAndAddNetworkSoundEventDef("Play_amyrose_voiceline_lobby_5");

            bossStart1 = Modules.Content.CreateAndAddNetworkSoundEventDef("Play_amyrose_voiceline_boss_start_1");
            bossStart2 = Modules.Content.CreateAndAddNetworkSoundEventDef("Play_amyrose_voiceline_boss_start_2");
            bossStart3 = Modules.Content.CreateAndAddNetworkSoundEventDef("Play_amyrose_voiceline_boss_start_3");
            bossStart4 = Modules.Content.CreateAndAddNetworkSoundEventDef("Play_amyrose_voiceline_boss_start_4");
            bossStarts = new[] { bossStart1, bossStart2, bossStart3, bossStart4 }; 

            finalBossStartGeneric = Modules.Content.CreateAndAddNetworkSoundEventDef("Play_amyrose_voiceline_final_boss_start");
            finalBossStartGenericEmeralds = Modules.Content.CreateAndAddNetworkSoundEventDef("Play_amyrose_voiceline_final_boss_start_emeralds");
            finalBossCards = Modules.Content.CreateAndAddNetworkSoundEventDef("Play_amyrose_voiceline_final_boss_cards");

            voidlingTitan = Modules.Content.CreateAndAddNetworkSoundEventDef("Play_amyrose_voiceline_voidling_titan");
            voidlingWorlds = Modules.Content.CreateAndAddNetworkSoundEventDef("Play_amyrose_voiceline_voidling_worlds");

            solusWingBackHome = Modules.Content.CreateAndAddNetworkSoundEventDef("Play_amyrose_voiceline_solus_wing_back_home");
            solusWingEggman = Modules.Content.CreateAndAddNetworkSoundEventDef("Play_amyrose_voiceline_solus_wing_eggman");

            neuralSanctumEnter = Modules.Content.CreateAndAddNetworkSoundEventDef("Play_amyrose_voiceline_neural_sanctum");
            solusHeartStart = Modules.Content.CreateAndAddNetworkSoundEventDef("Play_amyrose_voiceline_solus_heart_start");

            finalBossDefeat1 = Modules.Content.CreateAndAddNetworkSoundEventDef("Play_amyrose_voiceline_final_boss_defeat_1");
            finalBossDefeat2 = Modules.Content.CreateAndAddNetworkSoundEventDef("Play_amyrose_voiceline_final_boss_defeat_2");
            finalBossDefeat3 = Modules.Content.CreateAndAddNetworkSoundEventDef("Play_amyrose_voiceline_final_boss_defeat_3");
            finalBossDefeats = new[] { finalBossDefeat1, finalBossDefeat2, finalBossDefeat3 };
        }
        public override void SubscribeEvents()
        {
            formComponent = base.GetComponent<FormComponent>();
            VoicelineManager.OnStageStart += OnStageStart;
            VoicelineManager.OnBossStart += OnBossStart;
            VoicelineManager.OnBossDefeated += OnBossDefeated;
            VoicelineManager.OnFinalBossStart += OnFinalBossStart;
            VoicelineManager.OnFinalBossDefeated += OnFinalBossDefeated;
            characterBody.onJump += new CharacterBody.JumpDelegate(OnJump);
            if (stageRankingModFound && Util.HasEffectiveAuthority(gameObject)) SubscribeStageRanking();
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
            characterBody.onJump -= new CharacterBody.JumpDelegate(OnJump);
            if (stageRankingModFound && Util.HasEffectiveAuthority(gameObject)) UnsubscribeStageRanking();
        }
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private void UnsubscribeStageRanking()
        {
            StageRanking.StageRankingPanel.OnStageRankingPanelEnd -= OnStageRanking;
        }
        private void OnJump()
        {
            PlayVoiceline("Play_amyrose_voiceline_jump", VoicelinePriority.Any);
        }
        private void OnStageStart(Stage stage, List<NetworkedVoiceline> networkedVoicelines)
        {
            if (Stage.instance.sceneDef.cachedName == "solusweb") 
            { 
                networkedVoicelines.Add(new NetworkedVoiceline(this, neuralSanctumEnter.index, VoicelinePriority.PriorityDialogue)); 
                return;
            }
        }
        private void OnBossStart(BodyIndex boss, List<NetworkedVoiceline> networkedVoicelines)
        {
            networkedVoicelines.Add(new NetworkedVoiceline(this, bossStarts.GetRandom().index, VoicelinePriority.PriorityDialogue));
        }
        private void OnBossDefeated(BodyIndex boss, List<NetworkedVoiceline> networkedVoicelines)
        {
            
        }
        private void OnFinalBossStart(FinalBoss finalBoss, List<NetworkedVoiceline> networkedVoicelines)
        {
            switch (finalBoss)
            {
                case FinalBoss.Mithrix1:
                    Chat.AddMessage("hammer");
                    return;
                case FinalBoss.Voidling1:
                    networkedVoicelines.Add(new NetworkedVoiceline(this, voidlingTitan.index, VoicelinePriority.PriorityDialogue));
                    return;
                case FinalBoss.Voidling2:
                    networkedVoicelines.Add(new NetworkedVoiceline(this, voidlingWorlds.index, VoicelinePriority.PriorityDialogue));
                    return;
                case FinalBoss.FalseSon1:
                    Chat.AddMessage("electric punk");
                    return;
                case FinalBoss.FalseSon2:
                    Chat.AddMessage("faster than lightning");
                    return;
                case FinalBoss.SolusWing:
                    networkedVoicelines.Add(new NetworkedVoiceline(this, solusWingBackHome.index, VoicelinePriority.PriorityDialogue));
                    return;
                case FinalBoss.SolusHeart1:
                    networkedVoicelines.Add(new NetworkedVoiceline(this, solusHeartStart.index, VoicelinePriority.PriorityDialogue));
                    return;
            }
            if (finalBoss == FinalBoss.Mithrix3 || finalBoss == FinalBoss.Voidling3 || finalBoss == FinalBoss.FalseSon3 || finalBoss == FinalBoss.Arraign2 || finalBoss == FinalBoss.SolusHeart3) 
            {
                networkedVoicelines.Add(new NetworkedVoiceline(this, finalBossCards.index, VoicelinePriority.PriorityDialogue));
                return;
            }
            if (finalBoss == FinalBoss.LunarScavenger || finalBoss == FinalBoss.Arraign1) 
            {
                if (formComponent && Forms.formToHandler.TryGetValue(SuperFormDef.superFormDef, out var handler))
                {
                    if (handler.CanTransform(formComponent))
                    {
                        networkedVoicelines.Add(new NetworkedVoiceline(this, finalBossStartGenericEmeralds.index, VoicelinePriority.PriorityDialogue));
                        return;
                    }
                }
                networkedVoicelines.Add(new NetworkedVoiceline(this, finalBossStartGeneric.index, VoicelinePriority.PriorityDialogue));
                return;
            }

        }
        private void OnFinalBossDefeated(FinalBoss finalBoss, List<NetworkedVoiceline> networkedVoicelines)
        {
            if (finalBoss == FinalBoss.SolusWingWeakPoint)
            {
                networkedVoicelines.Add(new NetworkedVoiceline(this, solusWingEggman.index, VoicelinePriority.PriorityDialogue));
                return;
            }
            if (finalBoss == FinalBoss.Mithrix1) Chat.AddMessage("you don't know me");
            if (finalBoss == FinalBoss.Mithrix4 || finalBoss == FinalBoss.Voidling3 || finalBoss == FinalBoss.FalseSon3 || finalBoss == FinalBoss.SolusWing || finalBoss == FinalBoss.SolusHeart3 || finalBoss == FinalBoss.LunarScavenger || finalBoss == FinalBoss.Arraign2) 
            {
                networkedVoicelines.Add(new NetworkedVoiceline(this, finalBossDefeats.GetRandom().index, VoicelinePriority.PriorityDialogue));
                return;
            }
        }
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private void OnStageRanking(StageRanking.Ranking ranking)
        {
            switch (ranking)
            {
                case StageRanking.Ranking.S:
                    PlayVoiceline("Play_amyrose_voiceline_ranking_s", VoicelinePriority.Dialogue);
                    break;
                case StageRanking.Ranking.A:
                    PlayVoiceline("Play_amyrose_voiceline_ranking_a", VoicelinePriority.Dialogue);
                    break;
                case StageRanking.Ranking.B:
                    PlayVoiceline("Play_amyrose_voiceline_ranking_b", VoicelinePriority.Dialogue);
                    break;
                case StageRanking.Ranking.C:
                    PlayVoiceline("Play_amyrose_voiceline_ranking_c", VoicelinePriority.Dialogue);
                    break;
                case StageRanking.Ranking.D:
                    PlayVoiceline("Play_amyrose_voiceline_ranking_d", VoicelinePriority.Dialogue);
                    break;
            }
        }
    }
}