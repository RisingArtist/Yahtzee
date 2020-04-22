using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahtzee
{
    static class GameManger
    {
        public enum Score { ADD1, ADD2, ADD3, ADD4, ADD5, ADD6,
            THREE_KIND, FOUR_KIND, FULL_HOUSE, LOW_STRAIGHT, HIGH_STRAIGHT, YAHTZEE, CHANCE};

        static Dictionary<Score, bool> scoreBoard = new Dictionary<Score, bool>();

        static int points;
        static Random rng = new Random();
        public static int turnNumber;
        public static int rerollCount;
        public static bool turnEnd;

        public static List<Die> dice;
        static Dictionary<int, int> numToOccurences = new Dictionary<int, int>();

        // Set up the Game
        public static void Initialize()
        {
            points = 0;
            dice = new List<Die>();
            turnNumber = 1;
            turnEnd = true;
            rerollCount = 0;
            for(int i = 0; i < Constant.NUM_OF_DIE; i++)
            {
                dice.Add(new Die());
            }

            //Top Section
            scoreBoard.Add(Score.ADD1, false);
            scoreBoard.Add(Score.ADD2, false);
            scoreBoard.Add(Score.ADD3, false);
            scoreBoard.Add(Score.ADD4, false);
            scoreBoard.Add(Score.ADD5, false);
            scoreBoard.Add(Score.ADD6, false);

            //Lower Section
            scoreBoard.Add(Score.THREE_KIND, false);
            scoreBoard.Add(Score.FOUR_KIND, false);
            scoreBoard.Add(Score.FULL_HOUSE, false);
            scoreBoard.Add(Score.LOW_STRAIGHT, false);
            scoreBoard.Add(Score.HIGH_STRAIGHT, false);
            scoreBoard.Add(Score.YAHTZEE, false);
            scoreBoard.Add(Score.CHANCE, false);


        }

        // Post Game Message
        public static void EndGame()
        {
            Console.Clear();
            Console.WriteLine("Thank you for playing!");
            Console.WriteLine("Total Score: " + points.ToString());
        }

        // Initial Roll
        public static void Roll()
        {
            foreach (Die d in dice)
            {
                d.number = Convert.ToByte(rng.Next(Constant.DICE_SIDE_LOW_BOUND, Constant.DICE_SIDE_HIGH_BOUND) );
            }
        }

        // Roll a variable size dice set; Used for Reroll()
        static void Roll(ref List<Die> _diceSet)
        {
            foreach (Die d in _diceSet)
            {
                byte num = Convert.ToByte(rng.Next(Constant.DICE_SIDE_LOW_BOUND, Constant.DICE_SIDE_HIGH_BOUND));
                d.number = num;
            }
        }

        // Take the selected dice, roll and replace them
        public static void Reroll(char[] _selected)
        {
            if(rerollCount >= Constant.NUM_OF_REROLL_ALLOW)
            {
                Console.WriteLine("You can't reroll again!");
                return;
            }

            List<Die> _set = new List<Die>();

            foreach(char index in _selected)
            {
                int temp = index - '0';
                _set.Add(dice[temp]);
            }

            Roll(ref _set);

            for(int i = 0; i < _set.Count; i++)
            {
                int temp = Int32.Parse(_selected[i].ToString());
                dice[temp].number = _set[i].number;
            }
        }

        // Increment current turn number and resets reroll count
        public static void IncrementTurn()
        {
            turnNumber++;
            rerollCount = 0;
        }

        // Show the current turn, total points and dice set
        public static void Display()
        {
            Console.WriteLine("Turn #" + turnNumber.ToString());
            Console.WriteLine("Total Points: " + points.ToString());
            Console.WriteLine("Current Dice:");
            foreach(Die d in dice)
            {
                Console.Write(d.number.ToString() + " ");
            }
            Console.WriteLine();
        }

        // Invokes the proper score box based off player's choice.
        public static void CheckForCombo()
        {
            GetNumOfOccurences(dice,ref numToOccurences);

            int chosenBoxIndex = GetScoreBox();

            switch ((Score)chosenBoxIndex)
            {
                case Score.ADD1:
                    Console.WriteLine("Chosen: ADD1");
                    UpperSection(1);
                    break;
                case Score.ADD2:
                    Console.WriteLine("Chosen: ADD2");
                    UpperSection(2);
                    break;
                case Score.ADD3:
                    Console.WriteLine("Chosen: ADD3");
                    UpperSection(3);
                    break;
                case Score.ADD4:
                    Console.WriteLine("Chosen: ADD4");
                    UpperSection(4);
                    break;
                case Score.ADD5:
                    Console.WriteLine("Chosen: ADD5");
                    UpperSection(5);
                    break;
                case Score.ADD6:
                    Console.WriteLine("Chosen: ADD6");
                    UpperSection(6);
                    break;
                case Score.THREE_KIND:
                    Console.WriteLine("Chosen: THREE_KIND");
                    OfAKind(3);
                    break;
                case Score.FOUR_KIND:
                    Console.WriteLine("Chosen: FOUR_KIND");
                    OfAKind(4);
                    break;
                case Score.FULL_HOUSE:
                    Console.WriteLine("Chosen: FULL_HOUSE");
                    FullHouse();
                    break;
                case Score.LOW_STRAIGHT:
                    Console.WriteLine("Chosen: LOW_STRAIGHT");
                    LowStraight();
                    break;
                case Score.HIGH_STRAIGHT:
                    Console.WriteLine("Chosen: HIGH_STRAIGHT");
                    HighStraight();
                    break;
                case Score.YAHTZEE:
                    Console.WriteLine("Chosen: YAHTZEE");
                    Yahtzee();
                    break;
                case Score.CHANCE:
                    Console.WriteLine("Chosen: CHANCE");
                    Chance();
                    break;
            }

            Console.WriteLine("Total points: " + points.ToString() + "\n");
            numToOccurences.Clear();
        }

        // Prompts player for scorebox of their choice to score with.
        static int GetScoreBox()
        {
            Console.WriteLine("Which box do you want to fill out ( 0 thru 12 ): ");
            int i = 0;
            foreach(KeyValuePair<Score, Boolean> scoreBox in scoreBoard)
            {
                if(!scoreBox.Value)
                    Console.WriteLine(i.ToString() + ": " + scoreBox.Key.ToString());
                i++;
            }

            int num;
            bool isValid = false;
            do
            {
                string answer = Console.ReadLine();
                bool isNum = int.TryParse(answer, out num);
                if(!isNum || !(num > Constant.SCOREBOX_LOW_BOUND && num < Constant.SCOREBOX_HIGH_BOUND))
                {
                    Console.WriteLine("Please enter a number from the list (0 - 12): ");
                    continue;
                }

                if(!scoreBoard[(Score)num])
                {
                    scoreBoard[(Score)num] = true;
                    isValid = true;
                    break;
                }

            } while (!isValid);

            return num;
        }

        // Queries the dice set to get the number of occurences for each side of dice. 
        static void GetNumOfOccurences(List<Die> _diceSet, ref Dictionary<int, int> _occurenceSet)
        {
            var q = from d in _diceSet
                    group d by d.number into g
                    let count = g.Count()
                    orderby count descending
                    select new { Num = g.Key, Count = count };

            foreach (var x in q)
            {
                _occurenceSet.Add(x.Num, x.Count);
            }
        }

        /** SCORE BOXES **/
        /// Handles checking if the dice set satisfies with the requirement for the score boxes
        /// If so, add points. Otherwise, the player doesn't get anything. 
        #region SCORE_BOXES
        static void UpperSection( int _value)
        {
            if(numToOccurences.ContainsKey(_value))
            {
                points += (_value * numToOccurences[_value]);
            }
        }

        static void OfAKind(int _value)
        {
            if(numToOccurences.Values.Contains(_value))
            {
                foreach(KeyValuePair<int, int> num in numToOccurences)
                {
                    points += (num.Key * num.Value);
                }
            }
        }

        static void FullHouse()
        {
            if(numToOccurences.Values.Contains(3) && numToOccurences.Values.Contains(2))
            {
                points += 25;
            }
        }

        static void Chance()
        {
            foreach(KeyValuePair<int, int> num in numToOccurences)
            {
                points += (num.Key * num.Value);
            }
        }

        static void Yahtzee()
        {
            if(scoreBoard[Score.YAHTZEE])
            {
                points += 100;
                return;
            }

            if(numToOccurences.Values.Contains(5))
            {
                points += 50;
            }
        }

        static void HighStraight()
        {
            int[] possibleSet1 = { 1, 2, 3, 4, 5 };
            int[] possibleSet2 = { 2, 3, 4, 5, 6 };

            var _ascDice = dice.OrderBy(d => d.number).ToArray();
            for(int i = 0; i < possibleSet1.Length; i++)
            {
                if (_ascDice.First().number == 1 && !(_ascDice[i].number == possibleSet1[i]))
                {
                    return;
                }
                else if(_ascDice.First().number == 2 && !(_ascDice[i].number == possibleSet2[i]))
                {
                    return;
                }
            }
            points += 40;
        }

        static void LowStraight()
        {
            Dictionary<int, int> _tempNumToOccurences = new Dictionary<int, int>();

            int[] possibleSet1 = { 1, 2, 3, 4 };
            int[] possibleSet2 = { 2, 3, 4, 5 };
            int[] possibleSet3 = { 3, 4, 5, 6 };

            GetNumOfOccurences(dice,ref _tempNumToOccurences);

            var _ascDice = _tempNumToOccurences.OrderBy(d => d.Key).ToArray();

            for (int i = 0; i < 4; i++)
            {
                switch(_ascDice.First().Key)
                {
                    case 1:
                        if (!(_tempNumToOccurences.ContainsKey(possibleSet1[i])))
                        {
                            return;
                        }
                        break;
                    case 2:
                        if (!(_tempNumToOccurences.ContainsKey(possibleSet2[i])))
                        {
                            return;
                        }
                        break;
                    case 3:
                        if (!(_tempNumToOccurences.ContainsKey(possibleSet3[i])))
                        {
                            return;
                        }
                        break;
                }
            }
            points += 30;
        }
        #endregion


    }
}
