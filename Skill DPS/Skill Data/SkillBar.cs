using System.Collections.Generic;
using System.Linq;
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
            List<Data> ReturnSkills = new List<Data>();
            List<ushort> ids = CurrentIDS();
            for (int index = 0; index < ids.Count; index++)
            {
                ushort @ushort = ids[index];
                if (GetSkill(@ushort) == null) continue;
                ReturnSkills.Add(new Data
                {
                        Skill = GetSkill(@ushort),
                        SkillElement = BasePlugin.API.GameController.Game.IngameState.IngameUi.SkillBar.Children[index]
                });
            }

            return ReturnSkills;
        }

        public static ActorSkill GetSkill(ushort ID)
        {
            foreach (ActorSkill actorSkill in BasePlugin.API.GameController.Player.GetComponent<Actor>().ActorSkills)
            {
                if (actorSkill.Id != ID) continue;

                return actorSkill;
            }

            return null;
        }

        public class Data
        {
            public ActorSkill Skill { get; set; }
            public Element SkillElement { get; set; }
        }
    }
}