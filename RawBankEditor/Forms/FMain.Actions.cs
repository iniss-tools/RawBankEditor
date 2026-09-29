using System.Globalization;
using ExControls.Providers;
using RawBankEditor.Properties;
using RawBankEditor.Tools;
using ToolsCore.Iniss.Entities;
using ToolsCore.Tools;

namespace RawBankEditor.Forms;

partial class FMain
{
    public enum PropertyType
    {
        Name,
        Key,
        FileName,
        RelativePath,
        Text
    }

    /// <summary>
    /// Akcia pri akejkolvek zmene - pridanie, uprava, zmazanie
    /// </summary>
    public abstract class Action : IUndoRedoCommand
    {
        /// <summary>Initializes a new instance of the <see cref="Action" /> class.</summary>
        protected Action(FMain form)
        {
            Form = form ?? throw new ArgumentNullException(nameof(form));            
        }

        protected FMain Form { get; }
        
        /// <inheritdoc />
        public abstract string CommandName { get; }
        
        /// <inheritdoc />        
        public abstract void Undo();

        /// <inheritdoc />
        public abstract void Redo();
    }

    /// <summary>
    /// Akcia pri akejkolvek zmene - pridanie, uprava, zmazanie
    /// </summary>
    public abstract class MoveAction : IBackwardForwardCommand
    {
        /// <summary>Initializes a new instance of the <see cref="Action" /> class.</summary>
        protected MoveAction(FMain form)
        {
            Form = form ?? throw new ArgumentNullException(nameof(form));
        }

        protected FMain Form { get; }

        /// <inheritdoc />
        public abstract string CommandName { get; }

        /// <inheritdoc />        
        public abstract void Move();
    }

    /// <summary>
    /// Akcia pri zmene oznacenych buniek v tabulke so zvukmi
    /// </summary>
    public class SelectedCellSoundMoveAction : MoveAction
    {
        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public SelectedCellSoundMoveAction(FMain form, MovePosition position) : base(form)
        {
            Position = position;
        }

        private MovePosition Position { get; }

        /// <inheritdoc />
        public override string CommandName
        {
            get
            {
                var builder = new StringBuilder();
                if (Position.SelectedItems.Length >= 1) 
                    builder.Append($" {Position.SelectedItems[0].Name}");
                if (Position.SelectedItems.Length >= 2)
                    builder.Append($", {Position.SelectedItems[1].Name}");
                if (Position.SelectedItems.Length >= 3)
                    builder.Append($", {Position.SelectedItems[2].Name}");
                if (Position.SelectedItems.Length >= 4)
                    builder.Append($",... (+{Position.SelectedItems.Length - 3})");
                return $"Jazyk: '{Position.Language}', Skupina: {Position.Group}, Zvuky:{builder}";
            }
        }

        /// <inheritdoc />
        public override void Move() => Move(Position);

        private void Move(MovePosition pos)
        {
            if (pos?.Language is null || pos.Group is null)
                return;
            
            Form._programChange = true;
            Form.SelectGroup(pos.Group);

            Form.dgvSounds.ClearSelection();

            foreach (DataGridViewRow r in Form.dgvSounds.Rows)
                foreach (var s in pos.SelectedItems)
                    if (ReferenceEquals(r.DataBoundItem, s))
                        r.Selected = true;

            Form._programChange = false;
            if (!Form.dgvSounds.IsSelectionEmpty())
                EnsureVisibleRow(Form.dgvSounds, Form.dgvSounds.SelectedRows[0].Index);
        }
    }

    public class MovePosition
    {
        public FyzSound[] SelectedItems { get; }

        public FyzLanguage Language { get; }

        public FyzGroup Group { get; }        
        
        public MovePosition(FyzSound[] selected, FyzLanguage language, FyzGroup group)
        {
            SelectedItems = selected;
            Language = language;
            Group = group;
        }
    }

    public class AddSoundAction : Action
    {
        public AddSoundAction(FMain form, FyzSound sound) : base(form) => Sound = sound;

