using System;

namespace TaxiGame.Core
{
    /// <summary>Кубик. Через интерфейс игра получает число очков (в тестах можно подставить свой кубик).</summary>
    public interface IDice
    {
        /// <summary>Бросает кубик: число от 1 до 6.</summary>
        int Roll();
    }

    /// <summary>Обычный случайный кубик.</summary>
    public class RandomDice : IDice
    {
        private readonly Random random = new Random();

        public int Roll() => random.Next(1, 7);
    }
}
