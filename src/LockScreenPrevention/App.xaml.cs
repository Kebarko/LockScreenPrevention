using System.Reflection;
using System.Windows;

namespace KE.LockScreenPrevention
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        private static readonly Assembly assembly = Assembly.GetExecutingAssembly();
        private static readonly string appName = assembly.GetName().Name!;
        private static readonly string appPath = assembly.Location;

        private readonly ToolStripMenuItem autoStartItem;
        private readonly ToolStripMenuItem enabledItem;
        private readonly ToolStripMenuItem exitItem;

        public App()
        {
            var icon = new NotifyIcon();
            icon.Icon = LockScreenPrevention.Properties.Resources.LockScreenIcon;

            icon.ContextMenuStrip = new ContextMenuStrip();
            icon.ContextMenuStrip.Items.Add(autoStartItem = new ToolStripMenuItem("Auto-Start"));
            icon.ContextMenuStrip.Items.Add(enabledItem = new ToolStripMenuItem("Enabled"));
            icon.ContextMenuStrip.Items.Add(exitItem = new ToolStripMenuItem("Exit") { Image = LockScreenPrevention.Properties.Resources.ExitIcon.ToBitmap() });
            icon.ContextMenuStrip.ItemClicked += ContextMenuStrip_ItemClicked;

            icon.Visible = true;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            autoStartItem.Checked = Utils.IsInStartup(appName, appPath);

            Utils.Enable();
            enabledItem.Checked = true;
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            Utils.Disable();
        }

        private void ContextMenuStrip_ItemClicked(object? sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem == autoStartItem)
            {
                if (autoStartItem.Checked)
                {
                    Utils.RemoveFromStartup(appName);
                }
                else
                {
                    Utils.AddToStartup(appName, appPath);
                }

                autoStartItem.Checked = !autoStartItem.Checked;
            }
            else if (e.ClickedItem == enabledItem)
            {
                if (enabledItem.Checked)
                {
                    Utils.Disable();
                }
                else
                {
                    Utils.Enable();
                }

                enabledItem.Checked = !enabledItem.Checked;
            }
            else if (e.ClickedItem == exitItem)
            {
                Shutdown();
            }
        }
    }
}
