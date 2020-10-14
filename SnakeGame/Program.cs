using System;
using System.Collections.Generic;

using System.Linq;
using System.Reflection.PortableExecutable;

namespace SnakeGame
{
    class Program
    {
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

            int timeLimitShown = 10; // To store the time limit of the food shown on the Console
            int timeLimitCounter = 5; // To store the limit counter of the time limit shown on the Console

            
            // Generating random number for the amount of obstacles (create a number between 1 and 3)
            Random rnd = new Random();
            int numOfObstacles = rnd.Next(1, 4);


            // 2D Array to stores the x and y position of the obstacles
            int[,] obstaclePositions = new int[3, 2];

            
            // Generating the obstacles with random positions 
            for (int i = 0; i < numOfObstacles; i++)
            {
                // Generating random even values
                obstaclePositions[i, 0] = rnd.Next(5/2, 110/2)*2;
                obstaclePositions[i, 1] = rnd.Next(5/2, 30/2)*2;

                Console.SetCursorPosition(obstaclePositions[i, 0], obstaclePositions[i, 1]);
                Console.Write(obstacle);
               
            }

            //Generates food on random location
            int foodX = rnd.Next(5/2, 115/2)*2;
            int foodY = rnd.Next(5/2, 29/2)*2;

            Console.SetCursorPosition(foodX, foodY);
            
            //Checks if food location overlaps with obstacle
            if (foodX == obstaclePositions[0, 0] && foodY == obstaclePositions[0, 1] 
                || foodX == obstaclePositions[1, 0] && foodY == obstaclePositions[1, 1] 
                || foodX == obstaclePositions[2, 0] && foodY == obstaclePositions[2, 1])
            {
                foodX = rnd.Next(5/2, 115/2)*2;
                foodY = rnd.Next(5/2, 115/2)*2;
                Console.SetCursorPosition(foodX, foodY);
            }
            Console.Write(food);


            // The direction of the snake movement
            string direction = "";
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

                // find the current position in the console grid & erase the character there if don't want to see the trail
                //Console.SetCursorPosition(x, y);
                if (trail == false)
                {
                    for (int i = 0; i < snekLength; i++)    //Removes trail for each increased snake length
                    {
                        // Remove every snake body trail according to the snake direction 
                        if (direction == "up")
                        {
                            Console.SetCursorPosition(x, y + i);
                        }
                        else if (direction == "right")
                        {
                            Console.SetCursorPosition(x - i, y);
                        }
                        else if (direction == "down")
                        {
                            Console.SetCursorPosition(x, y - i);
                        }
                        else if (direction == "left")
                        {
                            Console.SetCursorPosition(x + i, y);
                        }
                        Console.Write(' ');
                    }

                    if (x == obstaclePositions[0, 0] && y == obstaclePositions[0, 1]
                        || x == obstaclePositions[1, 0] && y == obstaclePositions[1, 1]
                        || x == obstaclePositions[2, 0] && y == obstaclePositions[2, 1])
                    {
                        Console.Write("  ");
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
                            Console.ForegroundColor = ConsoleColor.Red;
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
                // note x set to 0 because we use the whole width, but y set to 1 because we use top row for instructions
                x += dx;
                if (x >= consoleWidthLimit)
                    x = snekLength;
                if (x < 0)
                    x = consoleWidthLimit - snekLength;

                y += dy;
                if (y >= consoleHeightLimit)
                    y = 2 + snekLength; // 2 due to top spaces used for directions
                if (y < 2)
                    y = consoleHeightLimit;


                // write the character in the new position
                // Console.SetCursorPosition(x, y);

                for (int i = 0; i < snekLength; i++)
                {
                    // Printing the snake body according to the direction of the snake movement
                    if (direction == "up")
                    {
                        Console.SetCursorPosition(x, y + i);
                    }
                    else if (direction == "right")
                    {
                        Console.SetCursorPosition(x - i, y);
                    }
                    else if (direction == "down")
                    {
                        Console.SetCursorPosition(x, y - i);
                    }
                    else if (direction == "left")
                    {
                        Console.SetCursorPosition(x + i, y);
                    }
                    Console.Write(ch);

                }

                // The limit counter is decreased by 1 every loop
                timeLimitCounter -= 1;
                
                // Decrease the time limit shown value on the Console by 1 and return back timeLimitCounter value to 5 if timeLimitCounter value is 0
                if(timeLimitCounter == 0)
                {
                    timeLimitShown -= 1;
                    timeLimitCounter = 5;
                }

                // Change the food location if timeLimitShown is less than 0 and return timeLimitShown & timeLimitCounter value back to initial value
                if (timeLimitShown < 0)
                {
                    timeLimitShown = 10;
                    timeLimitCounter = 5;

                    Console.SetCursorPosition(foodX, foodY);
                    Console.Write(' ');

                    foodX = rnd.Next(5 / 2, 115 / 2) * 2;
                    foodY = rnd.Next(5 / 2, 29 / 2) * 2;

                    Console.SetCursorPosition(foodX, foodY);


                    if (foodX == obstaclePositions[0, 0] && foodY == obstaclePositions[0, 1]
                        || foodX == obstaclePositions[1, 0] && foodY == obstaclePositions[1, 1]
                        || foodX == obstaclePositions[2, 0] && foodY == obstaclePositions[2, 1])
                    {
                        foodX = rnd.Next(5 / 2, 115 / 2) * 2;
                        foodY = rnd.Next(5 / 2, 115 / 2) * 2;
                        Console.SetCursorPosition(foodX, foodY);
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
                    timeLimitCounter = 5;

                    foodX = rnd.Next(5 / 2, 115 / 2) * 2;
                    foodY = rnd.Next(5 / 2, 29 / 2) * 2;

                    Console.SetCursorPosition(foodX, foodY);


                    if (foodX == obstaclePositions[0, 0] && foodY == obstaclePositions[0, 1] 
                        || foodX == obstaclePositions[1, 0] && foodY == obstaclePositions[1, 1] 
                        || foodX == obstaclePositions[2, 0] && foodY == obstaclePositions[2, 1])
                    {
                        foodX = rnd.Next(5 / 2, 115 / 2) * 2;
                        foodY = rnd.Next(5 / 2, 115 / 2) * 2;
                        Console.SetCursorPosition(foodX, foodY);
                    }
                    Console.Write(food);
                }

                // If the snake hit one of the obstacles then game over or end the game
                for (int i = 0; i < obstaclePositions.GetLength(0); i++)
                {
                    if (x == obstaclePositions[i, 0] && y == obstaclePositions[i, 1])
                    {
                        gameLive = false;
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
