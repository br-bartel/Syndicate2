﻿using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;

namespace TheSyndicate.Actions
{
    enum Attack { LeftHook, RightHook, LaserBeam}
    enum Dodge { DodgeRight, DodgeLeft, Duck, NoDodge }

    class FightAction : IAction
    {
        private static int SECONDS_USER_HAS_TO_DODGE = 1;
        private static int TIMES_USER_MUST_DODGE_ATTACKS = 3;
        private static int NUMBER_OF_ATTACKS_TO_DEFEND_AGAINST = 5;
        private static string INSTRUCTIONS = $"HALT. YOU ARE NOT AUTHORIZED TO ACCESS THIS LOCATION!!\n\nYou turn to find a relic of the war between The Syndicate and humans, a Watchman robot. You've gotten yourself into a fight! You're a lover, not a fighter though so you refuse to hurt your opponent. Looks like you're going to have to dodge.\nYour opponent will attack you {NUMBER_OF_ATTACKS_TO_DEFEND_AGAINST} time(s) and you must successfully dodge {TIMES_USER_MUST_DODGE_ATTACKS} time(s). \nYou will have {SECONDS_USER_HAS_TO_DODGE} second(s) to respond by pressing the correct arrow key.\nIf your opponent throws a left hook, you must dodge right (right arrow key)\nIf your opponent throws a right hook, you must dodge left (left arrow key)\nIf your opponent shoots a laser beam, you must duck(down arrow key)";
        private static int NumberOfTypesOfAttacks = Attack.GetNames(typeof(Attack)).Length;
        private Stopwatch Stopwatch { get; set; }
        private Random Random { get; }
        private int SuccessfullDodges { get; set; }
        private Attack CurrentAttack { get; set; }
        private Dodge CurrentDodge { get; set; }
        private ConsoleKey CurrentKeyPressed { get; set; }

        public FightAction()
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
            TextBox instructions = new TextBox(INSTRUCTIONS, Console.WindowWidth / 3, 2, Console.WindowWidth / 3, Console.WindowHeight / 4);
            Console.Clear();
            instructions.SetBoxPosition(instructions.TextBoxX, instructions.TextBoxY);
            instructions.FormatText(INSTRUCTIONS);
        }

        private void WaitForPlayerToPressEnter()
        {
            string enterPrompt = "Press ENTER to continue.";
            Console.SetCursorPosition(Console.WindowWidth / 2 - enterPrompt.Length / 2, Console.WindowHeight - (Console.WindowHeight / 5));
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
                Console.SetCursorPosition(Console.WindowWidth / 2 - 18, Console.WindowHeight / 2);
                Console.WriteLine($"Opponent's attack: {CurrentAttack}");
                SetCurrentDodge();
                if (UserSuccessfullyDodged())
                {
                    SuccessfullDodges++;
                }
            }
        }

        private void RenderFightOptions()
        {
            string options = "Left Hook  --> Right Dodge (Right Arrow Key)\nRight Hook --> Left Dodge (Left Arrow Key)\nLaser Beam --> Duck (Down Arrow Key)";
            TextBox instructions = new TextBox(options, Console.WindowWidth / 3, 2, Console.WindowWidth/2 - Console.WindowWidth / 6, Console.WindowHeight / 4);
            Console.Clear();
            instructions.SetBoxPosition(instructions.TextBoxX, instructions.TextBoxY);
            instructions.FormatText(options);
        }

        private void SetCurrentAttack()
        {
            CurrentAttack = (Attack)GetRandomNumberLessThanNumberOfAttacks();
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
                CurrentKeyPressed == ConsoleKey.DownArrow)
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
            else if (CurrentKeyPressed == ConsoleKey.DownArrow)
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
                string successMessage = $"Phew, that was close! You successfully dodged {SuccessfullDodges} attack(s).";
                string successMessage2 = "You've still been caught but at least you live to see another day.";
                string successMessage3 = "Off to the reclamation center you go.";
                Console.SetCursorPosition(Console.WindowWidth/2 - successMessage.Length/2, Console.WindowHeight/2);
                Console.WriteLine(successMessage);
                Console.SetCursorPosition(Console.WindowWidth/2 - successMessage2.Length/2, (Console.WindowHeight/2) + 1);
                Console.WriteLine(successMessage2);
                Console.SetCursorPosition(Console.WindowWidth/2 - successMessage3.Length/2, (Console.WindowHeight/2) + 2);
                Console.WriteLine(successMessage3);
            }
            else
            {
                string failMessage = $"Darn, you were too slow. It was an honor to narrate you.";
                Console.SetCursorPosition(Console.WindowWidth / 2 - failMessage.Length / 2, Console.WindowHeight / 2);
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
