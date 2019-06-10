using System;
using System.Collections.Generic;
using PoeHUD.Models.Enums;
using PoeHUD.Plugins;
using PoeHUD.Poe;
using PoeHUD.Poe.Components;
using PoeHUD.Poe.RemoteMemoryObjects;

namespace Skill_DPS.Skill_Data
{
    public class SkillBar
    {
        public static List<ushort> CurrentIds() => BasePlugin.API.GameController.Game.IngameState.ServerData.SkillBarIds;

        public static List<Data> CurrentSkills()
        {
            List<Data> returnSkills = new List<Data>();
            try
            {
                List<ushort> ids = CurrentIds();
                if (ids == null) return returnSkills;
                if (ids.Count > 100)
                {
                    BasePlugin.API.LogError("CurrentIDS.Count > 500", 10);
                    return returnSkills;
                }
                //BasePlugin.API.LogError($"ids Count: {ids.Count}", 10);

                for (int index = 0; index < ids.Count; index++)
                {
                    if (GetSkill(ids[index]) == null) continue;

                    ActorSkill skill = GetSkill(ids[index]);

                    returnSkills.Add(new Data
                    {
                            Skill = skill,
                            SkillStats = skill.Stats,
                            SkillElement = BasePlugin.API.GameController.Game.IngameState.IngameUi.SkillBar.Children[index]
                    });
                }
            }
            catch (Exception e)
            {
                BasePlugin.API.LogError(e, 10);
            }

            return returnSkills;
        }

        public static ActorSkill GetSkill(ushort id)
        {
            try
            {
                List<ActorSkill> actorSkills = BasePlugin.API.GameController.Player.GetComponent<Actor>().ActorSkills;
                if (actorSkills != null)
                {
                    foreach (ActorSkill skill in actorSkills)
                    {
                        if (skill.Id == id) return skill;
                    }
                }
            }
            catch (Exception e)
            {
                BasePlugin.API.LogError(e, 10);
            }

            return null;
        }

        public static Dictionary<GameStat, int> GetSkillStats(ActorSkill skill)
        {
            return skill.Stats;
        }

        public class Data
        {
            public ActorSkill Skill { get; set; }
            public Dictionary<GameStat, int> SkillStats { get; set; }
            public Element SkillElement { get; set; }
        }
    }
}