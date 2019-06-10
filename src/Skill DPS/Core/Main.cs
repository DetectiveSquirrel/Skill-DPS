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
// ReSharper disable All

namespace Skill_DPS.Core
{
    public class StoredSkillData
    {
        public ushort SkillId;
        public int HighestDamage;

        public StoredSkillData(ushort skillid, int highestdamage)
        {
            SkillId = skillid;
            HighestDamage = highestdamage;
        }
    }


    public class Main : BaseSettingsPlugin<Settings>
    {
        private readonly Stopwatch _updateTick = Stopwatch.StartNew();


        private readonly bool _renderStuff = true;

        private List<SkillBar.Data> _skillCache = new List<SkillBar.Data>();
        private List<StoredSkillData> _topSkillIdDamage = new List<StoredSkillData>();


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
            if (!_renderStuff) return;

            if (Settings.ClearCachedDps.PressedOnce()) _topSkillIdDamage.Clear();
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
                if (_updateTick.ElapsedMilliseconds > Settings.UpdateInterval)
                {
                    if (!CanTick())
                        return;
                    _skillCache = SkillBar.CurrentSkills();
                    _updateTick.Restart();
                }

                var hoverUi = GameController.Game.IngameState.UIHover.Tooltip;
                if (hoverUi.Address == null) return;
                foreach (var skill in _skillCache)
                {
                    if (skill == null)
                        continue;

                    var box = skill.SkillElement.GetClientRect();
                    var newBox = new RectangleF(box.X, box.Y - 2, box.Width, -15);
                    var value = -1;
                    var projectiles = 1;

                    if (hoverUi.GetClientRect().Intersects(newBox) && hoverUi.IsVisible)
                        continue;

                    if (skill.SkillStats != null)
                    {

                        value = (int)skill.Skill.Dps;
                        if (value <= 0)
                        {
                            if (TryGetStat(GameStat.HundredTimesAverageDamagePerHit, skill.SkillStats) > 0)
                                value = TryGetStat(GameStat.HundredTimesAverageDamagePerHit, skill.SkillStats) / 100;

                            else if (TryGetStat(GameStat.HundredTimesAverageDamagePerSkillUse, skill.SkillStats) > 0)
                                value = TryGetStat(GameStat.HundredTimesAverageDamagePerSkillUse, skill.SkillStats) / 100;
                        }
                    }

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
                    var text = ToKmb(value);
                    //string text = ToKMB(Convert.ToDecimal(Value / (decimal) 100 * Projectiles));
                    var position = new Vector2(newBox.Center.X, newBox.Center.Y - Settings.FontSize / 2);
                    Graphics.DrawText(text, Settings.FontSize, position, Settings.FontColor, FontDrawFlags.Center);
                    Graphics.DrawBox(newBox, Settings.BackgroundColor);
                    Graphics.DrawFrame(newBox, 1, Settings.BorderColor);


                    if (Settings.EnableCachedDps)
                    {
                        var containsItem = _topSkillIdDamage.Any(item => item.SkillId == skill.Skill.Id);
                        var highestDps = 0;

                        if (!containsItem)
                            _topSkillIdDamage.Add(new StoredSkillData(skill.Skill.Id, value));
                        else
                            foreach (var data in _topSkillIdDamage)
                                if (data.SkillId == skill.Skill.Id)
                                {
                                    if (data.HighestDamage < value)
                                        data.HighestDamage = value;

                                    highestDps = data.HighestDamage;
                                }

                        var topNewBox = new RectangleF(box.X, box.Y - 2 - 15, box.Width, -15);
                        var topText = ToKmb(highestDps);
                        //string text = ToKMB(Convert.ToDecimal(Value / (decimal) 100 * Projectiles));
                        var topPosition = new Vector2(newBox.Center.X, newBox.Center.Y - 15 - Settings.FontSize / 2);
                        Graphics.DrawText(topText, Settings.FontSize, topPosition, Settings.HighestDpsFontColor, FontDrawFlags.Center);
                        Graphics.DrawBox(topNewBox, Settings.BackgroundColor);
                        Graphics.DrawFrame(topNewBox, 1, Settings.BorderColor);
                    }
                }
            }
            catch (Exception e)
            {
                //LogError(e, 10);
            }
        }

        private int TryGetStat(GameStat stat, Dictionary<GameStat, int> statList)
        {
            return statList.TryGetValue(stat, out var statInt) ? statInt : 0;
        }

        public static string ToKmb(int num)
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