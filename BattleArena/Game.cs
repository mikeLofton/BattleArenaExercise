using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace BattleArena
{  
    public enum ItemType
    {
        DEFENSE,
        ATTACK,
        NONE
    }

    public enum Scene
    {
        STARTMENU,
        NAMECREATION,
        CHARACTERSELECTION,
        BATTLE,
        RESARTMENU
    }

    public struct Item
    {
        public string Name;
        public float StatBoost;
        public ItemType Type;
    }

    public class Game
    {
        private bool _gameOver = false;
        private Scene _currentScene = 0;
        private Player _player;
        private Entity[] _enemies;
        private int _currentEnemyIndex = 0;
        private Entity _currentEnemy;
        private string _playerName;
        private Item[] _wizardItems;
        private Item[] _knightItems;

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
            InitalizeItems();
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

        public void Save()
        {
            //Create a new stream writer
            StreamWriter writer = new StreamWriter("SaveData.txt");

            //Save current enemy index
            writer.WriteLine(_currentEnemyIndex);

            //Save current player and enemy stats
            _player.Save(writer);
            _currentEnemy.Save(writer);

            //Close the writer when done saving
            writer.Close();
        }

        public bool Load()
        {
            bool loadSuccessful = true;

            //If the file doesn't exist...
            if (!File.Exists("SaveData.txt"))
                //...returns false
                loadSuccessful = false;

            //Create a new reader to read from the text file
            StreamReader reader = new StreamReader("SaveData.txt");

            //If the first line can't be converted into an integer...
            if (!int.TryParse(reader.ReadLine(), out _currentEnemyIndex))
                //...returns false
                loadSuccessful = false;

            //Load player job
            string job = reader.ReadLine();

            if (job == "Wizard")
                _player = new Player(_wizardItems);
            else if (job == "Knight")
                _player = new Player(_knightItems);
            else
                loadSuccessful = false;

            _player.Job = job;

            //Creates a new instance and try load the player          
            if (!_player.Load(reader))
                loadSuccessful = false;

            //Create a new instance and try to load the enemy
            _currentEnemy = new Entity();
            if (!_currentEnemy.Load(reader))
                loadSuccessful = false;

            //Update the array to match the current enemy stats
            _enemies[_currentEnemyIndex] = _currentEnemy;

            //Close the reader once loading is finished
            reader.Close();

            return loadSuccessful;
        }

        /// <summary>
        /// Gets an input from the player based on some given decision
        /// </summary>
        /// <param name="description">The context for the input</param>
        /// <param name="options">The players choices</param>       
        /// <returns></returns>
        int GetInput(string description, params string[] options)
        {
            string input = "";
            int inputRecieved = -1;

            while (inputRecieved == -1)
            {
                //Print options
                Console.WriteLine(description);
                for (int i = 0; i < options.Length; i++)
                {
                    Console.WriteLine((i + 1) + ". " + options[i]);
                }
                Console.Write("> ");

                //Get input from player
                input = Console.ReadLine();

                //If the player typed an int...
                if (int.TryParse(input, out inputRecieved))
                {
                    //...decrement the input and check if it's within the bounds of the array
                    inputRecieved--;
                    if (inputRecieved < 0 || inputRecieved >= options.Length)
                    {
                        //Set input recieved to be default value
                        inputRecieved = -1;
                        //Display error message
                        Console.WriteLine("Invalid Input");
                        Console.ReadKey(true);
                    }
                }
                //If the player didn't type an int
                else
                {
                    //Set input recieved to default value
                    inputRecieved = -1;
                    Console.WriteLine("Invalid Input");
                    Console.ReadKey(true);
                }

                Console.Clear();
            }

            return inputRecieved;
        }

        /// <summary>
        /// Calls the appropriate function(s) based on the current scene index
        /// </summary>
        void DisplayCurrentScene()
        {
            switch (_currentScene)
            {
                case Scene.STARTMENU:
                    DisplayStartMenu();
                    break;

                case Scene.NAMECREATION:
                    GetPlayerName();                   
                    break;

                case Scene.CHARACTERSELECTION:
                    CharacterSelection();
                    break;

                case Scene.BATTLE:
                    Battle();                 
                    CheckBattleResults();
                    Console.ReadKey(true);
                    break;

                case Scene.RESARTMENU:
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

            if (input == 0)
            {
                InitalizeEnemies();
                _currentScene = 0;
            }
            else if (input == 1)
            {
                _gameOver = true;
            }
        }

        public void DisplayStartMenu()
        {
            int input = GetInput("Welcome to Battle Arena!", "Start New Game", "Load Game");

            if (input == 0)
            {
                _currentScene = Scene.NAMECREATION;
            }
            else if (input == 1)
            {
                if (Load())
                {
                    Console.WriteLine("Load Successful!");
                    Console.ReadKey(true);
                    Console.Clear();
                    _currentScene = Scene.BATTLE;
                }
                else
                {
                    Console.WriteLine("Load Failed.");
                    Console.ReadKey(true);
                    Console.Clear();
                }
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
                
            if (input == 0)
            {
             _currentScene = Scene.CHARACTERSELECTION;
            }
            else if (input == 1)
            {
             _currentScene = Scene.NAMECREATION;
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

            if (input == 0)
            {
                _player = new Player(_playerName, 50, 25, 5, _wizardItems, "Wizard");              
            }
            else if (input == 1)
            {
                _player = new Player(_playerName, 75, 15, 10, _knightItems, "Knight");             
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

        public void DisplayEquipItemMenu()
        {
            //Get item index
            int input = GetInput("Select an item to equip.", _player.GetItemNames());

            //Equip item at given index
            if (!_player.TryEquipItem(input))
                Console.WriteLine("You couldn't find that item in your bag.");

            //Print feedack
            Console.WriteLine("You equipped " + _player.CurrentItem.Name + "!");
        }

        /// <summary>
        /// Simulates one turn in the current monster fight
        /// </summary>
        public void Battle()
        {
            DisplayStats(_player);
            DisplayStats(_currentEnemy);

            int input = GetInput("A " + _currentEnemy.Name + " stands in front of you! What will you do?",
                "1. Attack", "2. Equip Item", "3. Remove current item", "4. Save");

            if (input == 0)
            {
                //The player attacks the enemy
                float damageDealt = _player.Attack(_currentEnemy);
                Console.WriteLine("You dealt " + damageDealt + " damage!");

                //The enemy attacks the player
                damageDealt = _currentEnemy.Attack(_player);
                Console.WriteLine("The " + _currentEnemy.Name + " dealt " + damageDealt);
            }
            else if (input == 1)
            {
                DisplayEquipItemMenu();              
            }
            else if (input == 2)
            {
                if (!_player.TryRemoveCurrentItem())
                    Console.WriteLine("You don't have anything equipped.");
                else
                    Console.WriteLine("You placed the item in your bag.");

                //Console.ReadKey(true);
                //Console.Clear();
                //return;
            }
            else if (input == 3)
            {
                Save();
                Console.WriteLine("Saved Game");
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
                _currentScene = Scene.RESARTMENU;
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

        public void InitalizeItems()
        {
            //Wizard Items
            Item bigWand = new Item { Name = "Big Wand", StatBoost = 5, Type = ItemType.ATTACK };
            Item bigShield = new Item { Name = "Big Shield", StatBoost = 15, Type = ItemType.DEFENSE};

            //Knight Items
            Item wand = new Item { Name = "Wand", StatBoost = 10, Type = ItemType.ATTACK };
            Item shoes = new Item { Name = "Shoes", StatBoost = 90, Type = ItemType.DEFENSE };

            //Initialize Arrays
            _wizardItems = new Item[] { bigWand, bigShield };
            _knightItems = new Item[] { wand, shoes };
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
                _currentScene = Scene.RESARTMENU;
            }

            return endGame;
        }
    }
}
