using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

public class BossDamageTracker : GlobalNPC
{
    private int previousLife;
    private int recentDamage;
    private int damageTimer;

    public override bool InstancePerEntity => true;

    public override void AI(NPC npc)
    {
        if (npc.boss && npc.active)
        {
            int damageThisTick = previousLife - npc.life;
            if (damageThisTick > 0)
            {
                recentDamage += damageThisTick;
                damageTimer = 60; // 1 second delay
            }

            if (damageTimer > 0)
                damageTimer--;
            else
                recentDamage = 0;

            previousLife = npc.life;
        }
    }

    public int GetRecentDamage() => recentDamage;

    public float GetYellowBarRatio(NPC npc)
    {
        if (npc.lifeMax == 0) return 0f;
        return MathHelper.Clamp((npc.life + recentDamage) / (float)npc.lifeMax, 0f, 1f);
    }
}
