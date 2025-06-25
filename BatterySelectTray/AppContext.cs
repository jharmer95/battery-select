using Microsoft.Win32;

using PowerModeSelect;

using System.Runtime.InteropServices;

namespace BatterySelectTray;

public class AppContext : ApplicationContext
{
    private readonly NotifyIcon _trayIcon;

    public AppContext()
    {
        _trayIcon = new NotifyIcon()
        {
            Icon = null,
            ContextMenuStrip = new ContextMenuStrip()
            {
                Items =
                {
                    new ToolStripMenuItem("Efficiency", null, SetEfficiencyMode),
                    new ToolStripMenuItem("Balanced", null, SetBalancedMode),
                    new ToolStripMenuItem("Performance", null, SetPerformanceMode),
                    new ToolStripMenuItem("Exit", null, Exit)
                }
            },
            Visible = true
        };

        _trayIcon.Click += ShowContext;
        _trayIcon.ContextMenuStrip.Opening += HandleUpdateStatus;

        UpdateTrayIcon();

        SystemEvents.PowerModeChanged += HandleUpdateStatus;
    }

    private void SetEfficiencyMode(object? sender, EventArgs e)
    {
        PowerSystem.SetPowerMode(PowerMode.Efficiency);

        if (_trayIcon.ContextMenuStrip == null)
        {
            return;
        }

        UpdateTrayIcon();
    }

    private void SetBalancedMode(object? sender, EventArgs e)
    {
        PowerSystem.SetPowerMode(PowerMode.Balanced);

        if (_trayIcon.ContextMenuStrip == null)
        {
            return;
        }

        UpdateTrayIcon();
    }

    private void SetPerformanceMode(object? sender, EventArgs e)
    {
        PowerSystem.SetPowerMode(PowerMode.Performance);

        if (_trayIcon.ContextMenuStrip == null)
        {
            return;
        }

        UpdateTrayIcon();
    }

    private void Exit(object? sender, EventArgs e)
    {
        SystemEvents.PowerModeChanged -= HandleUpdateStatus;

        if (_trayIcon.ContextMenuStrip != null)
        {
            _trayIcon.ContextMenuStrip.Opening -= HandleUpdateStatus;
            _trayIcon.ContextMenuStrip.Hide();
        }

        _trayIcon.Click -= ShowContext;

        _trayIcon.Visible = false;
        _trayIcon.Dispose();
        Application.Exit();
    }

    private void ShowContext(object? sender, EventArgs e)
    {
        UpdateTrayIcon();

        if (_trayIcon.ContextMenuStrip == null)
        {
            return;
        }

        SetForegroundWindow(new HandleRef(_trayIcon.ContextMenuStrip, _trayIcon.ContextMenuStrip.Handle));
        _trayIcon.ContextMenuStrip.Show(Cursor.Position);
    }

    [DllImport("User32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
    private static extern bool SetForegroundWindow(HandleRef hWnd);

    private void HandleUpdateStatus(object? sender, EventArgs e)
    {
        UpdateTrayIcon();
    }

    private void UpdateTrayIcon()
    {
        var currentPowerMode = PowerSystem.GetCurrentPowerMode();

        if (PowerSystem.IsAcPower())
        {
            _trayIcon.Icon = currentPowerMode switch
            {
                PowerMode.Efficiency => Properties.Resources.EffAc,
                PowerMode.Balanced => Properties.Resources.BalancedAc,
                PowerMode.Performance => Properties.Resources.PerfAc,
                _ => throw new InvalidOperationException("Invalid PowerMode detected")
            };

            var modeName = currentPowerMode.ToFriendlyName();
            _trayIcon.Text = $"Power Mode: {modeName} (AC)";
        }
        else
        {
            _trayIcon.Icon = currentPowerMode switch
            {
                PowerMode.Efficiency => Properties.Resources.EffBatt,
                PowerMode.Balanced => Properties.Resources.BalancedBatt,
                PowerMode.Performance => Properties.Resources.PerfBatt,
                _ => throw new InvalidOperationException("Invalid PowerMode detected")
            };

            var modeName = currentPowerMode.ToFriendlyName();
            _trayIcon.Text = $"Power Mode: {modeName} (Battery)";
        }

        if (_trayIcon.ContextMenuStrip == null)
        {
            return;
        }

        _trayIcon.ContextMenuStrip.Items[0].Font = currentPowerMode == PowerMode.Efficiency ? new Font(_trayIcon.ContextMenuStrip.Items[0].Font, FontStyle.Bold) : new Font(_trayIcon.ContextMenuStrip.Items[0].Font, FontStyle.Regular);
        _trayIcon.ContextMenuStrip.Items[1].Font = currentPowerMode == PowerMode.Balanced ? new Font(_trayIcon.ContextMenuStrip.Items[1].Font, FontStyle.Bold) : new Font(_trayIcon.ContextMenuStrip.Items[1].Font, FontStyle.Regular);
        _trayIcon.ContextMenuStrip.Items[2].Font = currentPowerMode == PowerMode.Performance ? new Font(_trayIcon.ContextMenuStrip.Items[2].Font, FontStyle.Bold) : new Font(_trayIcon.ContextMenuStrip.Items[2].Font, FontStyle.Regular);
    }
}