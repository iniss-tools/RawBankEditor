using ExControls.Providers;
using RawBankEditor.Tools;
using ToolsCore.Entities;
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
    ///     Akcia pri akejkolvek zmene - pridanie, uprava, zmazanie
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
    ///     Akcia pri akejkolvek zmene - pridanie, uprava, zmazanie
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
    ///     Akcia pri zmene oznacenych buniek v tabulke so zvukmi
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
            Form.dgvSounds.ResetBindings();
        }

        public override void Redo()
        {
            Form.SelectGroup(Sound.Group);

            Sound.Group.Sounds.Add(Sound);
            Form.dgvSounds.ResetBindings();
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
                sound.Group.Sounds.Remove(sound);
            Form.dgvSounds.ResetBindings();
        }

        public override void Redo()
        {
            foreach (var sound in Sounds)
                sound.Group.Sounds.Add(sound);
            Form.dgvSounds.ResetBindings();
        }
    }

    /// <summary>
    ///     Akcia pri uprave vlastnosti zvuku
    /// </summary>
    public class EditSoundAction : Action
    {
        public EditSoundAction(FMain form, FyzSound sound) : base(form)
        {
            Sound = sound;
        }

        /// <inheritdoc />
        public override string CommandName => "Zmena dát o zvuku";

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
        public override string CommandName => "Zmena súboru zvuku";
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
            Sounds = sounds;
            OldLocation = oldLocation;
            NewLocation = newLocation;
        }

        /// <inheritdoc />
        public override string CommandName => "Presun zvukov";
        private IEnumerable<FyzSound> Sounds { get; }
        private FyzGroup OldLocation { get; }
        private FyzGroup NewLocation { get; }

        /// <inheritdoc />
        public override void Undo()
        {
            var pathTobank = GlobData.OpenedProject!.AbsPathToBank;
            foreach (var sound in Sounds)
            {
                NewLocation.Sounds.Remove(sound);
                sound.Group = OldLocation;
                OldLocation.Sounds.Add(sound);
                var oldPath = NewLocation.GetAbsPath(pathTobank);
                var newPath = OldLocation.GetAbsPath(pathTobank);

                if (File.Exists(oldPath))
                    File.Move(oldPath, newPath);
            }
            Form.dgvSounds.ResetBindings();
            Form.SelectGroup(OldLocation);
        }

        /// <inheritdoc />
        public override void Redo()
        {
            var pathTobank = GlobData.OpenedProject!.AbsPathToBank;
            foreach (var sound in Sounds)
            {
                OldLocation.Sounds.Remove(sound);
                sound.Group = NewLocation;
                NewLocation.Sounds.Add(sound);
                var oldPath = OldLocation.GetAbsPath(pathTobank);
                var newPath = NewLocation.GetAbsPath(pathTobank);

                if (File.Exists(oldPath))
                    File.Move(oldPath, newPath);
            }
            Form.dgvSounds.ResetBindings();
            Form.SelectGroup(NewLocation);
        }
    }

    /// <summary>
    ///     Akcia pri odstraneni viacerych (alebo 1) zvukov
    /// </summary>
    public class RemovedSoundsAction : Action
    {
        /// <summary>Initializes a new instance of the <see cref="RemovedSoundsAction" /> class.</summary>
        public RemovedSoundsAction(FMain form, IEnumerable<FyzSound> sounds) : base(form)
        {
            Sounds = sounds;
        }

        /// <summary>Initializes a new instance of the <see cref="RemovedSoundsAction" /> class.</summary>
        public RemovedSoundsAction(FMain form, FyzSound sound) : base(form)
        {
            Sounds = new []{ sound };
        }

        /// <inheritdoc />
        public override string CommandName => "Odstránenie zvukov";

        private IEnumerable<FyzSound> Sounds { get;}

        public override void Undo()
        {
            var grp = Sounds.First().Group;
            Form.SelectGroup(grp);

            foreach (var sound in Sounds)
            {
                Form.MenuSounds.Add(sound);
                Form.SelectSound(sound);
            }

            Form.MenuGroups.ResetBindings();
            Form.dgvSounds.ResetBindings();
        }

        public override void Redo()
        {
            var grp = Sounds.First().Group;
            Form.SelectGroup(grp);

            foreach (var sound in Sounds)
                Form.MenuSounds.Remove(sound);

            Form.MenuGroups.ResetBindings();
            Form.dgvSounds.ResetBindings();
        }
    }

    public class AddGroupAction : Action
    {
        /// <inheritdoc />
        public AddGroupAction(FMain form, FyzGroup grp) : base(form)
        {
            Group = grp;
        }

        private FyzGroup Group { get; }

        /// <inheritdoc />
        public override string CommandName => "Pridanie skupiny zvukov";

        /// <inheritdoc />
        public override void Undo()
        {
            Form.SelectLanguage(Group.Language);

            Group.Language.Groups.Remove(Group);
            Form.dgvGroups.ResetBindings();
        }

        /// <inheritdoc />
        public override void Redo()
        {
            Form.SelectLanguage(Group.Language);

            Group.Language.Groups.Add(Group);
            Form.dgvGroups.ResetBindings();
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
        public override string CommandName => "Úprava skupiny zvukov";

        /// <inheritdoc />
        public override void Undo()
        {
            Group.Key = Keys.oldKey;
            Group.Name = Names.oldName;
            Group.RelativePath = RelativePaths.oldRPath;
        }

        /// <inheritdoc />
        public override void Redo()
        {
            Group.Key = Keys.newKey;
            Group.Name = Names.newName;
            Group.RelativePath = RelativePaths.newRPath;
        }
    }

    public class RemovedGroupsAction : Action
    {
        /// <inheritdoc />
        public RemovedGroupsAction(FMain form, IEnumerable<FyzGroup> grps) : base(form)
        {
            Groups = grps;
        }

        /// <inheritdoc />
        public RemovedGroupsAction(FMain form, FyzGroup grp) : base(form)
        {
            Groups = new []{ grp };
        }

        private IEnumerable<FyzGroup> Groups { get; }

        /// <inheritdoc />
        public override string CommandName => "Odstránenie skupín zvukov";

        /// <inheritdoc />
        public override void Undo()
        {
            Form.SelectLanguage(Groups.First().Language);

            foreach (var fyzGroup in Groups)
            {
                fyzGroup.Language.Groups.Add(fyzGroup);
            }

            Form.dgvGroups.ResetBindings();
        }

        /// <inheritdoc />
        public override void Redo()
        {
            Form.SelectLanguage(Groups.First().Language);

            foreach (var fyzGroup in Groups)
            {
                fyzGroup.Language.Groups.Remove(fyzGroup);
            }

            Form.dgvGroups.ResetBindings();
        }
    }

    public class AddLanguageAction : Action
    {
        /// <inheritdoc />
        public AddLanguageAction(FMain form, FyzLanguage lang) : base(form)
        {
            Language = lang;
        }

        /// <inheritdoc />
        public override string CommandName => "Pridanie zvuku";

        private FyzLanguage Language { get; }

        /// <inheritdoc />
        public override void Undo()
        {
            GlobData.OpenedProject!.Languages.Remove(Language);
        }

        /// <inheritdoc />
        public override void Redo()
        {
            GlobData.OpenedProject!.Languages.Add(Language);
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
        public override string CommandName => "Úprava jazyka";

        private FyzLanguage Language { get; }

        private (string oldKey, string newKey) Keys { get; }

        private (string oldName, string newName) Names { get; }

        private (string oldRPath, string newRPath) RelativePaths { get; }

        /// <inheritdoc />
        public override void Undo()
        {
            Language.Key = Keys.oldKey;
            Language.Name = Names.oldName;
            Language.RelativePath = RelativePaths.oldRPath;
        }

        /// <inheritdoc />
        public override void Redo()
        {
            Language.Key = Keys.newKey;
            Language.Name = Names.newName;
            Language.RelativePath = RelativePaths.newRPath;
        }
    }

    public class RemoveLanguageAction : Action
    {
        /// <inheritdoc />
        public RemoveLanguageAction(FMain form, FyzLanguage lang, bool removedWithData) : base(form)
        {
            Language = lang;
            RemovedWithData = removedWithData;
        }

        /// <inheritdoc />
        public override string CommandName => "Odstránenie jazyka";

        private FyzLanguage Language { get; }
        public bool RemovedWithData { get; }

        /// <inheritdoc />
        public override void Undo()
        {
            GlobData.OpenedProject!.Languages.Add(Language);
            if (RemovedWithData && !Utils.TryRecoverFileOrDirFromBin(Language.Directory.DirInfo.FullName))
                Utils.ShowError("Nepodarilo sa obnoviť priečinok so zvukmi jazyka z koša.\n\nPravdepodobne bol permanentne vymazaný.");
        }

        /// <inheritdoc />
        public override void Redo()
        {
            GlobData.OpenedProject!.Languages.Remove(Language);
            if (RemovedWithData) 
                Utils.DeleteDirectoryToRecycleBin(Language.Directory.DirInfo.FullName, true);
        }
    }

    public class ConvertLanguageEwaWawAction : Action
    {
        /// <inheritdoc />
        public ConvertLanguageEwaWawAction(FMain form, FyzLanguage language, bool toEwa) : base(form)
        {
            Language = language;
            ToEwa = toEwa;
        }

        private FyzLanguage Language { get; }
        private bool ToEwa { get; }

        /// <inheritdoc />
        public override string CommandName
        {
            get
            {
                var extention = ToEwa ? SoundUtils.EWA_EXT : SoundUtils.WAV_EXT;
                return $"Konvertovanie zvukov jazyka na {extention}";
            }
        }

        /// <inheritdoc />
        public override void Undo()
        {
            Form.ChangeStatus("Konvertujem zvuky jazyka");
            Form.tspbProgress.Visible = true;
            Form.ConvertSoundsInLanguage(Language, !ToEwa);
        }

        /// <inheritdoc />
        public override void Redo()
        {
            Form.ChangeStatus("Konvertujem zvuky jazyka");
            Form.tspbProgress.Visible = true;
            Form.ConvertSoundsInLanguage(Language, ToEwa);
        }
    }

    public class ConvertGroupEwaWawAction : Action
    {
        /// <inheritdoc />
        public ConvertGroupEwaWawAction(FMain form, FyzGroup group, bool toEwa) : base(form)
        {
            Group = group;
            ToEwa = toEwa;
        }

        private FyzGroup Group { get; }
        private bool ToEwa { get; }

        /// <inheritdoc />
        public override string CommandName
        {
            get
            {
                var extention = ToEwa ? SoundUtils.EWA_EXT : SoundUtils.WAV_EXT;
                return $"Konvertovanie zvukov skupiny na {extention}";
            }
        }

        /// <inheritdoc />
        public override void Undo()
        {
            Form.ChangeStatus("Konvertujem skupinu zvukov");
            Form.tspbProgress.Visible = true;
            Form.ConvertSoundsInGroup(Group, !ToEwa);
        }

        /// <inheritdoc />
        public override void Redo()
        {
            Form.ChangeStatus("Konvertujem skupinu zvukov");
            Form.tspbProgress.Visible = true;
            Form.ConvertSoundsInGroup(Group, ToEwa);
        }
    }

    public class ConvertSoundsEwaWawAction : Action
    {
        /// <inheritdoc />
        public ConvertSoundsEwaWawAction(FMain form, IList<FyzSound> sounds, bool toEwa) : base(form)
        {
            Sounds = sounds;
            ToEwa = toEwa;
        }

        private IList<FyzSound> Sounds { get; }
        private bool ToEwa { get; }

        /// <inheritdoc />
        public override string CommandName
        {
            get
            {
                var extention = ToEwa ? SoundUtils.EWA_EXT : SoundUtils.WAV_EXT;
                return $"Konvertovanie vybraných zvukov na {extention}";
            }
        }

        /// <inheritdoc />
        public override void Undo()
        {
            Form.ChangeStatus("Konvertujem vybrané zvuky");
            Form.tspbProgress.Visible = true;
            Form.ConvertSounds(Sounds, !ToEwa);
        }

        /// <inheritdoc />
        public override void Redo()
        {
            Form.ChangeStatus("Konvertujem vybrané zvuky");
            Form.tspbProgress.Visible = true;
            Form.ConvertSounds(Sounds, ToEwa);
        }
    }

    public class ConvertFilesEwaWawAction : Action
    {
        /// <inheritdoc />
        public ConvertFilesEwaWawAction(FMain form, IList<FileSystemElement> files, bool toEwa) : base(form)
        {
            Files = files;
            ToEwa = toEwa;
        }

        private IList<FileSystemElement> Files { get; }
        private bool ToEwa { get; }

        /// <inheritdoc />
        public override string CommandName
        {
            get
            {
                var extention = ToEwa ? SoundUtils.EWA_EXT : SoundUtils.WAV_EXT;
                return $"Konvertovanie vybraných súborov/priečinkov na {extention}";
            }
        }

        /// <inheritdoc />
        public override void Undo()
        {
            Form.ChangeStatus("Konvertujem vybrané súbory");
            Form.tspbProgress.Visible = true;
            Form.ConvertFiles(Files, !ToEwa);
        }

        /// <inheritdoc />
        public override void Redo()
        {
            Form.ChangeStatus("Konvertujem vybrané súbory");
            Form.tspbProgress.Visible = true;
            Form.ConvertFiles(Files, ToEwa);
        }
    }
}