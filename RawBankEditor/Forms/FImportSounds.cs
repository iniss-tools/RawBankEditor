using System.Globalization;
using RawBankEditor.Properties;
using RawBankEditor.Tools;
using ToolsCore.Iniss.Entities;
using ToolsCore.Iniss.Tools;
using ToolsCore.Tools;

namespace RawBankEditor.Forms;

/// <summary>
/// Import zvukov z tabulky (Excel, CSV, schranka) do jazyka. Typ stlpca sa vyberie kliknutim na hlavicku, s hlavickou
/// v prvom riadku sa rozpozna sam. Okno len skontroluje tabulku - zvuky prida hlavne okno podla <see cref="Plan" />.
/// </summary>
public partial class FImportSounds : Form
{
    private readonly FyzLanguage _language;

    // hlavicka stlpca s vybranym typom
    private readonly Font _boldFont;

    // nacitane riadky vratane pripadnej hlavicky
    private List<string[]> _rows = [];
    private List<SoundImportColumn> _columns = [];

    // naposledy nacitany CSV subor - pri zmene kodovania sa nacita znova
    private string? _lastCsvPath;

    // stlpec, ktoreho typ sa vybera v ponuke
    private int _menuColumn = -1;

    /// <summary>
    /// Skupiny a zvuky na pridanie (po OK).
    /// </summary>
    public SoundImportPlan? Plan { get; private set; }

    /// <param name="language">jazyk s nacitanymi skupinami, do ktoreho sa zvuky importuju</param>
    public FImportSounds(FyzLanguage language)
    {
        InitializeComponent();
        this.ApplyThemeAndFonts();

        _language = language ?? throw new ArgumentNullException(nameof(language));
        Text = string.Format(CultureInfo.CurrentCulture, Text, language.Name);
        cbEncoding.SelectedIndex = 0;
        _boldFont = new Font(dgvData.Font, FontStyle.Bold);
        Disposed += (_, _) => _boldFont.Dispose();

        foreach (var column in SoundImportColumn.Values.Append(SoundImportColumn.None))
            cmsColumns.Items.Add(new ToolStripMenuItem(column.Name, null, (_, _) => SetColumn(column)) { Tag = column });
    }

    private void BClipboard_Click(object sender, EventArgs e)
    {
        if (!Clipboard.ContainsText())
            return;

        _lastCsvPath = null;
        LoadText(Clipboard.GetText());
    }

    private void BFile_Click(object sender, EventArgs e)
    {
        if (ofdTable.ShowDialog(this) == DialogResult.OK)
            LoadFile(ofdTable.FileName);
    }

    private void LoadFile(string path)
    {
        try
        {
            if (Path.GetExtension(path).ToUpperInvariant() is ".XLS" or ".XLSX")
            {
                _lastCsvPath = null;
                using var reader = new XlsReader(path);
                SetRows(reader);
            }
            else
            {
                _lastCsvPath = path;
                LoadText(File.ReadAllText(path, cbEncoding.SelectedIndex == 1 ? Encodings.Win1250 : Encoding.UTF8));
            }
        }
        catch (Exception ex)
        {
            // poskodeny alebo neznamy subor (ExcelDataReader ma vlastne vynimky), zamknuty subor a pod.
            Log.Exception(ex);
            Utils.ShowError(string.Format(CultureInfo.CurrentCulture, Resources.Import_ReadFailed, path, ex.Message));
        }
    }

    private void LoadText(string text)
    {
        using var reader = new CsvStringReader(text, rowsep: CsvStringReader.DetectSeparator(text));
        SetRows(reader);
    }

    private void SetRows(TableFileReader reader)
    {
        _rows = [];
        for (var r = 0; r < reader.RowCount; r++)
        {
            var row = new string[reader.ColumnCount];
            for (var c = 0; c < row.Length; c++)
                row[c] = reader[r, c];
            _rows.Add(row);
        }

        _columns = [.. Enumerable.Repeat(SoundImportColumn.None, reader.ColumnCount)];
        if (cboxFirstHeader.Checked)
            DetectColumns();
        ShowTable();
    }

    /// <summary>
    /// Typy stlpcov podla hlavicky v prvom riadku; kazdy typ len raz.
    /// </summary>
    private void DetectColumns()
    {
        if (_rows.Count == 0)
            return;

        for (var i = 0; i < _columns.Count; i++)
        {
            var type = SoundImportColumn.ParseHeader(_rows[0][i]);
            _columns[i] = _columns.Take(i).Contains(type) ? SoundImportColumn.None : type;
        }
    }

