using System;
using System.Globalization;
using PoeHUD.Models.Enums;
using PoeHUD.Plugins;
using PoeHUD.Poe;
using SharpDX;
using SharpDX.Direct3D9;
using Skill_DPS.Skill_Data;

namespace Skill_DPS.Core
{
    public class Main : BaseSettingsPlugin<Settings>
    {
        public Main() => PluginName = "Skill DPS";

        public override void Initialise() { }

        public override void Render()
        {
            base.Render();
            Element HoverUI = GameController.Game.IngameState.UIHoverTooltip.Tooltip;
            foreach (SkillBar.Data skill in SkillBar.CurrentSkills())
            {
                RectangleF box = skill.SkillElement.GetClientRect();
                RectangleF newBox = new RectangleF(box.X, box.Y - 2, box.Width, -15);

                int value = -1;
                int projectileCount = 1;

                if (HoverUI.GetClientRect().Intersects(newBox) && HoverUI.IsVisibleLocal) continue;

                if (skill.Skill.Stats.TryGetValue(GameStat.HundredTimesDamagePerSecond, out int @return))
                    value = @return;

                else if (skill.Skill.Stats.TryGetValue(GameStat.HundredTimesAverageDamagePerHit, out int return2))
                    value = return2;

                if (Settings.XProjectileCount)
                if (skill.Skill.Stats.TryGetValue(GameStat.NumberOfAdditionalProjectiles, out int OutProjCount))
                    projectileCount = OutProjCount;

                if (value <= 0) continue;

                Graphics.DrawText(ToKMB(Convert.ToDecimal((value / (decimal) 100) * projectileCount)),
                        Settings.FontSize,
                        new Vector2(newBox.Center.X, newBox.Center.Y - Settings.FontSize / 2),
                        Settings.FontColor, FontDrawFlags.Center);
                Graphics.DrawBox(newBox, Settings.BackgroundColor);
                Graphics.DrawFrame(newBox, 1, Settings.BorderColor);
            }
        }

        public static string ToKMB(decimal num)
        {
            if (num > 999999999) return num.ToString("0,,,.###B", CultureInfo.InvariantCulture);
            if (num > 999999) return num.ToString("0,,.##M", CultureInfo.InvariantCulture);
            if (num > 999) return num.ToString("0,.#K", CultureInfo.InvariantCulture);
            return num.ToString("0.#", CultureInfo.InvariantCulture);
        }
    }
}