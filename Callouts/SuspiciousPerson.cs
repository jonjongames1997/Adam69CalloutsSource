﻿using CalloutInterfaceAPI;

namespace Adam69Callouts.Callouts
{
    [CalloutInterface("[Adam69 Callouts] Suspicious Person", CalloutProbability.Medium, "Suspicious Person Reported", "Code 2", "LSPD")]
    public class SuspiciousPerson : Callout
    {
        private static Ped suspect;
        private static Blip susBlip;
        private static Vector3 spawnpoint;
        private static readonly string[] wepList = new string[] { "WEAPON_PISTOL", "WEAPON_COMBATPISTOL", "WEAPON_COMBATMG", "WEAPON_TACTICALRIFLE", "weapon_snspistol", "weapon_marksmanpistol", "weapon_doubleaction" };
        private static int counter;
        private static string malefemale;
        private static string copGender;
        private static readonly Random random = new Random();

        public override bool OnBeforeCalloutDisplayed()
        {
            spawnpoint = World.GetNextPositionOnStreet(MainPlayer.Position.Around(1000f));
            ShowCalloutAreaBlipBeforeAccepting(spawnpoint, 100f);
            LSPD_First_Response.Mod.API.Functions.PlayScannerAudio("Adam69Callouts_SuspiciousPerson_Audio");
            CalloutInterfaceAPI.Functions.SendMessage(this, "Citizen's report of a suspicious person.");
            CalloutMessage = "Suspect may be armed with a deadly weapon.";
            CalloutPosition = spawnpoint;

            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            Game.LogTrivial("[Adam69 Callouts LOG]: Suspicious Person callout accepted!");
            Game.DisplayNotification("web_adam69callouts", "web_adam69callouts", "~w~Adam69 Callouts", "~w~Suspicious Person", "~b~Dispatch~w~: The suspect has been spotted! Respond ~r~Code 2~w~.");

            LSPD_First_Response.Mod.API.Functions.PlayScannerAudio("Adam69Callouts_Respond_Code_2_Audio");

            suspect = new Ped(spawnpoint);
            suspect.IsPersistent = true;
            suspect.BlockPermanentEvents = true;
            suspect.IsValid();

            suspect.Tasks.PlayAnimation(new AnimationDictionary("anim@heists@fleeca_bank@ig_7_jetski_owner"), "owner_idle", -1f, AnimationFlags.Loop);

            susBlip = suspect.AttachBlip();
            susBlip.Color = System.Drawing.Color.Red;
            susBlip.Alpha = 0.5f;
            susBlip.IsRouteEnabled = true;

            malefemale = suspect.IsMale ? "Sir" : "Ma'am";
            copGender = MainPlayer.IsMale ? "Sir" : "Ma'am";

            counter = 0;

            return base.OnCalloutAccepted();
        }

        public override void OnCalloutNotAccepted()
        {
            suspect?.Delete();
            susBlip?.Delete();

            base.OnCalloutNotAccepted();
        }

        public override void Process()
        {
            if (MainPlayer.DistanceTo(suspect) <= 10f)
            {
                if (Settings.HelpMessages)
                {
                    Game.DisplayHelp($"Press ~y~{Settings.Dialog}~w~ to interact with the suspect.", 5000);
                }

                if (Game.IsKeyDown(Settings.Dialog))
                {
                    counter++;

                    if (counter == 1)
                    {
                        Game.DisplaySubtitle("~b~You~w~: Hey there, " + malefemale + ". What's going on? Why are you in this alleyway?");
                    }
                    if (counter == 2)
                    {
                        suspect.Tasks.PlayAnimation(new AnimationDictionary("anim@amb@casino@brawl@fights@argue@"), "arguement_loop_mp_m_brawler_01", -1f, AnimationFlags.Loop);
                        Game.DisplaySubtitle("~r~Suspect~w~: Just chilling out. Why?");
                    }
                    if (counter == 3)
                    {
                        suspect.Tasks.PlayAnimation(new AnimationDictionary("rcmjosh1"), "idle", -1f, AnimationFlags.Loop);
                        Game.DisplaySubtitle("~b~You~w~: I have gotten reports that you were being suspicious. Do you have any weapons on you?");
                    }
                    if (counter == 4)
                    {
                        suspect.Tasks.PlayAnimation(new AnimationDictionary("anim@amb@casino@brawl@fights@argue@"), "arguement_loop_mp_m_brawler_01", -1f, AnimationFlags.Loop);
                        Game.DisplaySubtitle("~r~Suspect~w~: *gasps* That's a such accusation. Where did you get that from? How dare you?!");
                    }
                    if (counter == 5)
                    {
                        suspect.Tasks.PlayAnimation(new AnimationDictionary("rcmjosh1"), "idle", -1f, AnimationFlags.Loop);
                        Game.DisplaySubtitle("~b~You~w~: That's what I have heard in the report. Let me do a quick pat down and we'll go from there.");
                    }
                    if (counter == 6)
                    {
                        suspect.Tasks.PlayAnimation(new AnimationDictionary("anim@amb@casino@brawl@fights@argue@"), "arguement_loop_mp_m_brawler_01", -1f, AnimationFlags.Loop);
                        Game.DisplaySubtitle("~r~Suspect~w~: I DO NOT CONSENT TO A PAT DOWN!");
                    }
                    if (counter == 7)
                    {
                        Game.DisplaySubtitle("convo ended.");
                        suspect.Tasks.FightAgainst(MainPlayer);
                        suspect.Inventory.GiveNewWeapon(wepList[new Random().Next((int)wepList.Length)], 500, true);
                    }
                }
            }

            base.Process();

            if (MainPlayer.IsDead || Game.IsKeyDown(Settings.EndCall))
            {
                BigMessageThread bigMessage = new BigMessageThread();

                bigMessage.MessageInstance.ShowColoredShard("Callout Failed!", "~r~You have failed the callout.", RAGENativeUI.HudColor.Red, RAGENativeUI.HudColor.Black, 5000);

                End();
            }
        }

        

        public override void End()
        {
            suspect?.Dismiss();
            susBlip?.Delete();
            Game.DisplayNotification("web_adam69callouts", "web_adam69callouts", "~w~Adam69 Callouts", "~w~Suspicious Person", "~b~You~w~: Dispatch, we are ~g~Code 4~w~. Show me back 10-8..");
            LSPD_First_Response.Mod.API.Functions.PlayScannerAudio("Adam69Callouts_Code_4_Audio");

            BigMessageThread bigMessage = new BigMessageThread();

            bigMessage.MessageInstance.ShowColoredShard("Callout Completed!", "You are now ~g~CODE 4~w~.", RAGENativeUI.HudColor.Green, RAGENativeUI.HudColor.Black, 5000);

            Game.LogTrivial("Adam69 Callouts [LOG]: Suspicious Person callout is code 4!");
            base.End();
        }
    }
}