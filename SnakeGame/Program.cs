using System;
using System.Linq;

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
            bool gameLive = true;
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
            string food = "F";

            // The current score based on the food eaten
            int currentScore = 0;

            
            // Generating random number for the amount of obstacles (create a number between 1 and 3)
            Random rnd = new Random();
            int numOfObstacles = rnd.Next(1, 4);


            // 2D Array to stores the x and y position of the obstacles
            int[,] obstaclePositions = new int[3, 2];

            
            // Generating the obstacles with random positions 
            for (int i = 0; i < numOfObstacles; i++)
            {
                obstaclePositions[i, 0] = rnd.Next(5/2, 110/2)*2;
                obstaclePositions[i, 1] = rnd.Next(5/2, 30/2)*2;

                Console.SetCursorPosition(obstaclePositions[i, 0], obstaclePositions[i, 1]);
                Console.Write(obstacle);
               
            }

            //Generates food on random location
            int foodX = rnd.Next(5/2, 115/2)*2;
            int foodY = rnd.Next(5/2, 29/2)*2;

            Console.SetCursorPosition(foodX, foodY);
            

            if (foodX == obstaclePositions[0, 0] && foodY == obstaclePositions[0, 1] || foodX == obstaclePositions[1, 0] && foodY == obstaclePositions[1, 1] || foodX == obstaclePositions[2, 0] && foodY == obstaclePositions[2, 1])
            {
                foodX = rnd.Next(5/2, 115/2)*2;
                foodY = rnd.Next(5/2, 115/2)*2;
                Console.SetCursorPosition(foodX, foodY);
            }
            Console.Write(food);


            
            do // until escape
            {
                // print directions at top, then restore position
                // save then restore current color
                ConsoleColor cc = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.SetCursorPosition(0, 0);
                Console.Write("Arrows move up/down/right/left. Press 'esc' quit.");
                Console.WriteLine("                                     Score : " + currentScore);
                Console.SetCursorPosition(x, y);
                Console.ForegroundColor = cc;

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
                            break;
                        case ConsoleKey.DownArrow: // DOWN
                            dx = 0;
                            dy = 1;
                            Console.ForegroundColor = ConsoleColor.Cyan;                          
                            break;
                        case ConsoleKey.LeftArrow: //LEFT
                            dx = -2; // Changed to -2 so that the snake movement speed looks similar when moving vertically and horizontally
                            dy = 0;
                            Console.ForegroundColor = ConsoleColor.Green;                            
                            break;
                        case ConsoleKey.RightArrow: //RIGHT
                            dx = 2; // Changed to 2 so that the snake movement speed looks similar when moving vertically and horizontally
                            dy = 0;
                            Console.ForegroundColor = ConsoleColor.Black;       
                            break;
                        case ConsoleKey.Escape: //END
                            gameLive = false;
                            break;
                    }
                }

                // find the current position in the console grid & erase the character there if don't want to see the trail
                Console.SetCursorPosition(x, y);
                if (trail == false)
                {
                    Console.Write(' ');
                    if (x == obstaclePositions[0, 0] && y == obstaclePositions[0, 1] || x == obstaclePositions[1, 0] && y == obstaclePositions[1, 1] || x == obstaclePositions[2, 0] && y == obstaclePositions[2, 1])
                    {
                        Console.Write("  ");
                    }
                }
                    

                // calculate the new position
                // note x set to 0 because we use the whole width, but y set to 1 because we use top row for instructions
                x += dx;
                if (x >= consoleWidthLimit)
                    x = 0;
                if (x < 0)
                    x = consoleWidthLimit - 1;

                y += dy;
                if (y >= consoleHeightLimit)
                    y = 2; // 2 due to top spaces used for directions
                if (y < 2)
                    y = consoleHeightLimit;

                
                // write the character in the new position
                Console.SetCursorPosition(x, y);
                Console.Write(ch);

                // Increase the current score and spawn a new food location if the previous food has been eaten
                if (x == foodX && y == foodY)
                {
                    currentScore += 10;

                    foodX = rnd.Next(5 / 2, 115 / 2) * 2;
                    foodY = rnd.Next(5 / 2, 29 / 2) * 2;

                    Console.SetCursorPosition(foodX, foodY);


                    if (foodX == obstaclePositions[0, 0] && foodY == obstaclePositions[0, 1] || foodX == obstaclePositions[1, 0] && foodY == obstaclePositions[1, 1] || foodX == obstaclePositions[2, 0] && foodY == obstaclePositions[2, 1])
                    {
                        foodX = rnd.Next(5 / 2, 115 / 2) * 2;
                        foodY = rnd.Next(5 / 2, 115 / 2) * 2;
                        Console.SetCursorPosition(foodX, foodY);
                    }
                    Console.Write(food);
                }

                
                // pause to allow eyeballs to keep up
                System.Threading.Thread.Sleep(delayInMillisecs);

            } while (gameLive);
        }
    }
    
}
