using System.Collections.Generic;

namespace TaxiGame.Core
{
    /// <summary>Клетка игрового поля.</summary>
    public class Cell
    {
        /// <summary>Название клетки (сразу говорит, что она делает).</summary>
        public string Name { get; set; }

        /// <summary>На сколько клеток сдвинуть фишку: больше 0 — вперёд, меньше 0 — назад.</summary>
        public int Steps { get; set; }

        /// <summary>Пропускает ли игрок следующий ход.</summary>
        public bool SkipTurn { get; set; }

        public Cell() { }

        public Cell(string name, int steps = 0, bool skipTurn = false)
        {
            Name = name;
            Steps = steps;
            SkipTurn = skipTurn;
        }
    }

    /// <summary>Игровое поле: трасса из клеток от старта (0) до финиша (последняя клетка).</summary>
    public class Board
    {
        /// <summary>Клетки по порядку.</summary>
        public List<Cell> Cells { get; set; } = new List<Cell>();

        /// <summary>Номер клетки финиша.</summary>
        public int Finish => Cells.Count - 1;

        public Board() { }

        /// <summary>Создаёт стандартное поле из 30 клеток.</summary>
        public static Board CreateStandard()
        {
            Board board = new Board();
            for (int i = 0; i < 30; i++)
                board.Cells.Add(new Cell("Дорога"));

            board.Cells[0] = new Cell("СТАРТ");
            board.Cells[3] = new Cell("Зелёный свет: вперёд на 2", steps: 2);
            board.Cells[6] = new Cell("Пробка: пропусти ход", skipTurn: true);
            board.Cells[9] = new Cell("Поломка: назад на 4", steps: -4);
            board.Cells[12] = new Cell("Пассажир: вперёд на 2", steps: 2);
            board.Cells[15] = new Cell("ДПС: пропусти ход", skipTurn: true);
            board.Cells[17] = new Cell("Зелёный свет: вперёд на 3", steps: 3);
            board.Cells[21] = new Cell("Поломка: назад на 5", steps: -5);
            board.Cells[24] = new Cell("Пробка: пропусти ход", skipTurn: true);
            board.Cells[27] = new Cell("Не туда свернул: назад на 5", steps: -5);
            board.Cells[29] = new Cell("ФИНИШ");
            return board;
        }
    }
}
