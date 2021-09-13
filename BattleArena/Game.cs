using System;
using System.Collections.Generic;
using System.Text;

namespace BattleArena
{
    /// <summary>
    /// Represents any entity that exists in game
    /// </summary>
    struct Character
    {
        public string name;
        public float health;
        public float attackPower;
        public float defensePower;
    }

    class Game
    {
        bool gameOver = false;
        int currentScene = 0;
        Character player;
        Character[] enemies;
        private int currentEnemyIndex = 0;
        private Character currentEnemy;

        //Enemies
        Character slime;
        Character zomB;
        Character kris;

        /// <summary>
        /// Function that starts the main game loop
        /// </summary>
        public void Run()
        {
            Start();

            while (!gameOver)
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
            player.name = "";
            player.health = 0;
            player.attackPower = 0;
            player.defensePower = 0;

            slime.name = "Slime";
            slime.health = 10f;
            slime.attackPower = 1f;
            slime.defensePower = 0;

            zomB.name = "Zom-B";
            zomB.health = 15f;
            zomB.attackPower = 5f;
            zomB.defensePower = 2f;

            kris.name = "Kris";
            kris.health = 25f;
            kris.attackPower = 10f;
            kris.defensePower = 5f;

            enemies = new Character[] { slime, zomB, kris };

            ResetCurrentEnemy();
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
            switch (currentScene)
            {
                case 0:
                    GetPlayerName();
                    CharacterSelection();
                    break;

                case 1:
                    Battle();                 
                    CheckBattleResults();
                    Console.ReadKey(true);
                    break;

                case 2:
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
                ResetCurrentEnemy();
                currentScene = 0;
            }
            else if (input == 2)
            {
                gameOver = true;
            }
        }

        /// <summary>
        /// Displays text asking for the players name. Doesn't transition to the next section
        /// until the player decides to keep the name.
        /// </summary>
        void GetPlayerName()
        {
            bool confirmName = false;
            while (!confirmName)
            {
                Console.WriteLine("Welcome! Please enter your name.");
                Console.Write("> ");
                player.name = Console.ReadLine();

                Console.Clear();

                int input = GetInput("You've entered " + player.name + " are you sure you want to keep this name?",
                    "1. Yes", "2. No");

                if (input == 1)
                {
                    confirmName = true;
                }              
            }
        }

        /// <summary>
        /// Gets the players choice of character. Updates player stats based on
        /// the character chosen.
        /// </summary>
        public void CharacterSelection()
        {
            int input = GetInput("Nice to meet you " + player.name + ". Please select a character.",
                "1. Wizard", "2. Knight");

            if (input == 1)
            {
                player.health = 50f;
                player.attackPower = 25f;
                player.defensePower = 5f;
            }
            else if (input == 2)
            {
                player.health = 75f;
                player.attackPower = 15f;
                player.defensePower = 10f;
            }

            currentScene++;
        }

        /// <summary>
        /// Prints a characters stats to the console
        /// </summary>
        /// <param name="character">The character that will have its stats shown</param>
        void DisplayStats(Character character)
        {
            Console.WriteLine("Name: " + character.name);
            Console.WriteLine("Health: " + character.health);
            Console.WriteLine("Attack Power: " + character.attackPower);
            Console.WriteLine("Defense Power: " + character.defensePower + "\n");
        }

        /// <summary>
        /// Calculates the amount of damage that will be done to a character
        /// </summary>
        /// <param name="attackPower">The attacking character's attack power</param>
        /// <param name="defensePower">The defending character's defense power</param>
        /// <returns>The amount of damage done to the defender</returns>
        float CalculateDamage(float attackPower, float defensePower)
        {          
            return attackPower - defensePower;
        }

        /// <summary>
        /// Deals damage to a character based on an attacker's attack power
        /// </summary>
        /// <param name="attacker">The character that initiated the attack</param>
        /// <param name="defender">The character that is being attacked</param>
        /// <returns>The amount of damage done to the defender</returns>
        public float Attack(ref Character attacker, ref Character defender)
        {
            float damageTaken = CalculateDamage(attacker.attackPower, defender.defensePower);

           if (damageTaken >= 0)
            {
                defender.health -= damageTaken;
            }
           else if (damageTaken < 0)
            {
                damageTaken = 0;
            }
            
            return damageTaken;
        }

        /// <summary>
        /// Simulates one turn in the current monster fight
        /// </summary>
        public void Battle()
        {
            DisplayStats(player);
            DisplayStats(currentEnemy);

            int input = GetInput("A " + currentEnemy.name + " stands in front of you! What will you do?",
                "1. Attack", "2. Dodge");

            if (input == 1)
            {
                //The player attacks the enemy
                float damageDealt = Attack(ref player, ref currentEnemy);
                Console.WriteLine("You dealt " + damageDealt + " damage!");

                //The enemy attacks the player
                damageDealt = Attack(ref currentEnemy, ref player);
                Console.WriteLine("The " + currentEnemy.name + " dealt " + damageDealt);
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
            if (currentEnemy.health <= 0)
            {
                Console.ReadKey(true);
                Console.Clear();
                Console.WriteLine("You slayed the " + currentEnemy.name);
                

                currentEnemyIndex++;

                if (TryEndGame())
                {
                    return;
                }

                currentEnemy = enemies[currentEnemyIndex];
            }
        } 
        
        void ResetCurrentEnemy()
        {
            currentEnemyIndex = 0;

            currentEnemy = enemies[currentEnemyIndex];
        }

        bool TryEndGame()
        {
            bool endGame = currentEnemyIndex >= enemies.Length;

            if (endGame)
            {
                currentScene = 2;
            }

            return endGame;
        }
    }
}
