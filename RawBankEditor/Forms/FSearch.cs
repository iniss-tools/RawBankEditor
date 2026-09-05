using ToolsCore.Entities;
using ToolsCore.Tools;

namespace RawBankEditor.Forms;

public partial class FSearch : Form
{
    private readonly List<(FyzGroup group, int index)> _founded = new();

    private bool _textChanged;
    private int _foundIndex;
    private SearchType _lastSearchType = SearchType.Key;

    public FSearch()
    {
        InitializeComponent();
        this.ApplyThemeAndFonts();
    }

    private void trackBarOpacity_Scroll(object sender, EventArgs e) => Opacity = trackBarOpacity.Value / 100d;

    private void bSearch_Click(object sender, EventArgs e)
    {
        var searchType = GetSearchType();
        if (!_textChanged && _lastSearchType == searchType)
        {
            if (_foundIndex + 1 == _founded.Count) 
                _foundIndex = 0;
            else
                _foundIndex++;
            SelectRow();
            return;
        }

        _lastSearchType = searchType;
        _textChanged = false;

        Search();
        if (_founded.Count == 0)
        {
            Utils.ShowInfo("Nič sa nenašlo.");
            return;
        }

        _foundIndex = 0;
        SelectRow();
    }

    private void bStorno_Click(object sender, EventArgs e) => Close();

    private void TbText_TextChanged(object sender, EventArgs e) => _textChanged = true;

    private void Search()
    {
        _founded.Clear();

        var comparisonType = cboxIgnoreCase.Checked ? StringComparison.CurrentCultureIgnoreCase : StringComparison.CurrentCulture;

        foreach (var grp in Program.MainForm.CurrentLanguage.Groups)
        {
            for (var i = 0; i < grp.Sounds.Count; i++)
            {
                switch (_lastSearchType)
                {
                    case SearchType.Key:
                        if (grp.Sounds[i].Key.Contains(tbText.Text, comparisonType))
                            _founded.Add((grp, i));
                        break;
                    case SearchType.Name:
                        if (grp.Sounds[i].Name.Contains(tbText.Text, comparisonType))
                            _founded.Add((grp, i));
                        break;
                    case SearchType.Text:
                        if (grp.Sounds[i].Text.Contains(tbText.Text, comparisonType))
                            _founded.Add((grp, i));
                        break;
                    case SearchType.FileName:
                        if (grp.Sounds[i].FileName.Contains(tbText.Text, comparisonType))
                            _founded.Add((grp, i));
                        break;
                }
            }
        }
    }

    private SearchType GetSearchType()
    {
        if (rbKey.Checked)
            return SearchType.Key;
        if (rbName.Checked)
            return SearchType.Name;
        if (rbText.Checked)
            return SearchType.Text;
        return SearchType.FileName;
    }

    private void SelectRow()
    {
        if (_founded.Count == 0)
            return;
        var index = Program.MainForm.MenuGroups.IndexOf(_founded[_foundIndex].group);
        if (index != -1)
            Program.MainForm.dgvGroups.Rows[index].Selected = true;

        Program.MainForm.dgvSounds.ClearSelection();
        Program.MainForm.dgvSounds.Rows[_founded[_foundIndex].index].Selected = true;
    }

    private enum SearchType
    {
        Key,
        Name,
        Text,
        FileName
    }
}