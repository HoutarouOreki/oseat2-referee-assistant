using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace oSEAT2RefereeAssistant
{
    public partial class MatchAssistant : Form
    {
        private List<BeatMap> MapTable;
        private MatchStage MatchStage;

        private string PlayerOne;
        private string PlayerTwo;
        private string ActivePlayer;

        private int playerOneScore;
        private int playerTwoScore;

        private int PlayerOneScore
        {
            get { return playerOneScore; }
            set
            {
                playerOneScore = value;
                UpdateScore();
            }
        }
        private int PlayerTwoScore
        {
            get { return playerTwoScore; }
            set
            {
                playerTwoScore = value;
                UpdateScore();
            }
        }

        private int RollWinner;

        private int StepsLeft;
        private int matchRound;
        private int MatchRound
        {
            get { return matchRound; }
            set
            {
                matchRound = value;
                CurrentRoundTextBox.Text = value.ToString();
            }
        }

        private bool Player1Late = false;
        private bool Player2Late = false;
        private bool LatenessDone = false;

        private bool PlayerWasDC = false;

        private bool IsTiebreaker = false;
        private string TiebreakerCode;

        private int WinningScore;
        private int BanRoundsAmount;

        private TournamentStage TournamentStage;

        private List<string> Player1Bans = new List<string>();
        private List<string> Player2Bans = new List<string>();
        private List<string> Player1Picks = new List<string>();
        private List<string> Player2Picks = new List<string>();

        private Regex LinkRegex = new Regex(@"https:\/\/osu.ppy.sh\/.*");
        private Regex MapIdRegex = new Regex(@"\(([0-9]*)\)$");

        private Regex PasteScoreRegex1;
        private Regex PasteScoreRegex2;

        private string[] MatchData;
        private int LastDCPlayer;

        private int ActivePlayerNumber() => ActivePlayer == PlayerOne ? 1 : 2;
        private string RollWinnerName() => RollWinner == 1 ? PlayerOne : PlayerTwo;


        public MatchAssistant(string matchData, List<BeatMap> mapTable, TournamentStage tournamentStage)
        {
            InitializeComponent();
            TournamentStage = tournamentStage;
            TournamentStageTextBox.Text = tournamentStage == TournamentStage.groups ? "Group Stage" :
                tournamentStage == TournamentStage.ro32 ? "Round of 32" :
                tournamentStage == TournamentStage.ro16 ? "Round of 16" :
                tournamentStage == TournamentStage.quarterfinals ? "Quarter Finals" :
                tournamentStage == TournamentStage.semifinals ? "Semifinals" :
                tournamentStage == TournamentStage.finals ? "Finals" :
                "Grand Finals";
            MapTable = mapTable;

            foreach (var item in MapTable)
                MapList.Items.Add($"{item.Code} | {item.Name} ({item.Id.Split(' ')[2]})");

            MpSet1.Text = "!mp set 0 3 2";
            MpSet2.Text = "!mp set 0 3 3";

            CurrentActionLabel.TextChanged += CurrentActionLabel_TextChanged;
            MatchData = matchData.Split('\t');
            PlayerOne = Player1Label.Text = MatchData[1];
            PlayerTwo = Player2Label.Text = MatchData[2];

            InviteTextBox1.Text = $"!mp invite {PlayerOne.Replace(' ', '_')}";
            InviteTextBox2.Text = $"!mp invite {PlayerTwo.Replace(' ', '_')}";

            PasteScoreRegex1 = new Regex($@"({PlayerOne}) finished playing \(Score: ([0-9]*), (FAILED|PASSED)");
            PasteScoreRegex2 = new Regex($@"({PlayerTwo}) finished playing \(Score: ([0-9]*), (FAILED|PASSED)");

            ChangeMatchStage(tournamentStage != TournamentStage.groups ? MatchStage.warmups : MatchStage.roll);
            MapSelectButton.Click += MapSelectButton_Click;
            BanButton.Click += BanButton_Click;

            MpMap.GotFocus += MpMap_Click;

            MpMods.GotFocus += MpMods_Click;
            MpMods.TextChanged += MpMods_TextChanged;

            Player1Action.Text = $"{PlayerOne} rolled higher";
            Player2Action.Text = $"{PlayerTwo} rolled higher";

            P1DC.Click += P1DC_Click;
            P2DC.Click += P2DC_Click;

            mpMakeTextBox.Text = $"!mp make o!SEAT2: ({PlayerOne}) vs ({PlayerTwo})";

            switch (TournamentStage)
            {
                case TournamentStage.groups:
                    WinningScore = 5;
                    BanRoundsAmount = 1;
                    break;
                case TournamentStage.ro32:
                case TournamentStage.ro16:
                case TournamentStage.quarterfinals:
                    WinningScore = 6;
                    BanRoundsAmount = 2;
                    break;
                case TournamentStage.semifinals:
                case TournamentStage.finals:
                    WinningScore = 7;
                    BanRoundsAmount = 2;
                    break;
                case TournamentStage.grandfinals:
                    WinningScore = 8;
                    BanRoundsAmount = 2;
                    break;
                default:
                    break;
            }
        }

        private void CurrentActionLabel_TextChanged(object sender, EventArgs e)
        {
            CurrentActionTextBox.Text = CurrentActionLabel.Text;
            ScoreAndActionTextBox.Text = $"{PlayerOne} | {PlayerOneScore} - {PlayerTwoScore} | {PlayerTwo}"
                + Environment.NewLine + CurrentActionLabel.Text;
        }

        private void ChangeMatchStage(MatchStage matchStage)
        {
            MatchStage = matchStage;
            if (MatchStage == MatchStage.warmups)
            {
                CurrentActionLabel.Text = $"{PlayerOne}'s warmup";
                ProceedButton.Text = $"First warmup finished";
                ProceedButton.Click += NextWarmup;
            }
            else
            {
                RollRaw();
            }
        }

        private void NextWarmup(object sender, EventArgs e)
        {
            CurrentActionLabel.Text = $"{PlayerTwo}'s warmup";
            ProceedButton.Text = $"Second warmup finished";
            ProceedButton.Click -= NextWarmup;
            if (TournamentStage != TournamentStage.grandfinals)
                ProceedButton.Click += Roll;
            else
            {
                RollWinner = 1;
                ProceedButton.Click += BanningPhase;
            }
        }

        private void RollRaw()
        {
            CurrentActionLabel.Text = $"Both players !roll";
            CurrentActionTextBox.Focus();
            CurrentActionTextBox.SelectAll();
            ProceedButton.Visible = false;
            Player1Action.Visible = true;
            Player2Action.Visible = true;
            Player1Action.Click += BanningPhase;
            Player2Action.Click += BanningPhase;
        }

        private void Roll(object sender, EventArgs e)
        {
            ProceedButton.Click -= Roll;
            RollRaw();
        }

        private void BanningPhase(object sender, EventArgs e)
        {
            if (TournamentStage != TournamentStage.grandfinals)
                RollWinner = (sender as Button).Name == "Player1Action" ? 1 : 2;
            BanningPhaseRaw();
        }

        private void BanningPhaseRaw(int playerLate = 0)
        {
            InviteTextBox1.Visible = false;
            InviteTextBox2.Visible = false;
            InviteLabel1.Visible = false;
            InviteLabel2.Visible = false;

            PasteScoreTextBox.Visible = false;
            PasteScoreLabel.Visible = false;

            MapList.Visible = true;
            ProceedButton.Visible = false;

            if (playerLate > 0)
            {
                P1LateButton.Visible = false;
                P2LateButton.Visible = false;
                P1DC.Visible = false;
                P2DC.Visible = false;
            }

            if (MatchRound == 0 && StepsLeft == 0)
            {
                Player1Action.Click -= BanningPhase;
                Player2Action.Click -= BanningPhase;
                Player1Action.Click += (sender, e) => PlayerWonMap(1);
                Player2Action.Click += (sender, e) => PlayerWonMap(2);
                Player1Action.Visible = false;
                Player2Action.Visible = false;
                Player1Action.Text = $"{PlayerOne} won the map";
                Player2Action.Text = $"{PlayerTwo} won the map";
                P1LateButton.Visible = true;
                P2LateButton.Visible = true;
            }

            if ((StepsLeft == 0 && TournamentStage != TournamentStage.grandfinals)
                || (playerLate > 0 && ((!Player1Late && !Player2Late) || ((Player1Late || Player2Late) && LatenessDone))))
            {
                BanButton.Visible = true;
                if (playerLate == 0)
                {
                    MatchRound++;
                    StepsLeft = 2;
                }
                MapSelectButton.Visible = false;
            }

            if (playerLate == 0)
            {
                ActivePlayer = StepsLeft % 2 == 0 ?
                    RollWinner == 1 ? PlayerTwo : PlayerOne :
                    RollWinner == 1 ? PlayerOne : PlayerTwo;
            }
            else if (playerLate == 1)
                ActivePlayer = PlayerTwo;
            else
                ActivePlayer = PlayerOne;

            if (StepsLeft == 0 && TournamentStage == TournamentStage.grandfinals)
            {
                MatchRound++;
                if (MatchRound == 1)
                {
                    BanButton.Visible = true;
                    if (playerLate == 0)
                        StepsLeft = 2;
                    MapSelectButton.Visible = false;
                }
                else if (MatchRound == 2)
                {
                    BanButton.Visible = true;
                    MapSelectButton.Visible = false;
                    if (playerLate == 0)
                        StepsLeft = 1;
                    ActivePlayer = PlayerOne;
                }
            }

            CurrentActionLabel.Text = $"{ActivePlayer}'s ban";

            CurrentActionTextBox.Focus();
            CurrentActionTextBox.SelectAll();

            if (playerLate == 0)
                StepsLeft--;
            else
                LatenessDone = true;

            LastDCPlayer = playerLate;
        }

        private void BanButton_Click(object sender, EventArgs e)
        {
            if (ActivePlayerNumber() == 1)
            {
                Player1Bans.Add(MapList.SelectedItem.ToString().Split(' ')[0]);
                Player1BansTextBox.Text = string.Join(", ", Player1Bans);
            }
            else
            {
                Player2Bans.Add(MapList.SelectedItem.ToString().Split(' ')[0]);
                Player2BansTextBox.Text = string.Join(", ", Player2Bans);
            }

            if (StepsLeft > 0 && !PlayerWasDC)
                BanningPhaseRaw();
            else
            {
                if (Player1Late && !LatenessDone)
                    BanningPhaseRaw(1);
                else if (Player2Late && !LatenessDone)
                    BanningPhaseRaw(2);
                else
                {
                    MapSelectPhase();
                    if (PlayerWasDC)
                    {
                        CurrentActionLabel.Text = $"{(LastDCPlayer == 1 ? PlayerTwo : PlayerOne)}'s pick";
                    }
                    PlayerWasDC = false;
                    CurrentActionTextBox.Focus();
                    CurrentActionTextBox.SelectAll();
                    P1LateButton.Visible = false;
                    P2LateButton.Visible = false;
                }
            }

            MapList.Items.Remove(MapList.SelectedItem);

            GenerateMapsLeftText();

            if (PlayerOneScore + PlayerTwoScore == 2 * WinningScore - 2)
                Tiebreaker();
        }

        private void MapSelectPhase()
        {
            MapList.Visible = true;

            P1DC.Visible = true;
            P2DC.Visible = true;
            BanButton.Visible = false;

            if (StepsLeft == 0)
            {
                StepsLeft = 4;
            }

            if (IsTiebreaker)
            {
                MapSelectButton.Visible = true;
                CurrentActionLabel.Text = "Tiebreaker! Both players have to Private Message the referee" +
                    $" which map to pick out of the 3 left ({MapsLeftTextBox.Text})";
                return;
            }

            MapSelectButton.Visible = true;
            ActivePlayer = StepsLeft % 2 == 0 ?
                RollWinner == 1 ? PlayerOne : PlayerTwo :
                RollWinner == 1 ? PlayerTwo : PlayerOne;

            CurrentActionLabel.Text = $"{ActivePlayer}'s pick";

            StepsLeft--;
        }

        private void MapSelectButton_Click(object sender, EventArgs e)
        {
            P1DC.Visible = false;
            P2DC.Visible = false;

            if (IsTiebreaker)
            {
                TiebreakerCode = MapList.SelectedItem.ToString().Split(' ')[0];
            }
            else if (ActivePlayerNumber() == 1)
            {
                Player1Picks.Add(MapList.SelectedItem.ToString().Split(' ')[0]);
                Player1PicksTextBox.Text = string.Join(", ", Player1Picks);
            }
            else
            {
                Player2Picks.Add(MapList.SelectedItem.ToString().Split(' ')[0]);
                Player2PicksTextBox.Text = string.Join(", ", Player2Picks);
            }

            PlayPhase(MapList.SelectedItem.ToString().Split(' ')[0]);

            MpMap.Text = $"!mp map {MapIdRegex.Match(MapList.SelectedItem.ToString()).Groups[1].Value}";

            switch (MapList.SelectedItem.ToString().Substring(0, 2))
            {
                case "NM":
                    MpMods.Text = "!mp mods none";
                    break;
                case "HD":
                    MpMods.Text = "!mp mods hd";
                    break;
                case "HR":
                    MpMods.Text = "!mp mods hr";
                    break;
                case "DT":
                    MpMods.Text = "!mp mods dt";
                    break;
                case "FM":
                    MpMods.Text = "!mp mods freemod";
                    break;
                default:
                    break;
            }

            MapAndModsTextBox.Text = MpMap.Text + Environment.NewLine + MpMods.Text;

            MapList.Items.Remove(MapList.SelectedItem);

            GenerateMapsLeftText();

            MpMap.Focus();
            MpMap.SelectAll();
        }

        private void PlayPhase(string currentMap)
        {
            MapList.Visible = false;
            MapSelectButton.Visible = false;
            CurrentActionLabel.Text = $"Playing {currentMap}";
            Player1Action.Visible = true;
            Player2Action.Visible = true;
            PasteScoreTextBox.Visible = true;
            PasteScoreLabel.Visible = true;
        }

        private void PlayerWonMap(int player)
        {
            if (player == 1)
                PlayerOneScore++;
            else
                PlayerTwoScore++;
            Player1Action.Visible = false;
            Player2Action.Visible = false;

            PasteScoreTextBox.Visible = false;
            PasteScoreLabel.Visible = false;

            if (CheckWinOrTB())
                return;

            if (StepsLeft > 0)
                MapSelectPhase();
            else if (MatchRound < BanRoundsAmount)
                BanningPhaseRaw();
            else
            {
                MatchRound++;
                StepsLeft = 4;
                MapSelectPhase();
            }

            CopyableScoreTextBox.Focus();
            CopyableScoreTextBox.SelectAll();
        }

        private void UpdateScore()
        {
            vsLabel.Text = $"| {PlayerOneScore} - {PlayerTwoScore} |";
            CopyableScoreTextBox.Text = $"{PlayerOne} | {PlayerOneScore} - {PlayerTwoScore} | {PlayerTwo}";
        }

        private bool CheckWinOrTB()
        {
            if (PlayerOneScore == WinningScore)
            {
                PlayerWon(1);
                return true;
            }
            else if (PlayerTwoScore == WinningScore)
            {
                PlayerWon(2);
                return true;
            }
            else if (PlayerOneScore + PlayerTwoScore == 2 * WinningScore - 2)
            {
                Tiebreaker();
                return true;
            }
            return false;
        }

        private bool CheckWin()
        {
            if (PlayerOneScore == WinningScore)
            {
                PlayerWon(1);
                return true;
            }
            else if (PlayerTwoScore == WinningScore)
            {
                PlayerWon(2);
                return true;
            }
            return false;
        }

        private void PlayerWon(int player)
        {
            PasteScoreTextBox.Visible = false;
            PasteScoreLabel.Visible = false;
            CurrentActionLabel.Text = $"{(player == 1 ? PlayerOne : PlayerTwo)} won the match! Congratulations!";
            CopyableScoreTextBox.Focus();
            CopyableScoreTextBox.SelectAll();
            P1DC.Visible = false;
            P2DC.Visible = false;
            MapList.Visible = false;
            MapSelectButton.Visible = false;
            BanButton.Visible = false;

            MatchSummaryTextBox.Visible = true;

            MatchSummaryTextBox.Text = $"**{TournamentStageTextBox.Text}: Match {MatchData[0]}**";
            MatchSummaryTextBox.Text += Environment.NewLine;
            MatchSummaryTextBox.Text += $"{(player == 1 ? $"**{PlayerOne}**" : PlayerOne)} {PlayerOneScore}" +
                $" | {PlayerTwoScore} {(player == 2 ? $"**{PlayerTwo}**" : PlayerTwo)}";
            MatchSummaryTextBox.Text += Environment.NewLine;
            MatchSummaryTextBox.Text += $"<{MatchLinkTextBox.Text}>";
            MatchSummaryTextBox.Text += Environment.NewLine;
            MatchSummaryTextBox.Text += $"Winner of roll: {RollWinnerName()}";
            MatchSummaryTextBox.Text += Environment.NewLine;
            MatchSummaryTextBox.Text += "**Bans:**";
            MatchSummaryTextBox.Text += Environment.NewLine;
            MatchSummaryTextBox.Text += $"__{PlayerOne}__: {string.Join("/", Player1Bans)}";
            MatchSummaryTextBox.Text += Environment.NewLine;
            MatchSummaryTextBox.Text += $"__{PlayerTwo}__: {string.Join("/", Player2Bans)}";
            MatchSummaryTextBox.Text += Environment.NewLine;
            MatchSummaryTextBox.Text += "**Picks:**";
            MatchSummaryTextBox.Text += Environment.NewLine;
            MatchSummaryTextBox.Text += $"__{PlayerOne}__: {string.Join("/", Player1Picks)}";
            MatchSummaryTextBox.Text += Environment.NewLine;
            MatchSummaryTextBox.Text += $"__{PlayerTwo}__: {string.Join("/", Player2Picks)}";
            if (IsTiebreaker)
            {
                MatchSummaryTextBox.Text += Environment.NewLine;
                MatchSummaryTextBox.Text += $"**TB map:** {TiebreakerCode}";
            }

            RefSheetResult.Text = $"{PlayerOneScore} - {PlayerTwoScore}\t{(RollWinner == 1 ? "A" : "B")}\t" +
                $"{string.Join("; ", Player1Bans)}\t{string.Join("; ", Player2Bans)}\t{MatchLinkTextBox.Text}";
            RefSheetResult.Visible = true;
            RefSheetResultLabel.Visible = true;

        }

        private void Tiebreaker()
        {
            if (TournamentStage == TournamentStage.grandfinals)
            {
                MapSelectPhase();
                return;
            }
            IsTiebreaker = true;
            CurrentActionLabel.ForeColor = Color.Purple;
            MapSelectPhase();
            P1DC.Visible = false;
            P2DC.Visible = false;
        }

        private void MapList_SelectedIndexChanged(object sender, EventArgs e)
        {
            BanButton.Enabled = MapList.SelectedIndex >= 0;
            MapSelectButton.Enabled = MapList.SelectedIndex >= 0;
        }

        private void GenerateMapsLeftText()
        {
            List<string> t = new List<string>();
            foreach (string item in MapList.Items)
            {
                t.Add(item.Split(' ')[0]);
            }
            MapsLeftTextBox.Text = string.Join(" / ", t);
        }

        private void MatchLinkTextBox_TextChanged(object sender, EventArgs e)
        {
            MatchLinkTextBox.BackColor = LinkRegex.IsMatch(MatchLinkTextBox.Text) ?
                SystemColors.Window : Color.DarkOrange;
        }

        private void MpMap_TextChanged(object sender, EventArgs e) => MpMap.BackColor = !MpMap.Focused ? Color.DarkOrange : SystemColors.Window;
        private void MpMap_Click(object sender, EventArgs e) => MpMap.BackColor = SystemColors.Control;

        private void MpMods_TextChanged(object sender, EventArgs e) => MpMods.BackColor = Color.DarkOrange;
        private void MpMods_Click(object sender, EventArgs e) => MpMods.BackColor = SystemColors.Control;

        private void P1LateButton_Click(object sender, EventArgs e)
        {
            if (!Player1Late)
            {
                Player1Late = true;
                Player1Label.Text = $"(late) {PlayerOne}";
                P1LateButton.Text = "Not late 10+ minutes";
                PlayerTwoScore++;
                P2LateButton.Visible = false;
            }
            else
            {
                Player1Late = false;
                Player1Label.Text = PlayerOne;
                P1LateButton.Text = "Late 10+ minutes";
                PlayerTwoScore--;
                P2LateButton.Visible = true;
            }
        }

        private void P2LateButton_Click(object sender, EventArgs e)
        {
            if (!Player2Late)
            {
                Player2Late = true;
                Player2Label.Text = $"{PlayerTwo} (late)";
                P2LateButton.Text = "Not late 10+ minutes";
                PlayerOneScore++;
                P1LateButton.Visible = false;
            }
            else
            {
                Player2Late = false;
                Player2Label.Text = PlayerTwo;
                P2LateButton.Text = "Late 10+ minutes";
                PlayerOneScore--;
                P1LateButton.Visible = true;
            }
        }

        private void P1DC_Click(object sender, EventArgs e)
        {
            PlayerWasDC = true;
            PlayerTwoScore++;
            StepsLeft++;
            if (CheckWin())
                return;
            BanningPhaseRaw(1);
        }

        private void P2DC_Click(object sender, EventArgs e)
        {
            PlayerWasDC = true;
            PlayerOneScore++;
            StepsLeft++;
            if (CheckWin())
                return;
            BanningPhaseRaw(2);
        }

        private void PasteScoreTextBox_TextChanged(object sender, EventArgs e)
        {
            if (PasteScoreTextBox.Text == $"A finished playing (Score: 1000000, PASSED).{Environment.NewLine}"
                + "B finished playing(Score: -42374, FAILED).")
                return;

            if (PasteScoreRegex1.IsMatch(PasteScoreTextBox.Text) && PasteScoreRegex2.IsMatch(PasteScoreTextBox.Text))
            {
                if ((PasteScoreRegex1.Match(PasteScoreTextBox.Text).Groups[3].Value == "PASSED"
                && PasteScoreRegex2.Match(PasteScoreTextBox.Text).Groups[3].Value == "PASSED")
                || (PasteScoreRegex1.Match(PasteScoreTextBox.Text).Groups[3].Value == "FAILED"
                && PasteScoreRegex2.Match(PasteScoreTextBox.Text).Groups[3].Value == "FAILED"))
                {
                    PlayerWonMap(int.Parse(PasteScoreRegex1.Match(PasteScoreTextBox.Text).Groups[2].Value) >
                    int.Parse(PasteScoreRegex2.Match(PasteScoreTextBox.Text).Groups[2].Value) ? 1 : 2);
                }
                else
                {
                    if (PasteScoreRegex1.Match(PasteScoreTextBox.Text).Groups[3].Value == "PASSED")
                        PlayerWonMap(1);
                    else
                        PlayerWonMap(2);
                }

                PasteScoreTextBox.ResetText();
            }
        }

        private void MapAndModsTextBox_Click(object sender, EventArgs e)
        {
            MpMap.BackColor = SystemColors.Control;
            MpMods.BackColor = SystemColors.Control;
        }
    }

    public enum MatchStage
    {
        warmups,
        roll,
        matches,
        tiebreaker
    }
}
