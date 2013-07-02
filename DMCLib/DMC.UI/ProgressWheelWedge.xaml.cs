using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DMC.UI
{
	/// <summary>
	/// Interaction logic for ProgressWheelWedge.xaml
	/// </summary>
	public partial class ProgressWheelWedge : UserControl
	{
		public int Angle 
		{ 
			get { return (int)base.GetValue(AngleProperty); } 
			set { base.SetValue(AngleProperty, value); } 
		} 
		public static readonly DependencyProperty AngleProperty = 
   			DependencyProperty.Register("Angle", typeof(int), typeof(ProgressWheelWedge), null);

        public TimeSpan BeginTime
        {
            get { return (TimeSpan)base.GetValue(BeginTimeProperty); }
            set { base.SetValue(BeginTimeProperty, value); }
        }
        public static readonly DependencyProperty BeginTimeProperty =
            DependencyProperty.Register("BeginTime", typeof(TimeSpan), typeof(ProgressWheelWedge), null);

        public Brush Color
        {
            get { return (Brush)base.GetValue(ColorProperty); }
            set { base.SetValue(ColorProperty, value); }
        }
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Brush), typeof(ProgressWheelWedge), null);


        public const double SweepAngle = 22.5f * Math.PI / 180f;
        public const float OuterRadius = 100;
        public const float InnerRadius = 60;

        public Point Point1
        {
            get
            {
                return new Point(OuterRadius - OuterRadius * Math.Sin(SweepAngle / 2), OuterRadius - OuterRadius * Math.Cos(SweepAngle / 2));
            }
        }

        public Point Point2
        {
            get
            {
                return new Point(OuterRadius + OuterRadius * Math.Sin(SweepAngle / 2), OuterRadius - OuterRadius * Math.Cos(SweepAngle / 2));
            }
        }

        public Point Point3
        {
            get
            {
                double x = OuterRadius * Math.Tan(SweepAngle / 2);
                double x2 = (OuterRadius - InnerRadius) * Math.Tan(SweepAngle);
                double x3 = x - x2;
                return new Point(OuterRadius + x3, OuterRadius - InnerRadius);
            }
        }

        public Point Point4
        {
            get
            {
                double x = OuterRadius * Math.Tan(SweepAngle / 2);
                double x2 = (OuterRadius - InnerRadius) * Math.Tan(SweepAngle);
                double x3 = x - x2;
                return new Point(OuterRadius - x3, OuterRadius - InnerRadius);
            }
        }

		public ProgressWheelWedge()
        {
			this.InitializeComponent();
		}
	}
}