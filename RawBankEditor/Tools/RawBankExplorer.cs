using System.Globalization;
using RawBankEditor.Entities;
using ToolsCore.Entities;
using ToolsCore.Tools;

namespace RawBankEditor.Tools;

internal static class RawBankExplorer
{
    public static DirectoryElement ExploreFileSystem() => new(GlobData.OpenedProject!.AbsPathToBank);

    public static void MergeFilesAndData(DirectoryElement root, FyzLanguage lang, Dictionary<FyzLanguage,List<IRawBankMessage>> dict, bool onlyCheck = false)
    {
        List<IRawBankMessage> messages;
        if (onlyCheck)
        {
            dict[lang].Clear();
            messages = dict[lang];
        }
        else
        {
            dict.Remove(lang);

            messages = new List<IRawBankMessage>();
            dict.Add(lang, messages);
        }
        

        //languages
        var langName = lang.RelativePath.Replace("\\", "");
        if (root.Children.FirstOrDefault(langDir => EqualsPathNames(langDir.Name, langName)) is not DirectoryElement dir)
        {
            messages.Add(new LanguageDirMissing(lang));
            return;
        }

        if (!onlyCheck)
        {
            lang.Directory = dir;

            //groups
            foreach (var grpElement in dir.Children)
            {
                if (grpElement is not DirectoryElement de)
                    continue;

                foreach (var grp in lang.Groups)
                {
                    if (!EqualsPathNames(de.Name, grp.RelativePath.Replace("\\","")))
                        continue;

                    grp.Directory = de;
                    de.Group = grp;
                }
            }

            LinkSoundFiles(dir, lang);
        }
        
        //check files (link LOGICAL -> FILE)
        foreach (var langGroup in lang.Groups)
        {
            if (langGroup.Directory == null)
            {
                messages.Add(new GroupDirMissing(langGroup));
            }

            if (langGroup.Sounds.Count == 0)
            {
                messages.Add(new EmptyGroup(langGroup));
                continue;
            }

            foreach (var sound in langGroup.Sounds)
            {
                if (sound.File == null)
                {
                    messages.Add(new SoundFileMissing(sound));
                }
            }
        }

        //check files (link FILE -> LOGICAL)
        CheckDefOfFile(dir);

        void CheckDefOfFile(DirectoryElement de)
        {
            foreach (var child in de.Children)
            {
                if (child is DirectoryElement d)
                {
                    CheckDefOfFile(d);
                }
                else if (child is SoundFileElement sfe)
                {
                    if (sfe.Sound is null)
                        messages.Add(new SoundDataMissing(sfe));
                    // dlzka sa zistuje az pri zobrazeni priecinka v prieskumniku - vtedy sa ukaze aj neplatna nahravka
                    if (sfe.Duration == SoundUtils.SOUND_ERROR)
                        messages.Add(new InvalidSoundFile(sfe));
                }
            }
        }
    }

    /// <summary>
    /// Prepoji zvuky jazyka s ich nahravkami. Najprv zvuky s nahravkou priamo v priecinku skupiny, potom zvuky
    /// s pridavnou cestou - tie ukazuju casto do priecinka inej skupiny (napr. ..\Poz1\ZALOK.WAV) a subor tam uz
    /// moze patrit zvuku tej skupiny. Neexistujuci subor sa neprepoji - zoznam chyb ho ukaze ako chybajuci.
    /// </summary>
    private static void LinkSoundFiles(DirectoryElement languageDir, FyzLanguage lang)
    {
        var pathToBank = GlobData.OpenedProject!.AbsPathToBank;
        foreach (var additional in new[] { false, true })
        {
            foreach (var snd in lang.Groups.SelectMany(g => g.Sounds))
            {
                if (RawBankParser.AdditionalPathIsEmpty(snd.AdditionalRelativePath) == additional)
                    continue;

                SoundFileElement? file;
                if (!additional)
                {
                    file = snd.Group.Directory?.Children.OfType<SoundFileElement>()
                        .FirstOrDefault(f => snd.FileName is not null && EqualsPathNames(f.Name, snd.FileName));
                }
                else
                {
                    var path = Path.GetFullPath(snd.GetAbsPath(pathToBank));
                    file = FindInTree(languageDir, path) ?? (File.Exists(path) ? new SoundFileElement(path) : null);
                }

                if (file is null)
                    continue;

                snd.File = file;
                file.Sound ??= snd;
            }
        }
    }

