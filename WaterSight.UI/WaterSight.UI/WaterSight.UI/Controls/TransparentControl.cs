using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaterSight.UI.Controls
{
    public partial class TransparentControl : Control
    {
        public TransparentControl()
        {
            InitializeComponent();
            SetStyle(ControlStyles.Opaque, true);
            //ForeColor = Color.Transparent;
        }

       

        //private const int WS_EX_TRANSPARENT = 0x20;

       

        protected override CreateParams CreateParams
        {
            get
            {
                var cpar = base.CreateParams;
                cpar.ExStyle |= 0x20;
                return cpar;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // Create a region with a transparent background
            Region transparentRegion = new Region(this.ClientRectangle);
            e.Graphics.FillRegion(Brushes.Transparent, transparentRegion);

        }

        //[DefaultValue(50)]
        //public int Opacity
        //{
        //    get => this.Opacity;
        //    set
        //    {
        //        if (value < 0 || value > 100)
        //            throw new ArgumentOutOfRangeException("Value must be between 0 and 100");
        //        this.Opacity = value;
        //        SetStyle(ControlStyles.Opaque, true);
        //    }
        //}

    }
}
