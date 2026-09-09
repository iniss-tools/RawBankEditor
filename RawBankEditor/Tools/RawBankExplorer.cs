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

                    //sounds
                    foreach (var sndElement in de.Children)
                    {
                        if (sndElement is not SoundFileElement sfe)
                            continue;

                        foreach (var snd in grp.Sounds)
                        {
                            if (!RawBankParser.AdditionalPathIsEmpty(snd.AdditionalRelativePath))
                            {
                                snd.File = new SoundFileElement(snd.GetAbsPath(GlobData.OpenedProject!.AbsPathToBank)) { Sound = snd };
                            }
                            else if (EqualsPathNames(sfe.Name, snd.FileName))
                            {
                                snd.File = sfe;
                                snd.File.Sound = snd;
                            }
                        }
                    }
                }
            }
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
                else if (child is SoundFileElement {Sound: null} sfe)
                {
                    messages.Add(new SoundDataMissing(sfe));
                }
            }
        }
    }

    public static bool EqualsPathNames(string name1, string name2) => string.Equals(name1, name2, StringComparison.CurrentCultureIgnoreCase);

    /// <summary>
    ///     Ak uz bol Dir element vytvoreny skor, nie je potrebne ho v metode GetElement() vytvarat znova (pretoze asi nebude kompletny).
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
                        if (sfileElem.Sound is not null)
                        {
                            sfileElem.Parent?.Children.Remove(sfileElem);
                            sfileElem.Sound.FileName = null!;
                            sfileElem.Sound.File = null!;
                        }
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