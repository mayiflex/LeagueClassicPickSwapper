using LeagueClassicPickSwapper.DTO_s;
using mayLCU;
using System.CodeDom;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LeagueClassicPickSwapper {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private (TextBlock, Button)[] summonerElements = new (TextBlock, Button)[5];
        private System.Windows.Threading.DispatcherTimer? windowTrackerTimer;
        private bool wasClientMinimized = false;

        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
        private struct RECT {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
        private static extern bool GetWindowRect(nint hWnd, out RECT lpRect);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
        private static extern bool IsWindow(nint hWnd);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
        private static extern bool IsIconic(nint hWnd);

        public MainWindow() {
            InitializeComponent();
            summonerElements = [
                (lblSummonerName0, btnPick0),
                (lblSummonerName1, btnPick1),
                (lblSummonerName2, btnPick2),
                (lblSummonerName3, btnPick3),
                (lblSummonerName4, btnPick4)
            ];

            LCU_Handler.Innit();
            var uiUpdaterTask = RunUiUpdaterTask();
            CheckForAppUpdates();
            InitWindowTracker();
        }

        private void InitWindowTracker() {
            windowTrackerTimer = new System.Windows.Threading.DispatcherTimer {
                Interval = TimeSpan.FromMilliseconds(16)
            };
            windowTrackerTimer.Tick += WindowTrackerTimer_Tick;
            windowTrackerTimer.Start();
        }

        private void WindowTrackerTimer_Tick(object? sender, EventArgs e) {
            nint handle = LCU_Handler.GetHookedProcessMainWindowHandle;
            if (handle == nint.Zero || !IsWindow(handle)) {
                wasClientMinimized = false;
                return;
            }

            bool isClientMinimized = IsIconic(handle);

            if (isClientMinimized) {
                if (!wasClientMinimized) {
                    wasClientMinimized = true;
                    this.WindowState = WindowState.Minimized;
                }
                // When client is minimized, allow user to restore Pick Swapper from taskbar and drag freely
                return;
            }

            // Client is restored / active
            if (wasClientMinimized) {
                wasClientMinimized = false;
                if (this.WindowState == WindowState.Minimized) {
                    this.WindowState = WindowState.Normal;
                }
            }

            if (!GetWindowRect(handle, out RECT rect)) return;

            var dpi = VisualTreeHelper.GetDpi(this);
            double scaleX = dpi.DpiScaleX > 0 ? dpi.DpiScaleX : 1.0;
            double scaleY = dpi.DpiScaleY > 0 ? dpi.DpiScaleY : 1.0;

            double clientH = (rect.Bottom - rect.Top) / scaleY;
            double scale = clientH / 720.0;

            if (scale > 0) {
                uiScale.ScaleX = scale;
                uiScale.ScaleY = scale;
            }

            double targetWidth = 240.0 * scale;
            double targetHeight = 540.0 * scale;
            double targetLeft = ((rect.Left) / scaleX) - targetWidth;
            double targetTop = (rect.Top / scaleY) + (38.0 * scale);

            if (Math.Abs(this.Left - targetLeft) > 0.5 || Math.Abs(this.Top - targetTop) > 0.5) {
                this.Left = targetLeft;
                this.Top = targetTop;
            }
            if (Math.Abs(this.Width - targetWidth) > 0.5 || Math.Abs(this.Height - targetHeight) > 0.5) {
                this.Width = targetWidth;
                this.Height = targetHeight;
            }
        }

        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.ChangedButton == MouseButton.Left) {
                this.DragMove();
            }
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e) {
            this.WindowState = WindowState.Minimized;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }

        private async void CheckForAppUpdates() {
            var (isUpdateAvailable, latestVersionTag) = await UpdateChecker.CheckForUpdatesAsync();
            if (isUpdateAvailable) {
                txtUpdateVersion.Text = latestVersionTag;
                pnlUpdateNotice.Visibility = Visibility.Visible;
            }
        }

        private void linkUpdate_Click(object sender, RoutedEventArgs e) {
            try {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(UpdateChecker.LatestReleaseUrl) { UseShellExecute = true });
            } catch {
                // Ignore if browser launch fails
            }
        }

        public void ForceToForeground() {
            // If the window is minimized, restore it
            if (this.WindowState == WindowState.Minimized) {
                this.WindowState = WindowState.Normal;
            }

            // Force the window to the top
            this.Topmost = true;

            // Request focus
            this.Activate();
            this.Focus();

            // Remove the always-on-top lock so it behaves normally again
            this.Topmost = false;
        }

        private Task RunUiUpdaterTask() => Task.Run(() => {
            while (true) {
                Thread.Sleep(250);
                Dispatcher.Invoke(() => {
                    if (LCU_Handler.IsInChampSelect) {
                        if(txtLeagueStatus.Text != $"In Champ Select ({LCU_Handler.Team.ToString()})") {
                            ForceToForeground();
                        }
                        txtLeagueStatus.Text = $"In Champ Select ({LCU_Handler.Team.ToString()})";
                        dotStatus.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#5865F2"));
                    } else if (LCU_Handler.IsConnected) {
                        txtLeagueStatus.Text = "Connected to LCU";
                        dotStatus.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#23A55A"));
                    } else {
                        txtLeagueStatus.Text = "Not Connected to LCU";
                        dotStatus.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F23F43"));
                    }
                });

                if (!LCU_Handler.IsInChampSelect) {
                    resetSummonerElements();
                    continue;
                }
                Myteam[]? team = LCU_Handler.champSelect?.myTeam;
                if (team == null) continue;
                team = team.OrderBy(x => x.cellId).ToArray();
                foreach (var teamMember in team) {
                    var index = teamMember.cellId - ((int)LCU_Handler.Team - 1) * 5;
                    var teamMemberSummonerName = teamMember.nameVisibilityType == "HIDDEN" ? teamMember.assignedPosition == "utility" ? "Support" : teamMember.assignedPosition.Substring(0,1).ToUpperInvariant()+teamMember.assignedPosition.Substring(1) : $"{teamMember.gameName}#{teamMember.tagLine}";
                    var isSwapAvailable = LCU_Handler.AvailableSwaps?.Any(x => x.cellId == teamMember.cellId && x.state == "AVAILABLE") ?? false;
                    Dispatcher.Invoke(() => {
                        summonerElements[index].Item1.Text = teamMemberSummonerName;
                        summonerElements[index].Item2.IsEnabled = isSwapAvailable;
                    });
                    if (teamMemberSummonerName != LCU_Handler.CurrentSummoner) continue;
                    Dispatcher.Invoke(() => {
                        summonerElements[index].Item2.IsEnabled = false;
                    });
                }
            }
        });

        private void resetSummonerElements() {
            Dispatcher.Invoke(() => {
                txtSwapStatus.Text = "";
            });
            for (int i = 0; i < 5; i++) {
                Dispatcher.Invoke(() => {
                    summonerElements[i].Item1.Text = $"Summoner {i + 1}";
                    summonerElements[i].Item2.IsEnabled = false;
                });
            }
        }

        private async void btnPick0_Click(object sender, RoutedEventArgs e) {
            txtSwapStatus.Text = await LCU_Handler.SendSwapRequest(0);
        }

        private async void btnPick1_Click(object sender, RoutedEventArgs e) {
            txtSwapStatus.Text = await LCU_Handler.SendSwapRequest(1);
        }

        private async void btnPick2_Click(object sender, RoutedEventArgs e) {
            txtSwapStatus.Text = await LCU_Handler.SendSwapRequest(2);
        }

        private async void btnPick3_Click(object sender, RoutedEventArgs e) {
            txtSwapStatus.Text = await LCU_Handler.SendSwapRequest(3);
        }

        private async void btnPick4_Click(object sender, RoutedEventArgs e) {
            txtSwapStatus.Text = await LCU_Handler.SendSwapRequest(4);
        }
    }
}