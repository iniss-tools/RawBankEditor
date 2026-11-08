using System.Globalization;
using RawBankEditor.Properties;
using ToolsCore.Iniss.Entities;
using ToolsCore.Iniss.Tools;

namespace RawBankEditor.Tools;

/// <summary>
/// Import zvukov z tabulky (Excel, CSV, schranka) do jazyka banky. Kazdy riadok je jeden zvuk; skupina sa najde podla
/// kluca, alebo sa vytvori nova. Nevyplnene hodnoty sa doplnia rovnako ako v oknach Pridat skupinu a Pridat zvuk.
/// </summary>
public static class SoundImport
{
    /// <summary>
    /// Skontroluje tabulku a pripravi skupiny a zvuky na pridanie. Jazyk sa nemeni.
    /// </summary>
    /// <param name="language">jazyk s nacitanymi skupinami</param>
    /// <param name="rows">riadky s udajmi (bez hlavicky)</param>
    /// <param name="columns">typ kazdeho stlpca tabulky</param>
    /// <param name="skipExisting">zvuky s klucom, ktory v skupine uz je, preskocit (inak su chybou)</param>
    /// <param name="firstRowNumber">cislo prveho riadku v tabulke - do textov chyb</param>
    public static SoundImportPlan Build(FyzLanguage language, IReadOnlyList<IReadOnlyList<string>> rows, IReadOnlyList<SoundImportColumn> columns,
        bool skipExisting, int firstRowNumber = 1)
    {
        ArgumentNullException.ThrowIfNull(language);
        ArgumentNullException.ThrowIfNull(rows);
        ArgumentNullException.ThrowIfNull(columns);

        var plan = new SoundImportPlan();
        var missing = SoundImportColumn.Required.Where(c => !columns.Contains(c)).ToList();
        if (missing.Count > 0)
        {
            plan.Errors.Add(string.Format(CultureInfo.CurrentCulture, Resources.Import_MissingColumns, string.Join(", ", missing)));
            return plan;
        }

        var sounds = new List<(FyzSound Sound, int Row)>();
        var errors = new List<(int Row, string Text)>();
        for (var i = 0; i < rows.Count; i++)
        {
            var row = rows[i];
            var rowNumber = firstRowNumber + i;
            if (row.All(string.IsNullOrWhiteSpace))
                continue;

            string Cell(SoundImportColumn column)
            {
                var index = IndexOf(columns, column);
                return index >= 0 && index < row.Count ? row[index].Trim() : "";
            }

            void Error(string text) => errors.Add((rowNumber, text));

            var groupKey = Cell(SoundImportColumn.GroupKey);
            var soundKey = Cell(SoundImportColumn.SoundKey);
            if (groupKey.Length == 0 || soundKey.Length == 0)
            {
                Error(Resources.Import_KeysRequired);
                continue;
            }

            var group = FindGroup(language.Groups, groupKey) ?? FindGroup(plan.NewGroups, groupKey);
            if (group is null)
            {
                var name = Cell(SoundImportColumn.GroupName);
                var path = Cell(SoundImportColumn.GroupPath);
                if (name.Length == 0)
                    name = groupKey;
                if (path.Length == 0)
                    path = GroupRules.DefaultRelativePath(groupKey);
                else if (!path.EndsWith('\\'))
                    path += '\\';

                if (GroupRules.Validate(language.Groups.Concat(plan.NewGroups), null, groupKey, name, path) is { } groupError)
                {
                    Error(groupError);
                    continue;
                }

                group = new FyzGroup(language, groupKey, name, path);
                plan.NewGroups.Add(group);
            }
            else if (skipExisting && group.Sounds.Any(s => SoundRules.SameText(s.Key, soundKey)))
            {
                plan.Skipped.Add(group.Key + "/" + soundKey);
                continue;
            }

            var soundName = Cell(SoundImportColumn.SoundName);
            var file = Cell(SoundImportColumn.FileName);
            var additionalPath = "";
            if (file.Length == 0)
            {
                file = SoundUtils.GetDefaultFileName(group, soundKey);
            }
            else if (file.LastIndexOf('\\') is var slash and >= 0)
            {
                additionalPath = file[..(slash + 1)];
                file = file[(slash + 1)..];
            }

            var ext = Path.GetExtension(file);
            if (file.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0 || !(ext.EqualsIgnoreCase(SoundUtils.WAVExt) || ext.EqualsIgnoreCase(SoundUtils.EWAExt)))
            {
                Error(string.Format(CultureInfo.CurrentCulture, Resources.FAddSound_InvalidFileName, SoundUtils.WAVExt, SoundUtils.EWAExt));
                continue;
            }

            var sound = new FyzSound(group, soundKey, soundName.Length == 0 ? soundKey : soundName, file, additionalPath,
                Cell(SoundImportColumn.Text), 0);
            sounds.Add((sound, rowNumber));
        }

        // kluce a nazvy - voci zvukom skupiny aj navzajom, rovnako ako pri pridani zvukov
        var problems = SoundRules.ValidateNew(sounds.Select(s => s.Sound));
        foreach (var (sound, row) in sounds)
            if (problems.TryGetValue(sound, out var text))
                errors.Add((row, text));

        if (errors.Count > 0)
        {
            plan.Errors.AddRange(errors.OrderBy(e => e.Row)
                .Select(e => string.Format(CultureInfo.CurrentCulture, Resources.Import_RowError, e.Row, e.Text)));
            return plan;
        }

        foreach (var (sound, _) in sounds)
        {
            if (plan.NewGroups.Contains(sound.Group))
                sound.Group.Sounds.Add(sound);
            else
                plan.SoundsForExistingGroups.Add(sound);
        }

        return plan;
    }

    private static int IndexOf(IReadOnlyList<SoundImportColumn> columns, SoundImportColumn column)
    {
        for (var i = 0; i < columns.Count; i++)
            if (ReferenceEquals(columns[i], column))
                return i;

        return -1;
    }

    private static FyzGroup? FindGroup(IEnumerable<FyzGroup> groups, string key) => groups.FirstOrDefault(g => SoundRules.SameText(g.Key, key));
}
