using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TaxiGame.Core
{
    /// <summary>
    /// Игра-ходилка "Такси": игроки по очереди бросают кубик и двигают фишки к финишу.
    /// Клетки поля сдвигают фишку вперёд/назад или заставляют пропустить ход.
    /// Побеждает тот, кто первым дошёл до финиша.
    /// </summary>
    public class Game
    {
        private Board board = new Board();                   // игровое поле
        private List<Player> players = new List<Player>();   // игроки
        private int currentIndex;                            // номер игрока, который сейчас ходит
        private string winner;                               // имя победителя (null, пока игра идёт)
        private IDice dice = new RandomDice();               // кубик

        /// <summary>Игровое поле. Не может быть null.</summary>
        public Board Board
        {
            get { return board; }
            set
            {
                if (value == null)
                    throw new ArgumentException("Игровое поле не может быть null.");
                board = value;
            }
        }

        /// <summary>Игроки. Список не может быть null.</summary>
        public List<Player> Players
        {
            get { return players; }
            set
            {
                if (value == null)
                    throw new ArgumentException("Список игроков не может быть null.");
                players = value;
            }
        }

        /// <summary>Номер игрока, который сейчас ходит: от 0 до Players.Count - 1.</summary>
        public int CurrentIndex
        {
            get { return currentIndex; }
            set
            {
                if (value < 0 || (players.Count > 0 && value >= players.Count))
                    throw new ArgumentException("Нет игрока с таким номером.");
                currentIndex = value;
            }
        }

        /// <summary>Имя победителя (null, пока игра идёт). Пустая строка не допускается.</summary>
        public string Winner
        {
            get { return winner; }
            set
            {
                if (value != null && value.Trim() == "")
                    throw new ArgumentException("Имя победителя не может быть пустым.");
                winner = value;
            }
        }

        /// <summary>Кубик (в файл не сохраняется). Не может быть null.</summary>
        [JsonIgnore]
        public IDice Dice
        {
            get { return dice; }
            set
            {
                if (value == null)
                    throw new ArgumentException("Кубик не может быть null.");
                dice = value;
            }
        }

        /// <summary>Закончена ли игра.</summary>
        [JsonIgnore]
        public bool IsOver => Winner != null;

        /// <summary>Игрок, который сейчас ходит.</summary>
        [JsonIgnore]
        public Player CurrentPlayer => Players[CurrentIndex];

        /// <summary>Конструктор без параметров — нужен для загрузки из JSON.</summary>
        public Game() { }

        /// <summary>Новая игра на стандартном поле.</summary>
        /// <param name="names">Имена игроков (от 1 до 4).</param>
        public Game(List<string> names)
        {
            if (names == null || names.Count < 1 || names.Count > 4)
                throw new ArgumentException("Игроков должно быть от 1 до 4.");
            Board = Board.CreateStandard();
            foreach (string name in names)
                Players.Add(new Player(name));
        }

        /// <summary>
        /// Делает ход текущего игрока.
        /// </summary>
        /// <returns>Текст с описанием хода.</returns>
        public string MakeTurn()
        {
            if (IsOver) throw new InvalidOperationException("Игра окончена.");
            Player player = CurrentPlayer;
            string message;

            if (player.SkipTurn)
            {
                // игрок стоит в пробке — пропускает ход
                player.SkipTurn = false;
                message = $"{player.Name} пропускает ход.";
            }
            else
            {
                int points = Dice.Roll();
                player.Position = Math.Min(player.Position + points, Board.Finish);
                Cell cell = Board.Cells[player.Position];
                message = $"{player.Name}: выпало {points}, клетка {player.Position} «{cell.Name}».";

                // действие клетки
                if (cell.Steps != 0)
                {
                    player.Position = Math.Max(0, Math.Min(player.Position + cell.Steps, Board.Finish));
                    message += $" Теперь на клетке {player.Position}.";
                }
                if (cell.SkipTurn)
                    player.SkipTurn = true;

                if (player.Position == Board.Finish)
                {
                    Winner = player.Name;
                    return message + $" Финиш! Победитель — {player.Name}!";
                }
            }

            // ход переходит следующему игроку
            CurrentIndex = (CurrentIndex + 1) % Players.Count;
            return message;
        }
    }

    /// <summary>Сохранение и загрузка игры.</summary>
    public interface IGameStorage
    {
        /// <summary>Сохраняет игру в файл.</summary>
        void Save(Game game, string path);

        /// <summary>Загружает игру из файла.</summary>
        Game Load(string path);
    }

    /// <summary>Хранит игру в файле JSON (сериализация System.Text.Json).</summary>
    public class JsonGameStorage : IGameStorage
    {
        public void Save(Game game, string path)
        {
            if (game == null) throw new ArgumentNullException(nameof(game));
            File.WriteAllText(path, JsonSerializer.Serialize(game));
        }

        public Game Load(string path)
        {
            return JsonSerializer.Deserialize<Game>(File.ReadAllText(path));
        }
    }
}