        /// <inheritdoc />
        public override string CommandName => "Pridanie zvuku";

        private FyzSound Sound { get; }

        public override void Undo()
        {
            Form.SelectGroup(Sound.Group);

            Sound.Group.Sounds.Remove(Sound);
            // subor ostane bez udajov o zvuku (SoundDataMissing)
            if (Sound.File?.Sound == Sound)
                Sound.File.Sound = null!;
            Form.RefreshSoundViews();
        }

        public override void Redo()
        {
            Form.SelectGroup(Sound.Group);

            Sound.Group.Sounds.Add(Sound);
            if (Sound.File is not null)
                Sound.File.Sound = Sound;
            Form.RefreshSoundViews();
            Form.dgvSounds.Rows[Sound.Group.Sounds.Count - 1].Selected = true;
        }
    }

    public class AddSoundsAction : Action
    {
        public AddSoundsAction(FMain form, IEnumerable<FyzSound> sounds) : base(form) => Sounds = sounds;

        /// <inheritdoc />
        public override string CommandName => "Pridanie zvukov";

        private IEnumerable<FyzSound> Sounds { get; }

        public override void Undo()
        {
            foreach (var sound in Sounds)
            {
                sound.Group.Sounds.Remove(sound);
                // subor ostane bez udajov o zvuku (SoundDataMissing)
                if (sound.File?.Sound == sound)
                    sound.File.Sound = null!;
            }
            Form.RefreshSoundViews();
        }

        public override void Redo()
        {
            foreach (var sound in Sounds)
            {
                sound.Group.Sounds.Add(sound);
                if (sound.File is not null)
                    sound.File.Sound = sound;
            }
            Form.RefreshSoundViews();
        }
    }

    /// <summary>
    /// Akcia pri uprave vlastnosti zvuku
    /// </summary>
    public class EditSoundAction : Action
    {
        public EditSoundAction(FMain form, FyzSound sound) : base(form)
        {
            Sound = sound;
        }

        /// <inheritdoc />
        public override string CommandName => Resources.Action_SoundData;

        private FyzSound Sound { get; }

        public string OldValue { get; set; } = null!;

        public string NewValue { get; set; } = null!;

        public PropertyType Type { get; set; }

