using System;
using System.Collections;
using System.IO;
using System.Windows.Controls;

namespace Drawing {

    public class Program {

        private readonly Canvas _canvas;
        private readonly string[] _input;

        private int _numberOfBooks;
        private int _numberOfLibraries;
        private int _daysForScanning;

        private int[] _books;

        public Program(Canvas canvas) {
            this._canvas = canvas;
            _input = File.ReadAllLines("input.in");
            ParseInput();
        }

        private void ParseInput() {
            
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

    public struct Library {

        public int ID;
        public int SignUp;
        public int BooksPerDay;
        public int[] Books;

        public Library(int id, int signUp, int booksPerDay, int[] books) {
            ID = id;
            SignUp = signUp;
            BooksPerDay = booksPerDay;
            Books = books;
        }

    }

}
