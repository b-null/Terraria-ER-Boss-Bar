using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace ERBossBar.Content
{
    internal class Config : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [DefaultValue(true)]
        public bool EnableCustomBossBar;
    }
}
