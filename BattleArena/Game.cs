using System;
using System.Collections.Generic;
using System.Text;

namespace BattleArena
{  
    class Game
    {
        private bool _gameOver = false;
        private int _currentScene = 0;
        private Entity _player;
        private Entity[] _enemies;
        private int _currentEnemyIndex = 0;
        private Entity _currentEnemy;
        private string _playerName;

        //Enemies
        Entity slime;
        Entity zomB;
        Entity kris;

        /// <summary>
        /// Function that starts the main game loop
        /// </summary>
        public void Run()
        {
            Start();

            while (!_gameOver)
            {
                Update();
            }

            End();

        }

        /// <summary>
        /// Function used to initialize any starting values by default
        /// </summary>
        public void Start()
        {
            //Initialize Characters          
            slime = new Entity("Slime", 10, 1, 0);

            zomB = new Entity("Zom-B", 15, 5, 2);

            kris = new Entity("a guy named Kris", 25, 10, 5);         

            _enemies = new Entity[] { slime, zomB, kris };

            InitalizeEnemies();
        }

        /// <summary>
        /// This function is called every time the game loops.
        /// </summary>
        public void Update()
        {
            DisplayCurrentScene();
            Console.Clear();
        }

        /// <summary>
        /// This function is called before the applications closes
        /// </summary>
        public void End()
        {
            Console.WriteLine("Fairwell adventurer.");
        }

        /// <summary>
        /// Gets an input from the player based on some given decision
        /// </summary>
        /// <param name="description">The context for the input</param>
        /// <param name="option1">The first option the player can choose</param>
        /// <param name="option2">The second option the player can choose</param>
        /// <returns></returns>
        int GetInput(string description, string option1, string option2)
        {
            string input = "";
            int inputReceived = 0;

            while (inputReceived != 1 && inputReceived != 2)
            {//Print options
                Console.WriteLine(description);
                Console.WriteLine("1. " + option1);
                Console.WriteLine("2. " + option2);
                Console.Write("> ");

                //Get input from player
                input = Console.ReadLine();

                //If player selected the first option...
                if (input == "1" || input == option1)
                {
                    //Set input received to be the first option
                    inputReceived = 1;
                }
                //Otherwise if the player selected the second option...
                else if (input == "2" || input == option2)
                {
                    //Set input received to be the second option
                    inputReceived = 2;
                }
                //If neither are true...
                else
                {
                    //...display error message
                    Console.WriteLine("Invalid Input");
                    Console.ReadKey();
                }

                Console.Clear();
            }
            return inputReceived;
        }

        /// <summary>
        /// Calls the appropriate function(s) based on the current scene index
        /// </summary>
        void DisplayCurrentScene()
        {
            switch (_currentScene)
            {
                case 0:
                    GetPlayerName();                   
                    break;

                case 1:
                    CharacterSelection();
                    break;

                case 2:
                    Battle();                 
                    CheckBattleResults();
                    Console.ReadKey(true);
                    break;

                case 3:
                    DisplayMainMenu();
                    break;

                default:
                    Console.WriteLine("Invalid scene index");
                    break;
            }
        }

        /// <summary>
        /// Displays the menu that allows the player to start or quit the game
        /// </summary>
        void DisplayMainMenu()
        {
            int input = GetInput("Play Again?", "1. Yes", "2. No");

            if (input == 1)
            {
                InitalizeEnemies();
                _currentScene = 0;
            }
            else if (input == 2)
            {
                _gameOver = true;
            }
        }

        /// <summary>
        /// Displays text asking for the players name. Doesn't transition to the next section
        /// until the player decides to keep the name.
        /// </summary>
        void GetPlayerName()
        {
            
            
                Console.WriteLine("Welcome! Please enter your name.");
                Console.Write("> ");
                _playerName = Console.ReadLine();

                Console.Clear();

                int input = GetInput("You've entered " + _playerName + " are you sure you want to keep this name?",
                    "1. Yes", "2. No");
                
            if (input == 1)
            {
             _currentScene = 1;
            }
            else if (input == 2)
            {
             _currentScene = 0;
            }              
            
        }

        /// <summary>
        /// Gets the players choice of character. Updates player stats based on
        /// the character chosen.
        /// </summary>
        public void CharacterSelection()
        {
            int input = GetInput("Nice to meet you " + _playerName + ". Please select a character.",
                "1. Wizard", "2. Knight");

            if (input == 1)
            {
                _player = new Entity(_playerName, 50, 25, 5);              
            }
            else if (input == 2)
            {
                _player = new Entity(_playerName, 75, 15, 10);             
            }

            _currentScene++;
        }

        /// <summary>
        /// Prints a characters stats to the console
        /// </summary>
        /// <param name="character">The character that will have its stats shown</param>
        void DisplayStats(Entity character)
        {
            Console.WriteLine("Name: " + character.Name);
            Console.WriteLine("Health: " + character.Health);
            Console.WriteLine("Attack Power: " + character.AttackPower);
            Console.WriteLine("Defense Power: " + character.DefensePower + "\n");
        }     

        /// <summary>
        /// Simulates one turn in the current monster fight
        /// </summary>
        public void Battle()
        {
            DisplayStats(_player);
            DisplayStats(_currentEnemy);

            int input = GetInput("A " + _currentEnemy.Name + " stands in front of you! What will you do?",
                "1. Attack", "2. Dodge");

            if (input == 1)
            {
                //The player attacks the enemy
                float damageDealt = _player.Attack(_currentEnemy);
                Console.WriteLine("You dealt " + damageDealt + " damage!");

                //The enemy attacks the player
                damageDealt = _currentEnemy.Attack(_player);
                Console.WriteLine("The " + _currentEnemy.Name + " dealt " + damageDealt);
            }
            else if (input == 2)
            {
                Console.WriteLine("You dodged the enemy's attack!");
            }
        }

        /// <summary>
        /// Checks to see if either the player or the enemy has won the current battle.
        /// Updates the game based on who won the battle..
        /// </summary>
        void CheckBattleResults()
        {
           if (_player.Health <= 0)
            {
                Console.WriteLine("You were slain...");
                Console.ReadKey(true);
                Console.Clear();
                _currentScene = 3;
            }           
            else if (_currentEnemy.Health <= 0)
            {
                Console.ReadKey(true);
                Console.Clear();
                Console.WriteLine("You slayed the " + _currentEnemy.Name);
                

                _currentEnemyIndex++;

                if (TryEndGame())
                {
                    return;
                }

                _currentEnemy = _enemies[_currentEnemyIndex];
            }
        }        

        public void InitalizeEnemies()
        {
            _currentEnemyIndex = 0;

            slime = new Entity("Slime", 10, 1, 0);

            zomB = new Entity("Zom-B", 15, 5, 2);

            kris = new Entity("a guy named Kris", 25, 10, 5);

            _enemies = new Entity[] { slime, zomB, kris };

            _currentEnemy = _enemies[_currentEnemyIndex];
        }

        bool TryEndGame()
        {
            bool endGame = _currentEnemyIndex >= _enemies.Length;

            if (endGame)
            {
                _currentScene = 3;
            }

            return endGame;
        }
    }
}
