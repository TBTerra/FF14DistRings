using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using System.IO;
using Dalamud.Interface.Windowing;
using DistRings.Windows;
using Dalamud.Game.ClientState;
using Dalamud.Game.Gui;

namespace DistRings
{
    public partial class Plugin : IDalamudPlugin {
        public string Name => "Distance Rings";
        private const string CommandName = "/drings";

        private DalamudPluginInterface PluginInterface { get; init; }
        private CommandManager CommandManager { get; init; }
        public readonly ClientState CS;
        private readonly GameGui gui_;
        public Configuration Configuration { get; init; }
        public WindowSystem WindowSystem = new("DistRings");

        private ConfigWindow ConfigWindow { get; init; }

        public Plugin(
            [RequiredVersion("1.0")] DalamudPluginInterface pluginInterface,
            [RequiredVersion("1.0")] CommandManager commandManager,
            ClientState clientState,
            GameGui gameGui) {
            this.PluginInterface = pluginInterface;
            this.CommandManager = commandManager;
            this.CS = clientState;
            this.gui_ = gameGui;

            this.Configuration = this.PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            this.Configuration.Initialize(this.PluginInterface);

            // you might normally want to embed resources and load them from the manifest stream
            //var imagePath = Path.Combine(PluginInterface.AssemblyLocation.Directory?.FullName!, "goat.png");
            //var goatImage = this.PluginInterface.UiBuilder.LoadImage(imagePath);

            ConfigWindow = new ConfigWindow(this);
            WindowSystem.AddWindow(ConfigWindow);

            this.CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand) {
                HelpMessage = "Shows the config. use 'on' and 'off' to quick enable/disable the rings"
            });

            this.PluginInterface.UiBuilder.Draw += doDraw;
            this.PluginInterface.UiBuilder.Draw += DrawUI;
            this.PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;
        }

        public void Dispose() {
            this.WindowSystem.RemoveAllWindows();
            ConfigWindow.Dispose();
            this.CommandManager.RemoveHandler(CommandName);
        }

        private void OnCommand(string command, string args) {
            if (args=="")
            {
                ConfigWindow.IsOpen = true;
            }else if (args == "off") {
                Configuration.RingsEnabled = false;
            }else if (args == "on") {
                Configuration.RingsEnabled = true;
            }
            
        }

        private void DrawUI() {
            this.WindowSystem.Draw();
        }

        public void DrawConfigUI() {
            ConfigWindow.IsOpen = true;
        }
    }
}
