namespace RawBankEditor.Controls;

public partial class MinButton : UserControl
{
    private bool _hover;
    private readonly ToolTip _toolTip;
    private Color hoverColor;
    private Color iconColor;
    private Color iconHoverColor;

    private string toolTipText = "";

    public MinButton()
    {
        InitializeComponent();
        IconColor = Color.Black;
        HoverColor = Color.Gray;
        IconHoverColor = Color.White;

        _toolTip = new ToolTip();
    }

    public string ToolTipText
    {
        get => toolTipText;
        set
        {
            toolTipText = value;
            _toolTip.SetToolTip(this, toolTipText);
        }
    }

    public Color IconColor
    {
        get => iconColor;
        set
        {
            iconColor = value;
            Invalidate();
        }
    }

    public Color IconHoverColor
    {
        get => iconHoverColor;
        set
        {
            iconHoverColor = value;
            Invalidate();
        }
    }

    public Color HoverColor
    {
        get => hoverColor;
        set
        {
            hoverColor = value;
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