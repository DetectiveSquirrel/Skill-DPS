using System;
using System.Collections.Generic;
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
            if (GameController.Game.IsGameLoading) return;

            Element HoverUI = GameController.Game.IngameState.UIHoverTooltip.Tooltip;
            if (HoverUI != null)
            {
                foreach (SkillBar.Data skill in SkillBar.CurrentSkills())
                {
                    if (skill != null)
                    {
                        RectangleF box = skill.SkillElement.GetClientRect();
                        RectangleF newBox = new RectangleF(box.X, box.Y - 2, box.Width, -15);

                        int Value = -1;
                        int Projectiles = 1;
                        Dictionary<GameStat, int> Stats = skill.SkillStats;

                        if (!HoverUI.GetClientRect().Intersects(newBox) || !HoverUI.IsVisibleLocal)
                        {
                            if (Stats != null && Stats.TryGetValue(GameStat.HundredTimesDamagePerSecond, out int HTDPS))
                            {
                                Value = HTDPS;
                            }
                            else if (Stats != null && Stats.TryGetValue(GameStat.HundredTimesAverageDamagePerHit, out int HTADPS))
                            {
                                Value = HTADPS;
                            }

                            if (Settings.XProjectileCount)
                            {
                                if (Stats != null && Stats.TryGetValue(GameStat.NumberOfAdditionalProjectiles, out int NOAP))
                                {
                                    Projectiles = NOAP;
                                }
                            }

                            if (Value > 0)
                            {
                                string text = ToKMB(Convert.ToDecimal(Value / (decimal) 100 * Projectiles));
                                Vector2 position = new Vector2(newBox.Center.X, newBox.Center.Y - Settings.FontSize / 2);
                                Graphics.DrawText(text, Settings.FontSize, position, Settings.FontColor, FontDrawFlags.Center);
                                Graphics.DrawBox(newBox, Settings.BackgroundColor);
                                Graphics.DrawFrame(newBox, 1, Settings.BorderColor);
                            }
                        }
                    }
                }
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