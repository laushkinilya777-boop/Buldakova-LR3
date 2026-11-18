using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TaxiGame.Core;

namespace TaxiGame.ConsoleApp
{
    /// <summary>Консольная версия игры "Такси" (ЛР3). Вся логика игры — в классах TaxiGame.Core.</summary>
    public static class Program
    {
        private const string SaveFile = "game.json";
        private static readonly IGameStorage storage = new JsonGameStorage();
        private static Game game;

        public static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;
            Console.WriteLine("=== Ходилка «Такси» ===");

            // Восстанавливаем сохранённую игру, если она есть
            if (File.Exists(SaveFile))
            {
                game = storage.Load(SaveFile);
                Console.WriteLine("Загружена сохранённая игра.");
            }
            else
            {
                NewGame();
            }

            while (true)
            {
                Console.WriteLine();
                if (game.IsOver)
                    Console.WriteLine($"Игра окончена, победил {game.Winner}.");
                else
                    Console.WriteLine($"Ходит {game.CurrentPlayer.Name}.");
                Console.WriteLine("1 - бросить кубик   2 - показать поле   3 - новая игра   0 - выход (с сохранением)");
                Console.Write("> ");
                string choice = Console.ReadLine();
                if (choice == null) choice = "0";

                switch (choice)
                {
                    case "1":
                        if (game.IsOver) Console.WriteLine("Игра окончена. Начните новую.");
                        else Console.WriteLine(game.MakeTurn());
                        break;
                    case "2":
                        ShowBoard();
                        break;
                    case "3":
                        NewGame();
                        break;
                    case "0":
                        storage.Save(game, SaveFile);
                        Console.WriteLine("Игра сохранена. До свидания!");
                        return;
                    default:
                        Console.WriteLine("Нет такого пункта.");
                        break;
                }
            }
        }

        private static void NewGame()
        {
            Console.Write("Имена игроков через запятую: ");
            List<string> names = new List<string>();
            foreach (string name in (Console.ReadLine() ?? "").Split(','))
                if (name.Trim() != "") names.Add(name.Trim());
            if (names.Count == 0) names.Add("Игрок");
            game = new Game(names);
            Console.WriteLine("Новая игра! Игроки: " + string.Join(", ", names));
        }

        /// <summary>Печатает все клетки поля и показывает, где стоят игроки.</summary>
        private static void ShowBoard()
        {
            for (int i = 0; i < game.Board.Cells.Count; i++)
            {
                string players = "";
                foreach (Player p in game.Players)
                    if (p.Position == i) players += " [" + p.Name + "]";
                Console.WriteLine($"{i,2}. {game.Board.Cells[i].Name}{players}");
            }
        }
    }
}
