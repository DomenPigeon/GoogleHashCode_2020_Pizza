using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Controls;

namespace Drawing {

    public class Program {

        private readonly Canvas _canvas;
        private readonly string[] _input;
        private const string InputName = "d";

        private int _numberOfBooks;
        private int _numberOfLibraries;
        private int _daysForScanning;

        private int[] _books;
        private List<Library> Libraries = new List<Library>();
        private List<Library> _resultingLibraryList = new List<Library>();

        public Program(Canvas canvas) {
            this._canvas = canvas;
            _input = File.ReadAllLines(InputName);
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
            Libraries = new List<Library>((_input.Length - 2) / 2);
            for (var i = 2; i < _input.Length; i += 2) {
                var libraryID = i / 2 - 1;
                if (string.IsNullOrEmpty(_input[i])) break;
                var libraryData = _input[i].Split().Select(int.Parse).ToArray();
                var libraryBooks = _input[i + 1].Split().Select(int.Parse).ToArray();
                Libraries.Add(new Library(libraryID, libraryData[0], libraryData[1], libraryData[2], libraryBooks));
            }
        }

        public void Run() {
//            _canvas.DefineTick(CanvasTick());
            for (var i = 0; i < _daysForScanning; i++) {
                Tick();
            }
            GenerateOutput(_resultingLibraryList);

            void Tick() {
                var bestLibrary = Libraries.OrderByDescending(l => l.Importance).First();
                var duplicateID = bestLibrary.ID % 2 == 0 ? bestLibrary.ID + 1 : bestLibrary.ID;
                try {
                    Libraries.Remove(Libraries.Single(l => l.ID == duplicateID));
                    Libraries.Remove(bestLibrary);
                } catch (Exception e) {

                }
                _resultingLibraryList.Add(bestLibrary);
                UpdateImportanceForAllLibraries(bestLibrary.Books);
            }
        }

        private void UpdateImportanceForAllLibraries(List<int> books) {
            foreach (var library in Libraries) {
                library.UpdateImportance(books);
            }
        }

        private void GenerateOutput(List<Library> libraries) {
            var output = new List<string>();
            output.Add(libraries.Count.ToString());
            for (int i = 0; i < libraries.Count; i++) {
                var lib = libraries[i];
                output.Add(string.Join(' ', lib.ID, lib.Books.Count));
                output.Add(string.Join(' ', lib.Books));
            }
            File.WriteAllLines(InputName + ".out", output);
        }

        private IEnumerable CanvasTick() {
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
        public List<int> Books;

        public int Importance;

        public Library(int id, int numberOfBooks, int signUpTime, int booksPerDay, int[] books) {
            ID = id;
            NumberOfBooks = numberOfBooks;
            SignUpTime = signUpTime;
            BooksPerDay = booksPerDay;
            Books = books.OrderByDescending(i => i).ToList();
            Importance = 0;
            Importance = CalculateImportance();
        }

        private int CalculateImportance() {
            return Books.Count;
        }

        public void UpdateImportance(List<int> scannedBooks) {
            foreach (var book in scannedBooks) {
                if (Books.Contains(book)) {
                    Books.Remove(book);
                }
            }
            Importance = Books.Count;
        }

    }

}
