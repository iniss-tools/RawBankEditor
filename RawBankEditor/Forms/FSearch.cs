using ToolsCore.Entities;
using ToolsCore.Tools;

namespace RawBankEditor.Forms;

public partial class FSearch : Form
{
    // zvuky, nie indexy riadkov - po uprave, odstraneni alebo presune zvukov by indexy ukazovali inam
    private readonly List<FyzSound> _found = new();

    private int _foundIndex;
    private (string Text, SearchType Type, bool IgnoreCase)? _lastQuery;

    public FSearch()
    {
        InitializeComponent();
        this.ApplyThemeAndFonts();
    }

    private void trackBarOpacity_Scroll(object sender, EventArgs e) => Opacity = trackBarOpacity.Value / 100d;

    private void bSearch_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(tbText.Text))
        {
            Utils.ShowInfo("Zadajte hľadaný text.");
            return;
        }

        var query = (tbText.Text, GetSearchType(), cboxIgnoreCase.Checked);

        // rovnake hladanie ako naposledy - dalsi vysledok; inak (alebo ked vysledky medzitym zmizli) hladat znova
        if (_lastQuery == query && _found.Count > 0)
        {
            _foundIndex = (_foundIndex + 1) % _found.Count;
            if (SelectFound())
                return;
        }

        _lastQuery = query;
        Search(query.Item1, query.Item2, query.Item3);
        _foundIndex = 0;
        if (_found.Count == 0)
        {
            Text = "Hľadať";
            Utils.ShowInfo("Nič sa nenašlo.");
            return;
        }

        SelectFound();
    }

    private void bStorno_Click(object sender, EventArgs e) => Close();

    private void Search(string text, SearchType type, bool ignoreCase)
    {
        _found.Clear();

        var comparison = ignoreCase ? StringComparison.CurrentCultureIgnoreCase : StringComparison.CurrentCulture;
        foreach (var grp in Program.MainForm.CurrentLanguage!.Groups)
        {
            foreach (var sound in grp.Sounds)
            {
                var value = type switch
                {
                    SearchType.Key => sound.Key,
                    SearchType.Name => sound.Name,
                    SearchType.Text => sound.Text,
                    _ => sound.FileName
                };

                if (value is not null && value.Contains(text, comparison))
                    _found.Add(sound);
            }
        }
    }

    /// <summary>
    /// Vyberie aktualny vysledok v hlavnom okne; v titulku ukaze, kolky je.
    /// </summary>
    /// <returns><c>false</c>, ak zvuk medzitym zo skupiny zmizol - treba hladat znova.</returns>
    private bool SelectFound()
    {
        var sound = _found[_foundIndex];
        if (!sound.Group.Sounds.Contains(sound) || !Program.MainForm.CurrentLanguage!.Groups.Contains(sound.Group))
            return false;

        Program.MainForm.dgvSounds.ClearSelection();
        if (Program.MainForm.SelectSound(sound) == -1)
            return false;

        Text = $"Hľadať – {_foundIndex + 1} z {_found.Count}";
        return true;
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

    private enum SearchType
    {
        Key,
        Name,
        Text,
        FileName
    }
}
