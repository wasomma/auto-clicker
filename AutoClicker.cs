using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace AutoClickerApp
{
    public class MainForm : Form
    {
        [DllImport("user32.dll")]
        private static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, UIntPtr dwExtraInfo);
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);
        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
        private const uint MOUSEEVENTF_LEFTUP = 0x0004;
        private const int WM_HOTKEY = 0x0312;
        private const int HOTKEY_ID = 1;
        private const uint VK_F6 = 0x75;

        private Timer clickTimer;
        private NumericUpDown cpsInput;
        private Button toggleButton;
        private CheckBox topMostCheck;
        private Label statusLabel;
        private long totalClicks = 0;
        private bool clicking = false;
        private bool hotkeyOk = false;

        public MainForm()
        {
            Text = "Auto Clicker";
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;
            TopMost = true;
            Font = new Font("Segoe UI", 9F);
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(300, 195);

            Label cpsLabel = new Label();
            cpsLabel.Text = "Clicks per second:";
            cpsLabel.Location = new Point(15, 18);
            cpsLabel.AutoSize = true;
            Controls.Add(cpsLabel);

            cpsInput = new NumericUpDown();
            cpsInput.Minimum = 1;
            cpsInput.Maximum = 60;
            cpsInput.Value = 15;
            cpsInput.Location = new Point(170, 15);
            cpsInput.Width = 70;
            cpsInput.ValueChanged += new EventHandler(OnCpsChanged);
            Controls.Add(cpsInput);

            toggleButton = new Button();
            toggleButton.Text = "Start  (F6)";
            toggleButton.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            toggleButton.Location = new Point(15, 48);
            toggleButton.Size = new Size(270, 45);
            toggleButton.Click += new EventHandler(OnToggleClicked);
            Controls.Add(toggleButton);

            topMostCheck = new CheckBox();
            topMostCheck.Text = "Keep this window on top";
            topMostCheck.Checked = true;
            topMostCheck.Location = new Point(15, 103);
            topMostCheck.AutoSize = true;
            topMostCheck.CheckedChanged += new EventHandler(OnTopMostChanged);
            Controls.Add(topMostCheck);

            statusLabel = new Label();
            statusLabel.Location = new Point(15, 128);
            statusLabel.AutoSize = true;
            Controls.Add(statusLabel);

            Label hintLabel = new Label();
            hintLabel.Text = "F6 works globally: hover the mouse over the\r\ngame button and press F6 to start / stop.";
            hintLabel.ForeColor = Color.Gray;
            hintLabel.Location = new Point(15, 152);
            hintLabel.AutoSize = true;
            Controls.Add(hintLabel);

            clickTimer = new Timer();
            clickTimer.Tick += new EventHandler(OnClickTick);

            UpdateStatus();
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            hotkeyOk = RegisterHotKey(Handle, HOTKEY_ID, 0, VK_F6);
            UpdateStatus();
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_HOTKEY && m.WParam.ToInt32() == HOTKEY_ID)
            {
                ToggleClicking();
                return;
            }
            base.WndProc(ref m);
        }

        private void OnToggleClicked(object sender, EventArgs e)
        {
            ToggleClicking();
        }

        private void ToggleClicking()
        {
            clicking = !clicking;
            if (clicking)
            {
                clickTimer.Interval = IntervalFromCps();
                clickTimer.Start();
                toggleButton.Text = "Stop  (F6)";
            }
            else
            {
                clickTimer.Stop();
                toggleButton.Text = "Start  (F6)";
            }
            UpdateStatus();
        }

        private int IntervalFromCps()
        {
            int ms = (int)Math.Round(1000.0 / (double)cpsInput.Value);
            if (ms < 15) ms = 15;
            return ms;
        }

        private void OnCpsChanged(object sender, EventArgs e)
        {
            if (clicking)
            {
                clickTimer.Interval = IntervalFromCps();
            }
        }

        private void OnTopMostChanged(object sender, EventArgs e)
        {
            TopMost = topMostCheck.Checked;
        }

        private void OnClickTick(object sender, EventArgs e)
        {
            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, UIntPtr.Zero);
            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, UIntPtr.Zero);
            totalClicks++;
            UpdateStatus();
        }

        private void UpdateStatus()
        {
            string state = clicking ? "Clicking" : "Stopped";
            string suffix = hotkeyOk ? "" : "  (F6 hotkey unavailable)";
            statusLabel.Text = string.Format("{0} - {1:n0} clicks{2}", state, totalClicks, suffix);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            clickTimer.Stop();
            if (hotkeyOk)
            {
                UnregisterHotKey(Handle, HOTKEY_ID);
            }
            base.OnFormClosed(e);
        }

        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
