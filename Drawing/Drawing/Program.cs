using System;
using System.Collections;
using System.Windows.Controls;
using static System.Console;

namespace Drawing {
    public class Program {
        private readonly Canvas _canvas;
        public Program(Canvas canvas) {
            this._canvas = canvas;
        }

        public void Run() {
            _canvas.DefineTick(Tick());
            var a = new Node<string>("a");
            var b = new Node<string>("b");
            var c = new Node<string>("c");
            var d = new Node<string>("d");
            var e = new Node<string>("e");
            var f = new Node<string>("f");
            var g = new Node<string>("g");
            var h = new Node<string>("h");
            var i = new Node<string>("i");

            a.SetParent(b);
            b.SetParent(f);
            c.SetParent(d);
            d.SetParent(b);
            e.SetParent(d);
            f.SetParent(null);
            g.SetParent(f);
            h.SetParent(i);
            i.SetParent(g);
            
            
            foreach (var node in f.DFS) {
                WriteLine(node);
            }
        }
        
        private IEnumerable Tick() {
            for (var i = 0; i < 15; i++) {
                RandomRect();
                yield return null;
            }
        }
        
        private void RandomRect() {
            var r = new Random();
            var x = r.Next(0, (int) _canvas.ActualWidth / 5);
            var y = r.Next(0, (int) _canvas.ActualWidth / 5);
            var width = r.Next(0, (int) _canvas.ActualWidth / 5);
            var height = r.Next(0, (int) _canvas.ActualWidth / 5);

            _canvas.FillStyle($"255, {r.Next(0,256)}, {r.Next(0,256)}, {r.Next(0,256)}"); // ARGB value
            _canvas.StrokeStyle($"255, {r.Next(0,256)}, {r.Next(0,256)}, {r.Next(0,256)}");
            _canvas.FillRect(x, y, width, height);
        }
        private void RandomLine() {
            int R() => new Random().Next(0, 256);
            _canvas.StrokeStyle($"255, {R()}, {R()}, {R()}");
            _canvas.StrokeLine(R(), R(), R(), R());
        }
    }
}