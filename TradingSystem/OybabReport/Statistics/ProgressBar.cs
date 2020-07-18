using System.Drawing;
using System.ComponentModel;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports;
using DevExpress.XtraReports.UI;
// ...

namespace Oybab.Report.Statistics
{
    // The DefaultBindableProperty attribute is intended to make the Position 
    // property bindable when an item is dropped from the Field List.
    [
    ToolboxItem(true),
    DefaultBindableProperty("Position")
    ]
    internal sealed class ProgressBar : XRControl {

        // The current position value.
        private double pos = 0;

        // The maximum value for the progress bar position.
        private double maxVal = 100;

        public ProgressBar() {
            this.ForeColor = Color.DarkRed;//SystemColors.Highlight;
        }

        // Define the MaxValue property.
        [DefaultValue(100)]
        public double MaxValue
        {
            get { return this.maxVal; }
            set {
                if (value <= 0) return;
                this.maxVal = value;
            }
        }

        // Define the Position property. 
        [DefaultValue(0), Bindable(true)]
        public double Position
        {
            get { return this.pos; }
            set {
                if(value < 0 || value > maxVal)
                    return;
                this.pos = value;
            }
        }

        /// <summary>
        /// DoubleתFloat
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        internal float ConvetToFloat(double source)
        {
            float result = (float)source;
            if (float.IsPositiveInfinity(result))
            {
                result = float.MaxValue;
            }
            else if (float.IsNegativeInfinity(result))
            {
                result = float.MinValue;
            }
            return result;
        }

        // Override the XRControl.CreateBrick method.
        protected override VisualBrick CreateBrick(VisualBrick[] childrenBricks) {
            // Use this code to make the progress bar control 
            // always represented as a Panel brick.
            return new PanelBrick(this);
        }

        // Override the XRControl.PutStateToBrick method.
        protected override void PutStateToBrick(VisualBrick brick, PrintingSystemBase ps) {
            // Call the PutStateToBrick method of the base class.
            base.PutStateToBrick(brick, ps);

            // Get the Panel brick which represents the current progress bar control.
            PanelBrick panel = (PanelBrick)brick;

            // Create a new VisualBrick to be inserted into the panel brick.
            VisualBrick progressBar = new VisualBrick(this);

            // Hide borders.
            progressBar.Sides = BorderSide.None;

            // Set the foreground color to fill the completed area of a progress bar.
            progressBar.BackColor = panel.Style.ForeColor;

            // Calculate the rectangle to be filled by the foreground color.
            progressBar.Rect = new RectangleF(0, 0, (panel.Rect.Width * ConvetToFloat(Position / MaxValue)),
                panel.Rect.Height);

            // Add the VisualBrick to the panel.
            panel.Bricks.Add(progressBar);

            BrickGraphics gr = ps.Graph;

            TextBrick myText = new TextBrick();
            myText.Text = "";
            myText.Size = new SizeF(panel.Rect.Width, panel.Rect.Height);
            myText.Location = new PointF(0, 0);
            myText.HorzAlignment = DevExpress.Utils.HorzAlignment.Center;
            myText.VertAlignment = DevExpress.Utils.VertAlignment.Center;

            myText.Sides = BorderSide.None;
            myText.BackColor = Color.Transparent;

            panel.Bricks.Add(myText);
        }
    }
}