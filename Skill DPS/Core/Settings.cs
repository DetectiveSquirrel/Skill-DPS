using PoeHUD.Hud.Settings;
using PoeHUD.Plugins;
using ImGuiVector2 = System.Numerics.Vector2;

namespace Test_Environment.Core
{
    public class Settings : SettingsBase
    {
        public Settings()
        {
            ShowWindow = false;
            var centerPos = BasePlugin.API.GameController.Window.GetWindowRectangle().Center;
            LastSettingSize = new ImGuiVector2(620, 376);
            LastSettingPos = new ImGuiVector2(centerPos.X - LastSettingSize.X / 2, centerPos.Y - LastSettingSize.Y / 2);
        }

        [Menu("Show ImGui Settings")]
        public ToggleNode ShowWindow { get; set; }

        public ImGuiVector2 LastSettingPos { get; set; }
        public ImGuiVector2 LastSettingSize { get; set; }
    }
}