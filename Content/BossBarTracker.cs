using Microsoft.Xna.Framework;
using System;
using Terraria.ModLoader;

namespace ERBossBar.Content
{
    internal class BossBarTracker : ModSystem
    {
        private static int ticks = 0;

        public static void Ping()
        {
            ticks = 0;
        }

        public override void UpdateUI(GameTime gameTime)
        {                        
            if(ticks > 10)
            {
                // Assume boss bar is no longer visible
                if (EldenBossBar.Instance != null)
                {
                    EldenBossBar.Instance.ResetCachedValues();
                }
                else
                {
                    Console.WriteLine("EldenBossBar instance is null, cannot reset cached values.");
                }
            }
            else
            {
                ticks++;
            }
        }
    }
}
