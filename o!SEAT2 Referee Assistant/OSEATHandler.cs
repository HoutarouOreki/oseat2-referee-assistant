using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace oSEAT2RefereeAssistant
{
    public partial class OSEATHandler : Form
    {
        private MatchAssistant MatchAssistant;

        private Regex MatchCodeRegex = new Regex("([A-Z](?:[0-9]|[A-Z]))\t(.*)\t(.*)");
        private Regex MapTableRegex = new Regex("([A-Z]{2}[1-9])\t(.*)\t(!mp map [0-9]*)");

        private List<BeatMap> MapTable;
        private string MatchData;

        public OSEATHandler()
        {
            InitializeComponent();
            StageSelector.DropDownStyle = ComboBoxStyle.DropDownList;
            //StageSelector.SelectedItem = "Group Stage";
        }

        private void ConfirmStage_Click(object sender, EventArgs e)
        {
            MatchAssistant = new MatchAssistant(MatchData, MapTable, (TournamentStage)StageSelector.SelectedIndex);
            MatchAssistant.Show();
            Hide();
            MatchAssistant.FormClosed += (a, b) => Show();
        }

        private void MatchCodeTextBox_TextChanged(object sender, EventArgs e)
        {
            MatchCodeTextBox.ForeColor = MatchCodeRegex.IsMatch(MatchCodeTextBox.Text) ? Color.Green : Color.Red;
            MatchData = MatchCodeTextBox.Text;
            UpdateButtonState();
        }

        private void MapTableTextBox_TextChanged(object sender, EventArgs e)
        {
            MapTableTextBox.ForeColor = MapTableRegex.IsMatch(MapTableTextBox.Text) ? Color.Green : Color.Red;
            MapTable = new List<BeatMap>();
            foreach (Match item in MapTableRegex.Matches(MapTableTextBox.Text))
                MapTable.Add(new BeatMap(item.Value));
            UpdateButtonState();
        }

        private void UpdateButtonState()
        {
            if (MapTableTextBox.Text.Length < 1)
                StageMatchTableLabel.Text = "";
            else if ((TournamentStage)StageSelector.SelectedIndex == TournamentStage.groups
                && MapTableRegex.Matches(MapTableTextBox.Text).Count != 13
                || ((TournamentStage)StageSelector.SelectedIndex == TournamentStage.ro16
                || (TournamentStage)StageSelector.SelectedIndex == TournamentStage.ro32
                || (TournamentStage)StageSelector.SelectedIndex == TournamentStage.quarterfinals
                )
                && MapTableRegex.Matches(MapTableTextBox.Text).Count != 17
                || ((
                (TournamentStage)StageSelector.SelectedIndex == TournamentStage.semifinals
                || (TournamentStage)StageSelector.SelectedIndex == TournamentStage.finals)
                || (TournamentStage)StageSelector.SelectedIndex == TournamentStage.grandfinals)
                && MapTableRegex.Matches(MapTableTextBox.Text).Count != 19
                || StageSelector.SelectedIndex < 0)
            {
                StageMatchTableLabel.ForeColor = Color.Red;
                StageMatchTableLabel.Text = "The map table does not match the stage";
            }
            else
            {
                StageMatchTableLabel.ForeColor = SystemColors.MenuHighlight;
                StageMatchTableLabel.Text = "The map table matches the stage";
            }
            ConfirmStage.Enabled = MatchCodeRegex.IsMatch(MatchCodeTextBox.Text)
                && MapTableRegex.IsMatch(MapTableTextBox.Text) && StageSelector.SelectedIndex > -1;
        }

        private void StageSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateButtonState();
        }
    }

    public enum TournamentStage
    {
        groups,
        ro32,
        ro16,
        quarterfinals,
        semifinals,
        finals,
        grandfinals
    }
}
