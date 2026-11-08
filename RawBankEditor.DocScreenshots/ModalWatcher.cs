namespace RawBankEditor.DocScreenshots;

/// <summary>
/// Obslúži modálne okná, ktoré program otvára cez ShowDialog (výber jazyka, otázky) – bežia vo vlastnej
/// slučke správ, preto ich zachytí časovač. Obsluha vráti <c>true</c>, ak okno spracovala (zavrela).
/// </summary>
internal sealed class ModalWatcher : IDisposable
{
    private readonly System.Windows.Forms.Timer _timer = new() { Interval = 100 };
    private readonly HashSet<Form> _handled = [];

    public ModalWatcher(Func<Form, bool> handler)
    {
        _timer.Tick += (_, _) =>
        {
            var modal = Application.OpenForms.Cast<Form>().FirstOrDefault(f => f.Visible && f.Modal && !_handled.Contains(f));
            if (modal is null)
                return;

            _timer.Stop();
            _handled.Add(modal);
            Pump.Events();
            if (!handler(modal))
                _handled.Remove(modal);
            _timer.Start();
        };
        _timer.Start();
    }

    public void Dispose()
    {
        _timer.Stop();
        _timer.Dispose();
    }
}
