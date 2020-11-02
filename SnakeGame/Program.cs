using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace SnakeGame
{
    class Program
    {
        // This method is for printing the help page
        static void PrintHelp()
        {
            Console.Clear();
            Console.WriteLine("=====HELP PAGE=======\n");

            Console.OutputEncoding = System.Text.Encoding.Unicode;
            Console.WriteLine("HOW TO PLAY");
            Console.WriteLine("1. Move up   \t: \u2191");
            Console.WriteLine("2. Move right\t: \u2192");
            Console.WriteLine("2. Move down\t: \u2193");
            Console.WriteLine("2. Move left\t: \u2190");

            Console.WriteLine("\nGAME RULES");
            Console.WriteLine("1. The snake has lives if it hits one of the obstacles then decrease the snake's lives by 1");
            Console.WriteLine("2. Every time the snake eat the food, the score will increase by 10");
            Console.WriteLine("3. Game over if the snake's lives are 0");
            Console.WriteLine("4. As the score increases, The difficulty level increased as well");
        }

        static void Main(string[] args)
        {
            // start game
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();

            // display this char on the console during the game
            char ch = '*';

            int snekLength = 3; //Initial Snake Length
            bool gameLive = true; // As long as the value of this variable is true then the game will keep running 
            ConsoleKeyInfo consoleKey; // holds whatever key is pressed

            // location info & display
            int x = 0, y = 2; // y is 2 to allow the top row for directions & space
            int dx = 2, dy = 0;
            int consoleWidthLimit = Console.WindowWidth; // 120
            int consoleHeightLimit = Console.WindowHeight; // 30

            // clear to color
            Console.BackgroundColor = ConsoleColor.DarkGray;
            Console.Clear();

            // delay to slow down the character movement so you can see it
            int delayInMillisecs = 70;

            // whether to keep trails
            bool trail = false;

            // The obstacle in the game
            string obstacle = "||";

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

            Console.ForegroundColor = ConsoleColor.White;

            // Adding the first obstacle to the game
            List<int> firstObstacle = new List<int>();

            // Generating random even values
            firstObstacle.Add(rnd.Next(5/2, (consoleWidthLimit - 10)/2)*2);
            firstObstacle.Add(rnd.Next(5/2, (consoleHeightLimit)/2)*2);

            Console.SetCursorPosition(firstObstacle[0], firstObstacle[1]);
            Console.Write(obstacle);

            // Add it to the 2D list
            obstaclesPos.Add(firstObstacle);
               
            

            //Generates food on random location
            int foodX = rnd.Next(5/2, (consoleWidthLimit - 5)/2)*2;
            int foodY = rnd.Next(5/2, (consoleHeightLimit - 1)/2)*2;

            Console.SetCursorPosition(foodX, foodY);
            
            //Checks if food location overlaps with obstacle
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

            
            // The direction of the snake movement
            string direction = "right"; // "right" here means the initial direction is to the right
             do // until escape
            {
                // print directions at top, then restore position
                // save then restore current color
                ConsoleColor cc = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.SetCursorPosition(0, 0);
                Console.Write("Arrows move up/down/right/left. Press 'esc' quit.");
                Console.WriteLine("                                     Score : " + currentScore);
                if(timeLimitShown < 10)
                {
                    Console.Write(new string(' ', Console.WindowWidth));
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
                        List<int> newObstacle = new List<int>();
                        newObstacle.Add(rnd.Next(5 / 2, (consoleWidthLimit - 10) / 2) * 2);
                        newObstacle.Add(rnd.Next(5 / 2, (consoleHeightLimit) / 2) * 2);

                        Console.ForegroundColor = ConsoleColor.White;
                        Console.SetCursorPosition(newObstacle[0], newObstacle[1]);
                        Console.Write(obstacle);

                        // Add it to the 2D list
                        obstaclesPos.Add(newObstacle);
                    }
                    // Checking whether the determiner's value is still more than 5 thus reduce it by 1
                    else if (limitCounterDeterminer > 5)
                    {
                        limitCounterDeterminer--;
                    }
                    
                    increaseDifficult = false;
                }

                // find the current position in the console grid & erase the character there if don't want to see the trail
                if (trail == false)
                {
                    for (int i = 0; i < snekLength; i++)    //Removes trail for each increased snake length
                    {
                        // Remove every snake body trail according to the snake direction 
                        if (direction == "up")
                        {
                            // Checking to make sure if y + i is not more than the console height limit before clearing the trail
                            // Hence not triggering the ArgumentOutOfRangeException
                            if ((y + i) < consoleHeightLimit)
                            {
                                // clearing the snake body trail when it's going up
                                Console.SetCursorPosition(x, y + i); 
                                Console.Write(' ');
                            }
                        }
                        else if (direction == "right")
                        {
                            // Checking to make sure if x - i is not less than 0 before clearing the trail
                            // Hence not triggering the ArgumentOutOfRangeException
                            if ((x - i) > 0)
                            {
                                // clearing the snake body trail when it's going right
                                Console.SetCursorPosition(x - i, y); 
                                Console.Write(' ');
                            }
                        }
                        else if (direction == "down")
                        {
                            // Checking to make sure if y - i is not less than 0 before clearing the trail
                            // Hence not triggering the ArgumentOutOfRangeException
                            if ((y - i) > 0)
                            {
                                // clearing the snake body trail when it's going down
                                Console.SetCursorPosition(x, y - i); 
                                Console.Write(' ');
                            }
                        }
                        else if (direction == "left")
                        {
                            // Checking to make sure if x + i is not more than the console width limit before clearing the trail
                            // Hence not triggering the ArgumentOutOfRangeException
                            if ((x + i) < consoleWidthLimit)
                            {
                                // clearing the snake body trail when it's going left
                                Console.SetCursorPosition(x + i, y);
                                Console.Write(' ');
                            }
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
                            Console.ForegroundColor = ConsoleColor.DarkBlue;
                            direction = "up"; 
                            break;
                        case ConsoleKey.DownArrow: // DOWN
                            dx = 0;
                            dy = 1;
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            direction = "down"; 
                            break;
                        case ConsoleKey.LeftArrow: //LEFT
                            dx = -2; // Changed to -2 so that the snake movement speed looks similar when moving vertically and horizontally
                            dy = 0;
                            Console.ForegroundColor = ConsoleColor.Green;
                            direction = "left";
                            break;
                        case ConsoleKey.RightArrow: //RIGHT
                            dx = 2; // Changed to 2 so that the snake movement speed looks similar when moving vertically and horizontally
                            dy = 0;
                            Console.ForegroundColor = ConsoleColor.Black;
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
                    // Printing the snake body according to the direction of the snake movement
                    if (direction == "up")
                    {
                        // Checking to make sure if y + i is not more than console height limit before printing the snake's body
                        // Hence not triggering the ArgumentOutOfRangeException
                        if ((y + i) < consoleHeightLimit)
                        {
                            // Printing the snake body when it's going up
                            Console.SetCursorPosition(x, y + i);
                            Console.Write(ch);
                        }
                    }
                    else if (direction == "right")
                    {
                        // Checking to make sure if x - i is not less than 0 before printing the snake's body
                        // Hence not triggering the ArgumentOutOfRangeException
                        if ((x - i) > 0)
                        {
                            // Printing the snake body when it's going right
                            Console.SetCursorPosition(x - i, y);
                            Console.Write(ch);
                        }
                    }
                    else if (direction == "down")
                    {
                        // Checking to make sure if y - i is not less than 0 before printing the snake's body
                        // Hence not triggering the ArgumentOutOfRangeException
                        if ((y - i) > 0)
                        {
                            // Printing the snake body when it's going down
                            Console.SetCursorPosition(x, y - i);
                            Console.Write(ch);
                        }
                    }
                    else if (direction == "left")
                    {
                        // Checking to make sure if x + i is not more than console width limit before printing the snake's body
                        // Hence not triggering the ArgumentOutOfRangeException
                        if ((x + i) < consoleWidthLimit)
                        {
                            // Printing the snake body when it's going left
                            Console.SetCursorPosition(x + i, y);
                            Console.Write(ch);
                        }
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
                        if (direction == "up")
                        {
                            // Comparing every part of snake body with the obstacles' x and y position 
                            // for going up direction
                            if (x == obstaclesPos[i][0] && (y + j) == obstaclesPos[i][1])
                            {
                                gameLive = false;
                                break;
                            }
                        }
                        else if (direction == "right")
                        {
                            // Comparing every part of snake body with the obstacles' x and y position 
                            // for going right direction
                            if ((x - j) == obstaclesPos[i][0] && y == obstaclesPos[i][1])
                            {
                                gameLive = false;
                                break;
                            }
                        }
                        else if (direction == "down")
                        {
                            // Comparing every part of snake body with the obstacles' x and y position 
                            // for going down direction
                            if (x == obstaclesPos[i][0] && (y - j) == obstaclesPos[i][1])
                            {
                                gameLive = false;
                                break;
                            }
                        }
                        else if (direction == "left")
                        {
                            // Comparing every part of snake body with the obstacles' x and y position 
                            // for going left direction
                            if ((x + j) == obstaclesPos[i][0] && y == obstaclesPos[i][1])
                            {
                                gameLive = false;
                                break;
                            }
                        }
                    }
                }


                // pause to allow eyeballs to keep up
                System.Threading.Thread.Sleep(delayInMillisecs);

            } while (gameLive);

            Console.Clear(); // Clear the screen
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
            
            // Reading user input regarding button pressed
            consoleKey = Console.ReadKey();

            // If it's ENTER key then exit the program 
            if (consoleKey.Key == ConsoleKey.Enter)
            {
                Environment.Exit(0);
            }
        }
    }
    
}
