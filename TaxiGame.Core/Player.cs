namespace TaxiGame.Core
{
    /// <summary>Игрок (таксист) и его фишка на поле.</summary>
    public class Player
    {
        /// <summary>Имя игрока.</summary>
        public string Name { get; set; }

        /// <summary>Номер клетки, на которой стоит фишка.</summary>
        public int Position { get; set; }

        /// <summary>Пропускает ли игрок следующий ход.</summary>
        public bool SkipTurn { get; set; }

        public Player() { }

        public Player(string name)
        {
            Name = name;
        }

        public override string ToString() => $"{Name} — клетка {Position}";
    }
}
