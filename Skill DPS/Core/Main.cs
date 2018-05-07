using ImGuiNET;
using PoeHUD.Plugins;
using Test_Environment.Utilities;

namespace Test_Environment.Core
{
    public class Main : BaseSettingsPlugin<Settings>
    {
        public Main() => PluginName = "???????";

        public override void Initialise() { }

        public override void Render()
        {
            base.Render();
            RenderMenu();
        }

        private void RenderMenu()
        {
            var idPop = 1;
            if (!Settings.ShowWindow) return;
            ImGuiExtension.BeginWindow($"{PluginName} Settings", Settings.LastSettingPos.X, Settings.LastSettingPos.Y, Settings.LastSettingSize.X, Settings.LastSettingSize.Y);

            // Storing window Position and Size changed by the user
            if (ImGui.GetWindowHeight() > 21)
            {
                Settings.LastSettingPos = ImGui.GetWindowPosition();
                Settings.LastSettingSize = ImGui.GetWindowSize();
            }

            ImGui.EndWindow();
        }
    }
}