        public override void Undo()
        {
            Form.SelectGroup(Sound.Group);

            switch (Type)
            {
                case PropertyType.Name:
                    Sound.Name = OldValue;
                    break;
                case PropertyType.Key:
                    Sound.Key = OldValue;
                    break;
                case PropertyType.FileName:
                    Sound.FileName = OldValue;
                    break;
                case PropertyType.RelativePath:
                    Sound.AdditionalRelativePath = OldValue;
                    break;
                case PropertyType.Text:
                    Sound.Text = OldValue;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            // iny nazov suboru alebo pridavna cesta = iny subor na disku
            if (Type is PropertyType.FileName or PropertyType.RelativePath)
                Form.RelinkSoundFile(Sound);
            Form.MenuSounds.ResetBindings();
        }

        public override void Redo()
        {
            Form.SelectGroup(Sound.Group);

            switch (Type)
            {
                case PropertyType.Name:
                    Sound.Name = NewValue;
                    break;
                case PropertyType.Key:
                    Sound.Key = NewValue;
                    break;
                case PropertyType.FileName:
                    Sound.FileName = NewValue;
                    break;
                case PropertyType.RelativePath:
                    Sound.AdditionalRelativePath = NewValue;
                    break;
                case PropertyType.Text:
                    Sound.Text = NewValue;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            // iny nazov suboru alebo pridavna cesta = iny subor na disku
            if (Type is PropertyType.FileName or PropertyType.RelativePath)
                Form.RelinkSoundFile(Sound);
            Form.MenuSounds.ResetBindings();
        }
    }

    /*public class ChangeSoundFileInDataAction : Action
    {
        /// <inheritdoc />
        public ChangeSoundFileInDataAction(FMain form, FyzSound sound, SoundFileElement oldFile, SoundFileElement newFile) : base(form)
        {
            Sound = sound;
            OldFile = oldFile;
            NewFile = newFile;
        }

        /// <inheritdoc />
        public override string CommandName => Resources.Action_SoundFile;
        private FyzSound Sound { get; }
        private SoundFileElement OldFile { get; }
        private SoundFileElement NewFile { get; }

        /// <inheritdoc />
        public override void Undo()
        {
            Sound.File = OldFile;
            Sound.FileName = OldFile.Name;
            Form.dgvSounds.ResetBindings();
        }

        /// <inheritdoc />
        public override void Redo()
        {
            Sound.File = NewFile;
            Sound.FileName = NewFile.Name;
            Form.dgvSounds.ResetBindings();
        }
    }*/

    public class MoveSoundsAction : Action
    {
        /// <inheritdoc />
        public MoveSoundsAction(FMain form, IEnumerable<FyzSound> sounds, FyzGroup oldLocation, FyzGroup newLocation) : base(form)
        {
            Sounds = sounds.ToList();
            OldLocation = oldLocation;
            NewLocation = newLocation;
        }

        /// <inheritdoc />
        public override string CommandName => "Presun zvukov";
        private List<FyzSound> Sounds { get; }
        private FyzGroup OldLocation { get; }
        private FyzGroup NewLocation { get; }

        /// <inheritdoc />
        public override void Undo() => Form.MoveSoundsToGroup(Sounds, OldLocation);

        /// <inheritdoc />
        public override void Redo() => Form.MoveSoundsToGroup(Sounds, NewLocation);
    }

    /// <summary>
    /// Akcia pri odstraneni viacerych (alebo 1) zvukov. Vytvara sa pred odstranenim - pamata si poradie zvukov
    /// v skupine, aby ich Spat vratilo na povodne miesta.
    /// </summary>
    public class RemovedSoundsAction : Action
    {
        /// <summary>Initializes a new instance of the <see cref="RemovedSoundsAction" /> class.</summary>
        public RemovedSoundsAction(FMain form, IEnumerable<FyzSound> sounds) : base(form)
        {
            Removed = sounds
                .Select(s => (Sound: s, Index: s.Group.Sounds.IndexOf(s)))
                .OrderBy(x => x.Index)
                .ToList();
        }

        /// <summary>Initializes a new instance of the <see cref="RemovedSoundsAction" /> class.</summary>
        public RemovedSoundsAction(FMain form, FyzSound sound) : this(form, new[] { sound })
        {
        }

        /// <inheritdoc />
        public override string CommandName => Resources.Action_DeleteSounds;

        private List<(FyzSound Sound, int Index)> Removed { get; }

        /// <summary>
        /// Subor zvuku, ktory sa s nim presunul do kosa (odstranenie v prieskumniku) - Spat ho obnovi.
        /// </summary>
        public SoundFileElement? RecycledFile { get; init; }

        /// <summary>
        /// Odstrani zvuky zo skupiny; ich subory ostanu na disku bez udajov o zvuku.
        /// </summary>
        public void Apply()
        {
            foreach (var (sound, _) in Removed)
            {
                sound.Group.Sounds.Remove(sound);
                if (sound.File?.Sound == sound)
                    sound.File.Sound = null!;
            }

            Form.RefreshSoundViews();
        }

        public override void Undo()
        {
            var grp = Removed[0].Sound.Group;
            Form.SelectGroup(grp);

            // subor sa nepodarilo obnovit - zvuk sa vrati bez neho
            if (RecycledFile is not null && !Form.RestoreElement(RecycledFile))
                foreach (var (sound, _) in Removed.Where(r => ReferenceEquals(r.Sound.File, RecycledFile)))
                    sound.File = null!;

            // od najmensieho indexu - kazdy zvuk sa vrati na miesto, ktore mal pred odstranenim
            foreach (var (sound, index) in Removed)
            {
                var sounds = sound.Group.Sounds;
                sounds.Insert(index < 0 ? sounds.Count : Math.Min(index, sounds.Count), sound);
                if (sound.File is not null)
                    sound.File.Sound = sound;
            }

            Form.RefreshSoundViews();
            foreach (var (sound, _) in Removed)
                Form.SelectSound(sound);
        }

        public override void Redo()
        {
            Form.SelectGroup(Removed[0].Sound.Group);
            if (RecycledFile is not null && File.Exists(RecycledFile.FileInfo.FullName) && !Form.RecycleElement(RecycledFile))
                return;
            Apply();
            Form.FillExplorerList(Form.CurrentDirectory);
        }
    }

    /// <summary>
    /// Premenovanie suboru alebo priecinka v prieskumniku.
    /// </summary>
    public class RenameFileAction : Action
    {
        public RenameFileAction(FMain form, string oldPath, string newPath) : base(form)
        {
            OldPath = oldPath;
            NewPath = newPath;
        }

        private string OldPath { get; }
        private string NewPath { get; }

        /// <inheritdoc />
        public override string CommandName => Resources.Action_RenameFile;

        /// <inheritdoc />
        public override void Undo() => Form.RenameOnDisk(NewPath, OldPath);

        /// <inheritdoc />
        public override void Redo() => Form.RenameOnDisk(OldPath, NewPath);
    }

    public class AddGroupAction : Action
    {
        /// <param name="form">Hlavne okno.</param>
        /// <param name="grp">Pridana skupina.</param>
        /// <param name="index">Pozicia skupiny v zozname skupin jazyka.</param>
        /// <param name="createdDirectory">Priecinok skupiny, ktory sa pri pridani vytvoril; <see langword="null" />, ak uz existoval.</param>
        public AddGroupAction(FMain form, FyzGroup grp, int index, string? createdDirectory) : base(form)
        {
            Group = grp;
            Index = index;
            CreatedDirectory = createdDirectory;
        }

        private FyzGroup Group { get; }
        private int Index { get; }
        private string? CreatedDirectory { get; }

        /// <inheritdoc />
        public override string CommandName => "Pridanie skupiny zvukov";

        /// <inheritdoc />
        public override void Undo()
        {
            Form.SelectLanguage(Group.Language);
            Form.TakeOutGroup(Group);

            // vytvoreny priecinok sa zmaze, len ak v nom nic nie je
            if (CreatedDirectory == null || !Directory.Exists(CreatedDirectory) || Directory.EnumerateFileSystemEntries(CreatedDirectory).Any())
                return;

            Form.WithoutFileWatcher(() => Directory.Delete(CreatedDirectory));
            Group.Directory?.Parent?.Children.Remove(Group.Directory);
            Group.Directory = null!;
            Form.FillExplorerList(Form.CurrentDirectory);
        }

        /// <inheritdoc />
        public override void Redo()
        {
            Form.SelectLanguage(Group.Language);
            if (CreatedDirectory != null)
                Form.WithoutFileWatcher(() => Directory.CreateDirectory(CreatedDirectory));
            Form.InsertGroup(Group, Index);
        }
    }

    public class EditGroupAction : Action
    {
        /// <inheritdoc />
        public EditGroupAction(FMain form, FyzGroup group,
            (string oldkey, string newKey) keys,
            (string oldName, string newName) names,
            (string oldRPath, string newRPath) relativePaths) : base(form)
        {
            Group = group;
            Keys = keys;
            Names = names;
            RelativePaths = relativePaths;
        }

        private FyzGroup Group { get; }

        private (string oldKey, string newKey) Keys { get; }

        private (string oldName, string newName) Names { get; }

        private (string oldRPath, string newRPath) RelativePaths { get; }

        /// <inheritdoc />
        public override string CommandName => Resources.Action_EditGroup;

        /// <inheritdoc />
        public override void Undo()
        {
            Form.SelectLanguage(Group.Language);
            Form.ChangeGroup(Group, Keys.oldKey, Names.oldName, RelativePaths.oldRPath);
        }

        /// <inheritdoc />
        public override void Redo()
        {
            Form.SelectLanguage(Group.Language);
            Form.ChangeGroup(Group, Keys.newKey, Names.newName, RelativePaths.newRPath);
        }
    }

    public class RemovedGroupsAction : Action
    {
        /// <param name="form">Hlavne okno.</param>
        /// <param name="grp">Odstranovana skupina (este v zozname skupin - akcia si pamata jej poziciu).</param>
        /// <param name="withDirectory">Ci sa priecinok skupiny presuva do kosa.</param>
        public RemovedGroupsAction(FMain form, FyzGroup grp, bool withDirectory) : base(form)
        {
            Group = grp;
            Index = grp.Language.Groups.IndexOf(grp);
            WithDirectory = withDirectory;
            GroupDirectory = GroupDirectoryPath(grp);
        }

        private FyzGroup Group { get; }
        private int Index { get; }
        private bool WithDirectory { get; }
        private string GroupDirectory { get; }

        /// <inheritdoc />
        public override string CommandName => Resources.Action_DeleteGroup;

        /// <summary>
        /// Odstrani skupinu zo zoznamu, pripadne jej priecinok presunie do kosa.
        /// </summary>
        /// <returns><see langword="false" />, ak sa priecinok nepodarilo odstranit - skupina ostala.</returns>
        public bool Apply()
        {
            if (WithDirectory)
            {
                try
                {
                    Form.WithoutFileWatcher(() => Utils.DeleteDirectoryToRecycleBin(GroupDirectory));
                }
                catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or OperationCanceledException)
                {
                    if (ex is not OperationCanceledException)
                        Utils.ShowError(string.Format(CultureInfo.CurrentCulture, Resources.Action_GroupToRecycleFailed, GroupDirectory, ex.Message));
                    return false;
                }

                // prvok priecinka sa zo stromu vyberie, ale ostane v skupine - Spat ho vrati aj s prepojeniami na zvuky
                Group.Directory?.Parent?.Children.Remove(Group.Directory);
            }

            Form.TakeOutGroup(Group);
            return true;
        }

        /// <inheritdoc />
        public override void Undo()
        {
            Form.SelectLanguage(Group.Language);

            if (WithDirectory && !Directory.Exists(GroupDirectory))
            {
                var restored = false;
                Form.WithoutFileWatcher(() => restored = Utils.TryRecoverFileOrDirFromBin(GroupDirectory));
                if (!restored)
                    Utils.ShowError(Resources.Action_GroupRestoreFailed);
                else if (Group.Directory is { } directory && Group.Language.Directory is { } languageDir && !languageDir.Children.Contains(directory))
                    languageDir.Children.Add(directory);
            }

            Form.InsertGroup(Group, Index);
        }

        /// <inheritdoc />
        public override void Redo()
        {
            Form.SelectLanguage(Group.Language);
            Apply();
        }
    }

