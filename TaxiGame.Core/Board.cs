using System;
using System.Collections.Generic;

namespace TaxiGame.Core
{
    /// <summary>Клетка игрового поля.</summary>
    public class Cell
    {
        private string name = "Дорога";   // название клетки
        private int steps;                // сдвиг фишки (вперёд > 0, назад < 0)
        private bool skipTurn;            // пропуск следующего хода

        /// <summary>Название клетки (сразу говорит, что она делает). Не может быть пустым.</summary>
        public string Name
        {
            get { return name; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Название клетки не может быть пустым.");
                name = value;
            }
        }

        /// <summary>На сколько клеток сдвинуть фишку: от -10 до 10 (больше 0 — вперёд, меньше 0 — назад).</summary>
        public int Steps
        {
            get { return steps; }
            set
            {
                if (value < -10 || value > 10)
                    throw new ArgumentException("Сдвиг должен быть от -10 до 10 клеток.");
                steps = value;
            }
        }

        /// <summary>Пропускает ли игрок следующий ход.</summary>
        public bool SkipTurn
        {
            get { return skipTurn; }
            set { skipTurn = value; }
        }

        /// <summary>Конструктор без параметров — нужен для загрузки из JSON.</summary>
        public Cell() { }

        /// <summary>Создаёт клетку.</summary>
        /// <param name="name">Название клетки.</param>
        /// <param name="steps">Сдвиг фишки (по умолчанию 0).</param>
        /// <param name="skipTurn">Пропуск хода (по умолчанию нет).</param>
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
        private List<Cell> cells = new List<Cell>();   // клетки по порядку

        /// <summary>Клетки по порядку. Список не может быть null.</summary>
        public List<Cell> Cells
        {
            get { return cells; }
            set
            {
                if (value == null)
                    throw new ArgumentException("Список клеток не может быть null.");
                cells = value;
            }
        }

        /// <summary>Номер клетки финиша (последней клетки).</summary>
        public int Finish => cells.Count - 1;

        /// <summary>Создаёт пустое поле (нужно для загрузки из JSON).</summary>
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
