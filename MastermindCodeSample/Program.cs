using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MastermindCodeSample
{
    public class Program
    {
        private static int[] MasterMindCode;
        private static int CodeLength = 4;
        private static int TotalAttempts = 10;
        private static int MinValue = 1;
        private static int MaxValue = 6;

        public static void Main()
        {
            Console.WriteLine("Welcome to Mastermind!");
            Console.WriteLine("You have {0} attempts to guess the {1} digit code using values {2}-{3}.", TotalAttempts.ToString(), CodeLength.ToString(), MinValue.ToString(), MaxValue.ToString());
            Console.WriteLine("");

            //Generate code that player must guess
            GenerateMastermindCode();

            //Begin game loop
            bool endResult = TheGame();

            if(endResult)
            {
                Console.WriteLine("You have correctly guessed the code.");
            }
            else
            {
                Console.WriteLine("You have used all your attempts.");
            }

            Console.ReadLine();
        }

        private static void GenerateMastermindCode()
        {
            MasterMindCode = new int[CodeLength];
            Random random = new Random();

            for(int i = 0; i < CodeLength; i++)
            {
                MasterMindCode[i] = random.Next(MinValue, MaxValue);
            }
        }

        private static bool TheGame()
        {
            int playerAttempts = 0;

            while(playerAttempts < TotalAttempts)
            {
                Console.Write(string.Format("Attempt #{0}: ", (playerAttempts + 1).ToString()));

                //get player guess
                string playerGuessAsString = Console.ReadLine();

                try
                {
                    //convert the players guess from string to array of ints
                    int[,] playerGuessArray = ConvertInputStringToArray(playerGuessAsString);

                    //copy down fresh instance of the answer
                    int[,] masterMindAnswerArray = CreateNewMasterMindArray();

                    //check for right digit in right place
                    int plusCount = CheckForCorrectGuesses(playerGuessArray, masterMindAnswerArray);

                    //check if plus count is 4 for win
                    if(plusCount == CodeLength)
                    {
                        return true;
                    }

                    //check for right digit wrong place
                    int minusCount = CheckForNearGuesses(playerGuessArray, masterMindAnswerArray);

                    PrintResult(plusCount, minusCount);

                    playerAttempts++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return false;
        }

        public static int[,] CreateNewMasterMindArray()
        {
            //initialize a new array so that the check values are refreshed
            int[,] masterMindArray = new int[CodeLength, 2];

            for (int i = 0; i < CodeLength; i++)
            {
                masterMindArray[i, 0] = MasterMindCode[i];
            }

            return masterMindArray;
        }

        public static int[,] ConvertInputStringToArray(string stringToConvert)
        {
            //remove any spaces inbetween the numbers
            string cleanString = stringToConvert.Replace(" ", "");

            //trim any leading or trailing spaces
            cleanString = cleanString.Trim();

            //if string has too many characters do not convert
            if (cleanString.Length > CodeLength)
            {
                throw new Exception(string.Format("Guesses must not contain more than {0} digits", CodeLength.ToString()));
            }
            //if string has too few do not convert
            if(cleanString.Length < CodeLength)
            {
                throw new Exception(string.Format("Please guess all {0} digits", CodeLength.ToString()));
            }

            int[,] playerGuess = new int[CodeLength, 2];

            //convert string and make sure that guesses are valid
            for(int i = 0; i < CodeLength; i++)
            {
                int digitAsInt;
                if(int.TryParse(cleanString[i].ToString(), out digitAsInt))
                {
                    //make sure that the digit is between 1 and 6
                    if (digitAsInt >= MinValue && digitAsInt <= MaxValue)
                    {
                        playerGuess[i, 0] = digitAsInt;
                    }
                    else
                    {
                        throw new Exception(string.Format("Guesses must be a digit between {0} and {1}", MinValue.ToString(), MaxValue.ToString()));
                    }
                }
                else
                {
                    throw new Exception(string.Format("{0} is not a valid guess. Please enter a digit {1}-{2}", cleanString[i].ToString(), MinValue.ToString(), MaxValue.ToString()));
                }
            }

            return playerGuess;
        }

        public static int CheckForCorrectGuesses(int[,] playerArray, int[,] masterMindArray)
        {
            int plusCount = 0;

            //check for all right digit in right place
            for (int i = 0; i < CodeLength; i++)
            {
                if(playerArray[i,0] == masterMindArray[i,0])
                {
                    //set 2d array to 1 so we know this is checked
                    playerArray[i, 1] = 1;
                    masterMindArray[i, 1] = 1;
                    //add another plus
                    plusCount++;
                }
            }

            return plusCount;
        }

        public static int CheckForNearGuesses(int[,] playerArray, int[,] masterMindArray)
        {
            int minusCount = 0;

            for(int i = 0; i < CodeLength; i++)
            {
                //if digit was a plus it does not need checked again
                if(playerArray[i,1] == 0)
                {
                    for(int j = 0; j < CodeLength; j++)
                    {
                        //check if digit exists in another spot and was not already marked correctly
                        if(playerArray[i,0] == masterMindArray[j,0] && masterMindArray[j,1] == 0)
                        {
                            masterMindArray[j, 1] = 1;
                            minusCount++;
                            break;
                        }
                    }
                }
            }


            return minusCount;
        }

        public static void PrintResult(int plusCount, int minusCount)
        {
            Console.Write("Result: ");
            for (int i = 0; i < plusCount; i++)
            {
                Console.Write("+ ");
            }
            for (int i = 0; i < minusCount; i++)
            {
                Console.Write("- ");
            }

            //add another line
            Console.WriteLine();
        }
    }
}
