using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using RawBankEditor.Forms;
using RawBankEditor.Properties;
using ToolsCore.Iniss.Entities;
using ToolsCore.Tools;
// ReSharper disable MemberCanBePrivate.Global

namespace RawBankEditor.Entities;

[SuppressMessage("ReSharper", "UnusedMember.Global")]
public interface IRawBankMessage
{
    public abstract string Code { get; }

    public abstract MessageType Type { get; }

    public abstract string Message { get; }

    public abstract string ResolveMessage { get; }

    public abstract string Path { get; }

    /// <summary>
    /// Vyriesi problem (Vyriesit v zozname chyb).
    /// </summary>
    /// <param name="form">hlavne okno</param>
    public abstract void Resolve(FMain form);

    /// <summary>
    /// Ukaze problem v hlavnom okne (vyberie skupinu, zvuk alebo subor).
    /// </summary>
    /// <param name="form">hlavne okno</param>
    public abstract void Show(FMain form);
}

public enum MessageType
{
    Info,
    Warning,
    Error
}

public class LanguageDirMissing : IRawBankMessage
{
    /// <inheritdoc />
    public string Code => "LanguageDirMissing";

    /// <inheritdoc />
    public MessageType Type => MessageType.Error;

    /// <inheritdoc />
    public string Message => string.Format(CultureInfo.CurrentCulture, Resources.Msg_LanguageMissing, Language.Key);

    /// <inheritdoc />
    public string ResolveMessage => string.Format(CultureInfo.CurrentCulture, Resources.Msg_CreateDir, Language.RelativePath);

    /// <inheritdoc />
    public string Path => Language.GetAbsPath("");

    public FyzLanguage Language { get; }

    public LanguageDirMissing(FyzLanguage language)
    {
        Language = language;
    }

    public void Resolve(FMain form)
    {
        form.CreateLanguageDirectory(Language);
    }

    /// <inheritdoc />
    public void Show(FMain form)
    {
        // do nothing
    }
}

public class GroupDirMissing : IRawBankMessage
{
    /// <inheritdoc />
    public string Code => "GroupDirMissing";

    /// <inheritdoc />
    public MessageType Type => MessageType.Error;

    /// <inheritdoc />
    public string Message => string.Format(CultureInfo.CurrentCulture, Resources.Msg_GroupMissing, Group.Name);

    /// <inheritdoc />
    public string ResolveMessage => string.Format(CultureInfo.CurrentCulture, Resources.Msg_CreateDir, Group.GetAbsPath(""));

    /// <inheritdoc />
    public string Path => Group.GetAbsPath("");

    public FyzGroup Group { get; }

    public GroupDirMissing(FyzGroup group)
    {
        Group = group;
    }

    public void Resolve(FMain form)
    {
        // priecinok sa prida do prieskumnika a zvuky skupiny sa prepoja s nahravkami, ktore v nom uz su
        form.CreateGroupDirectory(Group);
    }

    /// <inheritdoc />
    public void Show(FMain form)
    {
        var index = form.CurrentLanguage!.Groups.IndexOf(Group);
        if (index != -1)
        {
            form.dgvGroups.ClearSelection();
            form.dgvGroups.Rows[index].Selected = true;
        }
    }    
}

public class SoundFileMissing : IRawBankMessage
{
    /// <inheritdoc />
    public string Code => "SoundFileMissing";

    /// <inheritdoc />
    public MessageType Type => MessageType.Error;

    /// <inheritdoc />
    public string Message => string.Format(CultureInfo.CurrentCulture, Resources.Msg_SoundMissing, Sound.Name);

    /// <inheritdoc />
    public string ResolveMessage => string.Format(CultureInfo.CurrentCulture, Resources.Msg_RemoveSound, Sound.Name);

    /// <inheritdoc />
    public string Path => Sound.GetAbsPath("");

    public FyzSound Sound { get; }

    public SoundFileMissing(FyzSound sound)
    {
        Sound = sound;
    }

    public void Resolve(FMain form)
    {
        var action = new FMain.RemovedSoundsAction(form, Sound);
        action.Apply();
        form.RegisterNewAction(action);
    }

    /// <inheritdoc />
    public void Show(FMain form)
    {
        var index = form.CurrentLanguage!.Groups.IndexOf(Sound.Group);
        if (index == -1) 
            return;

        form.dgvGroups.ClearSelection();
        var i = form.SelectSound(Sound);
        if (i != 0 && i != -1)
        {
            form.DoNotChangeSoundsSelection = true;
            form.dgvSounds.Rows[0].Selected = false;
            form.DoNotChangeSoundsSelection = false;
        }
    }
}

public class SoundDataMissing : IRawBankMessage
{
    /// <inheritdoc />
    public string Code => "SoundDataMissing";

    /// <inheritdoc />
    public MessageType Type => MessageType.Warning;

    /// <inheritdoc />
    public string Message => string.Format(CultureInfo.CurrentCulture, Resources.Msg_SoundUndefined, File.Name);

    /// <inheritdoc />
    public string ResolveMessage => string.Format(CultureInfo.CurrentCulture, Resources.Msg_AddSoundData, File.Name);

