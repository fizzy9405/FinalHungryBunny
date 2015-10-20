using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Media;
using System.Diagnostics;

class Game
{
    static void Main()
    {
        int gameDifficulty;
        StartScreen(out gameDifficulty); // Introduction to the Game
        int rabbitColor=int.Parse(Console.ReadLine());
        string[] mapRows;
        int mapSize;
        int mapHeight;
        string rabbitIcon;
        int rabbitX;
        int rabbitY;
        int carrotCount;
        int lives = 3;
        int yourScore;
        Stopwatch yourTime;
        long timeAndScore = new long();
        long finalScore = 0;
        Random rng = new Random();

        // Reads File:
        for (int gameLevel = 1; gameLevel <= 4; gameLevel++)//loops through the levels
        {
            bool trapIsSpawned = false;
            int positionOfTrapX = new int();
            int positionOfTrapY = new int();
            ReadFile(gameLevel, out mapRows, out mapSize, out mapHeight);
            char[,] charMap = new char[mapHeight, mapSize];
            // Console Height and Width
            Console.BufferHeight = Console.WindowHeight = mapHeight + 10;
            Console.BufferWidth = Console.WindowWidth = mapSize + 45;
            Console.OutputEncoding = System.Text.Encoding.Unicode;  // Must have this + Change font to Lucida in CMD

            // Creates Matrix:
            for (int row = 0; row < mapHeight; row++)
            {
                for (int col = 0; col < mapSize; col++)
                {
                    charMap[row, col] = mapRows[row].ElementAt(col);
                }
            }
            // Rabbit init:
            RabitInitialization(out rabbitIcon, out rabbitX, out rabbitY, out carrotCount, out yourScore, out yourTime);

            //One time events at start
            Console.Clear();
            DrawLabyrinth(mapHeight, mapSize, charMap);
            LegendSection();
            DrawScreenSeparator();
            Console.SetCursorPosition(rabbitX, rabbitY);
            RabbitColor(rabbitColor);
            Console.Write(rabbitIcon);
            Stopwatch timer = new Stopwatch();
            timer.Reset();
            Console.ForegroundColor = ConsoleColor.White;
            // Game Loop:
            while (carrotCount > 0)
            {
                if (gameDifficulty == 1)
                {
                    timeAndScore = (yourScore + (1500 - (yourTime.ElapsedMilliseconds / 1000)));
                }
                else if (gameDifficulty == 2)
                {
                    timeAndScore = (yourScore + (1500 - (yourTime.ElapsedMilliseconds / 100)));
                }
                else if (gameDifficulty == 3)
                {
                    timeAndScore = (yourScore + (1500 - (yourTime.ElapsedMilliseconds / 40)));
                }
                else if(gameDifficulty == 4)
                {
                    timeAndScore = (yourScore + (1500 - (yourTime.ElapsedMilliseconds/ 6)));
                }
                ScoreSection(carrotCount, yourTime, timeAndScore, finalScore, lives);
                MoveRabbit(mapHeight, mapSize, ref rabbitX, ref rabbitY, charMap, rabbitIcon, rabbitColor);
                GotBlueKey(mapHeight, mapSize, rabbitX, rabbitY, charMap);
                GotMagentaKey(mapHeight, mapSize, rabbitX, rabbitY, charMap);
                GotYellowKey(mapHeight, mapSize, rabbitX, rabbitY, charMap);
                EatVegetable(charMap, rabbitX, rabbitY, ref carrotCount, ref yourScore);
                int i, j;
                i = rng.Next(mapHeight);
                j = rng.Next(mapSize - 1);
                if (gameLevel != 1)
                {
                    TrapGeneration(ref trapIsSpawned, ref charMap, ref timer, i, j, ref positionOfTrapX, ref positionOfTrapY);
                    TrapRemoving(ref timer, ref trapIsSpawned, ref charMap, ref positionOfTrapX, ref positionOfTrapY, rabbitX, rabbitY, ref lives);
                }
                Thread.Sleep(30);

                Console.CursorVisible = false;
                if (lives == 0)
                {
                    Console.Clear();
                    Console.WriteLine("\n\n\n\t\t\t GAME OVER\n\n\t\tYou lost all your lives!\n\n\n");
                    Console.ReadKey();
                    return;
                }
                if (timeAndScore/* + finalScore */<= 0)
                {
                    Console.Clear();
                    Console.WriteLine("\n\n\n\t\t\t GAME OVER\n\n\tYou're too SLOW! Jump faster you fat rabbit!\n\n\n");
                    Console.ReadKey();
                    return;
                }
            } 
            finalScore += timeAndScore;
        }
        ScoreBoardOutputSaveAndLoadFile(finalScore);
        Console.ReadKey();
    }
    private static void StartScreen(out int gameDifficulty)
    {
        string title = "\n\n" +
                    " #     #                                   ######                             \n" +
                    " #     # #    # #    #  ####  #####  #   # #     # #    # #    # #    # #   # \n" +
                    " #     # #    # ##   # #    # #    #  # #  #     # #    # ##   # ##   #  # #  \n" +
                    " ####### #    # # #  # #      #    #   #   ######  #    # # #  # # #  #   #   \n" +
                    " #     # #    # #  # # #  ### #####    #   #     # #    # #  # # #  # #   #   \n" +
                    " #     # #    # #   ## #    # #   #    #   #     # #    # #   ## #   ##   #   \n" +
                    " #     #  ####  #    #  ####  #    #   #   ######   ####  #    # #    #   #   \n";
        Console.BufferHeight = Console.WindowHeight = 20;
        Console.BufferWidth = Console.WindowWidth = 80;
        Console.WriteLine(title);
        Console.WriteLine("\n\t\t\t\t(\\_/)\n\t\t\t\t(o.o)\n\t\t\t\t(___)0\n");
        Console.Write("Choose your game difficulty (1 -");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write("Easy");
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write(", 2 - ");
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.Write("Medium");
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write(", 3 - ");
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write("Hard");
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write(", 4 - ");
        Console.ForegroundColor = ConsoleColor.DarkRed;
        Console.Write("INSANE!!!");
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write(")\n");
        gameDifficulty=int.Parse(Console.ReadLine());
        if (gameDifficulty == 4)
        {
            Console.WriteLine("Are you sure? (y-yes, n-no)");
            string confirmation1=Console.ReadLine();
            if (confirmation1 == "y")
            {
                Console.WriteLine("You don't have to do this to yourself!!! Are you really sure?! (y-yes, n-no)");
                string confirmation2 = Console.ReadLine();
                if (confirmation2 != "y")
                {
                    Console.Clear();
                    Console.WriteLine("You did the right thing. Reopen the game and choose a sane difficulty.");
                    Console.ReadKey();
                    return;
                }
                if (confirmation1 == "n")
                {
                    Console.Clear();
                    Console.WriteLine("You did the right thing. Reopen the game and choose a sane difficulty.");
                    Console.ReadKey();
                    return;
                }
            }
        }
        Console.Write("Choose your bunny's color: 1 - White, 2 - ");
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write("Red");
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write(", 3 - ");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("Yellow");
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write(", 4 - ");
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write("Cyan");
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write(", 5 - ");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write("Green");
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write(", 6 - ");
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.Write("Blue");
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write(", 7 - ");
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.Write("Gray");
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write(", 8 - ");
        Console.ForegroundColor = ConsoleColor.DarkMagenta;
        Console.Write("Dark Magenta\n");
        Console.ForegroundColor = ConsoleColor.White;
    } // Prints the Game Name and the Player Preferences
    private static void ReadFile(int gameLevel, out string[] mapRows, out int mapSize, out int mapHeight)
    {
        //string map = File.ReadAllText(String.Format("level{0}.txt", gameLevel));
        mapRows = File.ReadAllLines(String.Format("level{0}.txt", gameLevel));
        mapSize = mapRows[0].Length;
        mapHeight = mapRows.Count();

    }
    private static void DrawScreenSeparator()
    {
        for (int row = 0; row < Console.WindowHeight; row++)
        {
            Console.SetCursorPosition(Console.WindowWidth / 2, row);
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("│");
        }
    } // Draws vertical lines to separate the screen in half
    static void DrawLabyrinth(int height, int width, char[,] array)
    {
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                if (array[i, j] == '1')
                    Console.Write("─");
                else if (array[i, j] == '2')
                    Console.Write("│");
                else if (array[i, j] == '3')
                    Console.Write("┌");
                else if (array[i, j] == '4')
                    Console.Write("┐");
                else if (array[i, j] == '5')
                    Console.Write("└");
                else if (array[i, j] == '6')
                    Console.Write("┘");
                else if (array[i, j] == '7')
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("▼");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else if (array[i, j] == '8')
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("\u00B8");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else if (array[i, j] == '9')
                {
                    Console.Write("┬");
                }
                else if (array[i, j] == '0')
                {
                    Console.Write("┴");
                }
                else if (array[i, j] == 'a')
                {
                    Console.Write('├');
                }
                else if (array[i, j] == 'b')
                {
                    Console.Write('┤');
                }
                else if (array[i, j] == 'c')
                {
                    Console.Write('┼');
                }
                else if (array[i, j] == 'd')
                {
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.Write('@'); //cabbage worth 1000 points
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else if (array[i, j] == 'e')
                {
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.Write('Φ'); //draw magenta key
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else if (array[i, j] == 'f')
                {
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.Write('|'); //magenta door 1
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else if (array[i, j] == 'g')
                {
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.Write('─'); //magenta door 2
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else if (array[i, j] == 'h')
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.Write('Φ'); //draw blue key
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else if (array[i, j] == 'i')
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.Write('|'); //blue door 1
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else if (array[i, j] == 'j')
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.Write('─'); //blue door 2
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else if (array[i, j] == 'k')
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write('Φ'); //yellow key
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else if (array[i, j] == 'l')
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write('|'); //yellow door 1
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else if (array[i, j] == 'm')
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write('─'); //yellow door 2
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else
                {
                    Console.Write(" ");
                }
            }
            Console.WriteLine();
        }
    }
    private static void RabitInitialization(out string rabbitIcon, out int rabbitX, out int rabbitY, out int carrotCount, out int yourScore, out Stopwatch yourTime)
    {
        rabbitIcon = "\u0150";   //  \u0150   \u014E    \u00D2     \u00D3 --> alternatives
        rabbitX = 1;
        rabbitY = 1;

        carrotCount = 3;
        yourScore = 0;
        yourTime = Stopwatch.StartNew();
    }
    private static void RabbitColor(int rabbitColor)
    {
        switch (rabbitColor)
        {
            case 1:
                Console.ForegroundColor = ConsoleColor.White;
                break;
            case 2:
                Console.ForegroundColor = ConsoleColor.Red;
                break;
            case 3:
                Console.ForegroundColor = ConsoleColor.Yellow;
                break;
            case 4:
                Console.ForegroundColor = ConsoleColor.Cyan;
                break;
            case 5:
                Console.ForegroundColor = ConsoleColor.Green;
                break;
            case 6:
                Console.ForegroundColor = ConsoleColor.Blue;
                break;
            case 7:
                Console.ForegroundColor = ConsoleColor.Gray;
                break;
            case 8:
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                break;
        }
    } // Dyes the rabbit
    static void MoveRabbit(int height, int width, ref int rabbitX, ref int rabbitY, char[,] theMap, string rabbitIcon, int rabbitColor)
    {
        if (Console.KeyAvailable == true)
        {
            ConsoleKeyInfo pressedKey = Console.ReadKey(true);
            while (Console.KeyAvailable) Console.ReadKey(true);
            if (pressedKey.Key == ConsoleKey.LeftArrow || pressedKey.Key == ConsoleKey.A)
            {
                if (theMap[rabbitY, rabbitX - 1] == ' ' || theMap[rabbitY, rabbitX - 1] == '7'
                    || theMap[rabbitY, rabbitX - 1] == '8' || theMap[rabbitY, rabbitX - 1] == 'd'
                    || theMap[rabbitY, rabbitX - 1] == 'e' || theMap[rabbitY, rabbitX - 1] == 'h'
                    || theMap[rabbitY, rabbitX - 1] == 'k' || theMap[rabbitY, rabbitX - 1] == 'n')
                {
                    Console.SetCursorPosition(rabbitX, rabbitY);
                    Console.Write(" ");
                    rabbitX -= 1;
                    Console.SetCursorPosition(rabbitX, rabbitY);
                    RabbitColor(rabbitColor);
                    Console.Write(rabbitIcon);
                    Console.ForegroundColor = ConsoleColor.White;
                    //(new SoundPlayer("jump.wav")).Play();
                }
            }
            else if (pressedKey.Key == ConsoleKey.RightArrow || pressedKey.Key == ConsoleKey.D)
            {
                if (theMap[rabbitY, rabbitX + 1] == ' ' || theMap[rabbitY, rabbitX + 1] == '7'
                    || theMap[rabbitY, rabbitX + 1] == '8' || theMap[rabbitY, rabbitX + 1] == 'd'
                    || theMap[rabbitY, rabbitX + 1] == 'e' || theMap[rabbitY, rabbitX + 1] == 'h'
                    || theMap[rabbitY, rabbitX + 1] == 'k' || theMap[rabbitY, rabbitX + 1] == 'n')
                {
                    Console.SetCursorPosition(rabbitX, rabbitY);
                    Console.Write(" ");
                    rabbitX += 1;
                    Console.SetCursorPosition(rabbitX, rabbitY);
                    RabbitColor(rabbitColor);
                    Console.Write(rabbitIcon);
                    Console.ForegroundColor = ConsoleColor.White;
                    //(new SoundPlayer("jump.wav")).Play();
                }
            }
            else if (pressedKey.Key == ConsoleKey.UpArrow || pressedKey.Key == ConsoleKey.W)
            {
                if (theMap[rabbitY - 1, rabbitX] == ' ' || theMap[rabbitY - 1, rabbitX] == '7'
                    || theMap[rabbitY - 1, rabbitX] == '8' || theMap[rabbitY - 1, rabbitX] == 'd'
                    || theMap[rabbitY - 1, rabbitX] == 'e' || theMap[rabbitY - 1, rabbitX] == 'h'
                    || theMap[rabbitY - 1, rabbitX] == 'k' || theMap[rabbitY - 1, rabbitX] == 'n')
                {
                    Console.SetCursorPosition(rabbitX, rabbitY);
                    Console.Write(" ");
                    rabbitY -= 1;
                    Console.SetCursorPosition(rabbitX, rabbitY);
                    RabbitColor(rabbitColor);
                    Console.Write(rabbitIcon);
                    Console.ForegroundColor = ConsoleColor.White;
                   // (new SoundPlayer("jump.wav")).Play();
                }
            }
            else if (pressedKey.Key == ConsoleKey.DownArrow || pressedKey.Key == ConsoleKey.S)
            {
                if (theMap[rabbitY + 1, rabbitX] == ' ' || theMap[rabbitY + 1, rabbitX] == '7'
                    || theMap[rabbitY + 1, rabbitX] == '8' || theMap[rabbitY + 1, rabbitX] == 'd'
                    || theMap[rabbitY + 1, rabbitX] == 'e' || theMap[rabbitY + 1, rabbitX] == 'h'
                    || theMap[rabbitY + 1, rabbitX] == 'k' || theMap[rabbitY + 1, rabbitX] == 'n')
                {
                    Console.SetCursorPosition(rabbitX, rabbitY);
                    Console.Write(" ");
                    rabbitY += 1;
                    Console.SetCursorPosition(rabbitX, rabbitY);
                    RabbitColor(rabbitColor);
                    Console.Write(rabbitIcon);
                    Console.ForegroundColor = ConsoleColor.White;
                    //(new SoundPlayer("jump.wav")).Play();
                }
            }
        }
    }
    static void ScoreSection(int carrotCount, Stopwatch yourTime, long timeAndScore, long finalScore, int lives)
    {
        Console.SetCursorPosition(Console.WindowWidth - 10, 0);
        Console.Write("Time: {0}s", yourTime.ElapsedMilliseconds / 1000);
        Console.SetCursorPosition(Console.WindowWidth / 2 + 2, Console.WindowHeight / 4 + 2);
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write("Remaining Carrots: {0}", carrotCount);
        Console.SetCursorPosition(Console.WindowWidth / 2 + 2, Console.WindowHeight / 4 + 4);
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write("Remaining Lives: {0}", lives);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.SetCursorPosition(Console.WindowWidth / 2 + 2, Console.WindowHeight / 4 - 2);
        Console.Write("Your score: {0}", timeAndScore + finalScore);
        Console.ForegroundColor = ConsoleColor.White;
    }
    private static void LegendSection()
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.SetCursorPosition(Console.WindowWidth / 2 + 1, Console.WindowHeight / 4 + 4);
        Console.WriteLine("--------------------------------");
        Console.SetCursorPosition(Console.WindowWidth / 2 + 1, Console.WindowHeight / 4 + 6);
        Console.ForegroundColor = ConsoleColor.DarkGreen;
        Console.Write(" @");
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine(" - cabbage (1000 points)");
        Console.SetCursorPosition(Console.WindowWidth / 2 + 1, Console.WindowHeight / 4 + 7);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write(" \u00B8");
        Console.SetCursorPosition(Console.WindowWidth / 2 + 1, Console.WindowHeight / 4 + 8);
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write(" ▼");
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine(" - carrot ( collect 3 to ");
        Console.SetCursorPosition(Console.WindowWidth / 2 + 5, Console.WindowHeight / 4 + 9);
        Console.WriteLine("advance to next level)");
        Console.SetCursorPosition(Console.WindowWidth / 2 + 1, Console.WindowHeight / 4 + 11);
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.Write(" Φ"); //draw magenta key
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine(" - magenta key opens ");
        Console.SetCursorPosition(Console.WindowWidth / 2 + 5, Console.WindowHeight / 4 + 12);
        Console.WriteLine("magenta doors");
        Console.SetCursorPosition(Console.WindowWidth / 2 + 1, Console.WindowHeight / 4 + 14);
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.Write(" Φ"); //draw blue key
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine(" - blue key opens blue doors");
        Console.SetCursorPosition(Console.WindowWidth / 2 + 1, Console.WindowHeight / 4 + 16);
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write(" Φ"); //yellow key
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine(" - yellow key opens ");
        Console.SetCursorPosition(Console.WindowWidth / 2 + 5, Console.WindowHeight / 4 + 17);
        Console.WriteLine("yellow doors");
        Console.SetCursorPosition(Console.WindowWidth / 2 + 1, Console.WindowHeight / 4 + 19);
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(" #");//trap
        Console.ForegroundColor = ConsoleColor.White;
        Console.SetCursorPosition(Console.WindowWidth / 2 +5, Console.WindowHeight / 4 + 19);
        Console.WriteLine(" - dangerous trap, avoid!");
    } // Displays the instructions on the right side of the screen
    static void EatVegetable(char[,] charMap, int rabbitX, int rabbitY, ref int carrotCount, ref int yourScore)
    {
        if (charMap[rabbitY, rabbitX] == '7')
        {
            charMap[rabbitY, rabbitX] = ' ';
            Console.SetCursorPosition(rabbitX, rabbitY - 1);
            Console.Write(" ");
            charMap[rabbitY - 1, rabbitX] = ' ';
            carrotCount--;
            (new SoundPlayer("carrot.wav")).Play();
        }
        else if (charMap[rabbitY, rabbitX] == '8')
        {
            charMap[rabbitY, rabbitX] = ' ';
            charMap[rabbitY + 1, rabbitX] = ' ';
            Console.SetCursorPosition(rabbitX, rabbitY + 1);
            Console.Write(" ");
            carrotCount--;
            (new SoundPlayer("carrot.wav")).Play();
        }
        else if (charMap[rabbitY, rabbitX] == 'd')
        {
            charMap[rabbitY, rabbitX] = ' ';
            (new SoundPlayer("cab.wav")).Play();
            yourScore += 1000;
        }
    }
    static void GotMagentaKey(int mapHeight, int mapSize, int rabbitX, int rabbitY, char[,] theMap)
    {
        if (theMap[rabbitY, rabbitX] == 'e')
        {
            for (int row = 1; row < mapSize; row++)
            {
                for (int col = 1; col < mapHeight; col++)
                {
                    if (theMap[col, row] == 'f' || theMap[col, row] == 'g')
                    {
                        (new SoundPlayer("key.wav")).Play();
                        Console.SetCursorPosition(row, col);
                        Console.Write(' ');
                        theMap[col, row] = ' ';
                    }
                }
            }
        }
    }
    static void GotBlueKey(int mapHeight, int mapSize, int rabbitX, int rabbitY, char[,] theMap)
    {
        if (theMap[rabbitY, rabbitX] == 'h')
        {
            for (int row = 1; row < mapSize; row++)
            {
                for (int col = 1; col < mapHeight; col++)
                {
                    if (theMap[col, row] == 'i' || theMap[col, row] == 'j')
                    {
                        (new SoundPlayer("key.wav")).Play();
                        Console.SetCursorPosition(row, col);
                        Console.Write(' ');
                        theMap[col, row] = ' ';
                    }
                }
            }
        }
    }
    static void GotYellowKey(int mapHeight, int mapSize, int rabbitX, int rabbitY, char[,] theMap)
    {

        if (theMap[rabbitY, rabbitX] == 'k')
        {
            for (int row = 1; row < mapSize; row++)
            {
                for (int col = 1; col < mapHeight; col++)
                {
                    if (theMap[col, row] == 'l' || theMap[col, row] == 'm')
                    {
                        (new SoundPlayer("key.wav")).Play();
                        Console.SetCursorPosition(row, col);
                        Console.Write(' ');
                        theMap[col, row] = ' ';
                    }
                }
            }
        }
    }
    private static void TrapGeneration(ref bool trapIsSpawned, ref char[,] theMap, ref Stopwatch timer, 
        int i, int j, ref int positionOfTrapX, ref int positionOfTrapY)
    {

        if (theMap[i, j] == ' ' && !trapIsSpawned)
        {
            while (theMap[i, j] == ' ' && !trapIsSpawned)
            {


                timer.Start();

                theMap[i, j] = 'n';
                Console.SetCursorPosition(j, i);
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.Write('#');
                Console.ForegroundColor = ConsoleColor.White;
                positionOfTrapX = i;
                positionOfTrapY = j;
                trapIsSpawned = true;
            }

        }

    }
    private static void TrapRemoving(ref Stopwatch timer, ref bool trapIsSpawned, ref char[,] theMap,
        ref int positionOfTrapX, ref int positionOfTrapY, int rabbitX, int rabbitY, ref int lives)
    {
        if (trapIsSpawned && timer.ElapsedMilliseconds % 500 == 0 && timer.ElapsedMilliseconds != 0
            && theMap[positionOfTrapX, positionOfTrapY] == 'n')
        {

            trapIsSpawned = false;
            theMap[positionOfTrapX, positionOfTrapY] = ' ';
            Console.SetCursorPosition(positionOfTrapY, positionOfTrapX);

            Console.Write(' ');

            timer.Stop();
            timer.Reset();

        }
        if (theMap[rabbitY, rabbitX] == theMap[positionOfTrapX, positionOfTrapY] && trapIsSpawned && timer.ElapsedMilliseconds != 0)
        {
            lives--;
            theMap[positionOfTrapX, positionOfTrapY] = ' ';
            //Console.SetCursorPosition(positionOfTrapY,positionOfTrapX);
            //Console.Write(" ");
            trapIsSpawned = false;
            timer.Stop();
            timer.Reset();
        }
    }
    static string ReturnName(string lineInFile)
    {
        int myDotIndex = lineInFile.IndexOf(".");
        int mySlashIndex = lineInFile.IndexOf("-");
        return lineInFile.Substring(myDotIndex + 1, mySlashIndex - 1);
    } // Returns the Name of the player from the File score.txt
    static string ReturnScore(string lineInFile)
    {
        int myDashIndex = lineInFile.IndexOf("-");
        return lineInFile.Substring(myDashIndex + 1);
    } // Returns the Score of the player from the File score.txt
    private static void ScoreBoardOutputSaveAndLoadFile(long finalScore)
    {
        Console.Clear();
        Console.SetCursorPosition(Console.WindowWidth / 4 - 2, Console.WindowHeight / 2 - 10);
        Console.WriteLine("Congratulations! The bunny is no longer hungry!");
        Console.SetCursorPosition(Console.WindowWidth / 2 - 6, Console.WindowHeight / 2 + -8);
        Console.WriteLine("Score: {0}", finalScore);
        Console.SetCursorPosition(2, Console.WindowHeight / 2 - 4);
        Console.Write("Enter your name: ");
        string playerName = Console.ReadLine();

        int numberOfEntries = File.ReadAllLines("score.txt").Length;
        using (StreamWriter writeFile = new StreamWriter("score.txt", true))
        {
            writeFile.WriteLine("{0}. {1} - {2}", numberOfEntries + 1, playerName, finalScore);
        }

        Console.Clear();
        Console.SetCursorPosition(Console.WindowWidth / 2 - 10, 3);
        System.Console.WriteLine("High Score Board:");
        int numberOfLinesInFile = 0;
        int counterForTextAlignment = 0;
        string lineInFile;
        StreamReader readFile = new StreamReader("score.txt");
        //Define list with multiple vars:
        List<Tuple<string, int>> dataInScoreFile = new List<Tuple<string, int>>();
        while ((lineInFile = readFile.ReadLine()) != null)
        {
            dataInScoreFile.Add(new Tuple<string, int>(ReturnName(lineInFile), int.Parse(ReturnScore(lineInFile))));
        }
        dataInScoreFile.Sort((firstElement, secondElement) => secondElement.Item2.CompareTo(firstElement.Item2));
        //Print High Score:
        for (int i = 0; i < dataInScoreFile.Count; i++)
        {
            Console.SetCursorPosition(Console.WindowWidth / 2 - 12, 5 + counterForTextAlignment);
            Console.WriteLine("{0}.{1} {2}", numberOfLinesInFile + 1, dataInScoreFile[i].Item1, dataInScoreFile[i].Item2);
            numberOfLinesInFile++;
            counterForTextAlignment++;
        }
        readFile.Close();
        using (StreamWriter writeFile = new StreamWriter("score.txt"))
        {
            for (int i = 0; i < dataInScoreFile.Count; i++)
            {
                writeFile.WriteLine("{0}.{1} {2}", i + 1, dataInScoreFile[i].Item1, dataInScoreFile[i].Item2);
            }
        }
        Console.WriteLine("\n\n\n");
    } 
}