using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace ERBossBar.Content
{
    internal class Config : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [Range(0.3f, 2f)]
        [DefaultValue(1f)]
        public float BossBarScale;
    }
}