    /// <inheritdoc />
    public string Path => System.IO.Path.GetRelativePath(PathToBank, File.FileInfo.FullName);

    public SoundFileElement File { get; }

    /// <summary>
    /// Priecinok RAWBANK - cesta suboru sa ukazuje vzhladom na neho, rovnako ako pri chybajucich priecinkoch a suboroch.
    /// </summary>
    private string PathToBank { get; }

    public SoundDataMissing(SoundFileElement file, string pathToBank)
    {
        File = file;
        PathToBank = pathToBank;
    }
    
    public void Resolve(FMain form)
    {
        var group = File.Parent?.Group;
        if (group is null)
        {
            Utils.ShowError(string.Format(CultureInfo.CurrentCulture, Resources.Msg_FileNotInGroup, File.Name));
            return;
        }

        var dialog = new FAddSound(group, form.PathToBank, File);
        if (dialog.ShowDialog(form) == DialogResult.OK)
        {
            form.RegisterNewAction(new FMain.AddSoundAction(form, dialog.Sound));
            group.Sounds.Add(dialog.Sound);
            form.RefreshSoundViews();
        }
    }

    /// <inheritdoc />
    public void Show(FMain form)
    {
        var grp = File.Parent!.Group;
        var index = form.CurrentLanguage!.Groups.IndexOf(grp);
        if (index != -1)
        {
            form.dgvGroups.ClearSelection();
            form.dgvGroups.Rows[index].Selected = true;
            form.SelectElement = File;
        }
        else
        {
            form.SelectElement = File;
            form.FillExplorerList(File.Parent);
        }
    }
}

public class InvalidSoundFile : IRawBankMessage
{
    /// <inheritdoc />
    public string Code => "InvalidSoundFile";

    /// <inheritdoc />
    public MessageType Type => MessageType.Error;

    /// <inheritdoc />
    public string Message => string.Format(CultureInfo.CurrentCulture, Resources.Msg_InvalidWav, File.Name);

    /// <inheritdoc />
    public string ResolveMessage => string.Format(CultureInfo.CurrentCulture, Resources.Msg_RecycleFile, File.Name);

    /// <inheritdoc />
    public string Path => System.IO.Path.GetRelativePath(PathToBank, File.FileInfo.FullName);

    public SoundFileElement File { get; }

    /// <summary>
    /// Priecinok RAWBANK - cesta suboru sa ukazuje vzhladom na neho, rovnako ako pri chybajucich priecinkoch a suboroch.
    /// </summary>
    private string PathToBank { get; }

    public InvalidSoundFile(SoundFileElement file, string pathToBank)
    {
        File = file;
        PathToBank = pathToBank;
    }    

    /// <inheritdoc />
    public void Resolve(FMain form)
    {
        // zvuk, ktoremu subor patril, ostane bez suboru
        if (File.Sound is { } sound && ReferenceEquals(sound.File, File))
        {
            sound.File = null!;
            File.Sound = null!;
        }

        form.RecycleElement(File);
        form.FillExplorerList(form.CurrentDirectory);
    }

    /// <inheritdoc />
    public void Show(FMain form)
    {
        var grp = File.Parent!.Group;
        var index = form.CurrentLanguage!.Groups.IndexOf(grp);
        if (index != -1)
        {
            form.dgvGroups.ClearSelection();
            form.dgvGroups.Rows[index].Selected = true;
        }
        else
        {
            form.FillExplorerList(File.Parent);
        }

        index = form.ExplorerContent.IndexOf(File);
        if (index == -1)
            return;

        form.dgvExplorer.ClearSelection();
        form.dgvExplorer.Rows[index].Selected = true;
    }
}

public class EmptyGroup : IRawBankMessage
{
    /// <inheritdoc />
    public string Code => "EmptyGroup";

    /// <inheritdoc />
    public MessageType Type => MessageType.Info;

    /// <inheritdoc />
    public string Message => string.Format(CultureInfo.CurrentCulture, Resources.Msg_GroupEmpty, Group.Name);

    /// <inheritdoc />
    public string ResolveMessage => string.Format(CultureInfo.CurrentCulture, Resources.Msg_RemoveGroup, Group.Name);

    /// <inheritdoc />
    public string Path => Group.GetAbsPath("");

    public FyzGroup Group { get; }

    public EmptyGroup(FyzGroup group)
    {
        Group = group;
    }

    /// <inheritdoc />
    public void Resolve(FMain form)
    {
        // priecinok ide do kosa len prazdny - nahravky bez udajov o zvuku by sa inak stratili nepozorovane
        var directory = form.GroupDirectoryPath(Group);
        var emptyDirectory = Directory.Exists(directory) && !Directory.EnumerateFileSystemEntries(directory).Any();
        form.RemoveGroup(Group, emptyDirectory);
    }

    /// <inheritdoc />
    public void Show(FMain form)
    {
        var index = form.CurrentLanguage!.Groups.IndexOf(Group);
        if (index != -1)
        {
            form.dgvGroups.ClearSelection();
            form.dgvGroups.Rows[index].Selected = true;
        }
    }
}