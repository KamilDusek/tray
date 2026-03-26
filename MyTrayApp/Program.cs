namespace MyTrayApp;

using System;
using System.Windows.Forms;

class Program : Form
{
    private readonly NotifyIcon trayIcon;
    private readonly ContextMenuStrip trayMenu;

    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new Program());
    }

    public Program()
    {
        // Set up the form
        this.Text = "My Tray App";
        this.Size = new System.Drawing.Size(400, 300);
        this.FormClosing += OnFormClosing;
        this.Resize += OnResize;

        // Set up the tray icon
        trayIcon = new NotifyIcon
        {
            Icon = new System.Drawing.Icon("app.ico"),
            Text = "TrayApp",
            Visible = true
        };

        // Set up the tray menu
        trayMenu = new ContextMenuStrip();
        trayMenu.Items.Add("Open", null, OnTrayIconClick);
        trayMenu.Items.Add("Exit", null, OnExit);

        trayIcon.ContextMenuStrip = trayMenu;

        // Handle tray icon click to restore the window
        trayIcon.MouseClick += OnTrayIconMouseClick;
    }

    private void OnTrayIconMouseClick(object? sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            // Restore the window state
            if (this.WindowState == FormWindowState.Minimized || !this.Visible)
            {
                this.Show();
                this.WindowState = FormWindowState.Normal; // Ensure the window is restored to normal state
                this.ShowInTaskbar = true;

                // Bring the window to the front and focus it
                this.Activate();
                this.BringToFront();

                // Ensure the window is not shrunk (restore size and position)
                var screenBounds = Screen.PrimaryScreen.WorkingArea;
                if (!screenBounds.Contains(this.Bounds))
                {
                    this.Location = new System.Drawing.Point(screenBounds.Left + 50, screenBounds.Top + 50);
                    this.Size = new System.Drawing.Size(800, 600); // Default size if shrunk
                }
            }
        }
    }

    private void OnResize(object? sender, EventArgs e)
    {
        if (this.WindowState == FormWindowState.Minimized)
        {
            this.Hide();
            this.ShowInTaskbar = false;
            trayIcon.ShowBalloonTip(500, "TrayApp", "Application minimized to tray.", ToolTipIcon.Info);
        }
    }

    private void OnTrayIconClick(object? sender, EventArgs e)
    {
        // Restore the window when "Open" is clicked in the tray menu
        this.WindowState = FormWindowState.Normal;
        this.ShowInTaskbar = true;
        this.Show();
    }

    private void OnExit(object? sender, EventArgs e)
    {
        // Exit the application
        trayIcon.Visible = false;
        Application.Exit();
    }

    private void OnFormClosing(object? sender, FormClosingEventArgs e)
    {
        // Prevent the app from closing when the close button is clicked
        if (e.CloseReason == CloseReason.UserClosing)
        {
            e.Cancel = true;
            this.WindowState = FormWindowState.Minimized;
        }
    }
}
