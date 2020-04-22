using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahtzee
{
    class Program
    {
        static void Main(string[] args)
        {
            GameManger.Initialize();

            // Game loop
            while (GameManger.turnNumber <= Constant.TOTAL_TURNS)
            {
                if(GameManger.turnEnd)
                    GameManger.Roll();
                // Turn Loop
                do
                {
                    GameManger.Display();
                    char response;
                    if (GameManger.rerollCount < Constant.NUM_OF_REROLL_ALLOW)
                    {
                        Console.WriteLine("Would you like to reroll? (y/n)");
                        response = Console.ReadLine()[0];
                        Console.WriteLine();
                    }
                    else
                    {
                        response = 'n';
                    }

                    switch(response)
                    {
                        case 'y':
                        case 'Y':
                            Console.WriteLine("Which die ( 0 - 4 ) would you like to re-roll: ");
                            Console.WriteLine("To pick multiple ones, space out the numbers.");
                            Console.WriteLine("For an example: 1 2 3 = re rolls the first 3 die");

                            bool isValid = false;
                            List<char> diceIndices = new List<char>();
                            do
                            {
                                string answer = Console.ReadLine();
                                if(answer.Any(x => char.IsLetter(x)))
                                {
                                    Console.WriteLine("Please enter numbers only");
                                    continue;
                                }

                                char[] splitAnswer = answer.ToCharArray();
                                int i = 0;
                                foreach (char s in splitAnswer)
                                {
                                    int temp = s - '0';
                                    if (temp > Constant.REROLL_LOW_BOUND && temp < Constant.REROLL_HIGH_BOUND)
                                    {
                                        diceIndices.Add(s);
                                        isValid = true;
                                        i++;
                                    } else if( s == ' ') { }
                                    else
                                    {
                                        Console.WriteLine("Please enter a number between 0 and 4");
                                    }
                                }

                            } while (!isValid);
                            GameManger.Reroll(diceIndices.ToArray());
                            GameManger.rerollCount++;
                            GameManger.turnEnd = false;
                            diceIndices.Clear();
                            break;
                        case 'n':
                        case 'N':
                            GameManger.CheckForCombo();
                            GameManger.turnEnd = true;
                            GameManger.IncrementTurn();
                            break;
                    }

                } while (!GameManger.turnEnd);
            }

            GameManger.EndGame();
        }
    }
}
