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
        private (Label, Button)[] summonerElements = new (Label, Button)[5];
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
            RunUiUpdaterTask();
        }

        private Task RunUiUpdaterTask() => Task.Run(() => {
            while (true) {
                Thread.Sleep(250);
                Dispatcher.Invoke(() => {
                    if (LCU_Handler.IsInChampSelect) {
                        txtLeagueStatus.Text = $"In Champ Select: {LCU_Handler.Team.ToString()}";
                    } else if (LCU_Handler.IsConnected) {
                        txtLeagueStatus.Text = "Connected to LCU";
                    } else {
                        txtLeagueStatus.Text = "Not Connected to LCU";
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
                        summonerElements[index].Item1.Content = teamMemberSummonerName;
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
                    summonerElements[i].Item1.Content = $"Summoner Name {i + 1}";
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