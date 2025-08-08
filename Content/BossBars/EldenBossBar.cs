using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using ERBossBar.Content;
using System;
using Terraria.GameContent;

public class EldenBossBar : ModBossBar
{

    private Config config = ModContent.GetInstance<Config>();
    private Texture2D hpBg = ModContent.Request<Texture2D>("ERBossBar/Assets/Textures/HP_BG", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
    private Texture2D hpFill = ModContent.Request<Texture2D>("ERBossBar/Assets/Textures/HP_FILL", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

    public override void PostDraw(SpriteBatch spriteBatch, NPC npc, BossBarDrawParams drawParams)
    {
        if (!config.EnableCustomBossBar)
        {
            // If the custom boss bar is disabled, use the default behavior
            base.PostDraw(spriteBatch, npc, drawParams);
            return;
        }

        if (hpBg == null)
            Console.WriteLine("hpBg texture not found!");
        if(hpFill == null)
            Console.WriteLine("hpFill texture not found!");

        var tracker = npc.GetGlobalNPC<BossDamageTracker>();

        Vector2 pos = new Vector2(Main.screenWidth / 2f, Main.screenHeight - 80);

        float scale = scale = (Main.screenWidth / 1920f) * (Main.UIScale);

        float hpRatio = drawParams.Life / drawParams.LifeMax;
        float delayedRatio = tracker.GetYellowBarRatio(npc);

        // Draw background
        spriteBatch.Draw(hpBg, pos - new Vector2(hpBg.Width * scale / 2, 0), null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        // Draw fill
        int fillWidth = (int)((hpFill.Width * hpRatio) * scale);
        Rectangle fillRect = new Rectangle(0, 0, (int)(fillWidth / scale), hpFill.Height);
        spriteBatch.Draw(hpFill, pos - new Vector2(hpBg.Width * scale / 2, 0), fillRect, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);

        // Boss Name centered
        Vector2 nameSize = FontAssets.MouseText.Value.MeasureString(npc.FullName);
        

        Utils.DrawBorderString(spriteBatch, npc.FullName, new Vector2(pos.X - hpBg.Width * scale / 2 + 20, pos.Y - 30), Color.White, 1f * scale, 0f);

        // Damage number to the right
        int damage = tracker.GetRecentDamage();
        if (damage > 0)
        {
            string dmgText = $"{damage}";
            Vector2 dmgPos = pos + new Vector2(hpBg.Width * scale / 2 - FontAssets.MouseText.Value.MeasureString(dmgText).X, -30);
            Utils.DrawBorderString(spriteBatch, dmgText, dmgPos, Color.White, 0.8f * scale, 0f);
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