﻿namespace Adam69Callouts.Stuff
{
    // Credits to Astro and Khorio or Echoo.

    internal static class Helper
    {
        internal static Ped MainPlayer => Game.LocalPlayer.Character;
        internal static Random Rndm = new(DateTime.Now.Millisecond);
    }
}
