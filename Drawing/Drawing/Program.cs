using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Windows.Controls;

namespace Drawing {

    public class Program {

        private readonly Canvas _canvas;
        private readonly string[] _input;

        private int _numberOfBooks;
        private int _numberOfLibraries;
        private int _daysForScanning;

        private int[] _books;
        private Library[] Libraries;

        public Program(Canvas canvas) {
            this._canvas = canvas;
            _input = File.ReadAllLines("a");
            ParseInput();
        }

        private void ParseInput() {
            var firstLine = _input[0].Split();
            var secondLine = _input[1].Split();

            _numberOfBooks = int.Parse(firstLine[0]);
            _numberOfLibraries = int.Parse(firstLine[1]);
            _daysForScanning = int.Parse(firstLine[2]);

            _books = secondLine.Select(int.Parse).ToArray();

            ParseLibraries();
        }

        private void ParseLibraries() {
            Libraries = new Library[(_input.Length - 2) / 2];
            for (var i = 2; i < _input.Length; i += 2) {
                var libraryID = i / 2 - 1;
                var libraryData = _input[i].Split().Select(int.Parse).ToArray();
                var libraryBooks = _input[i + 1].Split().Select(int.Parse).ToArray();
                Libraries[libraryID] = new Library(libraryID, libraryData[0], libraryData[1], libraryData[2], libraryBooks);
            }
        }

        public void Run() {
//            _canvas.DefineTick(Tick());
            Console.WriteLine();
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
        public int NumberOfBooks;
        public int SignUpTime;
        public int BooksPerDay;
        public int[] Books;

        public int Value;

        public Library(int id, int numberOfBooks, int signUpTime, int booksPerDay, int[] books) {
            ID = id;
            NumberOfBooks = numberOfBooks;
            SignUpTime = signUpTime;
            BooksPerDay = booksPerDay;
            Books = books.OrderBy(i => i).ToArray();
            Value = 0;
            Value = CalculateImportance();
        }

        private int CalculateImportance() {
            return Books.Sum() - Books.Take(SignUpTime).Sum();
        }

    }

}