    public class AddLanguageAction : Action
    {
        /// <param name="form">Hlavne okno.</param>
        /// <param name="lang">Pridany jazyk.</param>
        /// <param name="index">Pozicia jazyka v zozname jazykov banky.</param>
        /// <param name="previous">Jazyk otvoreny pred pridanim - po vrateni sa otvori znova.</param>
        /// <param name="createdDirectory">Priecinok jazyka, ktory sa pri pridani vytvoril; <see langword="null" />, ak uz existoval.</param>
        public AddLanguageAction(FMain form, FyzLanguage lang, int index, FyzLanguage? previous, string? createdDirectory) : base(form)
        {
            Language = lang;
            Index = index;
            Previous = previous;
            CreatedDirectory = createdDirectory;
        }

        /// <inheritdoc />
        public override string CommandName => "Pridanie jazyka";

        private FyzLanguage Language { get; }
        private int Index { get; }
        private FyzLanguage? Previous { get; }
        private string? CreatedDirectory { get; }

        /// <inheritdoc />
        public override void Undo()
        {
            Form.RemoveLanguage(Language, Previous);

            // vytvoreny priecinok sa zmaze, len ak v nom nic nie je
            if (CreatedDirectory != null && Directory.Exists(CreatedDirectory) && !Directory.EnumerateFileSystemEntries(CreatedDirectory).Any())
                Directory.Delete(CreatedDirectory);
        }

