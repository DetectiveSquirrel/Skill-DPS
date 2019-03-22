using System.Windows.Forms;
using PoeHUD.Hud.Settings;
using PoeHUD.Plugins;
using SharpDX;

namespace Skill_DPS.Core
{
    public class Settings : SettingsBase
    {
        [Menu("Font Size")]
        public RangeNode<int> FontSize { get; set; } = new RangeNode<int>(15, 1, 50);

        [Menu("Font Color")]
        public ColorNode FontColor { get; set; } = new Color(216, 216, 216, 255);

        [Menu("Highest DPS Font Color")]
        public ColorNode HighestDPSFontColor { get; set; } = new Color(216, 216, 216, 255);

        [Menu("Background Color")]
        public ColorNode BackgroundColor { get; set; } = new Color(0, 0, 0, 255);

        [Menu("Border Color")]
        public ColorNode BorderColor { get; set; } = new Color(146, 107, 43, 255);

        [Menu("DPS x # of Projectiles")]
        public ToggleNode XProjectileCount { get; set; } = false;

        [Menu("Update Interval (ms)")]
        public RangeNode<int> UpdateInterval { get; set; } = new RangeNode<int>(500, 1, 2000);

        [Menu("Store Highest DPS Per Skill")]
        public ToggleNode EnableCachedDPS { get; set; } = true;

        [Menu("Button To CLear Highested Stored DPS")]
        public HotkeyNode ClearCachedDPS { get; set; } = Keys.PageDown;
    }
}