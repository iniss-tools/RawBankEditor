using System.Diagnostics.CodeAnalysis;
using RawBankEditor.Forms;
using ToolsCore.Entities;
using ToolsCore.Tools;
using static RawBankEditor.Entities.BankMessagePaths;
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

    public abstract void Resolve();

    public abstract void Show();
}

/// <summary>
/// Spolocne pre spravy zoznamu chyb.
/// </summary>
internal static class BankMessagePaths
{
    /// <summary>
    /// Cesta vzhladom na priecinok banky (RAWBANK) - rovnako ako pri chybajucich priecinkoch a suboroch.
    /// </summary>
    public static string RelativeToBank(string fullPath)
        => System.IO.Path.GetRelativePath(GlobData.OpenedProject!.AbsPathToBank, fullPath);
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
    public string Message => $"Jazyk '{Language.Key}' je definovaný, ale neexistuje v súborovom systéme.";

    /// <inheritdoc />
    public string ResolveMessage => $"Vytvoriť priečinok '{Language.RelativePath}'.";

    /// <inheritdoc />
    public string Path => Language.GetAbsPath("");

    public FyzLanguage Language { get; }

    public LanguageDirMissing(FyzLanguage language)
    {
        Language = language;
    }

    public void Resolve()
    {
        Program.MainForm.CreateLanguageDirectory(Language);
    }

    /// <inheritdoc />
    public void Show()
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
    public string Message => $"Skupina '{Group.Name}' je definovaná, ale neexistuje v súborovom systéme.";

    /// <inheritdoc />
    public string ResolveMessage => $"Vytvoriť priečinok '{Group.GetAbsPath("")}'.";

    /// <inheritdoc />
    public string Path => Group.GetAbsPath("");

    public FyzGroup Group { get; }

    public GroupDirMissing(FyzGroup group)
    {
        Group = group;
    }

    public void Resolve()
    {
        try
        {
            var path = Group.GetAbsPath(GlobData.OpenedProject!.AbsPathToBank);
            Program.MainForm.WithoutFileWatcher(() => Directory.CreateDirectory(path));
        }
        catch (Exception e)
        {
            Utils.ShowError(e.Message);
            return;
        }

        // priecinok sa prida do prieskumnika a zvuky skupiny sa prepoja s nahravkami, ktore v nom uz su
        Program.MainForm.LinkGroupDirectory(Group);
        foreach (var sound in Group.Sounds)
            Program.MainForm.RelinkSoundFile(sound);
        Program.MainForm.FillExplorerList(Group.Directory);
    }