    // prvok suboru v strome jazyka podla absolutnej cesty (bez ohladu na velkost pismen)
    private static SoundFileElement? FindInTree(DirectoryElement languageDir, string path)
    {
        var relative = Path.GetRelativePath(languageDir.DirInfo.FullName, path);
        if (relative.StartsWith("..", StringComparison.Ordinal) || Path.IsPathRooted(relative))
            return null;

        FileSystemElement current = languageDir;
        foreach (var part in relative.Split(Path.DirectorySeparatorChar))
        {
            if (current is not DirectoryElement de)
                return null;
            var next = de.Children.FirstOrDefault(c => EqualsPathNames(c.Name, part));
            if (next is null)
                return null;
            current = next;
        }

        return current as SoundFileElement;
    }

    public static bool EqualsPathNames(string name1, string name2) => string.Equals(name1, name2, StringComparison.CurrentCultureIgnoreCase);

    /// <summary>
    /// Ak uz bol Dir element vytvoreny skor, nie je potrebne ho v metode GetElement() vytvarat znova (pretoze asi nebude kompletny).
    /// </summary>
    public static DirectoryElement? AddDirHandled { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public static bool ConvertSoundIsHandled { get; set; }

    public static bool MovingSoundIsHandled { get; set; }

    public static FileSystemElement? GetElement(string fullpath, DirectoryElement root, SearchOperation op = SearchOperation.None)
    {
        if (root is null)
            throw new ArgumentNullException(nameof(root));

        var path = Utils.GetRelativePath(fullpath, GlobData.OpenedProject!.AbsPathToBank);

        var splitted = path.Split('\\');
        for (var i = 1; i < splitted.Length; i++)
        {
            var slice = splitted[i];
            var elem = GetElem(root, slice);
            if (op == SearchOperation.Delete && elem != null && i == splitted.Length - 1)
            {
                root.Children.Remove(elem);
                switch (elem)
                {
                    case DirectoryElement dirElem:
                        if (dirElem.Group is not null)
                            dirElem.Group.Directory = null!;
                        break;
                    case SoundFileElement sfileElem:
                        // zvuk si nazov suboru necha - zoznam chyb ukaze chybajuci subor; vrateny subor sa so zvukom znova prepoji
                        if (sfileElem.Sound is not null && ReferenceEquals(sfileElem.Sound.File, sfileElem))
                            sfileElem.Sound.File = null!;
                        sfileElem.FileInfo = null!;
                        break;
                    case FileElement fileElem:
                        fileElem.FileInfo = null!;
                        break;
                }
                return elem;
            }

            switch (elem)
            {
                case null when op == SearchOperation.Create && i == splitted.Length - 1:
                    if (File.GetAttributes(fullpath).HasFlag(FileAttributes.Directory))
                    {
                        var newElement = AddDirHandled ?? new DirectoryElement(fullpath);
                        newElement.Parent = root;
                        root.Children.Add(newElement);
                        if (AddDirHandled is not null) 
                            AddDirHandled = null;
                        return newElement;
                    }
                    else
                    {
                        FileElement newElement;
                        if (Path.GetExtension(fullpath).ToUpper(CultureInfo.CurrentCulture) is SoundFileElement.EWAExt or SoundFileElement.WAVExt)
                            newElement = new SoundFileElement(fullpath);
                        else
                            newElement = new OtherFileElement(fullpath);

                        newElement.Parent = root;
                        root.Children.Add(newElement);
                        return newElement;
                    }
                case null:
                    return null;
                case DirectoryElement de when i != splitted.Length - 1:
                    root = de;
                    continue;
                case FileElement when i != splitted.Length - 1:
                    return null;
                default:
                    return elem;
            }
        }

        return null;

        static FileSystemElement? GetElem(DirectoryElement de, string slice)
        {
            foreach (var child in de.Children)
            {
                if (child.Name == slice)
                    return child;
            }
            return null;
        }
    }

    public enum SearchOperation
    {
        None,
        Create,
        Delete
    }
}