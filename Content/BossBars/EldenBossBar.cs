using ERBossBar.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;
using static System.Net.Mime.MediaTypeNames;

public class EldenBossBar : GlobalBossBar
{

    private Config config = ModContent.GetInstance<Config>();
    private Texture2D hpBg = ModContent.Request<Texture2D>("ERBossBar/Assets/Textures/HP_BG", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
    private Texture2D hpBase = ModContent.Request<Texture2D>("ERBossBar/Assets/Textures/HP_BASE", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
    private Texture2D hpFill = ModContent.Request<Texture2D>("ERBossBar/Assets/Textures/HP_FILL", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

    private float cachedLife = 0;

    public override void PostDraw(SpriteBatch spriteBatch, NPC npc, BossBarDrawParams drawParams)
    {
        Vector2 pos = new Vector2(Main.screenWidth / 2f, Main.screenHeight - 80);

        float scale = scale = (Main.screenWidth / 1920f) * Main.UIScale * config.BossBarScale;

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


        HandleDamageNumber(pos, spriteBatch, scale, drawParams);
        // Damage number to the right
        
        
    }

    private float previousLife;
    private int recentDamage;
    private ulong dmgUpdateTick;

    private void HandleDamageNumber(Vector2 pos, SpriteBatch spriteBatch, float scale, BossBarDrawParams drawParams)
    {
        // Draw
        if (recentDamage > 0)
        {
            string dmgText = $"{recentDamage}";
            Vector2 dmgPos = pos + new Vector2(hpBg.Width * scale / 2 - FontAssets.MouseText.Value.MeasureString(dmgText).X, -30);
            Utils.DrawBorderString(spriteBatch, dmgText, dmgPos, Color.White, 0.8f * scale, 0f);
        }

        // Calculate
        int damageThisFrame = (int)(previousLife - drawParams.Life);
        if(damageThisFrame > 0)
        {
            recentDamage += damageThisFrame;
            dmgUpdateTick = Main.GameUpdateCount; // Update the tick when damage occurs
        }

        if(Main.GameUpdateCount - dmgUpdateTick > 60)
        {
            // Reset recent damage after 1 second
            recentDamage = 0;
        }

        previousLife = drawParams.Life;

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
        return false;
    }
}