using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;

namespace TheSyndicate.Actions
{
    enum CarAttack { LeftLane, RightLane, RunOver }
    //enum Dodge { DodgeRight, DodgeLeft, Duck, NoDodge }

    class CrossAction : IAction
    {
        private static int SECONDS_USER_HAS_TO_DODGE = 2;
        private static int TIMES_USER_MUST_DODGE_ATTACKS = 5;
        private static int NUMBER_OF_ATTACKS_TO_DEFEND_AGAINST = 5;
        private static string INSTRUCTIONS = $"BEEP BEEP!! HONK!!!! AWOOGA!!! beep beep! \n\nIt seems as though your attempts to woo the passing vehicles will be a bit more complicated than you had orginally figured. Based on your advanced analytics and your knowledge of love and emotive behavior, you know that you will need to court at least {NUMBER_OF_ATTACKS_TO_DEFEND_AGAINST} objects of your affection before finding love. Your processor indicates that you will have {SECONDS_USER_HAS_TO_DODGE} seconds to evade the consequences of rejection and dodge the opposite direction or jump over your scorned lover. \nIf your suitor is in the left lane, then you must dodge to the right (right arrow key). \nIf they are in the right lane, then you must dodge to the left (left arrow key). If they are about to run you over, then you must jump over them (up arrow key).";
        private static int NumberOfTypesOfAttacks = Attack.GetNames(typeof(Attack)).Length;
        private Stopwatch Stopwatch { get; set; }
        private Random Random { get; }
        private int SuccessfullDodges { get; set; }
        private CarAttack CurrentAttack { get; set; }
        private Dodge CurrentDodge { get; set; }
        private ConsoleKey CurrentKeyPressed { get; set; }

        public CrossAction()
        {
            this.SuccessfullDodges = 0;
            this.Random = new Random();
        }

        public void ExecuteAction()
        {
            Console.CursorVisible = false;
            RenderInstructions();
            WaitForPlayerToPressEnter();
            Fight();
            RenderEndMessage();
            Console.CursorVisible = true;
        }

        private void RenderInstructions()
        {
            TextBox instructions = new TextBox(INSTRUCTIONS, ConsoleWindow.Width / 3, 2, ConsoleWindow.Width / 3, ConsoleWindow.Height / 4);
            Console.Clear();
            instructions.SetBoxPosition(instructions.TextBoxX, instructions.TextBoxY);
            instructions.FormatText(INSTRUCTIONS);
        }

        private void WaitForPlayerToPressEnter()
        {
            string enterPrompt = "Press ENTER to continue.";
            Console.SetCursorPosition(ConsoleWindow.Width / 2 - enterPrompt.Length / 2, ConsoleWindow.Height - (ConsoleWindow.Height / 5));
            Console.WriteLine(enterPrompt);

            ConsoleKey userInput = Console.ReadKey(true).Key;
            while (userInput != ConsoleKey.Enter)
            {
                userInput = Console.ReadKey(true).Key;
            }
        }

        private void Fight()
        {
            for (int i = 0; i < NUMBER_OF_ATTACKS_TO_DEFEND_AGAINST; i++)
            {
                CurrentDodge = Dodge.NoDodge;
                RenderFightOptions();
                SetCurrentAttack();
                Console.SetCursorPosition(ConsoleWindow.Width / 2 - 18, ConsoleWindow.Height / 2);
                Console.WriteLine($"Oncoming Suitor: {CurrentAttack}");
                SetCurrentDodge();
                if (UserSuccessfullyDodged())
                {
                    SuccessfullDodges++;
                }
            }
        }

        private void RenderFightOptions()
        {
            string options = "Left Lane  --> Dodge Right (RIGHT Arrow Key)\nRight Lane --> Dodge Left (LEFT Arrow Key)\nRun Over --> Jump (UP Arrow Key)";
            TextBox instructions = new TextBox(options, ConsoleWindow.Width / 3, 2, ConsoleWindow.Width / 2 - ConsoleWindow.Width / 6, ConsoleWindow.Height / 4);
            Console.Clear();
            instructions.SetBoxPosition(instructions.TextBoxX, instructions.TextBoxY);
            instructions.FormatText(options);
        }

        private void SetCurrentAttack()
        {
            CurrentAttack = (CarAttack)GetRandomNumberLessThanNumberOfAttacks();
        }

        private int GetRandomNumberLessThanNumberOfAttacks()
        {
            return Random.Next(0, NumberOfTypesOfAttacks);
        }

        private void SetCurrentDodge()
        {
            GetUserInput();
            if (CurrentKeyPressed == ConsoleKey.LeftArrow ||
                CurrentKeyPressed == ConsoleKey.RightArrow ||
                CurrentKeyPressed == ConsoleKey.UpArrow)
            {
                CurrentDodge = ConvertUserInputToDodge();
            }
        }

        private void GetUserInput()
        {
            Stopwatch = new Stopwatch();
            Stopwatch.Start();
            while (this.Stopwatch.Elapsed <= TimeSpan.FromSeconds(SECONDS_USER_HAS_TO_DODGE))
            {
                SetCurrentKeyPressed();
            }
            this.Stopwatch.Stop();
        }

        private void SetCurrentKeyPressed()
        {
            if (Console.KeyAvailable)
            {
                this.CurrentKeyPressed = Console.ReadKey(true).Key;
            }
        }

        private Dodge ConvertUserInputToDodge()
        {
            if (CurrentKeyPressed == ConsoleKey.LeftArrow)
            {
                return Dodge.DodgeLeft;
            }
            else if (CurrentKeyPressed == ConsoleKey.RightArrow)
            {
                return Dodge.DodgeRight;
            }
            else if (CurrentKeyPressed == ConsoleKey.UpArrow)
            {
                return Dodge.Duck;
            }
            else
            {
                return Dodge.NoDodge;
            }
        }

        private void RenderEndMessage()
        {
            Console.Clear();
            if (DidPlayerSucceed())
            {
                string successMessage = $"The stress of the courtship process has resulted in {SuccessfullDodges} jilted lovers.";
                string successMessage2 = "However, as an unmarked van rolls to a stop in front of you, you begin to feel your internal fluid pumps increase their flow.";
                string successMessage3 = "As the first individuals exit the van, you begin to have a good feeling about this.";
                Console.SetCursorPosition(ConsoleWindow.Width / 2 - successMessage.Length / 2, ConsoleWindow.Height / 2);
                Console.WriteLine(successMessage);
                Console.SetCursorPosition(ConsoleWindow.Width / 2 - successMessage2.Length / 2, (ConsoleWindow.Height / 2) + 1);
                Console.WriteLine(successMessage2);
                Console.SetCursorPosition(ConsoleWindow.Width / 2 - successMessage3.Length / 2, (ConsoleWindow.Height / 2) + 2);
                Console.WriteLine(successMessage3);
            }
            else
            {
                string failMessage = $"Darn, you were too slow. It was an honor to narrate you.";
                Console.SetCursorPosition(ConsoleWindow.Width / 2 - failMessage.Length / 2, ConsoleWindow.Height / 2);
                Console.WriteLine(failMessage);
            }
            WaitForPlayerToPressEnter();
        }

        private bool UserSuccessfullyDodged()
        {
            return (int)CurrentAttack == (int)CurrentDodge;
        }

        public int GetIndexOfDestinationBasedOnUserSuccessOrFail()
        {
            return DidPlayerSucceed() ? 1 : 0;
        }

        public bool DidPlayerSucceed()
        {
            return SuccessfullDodges >= TIMES_USER_MUST_DODGE_ATTACKS;
        }
    }
}
