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
        public static List<ushort> CurrentIDS() => BasePlugin.API.GameController.Game.IngameState.ServerData.SkillBarIds;

        public static List<Data> CurrentSkills()
        {
            List<ushort> ids = CurrentIDS();
            List<Data> ReturnSkills = new List<Data>();
            if (ids == null)
            {
                return ReturnSkills;
            }

            for (int index = 0; index < ids.Count; index++)
            {
                if (GetSkill(ids[index]) != null)
                {
                    var Skill = GetSkill(ids[index]);
                    ReturnSkills.Add(new Data
                    {
                            Skill = Skill,
                            SkillStats = GetSkillStats(Skill),
                            SkillElement = BasePlugin.API.GameController.Game.IngameState.IngameUi.SkillBar.Children[index]
                    });
                }
            }

            return ReturnSkills;
        }

        public static ActorSkill GetSkill(ushort ID)
        {
            List<ActorSkill> ActorSkills = BasePlugin.API.GameController.Player.GetComponent<Actor>().ActorSkills;
            if (ActorSkills != null)
            {
                foreach (ActorSkill actorSkill in ActorSkills)
                {
                    if (actorSkill != null && actorSkill.Id == ID)
                    {
                        return actorSkill;
                    }
                }
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