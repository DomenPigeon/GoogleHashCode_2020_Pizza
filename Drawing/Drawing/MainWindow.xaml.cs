using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Drawing {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            new Program(Canvas).Run();
            if (!Canvas.HasTick())
                Close();
            else {
                Window.Visibility = Visibility.Visible;
            }
        }

        private void OneTickBtn_Click(object sender, RoutedEventArgs e) => Canvas.Tick();
        private void AllTickBtn_Click(object sender, RoutedEventArgs e) => Canvas.AllTicks();
        private async void AnimateBtn_Click(object sender, RoutedEventArgs e) => await Canvas.Animate();
        private void ResetBtn_Click(object sender, RoutedEventArgs e) => Canvas.Reset();
    }

    public class Rectangle {
        // Properties
        public SolidColorBrush Stroke { get; set; } = Brushes.Black;
        public SolidColorBrush Fill { get; set; } = Brushes.Black;
        public double X { get; set; } = 0;
        public double Y { get; set; } = 0;
        public double Width { get; set; } = 10;
        public double Height { get; set; } = 10;

        // Fields
        private readonly Line[] _lines = new Line[4];
        private readonly Canvas _canvas;

        public Rectangle(Canvas canvas) {
            _canvas = canvas;
        }

        private void CreateFromRectangle() {
            var rec = new System.Windows.Shapes.Rectangle {
                Width = Width,
                Height = Height,
                Fill = Fill,
                Stroke = Stroke,
                StrokeThickness = 2,
            };

            _canvas.Children.Add(rec);
            Canvas.SetTop(rec, X);
            Canvas.SetLeft(rec, Y);
        }

        public void Draw() {
            CreateFromRectangle();
        }
    }

    public static class CanvasExtensionMethods {
        private static IEnumerator _enumerator;
        private static IEnumerable _enumerable;
        private static Stopwatch _stopwatch = new Stopwatch();
        private static SolidColorBrush _fillStyle = Brushes.Black;
        private static SolidColorBrush _strokeStyle = Brushes.Black;
        private static int _tickDuration = 500;
        private static bool _animating = false;

        public static bool HasTick(this Canvas c) {
            return _enumerator != null;
        }

        public static void DefineTick(this Canvas c, IEnumerable enumerable) {
            _enumerable = enumerable;
            _enumerator = enumerable.GetEnumerator();
        }

        public static void Tick(this Canvas c) {
            _enumerator.MoveNext();
        }

        public static void AllTicks(this Canvas c) {
            while (_enumerator.MoveNext()) {
            }
        }

        public static async Task Animate(this Canvas c) {
            if (_animating) return;
            _animating = true;
            
            while (_animating && _enumerator.MoveNext()) {
                await Task.Delay(_tickDuration);
            }
        }

        public static void Reset(this Canvas c) {
            _animating = false;
            _enumerator = _enumerable.GetEnumerator();
            c.Children.Clear();
        }

        public static void FillStyle(this Canvas c, string hex_argb) {
            _fillStyle = StringToColor(hex_argb);
        }

        public static void StrokeStyle(this Canvas c, string hex_argb) {
            _strokeStyle = StringToColor(hex_argb);
        }

        public static void FillRect(this Canvas c, int x, int y, int width, int height) {
            var r = new Rectangle(c) {
                X = x, Y = y,
                Width = width,
                Height = height,
                Stroke = _strokeStyle,
                Fill = _fillStyle
            };
            r.Draw();
        }

        public static void StrokeLine(this Canvas c, int x1, int y1, int x2, int y2, double thickness = 5) {
            c.Children.Add(new Line
                {X1 = x1, Y1 = y1, X2 = x2, Y2 = y2, Stroke = _strokeStyle, StrokeThickness = thickness});
        }
        
        public static void TickDuration(this Canvas c, int miliseconds) {
            _tickDuration = miliseconds;
        }

        #region Private Helper Methods

        private static SolidColorBrush StringToColor(string hexOrArgb) {
            return hexOrArgb[0] == '#' ? HexStringToColor(hexOrArgb) : ArgbStringToColor(hexOrArgb);
        }

        private static SolidColorBrush HexStringToColor(string hex) {
            return new BrushConverter().ConvertFrom(hex) as SolidColorBrush;
        }

        private static SolidColorBrush ArgbStringToColor(string argb) {
            var values = argb
                .Replace(')', ' ')
                .Replace('(', ' ')
                .Trim().Split(',').ToList();

            var color = new SolidColorBrush(Color.FromArgb(
                byte.Parse(values[0]), byte.Parse(values[1]),
                byte.Parse(values[2]), byte.Parse(values[3])));
            return color;
        }

        #endregion
    }
}