    /// <inheritdoc />
    public void Show()
    {
        var index = Program.MainForm.CurrentLanguage!.Groups.IndexOf(Group);
        if (index != -1)
        {
            Program.MainForm.dgvGroups.ClearSelection();
            Program.MainForm.dgvGroups.Rows[index].Selected = true;
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
    public string Message => $"Zvuk '{Sound.Name}' je definovaný, ale neexistuje v súborovom systéme.";

    /// <inheritdoc />
    public string ResolveMessage => $"Odstrániť zvuk '{Sound.Name}' zo zoznamu zvukov.";

    /// <inheritdoc />
    public string Path => Sound.GetAbsPath("");

    public FyzSound Sound { get; }

    public SoundFileMissing(FyzSound sound)
    {
        Sound = sound;
    }

    public void Resolve()
    {
        var action = new FMain.RemovedSoundsAction(Program.MainForm, Sound);
        action.Apply();
        Program.MainForm.RegisterNewAction(action);
    }

    /// <inheritdoc />
    public void Show()
    {
        var index = Program.MainForm.CurrentLanguage!.Groups.IndexOf(Sound.Group);
        if (index == -1) 
            return;

        Program.MainForm.dgvGroups.ClearSelection();
        var i = Program.MainForm.SelectSound(Sound);
        if (i != 0 && i != -1)
        {
            Program.MainForm.DoNotChangeSoundsSelection = true;
            Program.MainForm.dgvSounds.Rows[0].Selected = false;
            Program.MainForm.DoNotChangeSoundsSelection = false;
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
    public string Message => $"Zvuk '{File.Name}' existuje v súborovom systéme, ale nie je definovaný.";

    /// <inheritdoc />
    public string ResolveMessage => $"Pridať dáta o zvuku '{File.Name}'.";

    /// <inheritdoc />
    public string Path => RelativeToBank(File.FileInfo.FullName);

    public SoundFileElement File { get; }

    public SoundDataMissing(SoundFileElement file)
    {
        File = file;
    }
    
    public void Resolve()
    {
        var group = File.Parent?.Group;
        if (group is null)
        {
            Utils.ShowError($"Súbor '{File.Name}' nie je v priečinku žiadnej skupiny zvukov, údaje o zvuku sa nedajú pridať.");
            return;
        }

        var form = new FAddSound(group, File);
        if (form.ShowDialog() == DialogResult.OK)
        {
            Program.MainForm.RegisterNewAction(new FMain.AddSoundAction(Program.MainForm, form.Sound));
            group.Sounds.Add(form.Sound);
            Program.MainForm.RefreshSoundViews();
        }
    }

    /// <inheritdoc />
    public void Show()
    {
        var grp = File.Parent!.Group;
        var index = Program.MainForm.CurrentLanguage!.Groups.IndexOf(grp);
        if (index != -1)
        {
            Program.MainForm.dgvGroups.ClearSelection();
            Program.MainForm.dgvGroups.Rows[index].Selected = true;
            Program.MainForm.SelectElement = File;
        }
        else
        {
            Program.MainForm.SelectElement = File;
            Program.MainForm.FillExplorerList(File.Parent);
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
    public string Message => $"Súbor '{File.Name}' nie je platná nahrávka WAV.";

    /// <inheritdoc />
    public string ResolveMessage => $"Presunúť súbor '{File.Name}' do koša.";

    /// <inheritdoc />
    public string Path => RelativeToBank(File.FileInfo.FullName);

    public SoundFileElement File { get; }

    public InvalidSoundFile(SoundFileElement file)
    {
        File = file;
    }    

    /// <inheritdoc />
    public void Resolve()
    {
        // zvuk, ktoremu subor patril, ostane bez suboru
        if (File.Sound is { } sound && ReferenceEquals(sound.File, File))
        {
            sound.File = null!;
            File.Sound = null!;
        }

        Program.MainForm.RecycleElement(File);
        Program.MainForm.FillExplorerList(Program.MainForm.CurrentDirectory);
    }

    /// <inheritdoc />
    public void Show()
    {
        var grp = File.Parent!.Group;
        var index = Program.MainForm.CurrentLanguage!.Groups.IndexOf(grp);
        if (index != -1)
        {
            Program.MainForm.dgvGroups.ClearSelection();
            Program.MainForm.dgvGroups.Rows[index].Selected = true;
        }
        else
        {
            Program.MainForm.FillExplorerList(File.Parent);
        }

        index = Program.MainForm.ExplorerContent.IndexOf(File);
        if (index == -1)
            return;

        Program.MainForm.dgvExplorer.ClearSelection();
        Program.MainForm.dgvExplorer.Rows[index].Selected = true;
    }
}

public class EmptyGroup : IRawBankMessage
{
    /// <inheritdoc />
    public string Code => "EmptyGroup";

    /// <inheritdoc />
    public MessageType Type => MessageType.Info;

    /// <inheritdoc />
    public string Message => $"Skupina '{Group.Name}' je prázdna.";

    /// <inheritdoc />
    public string ResolveMessage => $"Odstrániť skupinu '{Group.Name}'.";

    /// <inheritdoc />
    public string Path => Group.GetAbsPath("");

    public FyzGroup Group { get; }

    public EmptyGroup(FyzGroup group)
    {
        Group = group;
    }

    /// <inheritdoc />
    public void Resolve()
    {
        // priecinok ide do kosa len prazdny - nahravky bez udajov o zvuku by sa inak stratili nepozorovane
        var directory = FMain.GroupDirectoryPath(Group);
        var emptyDirectory = Directory.Exists(directory) && !Directory.EnumerateFileSystemEntries(directory).Any();
        Program.MainForm.RemoveGroup(Group, emptyDirectory);
    }

    /// <inheritdoc />
    public void Show()
    {
        var index = Program.MainForm.CurrentLanguage!.Groups.IndexOf(Group);
        if (index != -1)
        {
            Program.MainForm.dgvGroups.ClearSelection();
            Program.MainForm.dgvGroups.Rows[index].Selected = true;
        }
    }
}