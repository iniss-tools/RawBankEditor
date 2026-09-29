namespace RawBankEditor.Controls;

public partial class MinButton : UserControl
{
    private bool _hover;
    private readonly ToolTip _toolTip;
    private Color _hoverColor;
    private Color _iconColor;
    private Color _iconHoverColor;

    private string _toolTipText = "";

    public MinButton()
    {
        InitializeComponent();
        IconColor = Color.Black;
        HoverColor = Color.Gray;
        IconHoverColor = Color.White;

        _toolTip = new ToolTip();
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public string ToolTipText
    {
        get => _toolTipText;
        set
        {
            _toolTipText = value;
            _toolTip.SetToolTip(this, _toolTipText);
        }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public Color IconColor
    {
        get => _iconColor;
        set
        {
            _iconColor = value;
            Invalidate();
        }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public Color IconHoverColor
    {
        get => _iconHoverColor;
        set
        {
            _iconHoverColor = value;
            Invalidate();
        }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public Color HoverColor
    {
        get => _hoverColor;
        set
        {
            _hoverColor = value;
            Invalidate();
        }
    }

    private void MinMaxButton_Paint(object sender, PaintEventArgs e)
    {
        e.Graphics.Clear(_hover ? HoverColor : BackColor);

        using var pen = new Pen(_hover ? IconHoverColor : IconColor, 2);

        e.Graphics.DrawLine(pen, 1, Height/2, Width - 1, Height/2);
    }

    private void MinMaxButton_MouseEnter(object sender, EventArgs e)
    {
        if (!_hover)
        {
            _hover = true;
            Invalidate();
        }
    }

    private void MinMaxButton_MouseLeave(object sender, EventArgs e)
    {
        if (_hover)
        {
            _hover = false;
            Invalidate();
        }
    }
}