        /// <inheritdoc />
        public override void Redo()
        {
            if (CreatedDirectory != null)
                Directory.CreateDirectory(CreatedDirectory);

            Form.InsertLanguage(Language, Index);
            Form.SelectLanguage(Language);
        }
    }

    public class EditLanguageAction : Action
    {
        /// <inheritdoc />
        public EditLanguageAction(FMain form, FyzLanguage lang,
            (string oldkey, string newKey) keys, 
            (string oldName, string newName) names, 
            (string oldRPath, string newRPath) relativePaths) : base(form)
        {
            Language = lang;
            Keys = keys;
            Names = names;
            RelativePaths = relativePaths;
        }

        /// <inheritdoc />
        public override string CommandName => Resources.Action_EditLanguage;

        private FyzLanguage Language { get; }

        private (string oldKey, string newKey) Keys { get; }

        private (string oldName, string newName) Names { get; }

        private (string oldRPath, string newRPath) RelativePaths { get; }

        /// <inheritdoc />
        public override void Undo()
        {
            Form.ChangeLanguage(Language, Keys.oldKey, Names.oldName, RelativePaths.oldRPath);
        }

        /// <inheritdoc />
        public override void Redo()
        {
            Form.ChangeLanguage(Language, Keys.newKey, Names.newName, RelativePaths.newRPath);
        }
    }

