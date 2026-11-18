using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TaxiGame.Core;

namespace TaxiGame.Tests
{
    /// <summary>Кубик для тестов: всегда выдаёт одно и то же число.</summary>
    public class FixedDice : IDice
    {
        private readonly int value;
        public FixedDice(int value) { this.value = value; }
        public int Roll() => value;
    }

    [TestClass]
    public class GameTests
    {
        private static Game NewGame(int dice)
        {
            Game game = new Game(new List<string> { "Маша", "Илья" });
            game.Dice = new FixedDice(dice);
            return game;
        }

        [TestMethod]
        public void NewGame_PlayersOnStart()
        {
            Game game = NewGame(1);
            Assert.AreEqual(2, game.Players.Count);
            Assert.AreEqual(0, game.Players[0].Position);
            Assert.AreEqual("Маша", game.CurrentPlayer.Name);
            Assert.IsFalse(game.IsOver);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void NewGame_TooManyPlayers_Throws()
        {
            new Game(new List<string> { "1", "2", "3", "4", "5" });
        }

        [TestMethod]
        public void Board_Has30CellsStartAndFinish()
        {
            Board board = Board.CreateStandard();
            Assert.AreEqual(30, board.Cells.Count);
            Assert.AreEqual(29, board.Finish);
            Assert.AreEqual("СТАРТ", board.Cells[0].Name);
            Assert.AreEqual("ФИНИШ", board.Cells[29].Name);
        }

        [TestMethod]
        public void MakeTurn_MovesPlayerAndPassesTurn()
        {
            Game game = NewGame(2);
            game.MakeTurn();
            Assert.AreEqual(2, game.Players[0].Position);
            Assert.AreEqual("Илья", game.CurrentPlayer.Name);
        }

        [TestMethod]
        public void GreenLight_MovesForward()
        {
            Game game = NewGame(3);       // клетка 3: вперёд на 2
            game.MakeTurn();
            Assert.AreEqual(5, game.Players[0].Position);
        }

        [TestMethod]
        public void TrafficJam_SkipsNextTurn()
        {
            Game game = NewGame(6);       // клетка 6: пробка
            game.MakeTurn();              // Маша попала в пробку
            Assert.IsTrue(game.Players[0].SkipTurn);
            game.MakeTurn();              // Илья
            game.MakeTurn();              // Маша пропускает ход
            Assert.AreEqual(6, game.Players[0].Position);
            Assert.IsFalse(game.Players[0].SkipTurn);
        }

        [TestMethod]
        public void Breakdown_MovesBack()
        {
            Game game = NewGame(3);
            game.Players[0].Position = 6;  // 6 + 3 = клетка 9: назад на 4
            game.MakeTurn();
            Assert.AreEqual(5, game.Players[0].Position);
        }

        [TestMethod]
        public void ReachFinish_PlayerWins()
        {
            Game game = NewGame(6);
            game.Players[0].Position = 25; // 25 + 6 = 31, но поле кончается на 29
            game.MakeTurn();
            Assert.AreEqual(29, game.Players[0].Position);
            Assert.IsTrue(game.IsOver);
            Assert.AreEqual("Маша", game.Winner);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void MakeTurn_AfterGameOver_Throws()
        {
            Game game = NewGame(6);
            game.Winner = "Маша";
            game.MakeTurn();
        }

        [TestMethod]
        public void RandomDice_From1To6()
        {
            IDice dice = new RandomDice();
            for (int i = 0; i < 100; i++)
            {
                int value = dice.Roll();
                Assert.IsTrue(value >= 1 && value <= 6);
            }
        }

        [TestMethod]
        public void SaveAndLoad_Json_RestoresGame()
        {
            string path = Path.Combine(Path.GetTempPath(), "taxi_test.json");
            Game game = NewGame(2);
            game.MakeTurn();

            IGameStorage storage = new JsonGameStorage();
            storage.Save(game, path);
            Game loaded = storage.Load(path);
            File.Delete(path);

            Assert.AreEqual(2, loaded.Players[0].Position);
            Assert.AreEqual(1, loaded.CurrentIndex);
            Assert.AreEqual(30, loaded.Board.Cells.Count);
            Assert.AreEqual(-4, loaded.Board.Cells[9].Steps);
        }
    }
}
