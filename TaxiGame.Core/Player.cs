using System;

namespace TaxiGame.Core
{
    /// <summary>Игрок (таксист) и его фишка на поле.</summary>
    public class Player
    {
        private string name = "Игрок";   // имя игрока
        private int position;            // номер клетки, на которой стоит фишка
        private bool skipTurn;           // пропускает ли игрок следующий ход

        /// <summary>Имя игрока. Не может быть пустым.</summary>
        public string Name
        {
            get { return name; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Имя игрока не может быть пустым.");
                name = value;
            }
        }

        /// <summary>Номер клетки, на которой стоит фишка. Не может быть отрицательным.</summary>
        public int Position
        {
            get { return position; }
            set
            {
                if (value < 0)
                    throw new ArgumentException("Номер клетки не может быть отрицательным.");
                position = value;
            }
        }

        /// <summary>Пропускает ли игрок следующий ход.</summary>
        public bool SkipTurn
        {
            get { return skipTurn; }
            set { skipTurn = value; }
        }

        /// <summary>Конструктор без параметров — нужен для загрузки из JSON.</summary>
        public Player() { }

        /// <summary>Создаёт игрока на клетке старта.</summary>
        /// <param name="name">Имя игрока.</param>
        public Player(string name)
        {
            Name = name;
        }

        public override string ToString() => $"{Name} — клетка {Position}";
    }
}
