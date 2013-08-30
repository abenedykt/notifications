using System;
using Xunit;
using Xunit.Extensions;

namespace Notifications.BusiessLogic.Tests
{
    public class FizzBuzzTests
    {
        [Fact]
        public void Game_For_1_returns_1()
        {
            // arange
            var game = new Game();

            //act
            string result = game.Play(1);

            //assert
            Assert.Equal("1", result);
        }

        [Fact]
        public void Game_for_2_return_1_2()
        {
            // arange
            var game = new Game();

            //act
            string result = game.Play(2);

            //assert
            Assert.Equal("1, 2", result);
        }

        [Theory]
        [InlineData(1, "1")]
        [InlineData(2, "1, 2")]
        [InlineData(3, "1, 2, Fizz")]
        [InlineData(4, "1, 2, Fizz, 4")]
        [InlineData(5, "1, 2, Fizz, 4, Buzz")]
        [InlineData(15, "1, 2, Fizz, 4, Buzz, Fizz, 7, 8, Fizz, Buzz, 11, Fizz, 13, 14, Fizz Buzz")]
        public void Game_Returns_propper_value(int n, string expected)
        {
            // arange
            var game = new Game();

            //act
            string result = game.Play(n);

            //assert
            Assert.Equal(expected, result);
        }
    }

    public class Game
    {
        public string Play(int n)
        {
            string result = "1";
            for (int i = 2; i <= n; i++)
            {
                if (i%3 == 0)
                {
                    if (i%5 == 0)
                        result += ", Fizz Buzz";
                    else
                        result += ", Fizz";
                }
                else if (i%5 == 0)
                    result += ", Buzz";
                else
                    result += ", " + Convert.ToString(i);
            }

            return result;
        }
    }

    public class PersonTests
    {
        [Theory]
        [InlineData("Jan", "Kowalski", "Jan Kowalski")]
        [InlineData("", "Kowalski", "Kowalski")]
        [InlineData("Jan", "", "Jan")]
        public void ToString_Returns_Name_And_Surname(string name, string surname, string exptected)
        {
            var person = new Person
            {
                Name = name,
                Surname = surname
            };

            Assert.Equal(exptected, person.ToString());
        }
    }

    public class Person
    {
        public string Name { get; set; }
        public string Surname { get; set; }

        public override string ToString()
        {
            return string.Format("{0} {1}", Name, Surname).Trim();
        }
    }
}