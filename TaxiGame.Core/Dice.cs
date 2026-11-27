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
        private readonly Random random;   // генератор случайных чисел

        /// <summary>Создаёт кубик со своим генератором случайных чисел.</summary>
        public RandomDice()
        {
            random = new Random();
        }

        /// <summary>Бросает кубик.</summary>
        /// <returns>Случайное число от 1 до 6.</returns>

        public int Roll() => random.Next(1, 7);
    }
}