    public class RemoveLanguageAction : Action
    {
        /// <param name="form">Hlavne okno.</param>
        /// <param name="lang">Odstraneny jazyk.</param>
        /// <param name="index">Pozicia jazyka v zozname jazykov banky pred odstranenim.</param>
        /// <param name="directory">Priecinok jazyka.</param>
        /// <param name="removedWithData">Ci sa priecinok jazyka presunul do kosa.</param>
        public RemoveLanguageAction(FMain form, FyzLanguage lang, int index, string directory, bool removedWithData) : base(form)
        {
            Language = lang;
            Index = index;
            LanguageDirectory = directory;
            RemovedWithData = removedWithData;
        }

        /// <inheritdoc />
        public override string CommandName => Resources.Action_DeleteLanguage;

        private FyzLanguage Language { get; }
        private int Index { get; }
        private string LanguageDirectory { get; }
        public bool RemovedWithData { get; }

        /// <inheritdoc />
        public override void Undo()
        {
            // priecinok sa obnovi pred vlozenim - v prazdnej banke sa jazyk hned nacita
            if (RemovedWithData && !Utils.TryRecoverFileOrDirFromBin(LanguageDirectory))
                Utils.ShowError(Resources.Action_LanguageRestoreFailed);
            Form.InsertLanguage(Language, Index);
        }

        /// <inheritdoc />
        public override void Redo()
        {
            if (RemovedWithData)
                Form.DeleteLanguageDirectory(LanguageDirectory);
            Form.RemoveLanguage(Language);
        }
    }

    /// <summary>
    /// Konverzia nahravok na .EWA alebo .WAV - pamata si len subory, ktore sa naozaj skonvertovali.
    /// </summary>
    public class ConvertFilesAction : Action
    {
        public ConvertFilesAction(FMain form, IReadOnlyList<SoundFileElement> files, bool toEwa, string commandName) : base(form)
        {
            Files = files;
            ToEwa = toEwa;
            Name = commandName;
        }

        private IReadOnlyList<SoundFileElement> Files { get; }
        private bool ToEwa { get; }
        private string Name { get; }

        /// <inheritdoc />
        public override string CommandName => $"{Name} na {(ToEwa ? SoundUtils.EWA_EXT : SoundUtils.WAV_EXT)}";

        /// <inheritdoc />
        public override void Undo() => _ = Form.ConvertInBackground(Files, !ToEwa, "Vraciam konverziu");

        /// <inheritdoc />
        public override void Redo() => _ = Form.ConvertInBackground(Files, ToEwa, "Konvertujem");
    }
}