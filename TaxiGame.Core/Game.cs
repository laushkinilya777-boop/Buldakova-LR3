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
        /// <summary>Игровое поле.</summary>
        public Board Board { get; set; }

        /// <summary>Игроки.</summary>
        public List<Player> Players { get; set; } = new List<Player>();

        /// <summary>Номер игрока, который сейчас ходит.</summary>
        public int CurrentIndex { get; set; }

        /// <summary>Имя победителя (null, пока игра идёт).</summary>
        public string Winner { get; set; }

        /// <summary>Кубик (в файл не сохраняется).</summary>
        [JsonIgnore]
        public IDice Dice { get; set; } = new RandomDice();

        /// <summary>Закончена ли игра.</summary>
        [JsonIgnore]
        public bool IsOver => Winner != null;

        /// <summary>Игрок, который сейчас ходит.</summary>
        [JsonIgnore]
        public Player CurrentPlayer => Players[CurrentIndex];

        /// <summary>Пустой конструктор — нужен для загрузки из JSON.</summary>
        public Game() { }

        /// <summary>Новая игра на стандартном поле.</summary>
        public Game(List<string> names)
        {
            if (names.Count < 1 || names.Count > 4)
                throw new ArgumentException("Игроков должно быть от 1 до 4.");
            Board = Board.CreateStandard();
            foreach (string name in names)
                Players.Add(new Player(name));
        }

        /// <summary>
        /// Ход текущего игрока. Возвращает текст: что произошло.
        /// </summary>
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
                int dice = Dice.Roll();
                player.Position = Math.Min(player.Position + dice, Board.Finish);
                Cell cell = Board.Cells[player.Position];
                message = $"{player.Name}: выпало {dice}, клетка {player.Position} «{cell.Name}».";

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
                    return message + $" {player.Name} доехал до финиша и победил!";
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
        void Save(Game game, string path);
        Game Load(string path);
    }

    /// <summary>Хранит игру в файле JSON (сериализация System.Text.Json).</summary>
    public class JsonGameStorage : IGameStorage
    {
        public void Save(Game game, string path)
        {
            File.WriteAllText(path, JsonSerializer.Serialize(game));
        }

        public Game Load(string path)
        {
            return JsonSerializer.Deserialize<Game>(File.ReadAllText(path));
        }
    }
}
