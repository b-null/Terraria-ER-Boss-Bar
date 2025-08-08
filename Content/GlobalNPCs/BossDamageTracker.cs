using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

public class BossDamageTracker : GlobalNPC
{
    private int previousLife;
    private int recentDamage;
    private int damageTimer;

    public override bool InstancePerEntity => true;

    public override void SetDefaults(NPC entity)
    {
        if (entity.boss)
        {
            entity.BossBar = ModContent.GetInstance<EldenBossBar>();
        }
    }

    public int GetRecentDamage() => recentDamage;

    public float GetYellowBarRatio(NPC npc)
    {
        if (npc.lifeMax == 0) return 0f;
        return MathHelper.Clamp((npc.life + recentDamage) / (float)npc.lifeMax, 0f, 1f);
    }

    public override bool PreAI(NPC npc)
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
        return base.PreAI(npc);
    }
}
