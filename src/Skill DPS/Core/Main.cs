using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using PoeHUD.Framework;
using PoeHUD.Models.Enums;
using PoeHUD.Plugins;
using PoeHUD.Poe;
using SharpDX;
using SharpDX.Direct3D9;
using Skill_DPS.Skill_Data;

namespace Skill_DPS.Core
{

    public class StoredSkillData
    {
        public ushort SkillID;
        public int HighestDamage;

        public StoredSkillData(ushort skillid, int highestdamage)
        {
            SkillID = skillid;
            HighestDamage = highestdamage;
        }
    }


    public class Main : BaseSettingsPlugin<Settings>
    {
        private readonly Stopwatch UpdateTick = Stopwatch.StartNew();


        private readonly bool RenderStuff = true;

        private List<SkillBar.Data> SkillCache = new List<SkillBar.Data>();
        private List<StoredSkillData> TopSkillIDDamage = new List<StoredSkillData>();


        public Main()
        {
            PluginName = "Skill DPS";
        }

        public override void Initialise()
        {
        }
        

        public override void Render()
        {
            base.Render();
            if (!RenderStuff) return;

            if (Settings.ClearCachedDPS.PressedOnce())
            {
                TopSkillIDDamage.Clear();
            }
            ShowDps();
        }

        private IEnumerator PauseRender()
        {
            yield return new WaitFunction(() => GameController.Game.IsGameLoading);
        }

        public bool CanTick()
        {
            if (GameController.IsLoading)
                return false;
            if (!GameController.Game.IngameState.ServerData.IsInGame)
                return false;
            if (GameController.Player == null || GameController.Player.Address == 0 || !GameController.Player.IsValid)
                return false;
            if (!GameController.Window.IsForeground())
                return false;
            //else if (Core.Cache.InTown)
            //{
            //    //TreeRoutine.LogMessage("Player is in town.", 0.2f);
            //    return false;
            //}
            return true;
        }

        private void ShowDps()
        {
            try
            {
                if (UpdateTick.ElapsedMilliseconds > Settings.UpdateInterval)
                {
                    if (!CanTick())
                        return;
                    SkillCache = SkillBar.CurrentSkills();
                    UpdateTick.Restart();
                }

                Element HoverUI = GameController.Game.IngameState.UIHover.Tooltip;
                if (HoverUI.Address == null) return;
                foreach (SkillBar.Data skill in SkillCache)
                {
                    if (skill == null)
                        continue;

                    RectangleF box = skill.SkillElement.GetClientRect();
                    RectangleF newBox = new RectangleF(box.X, box.Y - 2, box.Width, -15);
                    int value = -1;
                    int Projectiles = 1;
                    
                    if (HoverUI.GetClientRect().Intersects(newBox) && HoverUI.IsVisible)
                        continue;
                    
                    if (skill.SkillStats != null)
                        if (TryGetStat(GameStat.HundredTimesDamagePerSecond, skill.SkillStats) > 0)
                            value = TryGetStat(GameStat.HundredTimesDamagePerSecond, skill.SkillStats);

                        else if (TryGetStat(GameStat.HundredTimesAverageDamagePerSkillUse, skill.SkillStats) > 0)
                            value = TryGetStat(GameStat.HundredTimesAverageDamagePerSkillUse, skill.SkillStats);

                    //if (Settings.XProjectileCount)
                    //{
                    //    if (Stats != null && Stats.TryGetValue(GameStat.NumberOfAdditionalProjectiles, out int NOAP))
                    //    {
                    //        Projectiles = NOAP;
                    //    }
                    //}

                    //LogMessage($"Skill: {skill.Skill.Id}, value: {Value}, stat: {TryGetStat(GameStat.HundredTimesAverageDamagePerHit, skill.SkillStats)}", 1);
                    //Graphics.DrawFrame(box, 1, Color.Red);

                    if (value <= 0) continue;

                    int textValue = value / 100;
                    string text = ToKMB(textValue);
                    //string text = ToKMB(Convert.ToDecimal(Value / (decimal) 100 * Projectiles));
                    Vector2 position = new Vector2(newBox.Center.X, newBox.Center.Y - Settings.FontSize / 2);
                    Graphics.DrawText(text, Settings.FontSize, position, Settings.FontColor, FontDrawFlags.Center);
                    Graphics.DrawBox(newBox, Settings.BackgroundColor);
                    Graphics.DrawFrame(newBox, 1, Settings.BorderColor);


                    if (Settings.EnableCachedDPS)
                    {
                        bool containsItem = TopSkillIDDamage.Any(item => item.SkillID == skill.Skill.Id);
                        var highestDPS = 0;

                        if (!containsItem)
                        {
                            TopSkillIDDamage.Add(new StoredSkillData(skill.Skill.Id, value));
                        }
                        else
                        {
                            foreach (var data in TopSkillIDDamage)
                            {
                                if (data.SkillID == skill.Skill.Id)
                                {
                                    if (data.HighestDamage < value)
                                        data.HighestDamage = value;

                                    highestDPS = data.HighestDamage;
                                }
                            }
                        }

                        RectangleF TOP_newBox = new RectangleF(box.X, box.Y - 2 - 15, box.Width, -15);
                        int TOP_textValue = highestDPS / 100;
                        string TOP_text = ToKMB(TOP_textValue);
                        //string text = ToKMB(Convert.ToDecimal(Value / (decimal) 100 * Projectiles));
                        Vector2 TOP_position = new Vector2(newBox.Center.X, newBox.Center.Y - 15 - Settings.FontSize / 2);
                        Graphics.DrawText(TOP_text, Settings.FontSize, TOP_position, Settings.HighestDPSFontColor, FontDrawFlags.Center);
                        Graphics.DrawBox(TOP_newBox, Settings.BackgroundColor);
                        Graphics.DrawFrame(TOP_newBox, 1, Settings.BorderColor);
                    }
                }
            }
            catch (Exception e)
            {
                LogError(e, 10);
            }
        }

        private int TryGetStat(GameStat stat, Dictionary<GameStat, int> statList)
        {
            return statList.TryGetValue(stat, out int statInt) ? statInt : 0;
        }

        public static string ToKMB(int num)
        {
            try
            {
                if (num > 999999999) return num.ToString("0,,,.###B", CultureInfo.InvariantCulture);
                if (num > 999999) return num.ToString("0,,.##M", CultureInfo.InvariantCulture);
                if (num > 999) return num.ToString("0,.#K", CultureInfo.InvariantCulture);
                return num.ToString("0.#", CultureInfo.InvariantCulture);
            }
            catch (Exception e)
            {
                LogError(e, 10);
            }

            return num.ToString("0.#", CultureInfo.InvariantCulture);
        }
    }
}