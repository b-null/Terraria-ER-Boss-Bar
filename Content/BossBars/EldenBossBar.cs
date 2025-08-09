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
    private Texture2D hpBase = ModContent.Request<Texture2D>("ERBossBar/Assets/Textures/HP_BASE", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
    private Texture2D hpFill = ModContent.Request<Texture2D>("ERBossBar/Assets/Textures/HP_FILL", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

    private float cachedLife = 0;

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

        float dmgRatio = cachedLife / drawParams.LifeMax;

        float hpRatio = drawParams.Life / drawParams.LifeMax;

        // Update cached life to current life
        cachedLife = drawParams.Life;

        // Draw background
        spriteBatch.Draw(hpBg, pos - new Vector2(hpBg.Width * scale / 2, 0), null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);

        // Draw damage (yellow) bar
        int fillDmgWidth = GetDmgFillWidth(dmgRatio, hpRatio, scale);
        Rectangle dmgRect = new Rectangle(0, 0, (int)((fillDmgWidth) / scale), hpFill.Height - 5);
        spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Vector2(Main.screenWidth / 2 + 15 - hpBg.Width * scale / 2, Main.screenHeight - 78), dmgRect, new Color(158, 136, 37), 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);

        // Draw fill
        int fillWidth = (int)(hpFill.Width * hpRatio * scale);
        Rectangle fillRect = new Rectangle(0, 0, (int)(fillWidth / scale), hpFill.Height);
        spriteBatch.Draw(hpFill, new Vector2(Main.screenWidth / 2 + 15 - (hpBg.Width * scale / 2), Main.screenHeight - 80), fillRect, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);

        // Draw the end marker
        float fillEndX = pos.X + 14 - hpBg.Width / 2 + fillRect.Width * scale; // X coordinate where fill ends
        float markerHeight = hpFill.Height * scale; // Match bar height
        float markerWidth = 2f * scale; // Small thin bar
        spriteBatch.Draw(TextureAssets.MagicPixel.Value,
            new Rectangle((int)fillEndX, (int)pos.Y, (int)markerWidth, (int)markerHeight),
            new Color(230, 230, 196));

        // Draw base
        spriteBatch.Draw(hpBase, pos - new Vector2(hpBase.Width * scale / 2, 0), null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);

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


    private float markedRatio;
    private bool animating = false;
    private ulong startTick = 0;

    public int GetDmgFillWidth(float dmgRatio, float hpRatio, float scale)
    {
        
        if (!animating && dmgRatio > hpRatio)
        {
            //Mark it
            markedRatio = dmgRatio;
            animating = true;
            startTick = Main.GameUpdateCount;
        }

        if (animating)
        {
            // Later inside PostDraw
            ulong elapsedTicks = Main.GameUpdateCount - startTick;
            if(elapsedTicks >= 60 && elapsedTicks < 90)
            {
                // Begin shrinking
                float diff = markedRatio - hpRatio;
                float step = (diff / 30f) * (elapsedTicks - 60);
                return (int)(hpFill.Width * (markedRatio - step) * scale);
            }else if(elapsedTicks >= 90)
            {
                // End animation
                animating = false;
                return (int)(hpFill.Width * hpRatio * scale);
            }
            else
            {
                // Freeze for 1 second.
                return (int)(hpFill.Width * markedRatio * scale);
            }
        }

        return (int)(hpFill.Width * hpRatio * scale);
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