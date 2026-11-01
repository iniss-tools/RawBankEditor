# RawBankEditor – pokyny pre agentov

Editor zvukovej banky INISS (`RAWBANK`, súbory `FYZBANK.DAT` a `FYZZVUK.DAT` jazykov, nahrávky EWA/WAV) s generovaním nahrávok hlasom AI. Repozitár je súčasťou pracovného priečinka `INISSTools` (nie je git repozitár, každý podpriečinok má vlastné repo); vedľa musia byť naklonované `ExControls` a `ToolsCore`. Komunikácia, texty UI aj komentáre v kóde (bez diakritiky) sú slovenské.

Pravidlá desktopových programov (WinForms, texty v `Resources`, ručné `*.Designer.cs`, príkazy `CommandSet`, téma, dialógy, ikony SVG) a logovania (Serilog) sú v `../ToolsCore/AGENTS.md` – platia aj tu.

## Štruktúra a zásady

- `Program` je composition root – `BankEditor` (`Services/`, bez UI: `Session`, otvorená banka `Project`, nové jazyky, `Save`/`Close`/`DiscardLanguage`) a `DialogService` idú do `FMain`; ostatné okná a správy zoznamu chýb (`IRawBankMessage.Resolve/Show(FMain)`) dostanú hlavné okno alebo cestu k banke explicitne.
- Každá zmena súborov banky ide cez `BankEditor.Journal` (`Tools/BankJournal`, v `FMain.WithoutFileWatcher`): operácie sa vykonajú hneď, odstránené položky čakajú v skrytom `<INISS>\.rbe-zurnal-<pid>` a do Koša idú pri uložení (`Commit`), zahodenie zmien ich vráti (`Rollback`, pri odchode z jazyka len jeho rozsah). Nikdy `File.Move/Delete`, `Directory.*` ani `Utils.Delete*ToRecycleBin` priamo nad bankou.
- Ukladanie banky je transakčné (`ToolsCore.Iniss/Tools/FileTransaction`) – nové súbory banky patria do zoznamu v `BankEditor.Save`. Kódovanie EWA↔WAV len cez `EwaCodec` (ToolsCore.Iniss).
- `Speech/` – syntéza a prepis reči (ElevenLabs, Azure, Piper): HTTP cez `SpeechHttp` (opakovanie pri 429/5xx, chyby ako `SpeechException`, log bez kľúčov, tiel a dotazu URL), Piper ako proces (`PiperClient`, hlasy `PiperVoiceCatalog`), vyrovnávacia pamäť `SpeechCache`, údaje jazyka v `<INISS>\.rbe-ai` (`BankAiFolder`: hlas, intonácia, výslovnosť, pôvod nahrávok). Kľúče služieb sú v nastaveniach šifrované DPAPI (`UserSecret`) a nikdy sa nelogujú.
- Príkazy hlavného okna cez katalóg `RbeCommands` (ID = názov prvku skratky v config.xml, nemeniť).

## Príkazy

```bash
dotnet build RawBankEditor.slnx -nologo -v q
dotnet test RawBankEditor.slnx
"/c/Program Files/Inno Setup 7/ISCC.exe" RawBankEditorInstaller.iss   # instalator do Output/; najprv Release build
```

Vydanie: značka `vX.Y` spustí `.github/workflows/release.yml` (spoločný postup v `ToolsCore/Installer/README.md`).

## Spoločné pravidlá

- `Directory.Build.props`: `TreatWarningsAsErrors`, `Nullable`, `ImplicitUsings`, `AnalysisLevel latest-recommended` – build bez jediného varovania; zámerne vypnuté pravidlá len tam (`NoWarn` s dôvodom), varovanie opraviť v kóde, výnimočné potlačenie priamo na nájdenom člene (`[SuppressMessage(..., Justification = ...)]`). Kopírovací konštruktor recordu s primárnym konštruktorom nespúšťa inicializátory polí – priradiť všetky členy.
- Kultúra: UI `CultureInfo.CurrentCulture`, formáty a logy `InvariantCulture`; súbory INISS sú CP1250.
- GUI nespúšťať cez Bash – overuje ho používateľ ručne; bez klikania harness (reflexia + `DrawToBitmap`).
- Testy MSTest, metódy `Oblast_CoOveruje`; do repa len testy s vlastnými dátami (korpusové overenie nad `D:\INISSroot` len na stroji používateľa).
- Skripty: generátory ukladať ako súbor (bash heredoc mení `\\` a `\n`); v `.ps1` žiadna diakritika.

## Commity (Conventional Commits)

Commitovať len na požiadanie. Správy podľa [Conventional Commits](https://www.conventionalcommits.org/): `<typ>(<oblasť>): <popis>`.

- Typ: `feat`, `fix`, `refactor`, `perf`, `docs`, `test`, `build` (projekty, balíky, inštalátor), `ci` (workflow), `style`, `chore`, `revert`.
- Oblasť nepovinná, malými písmenami (napr. `journal`, `speech`, `piper`, `import`, `convert`).
- Popis anglicky v rozkazovacom spôsobe, malým písmenom, bez bodky, najviac 72 znakov; názvy tried v backtickoch. Telo po prázdnom riadku vysvetľuje prečo; nekompatibilná zmena (formát banky, `.rbe-ai`, config.xml) má `!` a pätičku `BREAKING CHANGE: …`.
- Príklad: ``fix(journal): restore deleted group when `Rollback` runs twice``.
