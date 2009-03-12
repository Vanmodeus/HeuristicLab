using System.Drawing;
using HeuristicLab.Visualization.LabelProvider;

namespace HeuristicLab.Visualization {
  public class XAxis : WorldShape {
    public const int PixelsPerInterval = 100;
    
    private ILabelProvider labelProvider = new ContinuousLabelProvider("0.####");

    private Color color = Color.Blue;
    private Font font = new Font("Arial", 8);

    public ILabelProvider LabelProvider {
      get { return labelProvider; }
      set { labelProvider = value; }
    }

    public override void Draw(Graphics graphics) {
      ClearShapes();

      foreach (double x in AxisTicks.GetTicks(PixelsPerInterval, Parent.Viewport.Width,
                                              ClippingArea.Width,
                                              ClippingArea.X1)) {
        TextShape label = new TextShape(x, ClippingArea.Height - 3,
                                        labelProvider.GetLabel(x), Font, Color);
        label.AnchorPositionX = AnchorPositionX.Middle;
        label.AnchorPositionY = AnchorPositionY.Top;
        AddShape(label);
      }

      base.Draw(graphics);
    }

    public Color Color {
      get { return color; }
      set { color = value; }
    }

    public Font Font {
      get { return font; }
      set { font = value; }
    }
  }
}