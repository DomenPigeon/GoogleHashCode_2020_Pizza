using System.Collections;
using System.Windows.Controls;

namespace Drawing {

    public class Program {

        private readonly Canvas _canvas;

        public Program(Canvas canvas) {
            this._canvas = canvas;
        }

        public void Run() {
//            _canvas.DefineTick(Tick());
        }

        private IEnumerable Tick() {
            for (var i = 0; i < 15; i++) {
                OneTick();
                yield return null;
            }
        }

        private void OneTick() {

        }

    }

}
