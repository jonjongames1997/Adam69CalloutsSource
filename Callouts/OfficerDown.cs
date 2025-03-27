﻿using CalloutInterfaceAPI;

namespace Adam69Callouts.Callouts
{


    [CalloutInterface("[Adam69 Callouts] Officer Down", CalloutProbability.Medium, "Reports of an officer down", "Code 3", "LEO")]

    public class OfficerDown : Callout
    {

        private static readonly string[] pedsList = new string[] { "s_f_y_cop_01", "s_m_y_cop_01", "csb_cop", "s_f_y_sheriff_01", "s_m_y_sheriff_01", "s_m_y_hwaycop_01", "s_m_m_security_01", "s_f_y_ranger_01", "s_m_y_ranger_01" };
        private static Ped suspect;
        private static Ped officer;
        private static Blip copBlip;
        private static Vehicle emergencyVehicle;
        private static readonly string[] officerVehicle = new string[] { "police", "police2", "police3", "police4", "police5", "polgauntlet", "poldominator10", "poldorado", "polgreenwood", "polimpaler5", "polimpaler6", "polcaracara", "polcoquette4", "polfaction2", "polterminus", "dilettante2", "fbi", "pbus", "policeb", "pranger", "riot", "riot2", "sheriff", "sheriff2" };
        private static Blip officerVehicleBlip;
        private static Vector3 spawnpoint;
        private static Vector3 vehicleSpawn;
        private static Vector3 susSpawn;
        private static float vehicleHeading;
        private static float officerheading;
        private static float susHeading;
        private static Blip suspectBlip;
        private static int counter;
        private static string malefemale;

        public override bool OnBeforeCalloutDisplayed()
        {
            spawnpoint = new(132.69f, -1308.34f, 29.03f);
            officerheading = 318.26f;
            susSpawn = new(116.04f, -1291.59f, 28.26f);
            susHeading = 246.21f;
            vehicleSpawn = new(140.00f, -1308.37f, 29.00f);
            vehicleHeading = 46.70f;
            ShowCalloutAreaBlipBeforeAccepting(spawnpoint, 100f);
            LSPD_First_Response.Mod.API.Functions.PlayScannerAudio("Adam69Callouts_OfficerDown_Audio");
            CalloutInterfaceAPI.Functions.SendMessage(this, "Officer Down Reported by an unkown civilian");
            CalloutMessage = "Officer Down Reported";
            CalloutPosition = spawnpoint;

            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            Game.LogTrivial("Adam69 Callouts [LOG]: Officer Down callout has been accepted!");
            Game.DisplayNotification("web_adam69callouts", "web_adam69callouts", "~w~Adam69 Callouts", "~w~Officer Down", "~b~Dispatch~w~: The suspect has been spotted! Respond ~r~Code 3~w~.");


            LSPD_First_Response.Mod.API.Functions.PlayScannerAudio("Adam69Callouts_Respond_Code_3_Audio");

            officer = new Ped(pedsList[new Random().Next((int)pedsList.Length)], spawnpoint, 0f);
            officer.IsPersistent = true;
            officer.BlockPermanentEvents = true;
            officer.Kill();
            officer.IsValid();

            NativeFunction.Natives.APPLY_PED_DAMAGE_PACK(officer, "TD_PISTOL_FRONT", 1f, 1f);

            emergencyVehicle = new Vehicle(officerVehicle[new Random().Next((int)officerVehicle.Length)], vehicleSpawn, 0f);
            emergencyVehicle.IsPersistent = true;
            emergencyVehicle.IsValid();

            suspect = new Ped(susSpawn);
            suspect.IsPersistent = true;
            suspect.IsValid();
            suspect.BlockPermanentEvents = true;

            copBlip = officer.AttachBlip();
            copBlip.Color = System.Drawing.Color.Blue;
            copBlip.IsRouteEnabled = true;

            suspectBlip = suspect.AttachBlip();
            suspectBlip.Color = System.Drawing.Color.Red;

            officerVehicleBlip = emergencyVehicle.AttachBlip();
            officerVehicleBlip.Color = System.Drawing.Color.LightBlue;

            if (suspect.IsMale)
                malefemale = "Sir";
            else
                malefemale = "Ma'am";

            counter = 0;

            return base.OnCalloutAccepted();
        }

