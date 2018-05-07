﻿using PoeHUD.Hud.Settings;
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

        [Menu("Background Color")]
        public ColorNode BackgroundColor { get; set; } = new Color(0, 0, 0, 255);

        [Menu("Border Color")]
        public ColorNode BorderColor { get; set; } = new Color(146, 107, 43, 255);
    }
}