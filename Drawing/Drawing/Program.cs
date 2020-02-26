using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Controls;

namespace Drawing {
    public class Program {
        private          bool     isglobal;
        private readonly Canvas   _canvas;
        private readonly string[] _input;
        private const    string   InputName = "e";

        private int _numberOfBooks;
        private int _numberOfLibraries;
        private int _daysForScanning;

        private List<Book>    _books                = new List<Book>();
        private List<Library> Libraries             = new List<Library>();
        private List<Library> _resultingLibraryList = new List<Library>();

        public Program(Canvas canvas) {
            _canvas = canvas;
            _input  = File.ReadAllLines(InputName);
            ParseInput();
        }

        private void ParseInput() {
            var firstLine  = _input[0].Split();
            var secondLine = _input[1].Split();

            _numberOfBooks     = int.Parse(firstLine[0]);
            _numberOfLibraries = int.Parse(firstLine[1]);
            _daysForScanning   = int.Parse(firstLine[2]);

            var books = secondLine.Select(int.Parse).ToArray();
            for (var i = 0; i < books.Length; i++) {
                _books.Add(new Book(i, books[i], ref _books));
            }

            ParseLibraries();
        }

        private void ParseLibraries() {
            Libraries = new List<Library>((_input.Length - 2) / 2);
            for (var i = 2; i < _input.Length; i += 2) {
                var libraryID = i / 2 - 1;
                if (string.IsNullOrEmpty(_input[i])) break;
                var libraryData = _input[i].Split().Select(int.Parse).ToArray();
                var booksData   = _input[i + 1].Split().Select(int.Parse).ToArray();

                var books = new Book[booksData.Length];
                for (var j = 0; j < booksData.Length; j++) {
                    books[j] = _books[booksData[j]];
                }

                var lib = new Library(libraryID, libraryData[0], libraryData[1], libraryData[2], books, ref Libraries);
                lib.CalculateImportance(_daysForScanning);
                Libraries.Add(lib);
            }
        }

        public void Run() {
            // _canvas.DefineTick(CanvasTick());

            var     signedUpLibraries = new List<Library>();
            Library librarySigningUp  = null;
            for (var i = 0; i < _daysForScanning; i++) {
                Tick(_daysForScanning - i);
                if (i % 100 == 0)
                    Console.WriteLine($"{i}/{_daysForScanning}");
            }

            GenerateOutput();

            void Tick(int daysLeft) {
                PassDayForEachSignedUpLibrary();
                FindNewBestLibrary(daysLeft);
                librarySigningUp?.DayPasses();
            }

            void UpdateImportanceForAllLibraries(int daysLeft) {
                foreach (var library in Libraries) {
                    library.CalculateImportance(daysLeft);
                }
            }

            void PassDayForEachSignedUpLibrary() {
                foreach (var library in signedUpLibraries) {
                    library.DayPasses();
                }
            }

            void FindNewBestLibrary(int daysLeft) {
                if (Libraries.Count < 1) return;
                if (librarySigningUp == null || librarySigningUp.SignedUp) {
                    if (librarySigningUp != null && librarySigningUp.SignedUp) {
                        librarySigningUp.DayPasses();
                        signedUpLibraries.Add(librarySigningUp);
                    }

                    UpdateImportanceForAllLibraries(daysLeft);
                    librarySigningUp = Libraries.OrderByDescending(l => l.Importance).First();
                    Libraries.Remove(librarySigningUp);
                    _resultingLibraryList.Add(librarySigningUp);
                }
            }
        }

        private void GenerateOutput() {
            var output            = new List<string>();
            var notEmptyLibraries = new List<Library>();
            foreach (var library in _resultingLibraryList) {
                if (library.ScannedBooks.Count > 0)
                    notEmptyLibraries.Add(library);
            }

            output.Add(notEmptyLibraries.Count.ToString());
            for (int i = 0; i < notEmptyLibraries.Count; i++) {
                var lib = notEmptyLibraries[i];
                output.Add(string.Join(' ', lib.ID, lib.ScannedBooks.Count));
                output.Add(string.Join(' ', lib.ScannedBooks));
            }

            File.WriteAllLines(InputName + ".out", output);
        }

        private IEnumerable CanvasTick() {
            for (var i = 0; i < 15; i++) {
                OneTick();
                yield return null;
            }
        }

        private void OneTick() { }
    }

    public class Book {
        public int        ID;
        public int        Score;
        public bool       Scanned;
        public List<Book> AllBooks;

        public Book(int id, int score, ref List<Book> allBooks) {
            ID       = id;
            Score    = score;
            Scanned  = false;
            AllBooks = allBooks;
        }

        public bool Scan() {
            if (AllBooks[ID].Scanned) return false;

            Scanned = true;
            return true;
        }

        public override string ToString() {
            return ID.ToString();
        }
    }

    public class Library {
        public readonly int           ID;
        public readonly int           NumberOfBooks;
        public readonly int           SignUpTime;
        public readonly int           BooksPerDay;
        public readonly List<Book>    ScannedBooks;
        public readonly List<Book>    ReadOnlyBooks;
        public readonly List<Library> Libraries;
        public          List<Book>    Books;
        public          int           Importance;
        public          bool          SignedUp;
        public          int           TimeLeftToSignUp;

        public Library(int    id, int numberOfBooks, int signUpTime, int booksPerDay, Book[] books,
            ref List<Library> libraries) {
            ID               = id;
            NumberOfBooks    = numberOfBooks;
            SignUpTime       = signUpTime;
            BooksPerDay      = booksPerDay;
            ReadOnlyBooks    = books.ToList();
            Books            = books.OrderByDescending(b => b.Score).ToList();
            SignedUp         = false;
            TimeLeftToSignUp = signUpTime;
            Importance       = 0;
            ScannedBooks     = new List<Book>();
            Libraries        = libraries;
        }

        public bool DayPasses() {
            if (SignedUp) {
                ScanNewBook();
                return true;
            }
            else {
                Registering();
                return false;
            }
        }

        private void Registering() {
            TimeLeftToSignUp--;
            if (TimeLeftToSignUp < 1)
                SignedUp = true;
        }

        private void ScanNewBook() {
            if (Books.Count < 1) return;
            while (!Books[0].Scan()) {
                Books.Remove(Books[0]);
                if (Books.Count < 1) return;
            }

            ScannedBooks.Add(Books[0]);
        }

        public void CalculateImportance(int daysLeft) {
            // var scoreSum = 0;
            // foreach (var book in Books) {
            //     scoreSum += book.Score;
            // }

            var daysForScanning = daysLeft - SignUpTime;
            Importance = Books.Where(b => b.Scanned == false).Take(daysForScanning).Sum(b => b.Score);
        }
    }
}