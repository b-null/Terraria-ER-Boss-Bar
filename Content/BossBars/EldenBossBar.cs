using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.UI;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.BigProgressBar;
using Terraria;
using Terraria.DataStructures;
using ERBossBar.Content;

public class EldenBossBar : ModBossBar
{

    private Config config = ModContent.GetInstance<Config>();
    private Texture2D hpBg = ModContent.Request<Texture2D>("ERBossBar/Assets/Textures/HP_BG").Value;
    private Texture2D hpFill = ModContent.Request<Texture2D>("ERBossBar/Assets/Textures/HP_FILL").Value;

    public override void PostDraw(SpriteBatch spriteBatch, NPC npc, BossBarDrawParams drawParams)
    {
        if (!config.EnableCustomBossBar)
        {
            // If the custom boss bar is disabled, use the default behavior
            base.PostDraw(spriteBatch, npc, drawParams);
            return;
        }
        var tracker = npc.GetGlobalNPC<BossDamageTracker>();

        Texture2D whiteTex = Terraria.GameContent.TextureAssets.MagicPixel.Value;

        Vector2 pos = new Vector2(Main.screenWidth / 2f, Main.screenHeight - 50);
        float barWidth = 900f;
        float barHeight = 8f;

        float hpRatio = drawParams.Life / drawParams.LifeMax;
        float delayedRatio = tracker.GetYellowBarRatio(npc);

        // Bar positions
        Rectangle border = new Rectangle(
            (int)(pos.X - barWidth / 2 - 1),
            (int)(pos.Y - 1),
            (int)(barWidth + 2),
            (int)(barHeight + 2)
        );

        Rectangle backgroundBar = new Rectangle(
            (int)(pos.X - barWidth / 2),
            (int)pos.Y,
            (int)(barWidth),
            (int)barHeight
        );

        Rectangle redBar = new Rectangle(
            (int)(pos.X - barWidth / 2),
            (int)pos.Y,
            (int)(barWidth * hpRatio),
            (int)barHeight
        );

        // Draw white (border) bar
        spriteBatch.Draw(whiteTex, border, Color.White);
        // Draw black (background) bar
        spriteBatch.Draw(whiteTex, backgroundBar, Color.Black);
        // Draw red (actual HP) bar on top
        spriteBatch.Draw(whiteTex, redBar, Color.Red);

        // Boss Name centered
        Utils.DrawBorderString(spriteBatch, npc.FullName, pos - new Vector2(barWidth / 2, 30), Color.White, 1f, 0.5f);

        // Damage number to the right
        int damage = tracker.GetRecentDamage();
        if (damage > 0)
        {
            string dmgText = $"{damage}";
            Vector2 dmgPos = pos + new Vector2(barWidth / 2, -30);
            Utils.DrawBorderString(spriteBatch, dmgText, dmgPos, Color.White, 0.8f, 0f);
        }
    }

    public override bool PreDraw(SpriteBatch spriteBatch, NPC npc, ref BossBarDrawParams drawParams)
    {
        if(config.EnableCustomBossBar)
        {
            // Only draw if the custom boss bar is enabled
            return false;
        }
        return base.PreDraw(spriteBatch, npc, ref drawParams);
    }
}