        public override void OnCalloutNotAccepted()
        {
            if (suspect) suspect.Delete();
            if (suspectBlip) suspectBlip.Delete();
            if (copBlip) copBlip.Delete();
            if (officer) officer.Delete();
            if (officerVehicleBlip) officerVehicleBlip.Delete();
            if (copBlip) copBlip.Delete();
            if (emergencyVehicle) emergencyVehicle.Delete();

            base.OnCalloutNotAccepted();
        }

        public override void Process()
        {
            if (MainPlayer.DistanceTo(officer) <= 10f)
            {
                bool helpMessages = Settings.HelpMessages;
                if (helpMessages)
                {
                    Game.DisplayHelp("Press ~y~" + Settings.Dialog.ToString() + " ~w~to notify dispatch of an officer down.", 5000);
                }

                if (Game.IsKeyDown(Settings.Dialog))
                {
                    counter++;

                    if (counter == 1)
                    {
                        Game.DisplaySubtitle("~b~You~w~: Dispatch, we got an officer down, requesting medic but have them stage a few blocks away from the scene until the scene is secured.");
                    }
                    if (counter == 2)
                    {
                        LSPD_First_Response.Mod.API.Functions.PlayScannerAudio("Adam69Callouts_OfficerDown_Audio_2");
                        UltimateBackup.API.Functions.callAmbulance();
                        UltimateBackup.API.Functions.callCode3Backup();

                    }
                    if (counter == 3)
                    {
                        Game.DisplaySubtitle("~r~Suspect~w~: Time to die, you donut pigs!");
                        suspect.Tasks.FightAgainst(MainPlayer);
                        suspect.Inventory.GiveNewWeapon("WEAPON_COMBATPISTOL", 500, true);
                        suspect.Armor = 1000;
                    }
                    if (counter == 4)
                    {
                        LSPD_First_Response.Mod.API.Functions.PlayScannerAudio("Adam69Callouts_ShotsFired_Audio_Remastered_01");
                        UltimateBackup.API.Functions.callPanicButtonBackup(true);
                    }
                }

                if (MainPlayer.IsDead || Game.IsKeyDown(Settings.EndCall))
                {
                    BigMessageThread bigMessage = new BigMessageThread();
                    bigMessage.MessageInstance.ShowColoredShard("MISSION FAILED!", "You'll get 'em next time!", RAGENativeUI.HudColor.Red, RAGENativeUI.HudColor.Black, 5000);
                    End();
                }
            }

            base.Process();
        }

        public override void End()
        {
            if (officer) officer.Dismiss();
            if (copBlip) copBlip.Delete();
            if (suspect) suspect.Dismiss();
            if (suspectBlip) suspectBlip.Delete();
            if (emergencyVehicle) emergencyVehicle.Delete();
            if (officerVehicleBlip) officerVehicleBlip.Delete();
            LSPD_First_Response.Mod.API.Functions.PlayScannerAudio("Adam69Callouts_Code_4_Audio");
            Game.DisplayNotification("web_adam69callouts", "web_adam69callouts", "~w~Adam69 Callouts", "~w~Officer Down", "~b~You~w~: We are Code 4. Show me back 10-8!");
            base.End();

            BigMessageThread bigMessage = new BigMessageThread();

            bigMessage.MessageInstance.ShowColoredShard("CODE 4", "The scene is now secure.", RAGENativeUI.HudColor.Green, RAGENativeUI.HudColor.Black, 5000);

            Game.LogTrivial("Adam69 Callouts [LOG]: Officer Down callout is code 4!");

        }

    }
}