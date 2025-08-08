using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace ERBossBar.Content.GlobalNPCs
{
    internal class ERBossBarGlobalNPC : GlobalNPC
    {
        public override void SetDefaults(NPC entity)
        {
            if (entity.boss)
            {
                entity.BossBar = ModContent.GetInstance<EldenBossBar>();
            }
        }
    }
}
