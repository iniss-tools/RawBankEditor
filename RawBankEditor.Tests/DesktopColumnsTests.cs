using System.Globalization;
using System.Xml;
using System.Xml.Serialization;
using RawBankEditor.XML;
using ToolsCore.XML;

namespace RawBankEditor.Tests;

/// <summary>
/// Stlpce pracovnej plochy - nazvy v jazyku programu, nie v jazyku pri nacitani config.xml.
/// </summary>
[TestClass]
public class DesktopColumnsTests
{
    // AppInit cita config.xml skor, nez nastavi jazyk programu - nazvy sa nesmu zapamatat pri nacitani
    [TestMethod]
    [DataRow("sk", "cs", "Klíč", "Název souboru", "Trvání")]
    [DataRow("cs", "sk", "Kľúč", "Názov súboru", "Trvanie")]
    public void GetValues_NazvyVJazykuProgramuPoNacitaniKonfiguracie(string loadCulture, string appCulture, string key, string fileName, string duration)
    {
        var old = CultureInfo.CurrentUICulture;
        try
        {
            CultureInfo.CurrentUICulture = new CultureInfo(loadCulture);
            var config = Deserialize(Serialize(new RawBankEditorConfig()));

            CultureInfo.CurrentUICulture = new CultureInfo(appCulture);
            foreach (var columns in new[] { config.DesktopCols, config.DesktopCols with { }, new DesktopColumns() })
            {
                var values = columns.GetValues();
                Assert.AreEqual(key, NameOf(values, nameof(DesktopColumns.Key)));
                Assert.AreEqual(fileName, NameOf(values, nameof(DesktopColumns.FileName)));
                Assert.AreEqual(duration, NameOf(values, nameof(DesktopColumns.Duration)));
            }
        }
        finally
        {
            CultureInfo.CurrentUICulture = old;
        }
    }

    private static string NameOf(IList<DesktopColumn> columns, string propertyName) => columns.Single(c => c.PropertyName == propertyName).Name;

    private static string Serialize(RawBankEditorConfig config)
    {
        using var writer = new StringWriter();
        new XmlSerializer(typeof(RawBankEditorConfig)).Serialize(writer, config);
        return writer.ToString();
    }

    private static RawBankEditorConfig Deserialize(string xml)
    {
        using var reader = XmlReader.Create(new StringReader(xml), new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit });
        return (RawBankEditorConfig)new XmlSerializer(typeof(RawBankEditorConfig)).Deserialize(reader)!;
    }
}