    private IEnumerable<string[]> DataRows => cboxFirstHeader.Checked ? _rows.Skip(1) : _rows;

    private void ShowTable()
    {
        dgvData.SuspendLayout();
        dgvData.Rows.Clear();
        dgvData.Columns.Clear();
        for (var i = 0; i < _columns.Count; i++)
        {
            dgvData.Columns.Add(new DataGridViewTextBoxColumn
            {
                SortMode = DataGridViewColumnSortMode.NotSortable,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None
            });
        }

        foreach (var row in DataRows)
            dgvData.Rows.Add(row.Cast<object>().ToArray());

        UpdateHeaders();
        // sirka podla obsahu, no dlhe texty hlaseni nezaberu celu tabulku
        dgvData.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
        foreach (DataGridViewColumn column in dgvData.Columns)
            column.Width = Math.Clamp(column.Width, 90, 320);
        dgvData.ResumeLayout();
    }

    private void UpdateHeaders()
    {
        for (var i = 0; i < _columns.Count; i++)
        {
            var column = dgvData.Columns[i];
            column.HeaderText = _columns[i].Name;
            // povodna hlavicka zo suboru ostane v tipe
            column.ToolTipText = cboxFirstHeader.Checked && _rows.Count > 0 ? _rows[0][i] : "";
            column.HeaderCell.Style.Font = _columns[i] == SoundImportColumn.None ? null : _boldFont;
        }
    }

    private void SetColumn(SoundImportColumn type)
    {
        if (_menuColumn < 0 || _menuColumn >= _columns.Count)
            return;

        // kazdy typ len v jednom stlpci - predchadzajuci stlpec s tymto typom sa uvolni
        if (type != SoundImportColumn.None)
            for (var i = 0; i < _columns.Count; i++)
                if (_columns[i] == type)
                    _columns[i] = SoundImportColumn.None;

        _columns[_menuColumn] = type;
        UpdateHeaders();
    }

    private void DgvData_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
    {
        _menuColumn = e.ColumnIndex;
        foreach (ToolStripMenuItem item in cmsColumns.Items)
            item.Checked = ReferenceEquals(item.Tag, _columns[_menuColumn]);
        cmsColumns.Show(Cursor.Position);
    }

    private void CboxFirstHeader_CheckedChanged(object sender, EventArgs e)
    {
        if (cboxFirstHeader.Checked)
            DetectColumns();
        ShowTable();
    }

    private void CbEncoding_SelectedIndexChanged(object sender, EventArgs e)
    {
        // kodovanie sa tyka len CSV suboru - schranka aj Excel uz text maju; subor sa nacita znova
        if (_lastCsvPath != null && File.Exists(_lastCsvPath))
            LoadFile(_lastCsvPath);
    }

    private void BImport_Click(object sender, EventArgs e)
    {
        var rows = DataRows.Select(r => (IReadOnlyList<string>)r).ToList();
        if (rows.Count == 0)
        {
            Utils.ShowError(Resources.Import_NoData);
            return;
        }

        var plan = SoundImport.Build(_language, rows, _columns, cboxSkipExisting.Checked, cboxFirstHeader.Checked ? 2 : 1);
        if (plan.Errors.Count > 0)
        {
            var more = plan.Errors.Count > 10 ? "\n" + string.Format(CultureInfo.CurrentCulture, Resources.Convert_More, plan.Errors.Count - 10) : "";
            Utils.ShowError(string.Format(CultureInfo.CurrentCulture, Resources.Import_Errors, string.Join("\n", plan.Errors.Take(10)) + more));
            return;
        }

        if (!plan.AllSounds.Any())
        {
            Utils.ShowInfo(string.Format(CultureInfo.CurrentCulture, Resources.Import_NothingNew, plan.Skipped.Count));
            return;
        }

        Plan = plan;
        // tlacidlo nema DialogResult v navrhu - okno zavrie az toto
        DialogResult = DialogResult.OK;
    }

    private void FImportSounds_DragEnter(object sender, DragEventArgs e)
        => e.Effect = e.Data?.GetDataPresent(DataFormats.FileDrop) == true ? DragDropEffects.Copy : DragDropEffects.None;

    private void FImportSounds_DragDrop(object sender, DragEventArgs e)
    {
        if (e.Data?.GetData(DataFormats.FileDrop) is string[] { Length: > 0 } files)
            LoadFile(files[0]);
    }
}
