using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace SnakeGame
{
    class Program
    {
        static void PrintScore(int currentScore, int consoleWidthLimit, int consoleHeightLimit)
        {
            Console.ForegroundColor = ConsoleColor.Red;

            // Write "Game Over" to the screen with red color
            Console.SetCursorPosition((consoleWidthLimit / 2) - 5, (consoleHeightLimit / 2) - 5);
            Console.Write("Game Over");

            // Write the score and display to the screen
            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition((consoleWidthLimit / 2) - 6, (consoleHeightLimit / 2) - 4);
            Console.Write("The score: " + currentScore);

            // Write text telling the user to press "ENTER" to exit the program
            Console.SetCursorPosition((consoleWidthLimit / 2) - 12, (consoleHeightLimit / 2) - 2);
            Console.Write("Press ENTER key to exit...");
            Console.ReadKey();

            // Reading user input regarding pressed button
            ConsoleKeyInfo consoleKey = Console.ReadKey();

            // If it's ENTER key then exit the program 
            if (consoleKey.Key == ConsoleKey.Enter)
            {
                Environment.Exit(0);
            }
        }
        
        // This method is for printing the help page
        static void PrintHelp()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("=====HELP PAGE=======\n");

            Console.OutputEncoding = System.Text.Encoding.Unicode;
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("HOW TO PLAY");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("1. Move up   \t: \u2191");
            Console.WriteLine("2. Move right\t: \u2192");
            Console.WriteLine("2. Move down\t: \u2193");
            Console.WriteLine("2. Move left\t: \u2190");

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("\nGAME RULES");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("1. The snake has lives if it hits one of the obstacles then decrease the snake's lives by 1");
            Console.WriteLine("2. Every time the snake eat the food, the score will increase by 10");
            Console.WriteLine("3. Game over if the snake's lives are 0");
            Console.WriteLine("4. As the score increases, The difficulty level increased as well");
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write("\nBack to Main Menu? (enter 'Y' to proceed): ");
            
            bool isActive = true;
            while (isActive)
            {
                string input = Console.ReadLine();
                switch (input)
                {
                    case "Y":
                    case "y":
                        isActive = false;
                        MainMenu();
                        break;
                    default:
                        Console.WriteLine("Invalid Input!");
                        Thread.Sleep(500);
                        isActive = false;
                        PrintHelp();
                        break;
                }
            }
        }

        // print the scoreboard to the console
        static void PrintScoreboard()
        {
            string path = Directory.GetCurrentDirectory();

            Console.Clear();
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine("====Scoreboard====");
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("List of Top 5 Players");
            Console.ForegroundColor = ConsoleColor.Cyan;

            string file = path + "\\Scoreboard.txt";
            if (File.Exists(file))
            {
                string str = File.ReadAllText(file);
                Console.WriteLine(str);
            }
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write("\nBack to Main Menu? (enter 'Y' to proceed): ");
            
            bool isActive = true;
            while (isActive)
            {
                string input = Console.ReadLine();
                switch (input)
                {
                    case "Y":
                    case "y":
                        isActive = false;
                        MainMenu();
                        break;
                    default:
                        Console.WriteLine("Invalid Input!");
                        Thread.Sleep(500);
                        isActive = false;
                        PrintScoreboard();
                        
                        break;
                }
            }
        }

        // add new score to the scoreboard if the player beats one of the top 5 players who has the lowest score
        static void AddNewScore(int newScore)
        {
            // open the ScoreData file
            string path = Directory.GetCurrentDirectory();
            string scoreDataFile = path + "\\ScoreData.txt";

            List<string> lines = File.ReadAllLines(scoreDataFile).ToList(); // read the data line by line and store to it into the list

            List<string> names = new List<string>(); // new list to store names of the top 5 players
            List<int> scores = new List<int>(); // new list to store scores of the top 5 players

            // go through the lines list line by line and store the name & score at each line to the names & scores lists
            foreach (string line in lines)
            {
                var tmp = line.Split(',');
                names.Add(tmp[0]);
                scores.Add(Convert.ToInt32(tmp[1]));
            }


            string scoreboardFile = path + "\\Scoreboard.txt"; // open the Scoreboard file

            // store the list of the top 5 players (the names & scores) which will be used to fill the Scoreboard file
            string[] textLines = { names[0] + " = " + scores[0],
                                   names[1] + " = " + scores[1],
                                   names[2] + " = " + scores[2],
                                   names[3] + " = " + scores[3],
                                   names[4] + " = " + scores[4]};

            // check if the score of current player beats one of the top 5 players
            if (scores.Min() < newScore)
            {
                Console.Write("You reaches the top 5 highscores! Please enter your name: ");
                string name = Console.ReadLine();

                // go through the scores of the top players to find the score which will be changed
                for (int i = 0; i < scores.Count; i++)
                {
                    // if the score is already found, the name & score of the previous player will be changed with the new ones 
                    // and the Scoreboard & ScoreData files will be updated as well
                    if (scores.Min() == scores[i])
                    {
                        names[i] = name;
                        scores[i] = newScore;
                        textLines[i] = name + " = " + newScore;
                        lines[i] = name + ',' + Convert.ToString(newScore);
                        break;
                    }
                }

                Console.Clear();
                Console.Write("Congrats! You have been added into the top 5 players list. You can check it on the Scoreboard menu");

                File.WriteAllLines(scoreDataFile, lines); // update the ScoreData file
                File.WriteAllLines(scoreboardFile, textLines); // update the Scoreboard file
            }

        }

        // Control the option selected by the user to preceed the game
        static void MainMenu()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("===Welcome to The Snake Game===");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("1. Start");
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine("2. Scoreboard");
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("3. Help");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("4. Exit");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write("Select your option (input 1,2,3 or 4): ");           
            bool isActive = true;
            while (isActive)
            {
                string input = Console.ReadLine();
                switch (input)
                {
                    // play the game
                    case "1":
                        isActive = false;
                        break;
                    // see the scoreboard
                    case "2":
                        isActive = false;
                        PrintScoreboard();
                        break;
                    // see the help page
                    case "3":
                        isActive = false;
                        PrintHelp();
                        break;
                    // exit the game
                    case "4":
                        Environment.Exit(0);
                        break;
                    // wrong option selected
                    default:
                        Console.WriteLine("Invalid input!");
                        Thread.Sleep(500);
                        isActive = false;
                        MainMenu();
                        break;
                }
                
            }

        }

        /// <summary>
        /// A function to create a new obstacle
        /// </summary>
        /// <param name="listOfObstacles">The 2D list for storing the obstacles' position</param>
        /// <param name="consoleWidthLimit">The value of console width limit</param>
        /// <param name="consoleHeightLimit">The value of console height limit</param>
        static void CreateNewObstacle(List<List<int>> listOfObstacles, int consoleWidthLimit, int consoleHeightLimit)
        {
            // The obstacle icon in the game
            string obstacle = "||";

            // The new obstacle which we are going to create
            List<int> newObstacle = new List<int>();

            Random rnd = new Random();

            // Generating random even values
            newObstacle.Add(rnd.Next(5 / 2, (consoleWidthLimit - 10) / 2) * 2);
            newObstacle.Add(rnd.Next(5 / 2, (consoleHeightLimit) / 2) * 2);

            Console.ForegroundColor = ConsoleColor.Red;// Obstacle spawn color

            Console.SetCursorPosition(newObstacle[0], newObstacle[1]);
            Console.Write(obstacle);

            // Add it to the 2D list
            listOfObstacles.Add(newObstacle);
        }

        static void Main(string[] args)
        {
            // run the program, proceed to the Main Menu
            MainMenu();

            // display this char on the console during the game
            char ch = '*';
            int snakeLives = 3; //Snake's Lives
            int snekLength = 3; //Initial Snake Length
            bool gameLive = true; // As long as the value of this variable is true then the game will keep running 
            ConsoleKeyInfo consoleKey; // holds whatever key is pressed

            // location info & display
            int x = 0, y = 2; // y is 2 to allow the top row for directions & space
            int dx = 2, dy = 0;
            int consoleWidthLimit = Console.WindowWidth; // 120
            int consoleHeightLimit = Console.WindowHeight; // 30

            // clear to color
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Clear();

            // delay to slow down the character movement so you can see it
            int delayInMillisecs = 70;

            // whether to keep trails
            bool trail = false;

            // The food in the game
            string food = "F";

            // The current score based on the food eaten
            int currentScore = 0;

            // This variable will be used to change the value of the timeLimitCounter in the run time hence can set the difficulty level
            int limitCounterDeterminer = 15;

            int timeLimitShown = 10; // To store the time limit of the food shown on the Console
            int timeLimitCounter = limitCounterDeterminer; // To store the limit counter of the time limit shown on the Console

            
            // Generating random number for the amount of obstacles (create a number between 1 and 3)
            Random rnd = new Random();


            // This is a list containing a list hence it is like a 2D dynamic List container
            // This is for storing the x and y position of the obstacles 
            List<List<int>> obstaclesPos = new List<List<int>>();

            // This boolean value will determine whether the program should increase the difficulty level
            bool increaseDifficult = false;

            int maxObstaclesNumber = 4;

            // Adding the first obstacle to the game
            CreateNewObstacle(obstaclesPos, consoleWidthLimit, consoleHeightLimit);
               

            // Generates food on random location
            int foodX = rnd.Next(5/2, (consoleWidthLimit - 5)/2)*2;
            int foodY = rnd.Next(5/2, (consoleHeightLimit - 1)/2)*2;
            Console.ForegroundColor = ConsoleColor.DarkYellow; // Food Color spawn/after eat
            Console.SetCursorPosition(foodX, foodY);
            
            // Checks if food location overlaps with obstacle
            for (int i = 0; i < obstaclesPos.Count; i++)
            {
                if ( (foodX == obstaclesPos[i][0]) && (foodY == obstaclesPos[i][1]))
                {
                    foodX = rnd.Next(5 / 2, (consoleWidthLimit - 5)/ 2) * 2;
                    foodY = rnd.Next(5 / 2, (consoleHeightLimit - 1) / 2) * 2;
                    Console.SetCursorPosition(foodX, foodY);
                }
            }
            Console.Write(food);

            // Ensures Unicode Characters can be printed properly
            Console.OutputEncoding = System.Text.Encoding.Unicode;
            // The direction of the snake movement
            string direction = "right"; // "right" here means the initial direction is to the right
             do // until escape
            {
                // print directions at top, then restore position
                // save then restore current color
                ConsoleColor cc = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.DarkYellow;// Instructions Color
                Console.SetCursorPosition(0, 0);
                Console.Write("Arrows move up/down/right/left. Press 'esc' quit.");
                Console.ForegroundColor = ConsoleColor.Red; // Life Color

                //Checks for remaining Snake Lives and prints remaining lives into heart shapes
                switch (snakeLives)
                {
                    case 3:
                        Console.SetCursorPosition(60, 0);
                        Console.Write("Lives: \u2665 \u2665 \u2665");
                        break;
                    case 2:
                        Console.SetCursorPosition(60, 0);
                        Console.Write("                    ");// Clears area for new changes to snake Lives

                        Console.SetCursorPosition(60, 0);
                        Console.Write("Lives: \u2665 \u2665");
                        break;
                    case 1:
                        Console.SetCursorPosition(60, 0);
                        Console.Write("                    ");// Clears area for new changes to snake Lives

                        Console.SetCursorPosition(60, 0);
                        Console.Write("Lives: \u2665");
                        break;
                }
                
                Console.ForegroundColor = ConsoleColor.Green; // Score Color
                Console.SetCursorPosition(90, 0);
                Console.Write("Score : " + currentScore);

                Console.SetCursorPosition(0, 1);
                Console.ForegroundColor = ConsoleColor.Cyan; // Time Limit Color
                if (timeLimitShown < 10)
                {
                    Console.SetCursorPosition(0, 1);
                    Console.Write("                    ");

                    Console.SetCursorPosition(0, 1);
                    Console.WriteLine("Food Time Limit: " + timeLimitShown.ToString().Trim());
                }
                else
                {
                    Console.WriteLine("Food Time Limit: " + timeLimitShown);
                }
                Console.SetCursorPosition(x, y);


                Console.ForegroundColor = cc;

                // This section is for increasing the difficulty level of the game
                // Such as increased the number of the obstacles until it reachs the max number of the obstacles
                // Next is to shorten the food respawn time interval
                if (increaseDifficult)
                {
                    // Checking whether the number of obstacles in the list is smaller than the maximum amount
                    if (obstaclesPos.Count < maxObstaclesNumber)
                    {
                        // Create new obstacle
                        CreateNewObstacle(obstaclesPos, consoleWidthLimit, consoleHeightLimit);
                    }
                    // Checking whether the determiner's value is still more than 5 thus reduce it by 1
                    if (limitCounterDeterminer > 5)
                    {
                        limitCounterDeterminer--;
                    }
                    
                    increaseDifficult = false;
                }

                // find the current position in the console grid & erase the character there if don't want to see the trail
                if (trail == false)
                {
                    for (int i = 0; i < snekLength; i++)    // Removes trail for each increased snake length
                    {
                        // Remove every snake body trail according to the snake direction 
                        switch (direction)
                        {
                            case "up":
                                // Checking to make sure if y + i is not more than the console height limit before clearing the trail
                                // Hence not triggering the ArgumentOutOfRangeException
                                if ((y + i) < consoleHeightLimit)
                                {
                                    // clearing the snake body trail when it's going up
                                    Console.SetCursorPosition(x, y + i);
                                    Console.Write(' ');
                                }
                                break;
                            case "right":
                                // Checking to make sure if x - i is not less than 0 before clearing the trail
                                // Hence not triggering the ArgumentOutOfRangeException
                                if ((x - i) > 0)
                                {
                                    // clearing the snake body trail when it's going right
                                    Console.SetCursorPosition(x - i, y);
                                    Console.Write(' ');
                                }
                                break;
                            case "down":
                                // Checking to make sure if y - i is not less than 0 before clearing the trail
                                // Hence not triggering the ArgumentOutOfRangeException
                                if ((y - i) > 0)
                                {
                                    // clearing the snake body trail when it's going down
                                    Console.SetCursorPosition(x, y - i);
                                    Console.Write(' ');
                                }
                                break;
                            case "left":
                                // Checking to make sure if x + i is not more than the console width limit before clearing the trail
                                // Hence not triggering the ArgumentOutOfRangeException
                                if ((x + i) < consoleWidthLimit)
                                {
                                    // clearing the snake body trail when it's going left
                                    Console.SetCursorPosition(x + i, y);
                                    Console.Write(' ');
                                }
                                break;
                        }
                    }
                }

                // see if a key has been pressed
                if (Console.KeyAvailable)
                {
                    // get key and use it to set options
                    consoleKey = Console.ReadKey(true);
                    switch (consoleKey.Key)
                    {
                        case ConsoleKey.UpArrow: //UP
                            dx = 0;
                            dy = -1;
                            direction = "up"; 
                            break;
                        case ConsoleKey.DownArrow: // DOWN
                            dx = 0;
                            dy = 1;
                            direction = "down"; 
                            break;
                        case ConsoleKey.LeftArrow: //LEFT
                            dx = -2; // Changed to -2 so that the snake movement speed looks similar when moving vertically and horizontally
                            dy = 0;
                            direction = "left";
                            break;
                        case ConsoleKey.RightArrow: //RIGHT
                            dx = 2; // Changed to 2 so that the snake movement speed looks similar when moving vertically and horizontally
                            dy = 0;
                            direction = "right";
                            break;
                        case ConsoleKey.Escape: //END
                            gameLive = false;
                            break;
                    }
                }


                // calculate the new position
                // note x set to 0 because we use the whole width, but y set to 2 because we use top row for instructions
                x += dx;
                if (x >= consoleWidthLimit)
                {
                    x = snekLength;
                    // Since the value of dx is even value 
                    // hence if the x is odd, we need to add by 1 so it become even value
                    // Add by 1 because the x value start at the left side of the screen
                    // Convert it to an even value because the food and obstacle positions are in even value
                    if ((x % 2) == 1)
                    {
                        x += 1;
                    }
                }
                    

                if (x < 0)
                {
                    x = consoleWidthLimit - snekLength;
                    // Since the value of dx is even value 
                    // hence if the x is odd, we need to subtract by 1 so it become even value
                    // Subtract by 1 because the x value start at the right side of the screen
                    // Convert it to an even value because the food and obstacle positions are in even value
                    if ((x % 2) == 1)
                    {
                        x -= 1;
                    }
                }
                    

                y += dy;
                if (y >= consoleHeightLimit)
                {
                    y = 2 + snekLength; // 2 due to top spaces used for directions
                }
                if (y < 2)
                {
                    y = consoleHeightLimit;
                }

                // write the character in the new position
                // Console.SetCursorPosition(x, y);
                
                for (int i = 0; i < snekLength; i++)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan; //Snake Color
                    // Printing the snake body according to the direction of the snake movement
                    switch (direction)
                    {
                        case "up":
                            // Checking to make sure if y + i is not more than console height limit before printing the snake's body
                            // Hence not triggering the ArgumentOutOfRangeException
                            if ((y + i) < consoleHeightLimit)
                            {
                                // Printing the snake body when it's going up
                                Console.SetCursorPosition(x, y + i);
                                Console.Write(ch);
                            }
                            break;

                        case "right":
                            // Checking to make sure if x - i is not less than 0 before printing the snake's body
                            // Hence not triggering the ArgumentOutOfRangeException
                            if ((x - i) > 0)
                            {
                                // Printing the snake body when it's going right
                                Console.SetCursorPosition(x - i, y);
                                Console.Write(ch);
                            }
                            break;

                        case "down":
                            // Checking to make sure if y - i is not less than 0 before printing the snake's body
                            // Hence not triggering the ArgumentOutOfRangeException
                            if ((y - i) > 0)
                            {
                                // Printing the snake body when it's going down
                                Console.SetCursorPosition(x, y - i);
                                Console.Write(ch);
                            }
                            break;

                        case "left":
                            // Checking to make sure if x + i is not more than console width limit before printing the snake's body
                            // Hence not triggering the ArgumentOutOfRangeException
                            if ((x + i) < consoleWidthLimit)
                            {
                                // Printing the snake body when it's going left
                                Console.SetCursorPosition(x + i, y);
                                Console.Write(ch);
                            }
                            break;
                    }                   
                }

                // The limit counter is decreased by 1 every loop
                timeLimitCounter -= 1;
                
                // Decrease the time limit shown value on the Console by 1 and return back timeLimitCounter value to 5 if timeLimitCounter value is 0
                if(timeLimitCounter == 0)
                {
                    timeLimitShown -= 1;
                    timeLimitCounter = limitCounterDeterminer;
                }

                // Change the food location if timeLimitShown is less than 0 and return timeLimitShown & timeLimitCounter value back to initial value
                Console.ForegroundColor = ConsoleColor.DarkYellow; //Food Color after timer ends
                if (timeLimitShown < 0)
                {
                    timeLimitShown = 10;
                    timeLimitCounter = limitCounterDeterminer;

                    Console.SetCursorPosition(foodX, foodY);
                    Console.Write(' ');

                    foodX = rnd.Next(5 / 2, (consoleWidthLimit - 5) / 2) * 2;
                    foodY = rnd.Next(5 / 2, (consoleHeightLimit - 1) / 2) * 2;
                    
                    Console.SetCursorPosition(foodX, foodY);

                    for (int i = 0; i < obstaclesPos.Count; i++)
                    {
                        
                        if ((foodX == obstaclesPos[i][0]) && (foodY == obstaclesPos[i][1]))
                        {
                            foodX = rnd.Next(5 / 2, (consoleWidthLimit - 5) / 2) * 2;
                            foodY = rnd.Next(5 / 2, (consoleHeightLimit - 1) / 2) * 2;
                            Console.SetCursorPosition(foodX, foodY);
                        }
                    }
                    
                    Console.Write(food);
                }

                // Increase the current score and spawn a new food location if the previous food has been eaten
                // Also, return timeLimitShown & timeLimitCounter value back to initial value
                if (x == foodX && y == foodY)
                {
                    currentScore += 10;
                    snekLength += 1; //Increment Snake Length after eating food by 1
                    timeLimitShown = 10;
                    timeLimitCounter = limitCounterDeterminer;

                    // If the score is only able be modulo with 50 then set the increaseDifficult's variable to true 
                    if ((currentScore % 50 == 0) && (currentScore != 0))
                    {
                        increaseDifficult = true;
                    }

                    foodX = rnd.Next(5 / 2, (consoleWidthLimit - 5) / 2) * 2;
                    foodY = rnd.Next(5 / 2, (consoleHeightLimit - 1) / 2) * 2;

                    Console.SetCursorPosition(foodX, foodY);

                    for (int i = 0; i < obstaclesPos.Count; i++)
                    {
                        if ((foodX == obstaclesPos[i][0]) && (foodY == obstaclesPos[i][1]))
                        {
                            foodX = rnd.Next(5 / 2, (consoleWidthLimit - 5) / 2) * 2;
                            foodY = rnd.Next(5 / 2, (consoleHeightLimit - 1) / 2) * 2;
                            Console.SetCursorPosition(foodX, foodY);
                        }
                    }
                    Console.Write(food);
                }

                // If the snake hit one of the obstacles then game over or end the game
                for (int i = 0; i < obstaclesPos.Count; i++)
                {
                    // This loop for looping every part of snake body 
                    // and comparing it with obstacles' x and y position 
                    for (int j = 0; j < snekLength; j++)
                    {
                        switch (direction)
                        {
                            case "up":
                                // Comparing every part of snake body with the obstacles' x and y position 
                                // for going up direction
                                if (x == obstaclesPos[i][0] && (y + j) == obstaclesPos[i][1])
                                {
                                    snakeLives--;

                                    // If the snakeLives is equal to 0 then end the game
                                    if (snakeLives == 0)
                                    {
                                        gameLive = false;
                                    }
                                    else
                                    {
                                        // Delete the collided obstacle at the current x and y position
                                        obstaclesPos.RemoveAt(i);

                                        // Create a new obstacle to replace the collided obstacle 
                                        CreateNewObstacle(obstaclesPos, consoleWidthLimit, consoleHeightLimit);
                                    }
                                    break;
                                }
                                break;

                            case "right":
                                // Comparing every part of snake body with the obstacles' x and y position 
                                // for going right direction
                                if ((x - j) == obstaclesPos[i][0] && y == obstaclesPos[i][1])
                                {
                                    snakeLives--;

                                    // If the snakeLives is equal to 0 then end the game
                                    if (snakeLives == 0)
                                    {
                                        gameLive = false;
                                    }
                                    else
                                    {
                                        // Delete the collided obstacle at the current x and y position
                                        obstaclesPos.RemoveAt(i);

                                        // Create a new obstacle to replace the collided obstacle 
                                        CreateNewObstacle(obstaclesPos, consoleWidthLimit, consoleHeightLimit);
                                    }
                                    break;
                                }
                                break;

                            case "down":
                                // Comparing every part of snake body with the obstacles' x and y position 
                                // for going down direction
                                if (x == obstaclesPos[i][0] && (y - j) == obstaclesPos[i][1])
                                {
                                    snakeLives--;

                                    // If the snakeLives is equal to 0 then end the game
                                    if (snakeLives == 0)
                                    {
                                        gameLive = false;
                                    }
                                    else
                                    {
                                        // Delete the collided obstacle at the current x and y position
                                        obstaclesPos.RemoveAt(i);

                                        // Create a new obstacle to replace the collided obstacle 
                                        CreateNewObstacle(obstaclesPos, consoleWidthLimit, consoleHeightLimit);
                                    }
                                    break;

                                }
                                break;

                            case "left":
                                // Comparing every part of snake body with the obstacles' x and y position 
                                // for going left direction
                                if ((x + j) == obstaclesPos[i][0] && y == obstaclesPos[i][1])
                                {
                                    snakeLives--;

                                    // If the snakeLives is equal to 0 then end the game
                                    if (snakeLives == 0)
                                    {
                                        gameLive = false;
                                    }
                                    else
                                    {
                                        // Delete the collided obstacle at the current x and y position
                                        obstaclesPos.RemoveAt(i);

                                        // Create a new obstacle to replace the collided obstacle 
                                        CreateNewObstacle(obstaclesPos, consoleWidthLimit, consoleHeightLimit);
                                    }
                                    break;
                                }
                                break;
                        }
                    }
                }


                // pause to allow eyeballs to keep up
                System.Threading.Thread.Sleep(delayInMillisecs);

            } while (gameLive);

            Console.Clear(); // Clear the screen

            AddNewScore(currentScore); // Add new highscore to the Scoreboard

            // Print out the score and Game Over text to the screen
            PrintScore(currentScore, consoleWidthLimit, consoleHeightLimit);
        }
    }
    
}
