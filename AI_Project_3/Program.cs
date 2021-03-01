using System;
using System.Collections.Generic;
using System.Linq;

namespace AI_Project_3
{
    class Program
    {
        static Node[,] maze = new Node[5, 6];
        static Node[,] accessMaze = new Node[5, 6];
        static List<Position> blockedPositions = new List<Position>
        {
            new Position
            {
                X = 1,
                Y = 1,
            },
            new Position
            {
                X = 2,
                Y = 1
            },
            new Position
            {
                X = 1,
                Y = 2,
            },
            new Position
            {
                X = 1,
                Y = 3
            },
            new Position
            {
                X = 2,
                Y = 3,
            },
            new Position
            {
                X = 1,
                Y = 4
            }
        };

        //Curtis Code
        static void Main(string[] args)
        {
            FillGraph(maze, true);
            FillGraph(accessMaze, false);
            Random rnd = new Random();
            for (int i = 0; i < 10000; i++)
            {
                int x = rnd.Next(0, 5);
                int y = rnd.Next(0, 6);
                while (maze[x, y] == null || (x == 2 && y == 2))
                {
                    x = rnd.Next(0, 5);
                    y = rnd.Next(0, 6);
                }

                int counter = 0;
                Node node = maze[x, y];
                while (counter < 100 && (node.Position.X != 2 || node.Position.Y != 2))
                {
                    var tuple = EGreedy(node);
                    var targetNode = Environment(tuple);
                    QLearning(tuple, targetNode);
                    node = targetNode;
                    counter++;
                }
            }
            Console.WriteLine("Access Value Maze:");
            PrintGraph(accessMaze);
            Console.WriteLine("\nQ-Value Maze:");
            PrintGraph(maze);
            Console.WriteLine("\nOptimal Direction Maze:");
            PrintOptDirection();
        }

        //Curtis Code
        //Aligns the text center for each row so output looks like a maze with four values for each direction
        static string AlignCenter(string text, int width)
        {
            text = text.Length > width ? text.Substring(0, width - 3) + "..." : text;

            if (string.IsNullOrEmpty(text))
            {
                return new string(' ', width);
            }
            else
            {
                return text.PadRight(width - (width - text.Length) / 2).PadLeft(width);
            }
        }
 
        //Curtis Code
        //Has a 95% chance to go in the best direction, that is the direction with the highest reward.
        //Does this by looking at the node and it's 4 values and going with the highest value
        //In case of a tie will randomly pick a direction to go of the values that tied
        static Tuple EGreedy(Node node)
        {
            var rnd = new Random();
            var randomOrGreedy = rnd.Next(1, 101);
            if (randomOrGreedy > 95)
            {
                var directionPicker = rnd.Next(1, 5);
                if (directionPicker == 1)
                {
                    return new Tuple { Direction = "N", Node = node, Reward = -3 };
                }
                else if (directionPicker == 2)
                {
                    return new Tuple { Direction = "E", Node = node, Reward = -2 };
                }
                else if (directionPicker == 3)
                {
                    return new Tuple { Direction = "S", Node = node, Reward = -1 };
                }
                else
                {
                    return new Tuple { Direction = "W", Node = node, Reward = -2 };
                }
            }
            else
            {
                var directionValues = new[] { new { value = node.North, direction = "N" }, new { value = node.East, direction = "E" }, new { value = node.South, direction = "S" }, new { value = node.West, direction = "W" } }.ToList();
                double max = directionValues.Max(x => x.value);
                var equalValues = directionValues.Where(x => Math.Round(x.value, 4) == Math.Round(max, 4)).ToList();
                var picker = rnd.Next(0, equalValues.Count - 1);
                var pickedDirection = equalValues[picker];
                int reward = 0;
                if (pickedDirection.direction == "N")
                {
                    reward = -3;
                }
                else if (pickedDirection.direction == "S")
                {
                    reward = -1;
                }
                else
                {
                    reward = -2;
                }

                return new Tuple { Direction = pickedDirection.direction, Node = node, Reward = reward };
            }
        }

        //Curtis Code
        // Prints a Graph row bt row and helps with formatting of the print graph.
        static void PrintGraph(Node[,] maze)
        {
            // controls y-axis
            for (int i = 0; i < 6; i++)
            {
                //controls x-axis
                for (int j = 0; j < 5; j++)
                {
                    var node = maze[j, i];
                    if (node != null)
                    {
                        Console.Write($"{AlignCenter(Math.Round(node.North, 2).ToString(), 12)}");
                    }
                    else
                    {
                        Console.Write($"{AlignCenter(" ", 12)}");
                    }
                }
                Console.WriteLine();
                for (int j = 0; j < 5; j++)
                {
                    var node = maze[j, i];
                    if (node != null)
                    {
                        Console.Write($"{AlignCenter(Math.Round(node.West, 2).ToString(), 6)}{AlignCenter(Math.Round(node.East, 2).ToString(), 6)}");
                    }
                    else
                    {
                        Console.Write($"{AlignCenter("####", 12)}");
                    }
                }
                Console.WriteLine();
                for (int j = 0; j < 5; j++)
                {
                    var node = maze[j, i];
                    if (node != null)
                    {
                        Console.Write($"{AlignCenter(Math.Round(node.South, 2).ToString(), 12)}");
                    }
                    else
                    {
                        Console.Write($"{AlignCenter(" ", 12)}");
                    }
                }
                Console.WriteLine();
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        //Curtis Code
        //fills a graph with a bunch of nodes that all have values of zero and gives them the correct position
        //if has goal is true then node and postion 2,2 will have reward values of 100 for all directions
        static void FillGraph(Node[,] maze, bool hasGoal)
        {
            // controls y-axis
            for (int i = 0; i < 6; i++)
            {
                //controls x-axis
                for (int j = 0; j < 5; j++)
                {
                    Node node = new Node();
                    node.Position = new Position();
                    node.Position.X = j;
                    node.Position.Y = i;
                    node.North = 0.0;
                    node.South = 0.0;
                    node.East = 0.0;
                    node.West = 0.0;
                    maze[j, i] = node;
                }
            }
            if (hasGoal)
            {
                maze[2, 2].North = 100.0;
                maze[2, 2].South = 100.0;
                maze[2, 2].East = 100.0;
                maze[2, 2].West = 100.0;
            }
            FillGraphWithNulls(maze);
        }

        //Curtis Code
        //Adds nulls to the maze and the postion where the walls would be. We use nulls for determineing if it's a wall or not.
        static void FillGraphWithNulls(Node[,] maze)
        {
            foreach (var position in blockedPositions)
            {
                maze[position.X, position.Y] = null;
            }
        }

        //Noel Code
        //decides if the node will reach goal or bounce back (.70 vs .15 vs .15)
        //returns the node it ACTUALLY ends up at
        static Node Environment(Tuple startNode)
        {
            //set target node
            int startX = startNode.Node.Position.X;
            int startY = startNode.Node.Position.Y;
            Node target = startNode.Node; //starts at current node - in case there is bounceback, then it will stay here
            if (startNode.Direction == "N" && CheckBounce(startX, startY - 1, "N") == false) //if going north w/ no bounceback
            {
                target = maze[startX, startY - 1];
            }
            else if (startNode.Direction == "S" && CheckBounce(startX, startY + 1, "S") == false)
            {
                target = maze[startX, startY + 1];
            }
            else if (startNode.Direction == "W" && CheckBounce(startX - 1, startY, "W") == false)
            {
                target = maze[startX - 1, startY];
            }
            else if (startNode.Direction == "E" && CheckBounce(startX + 1, startY, "E") == false)
            {
                target = maze[startX + 1, startY];
            }

            //find drift probability
            Random probability = new Random();
            int randomProb = probability.Next(1, 101);

            //if target node is north(y-1) or south(y+1): drift can go to east + west
            if ((startNode.Direction == "N") || (startNode.Direction == "S"))
            {
                //drifts east if no bounceback
                if ((randomProb >= 1) && (randomProb <= 15) && CheckBounce(startX + 1, startY, "E") == false)
                {
                    target = maze[startX + 1, startY];
                }
                //drifts west
                else if ((randomProb >= 16) && (randomProb <= 30) && CheckBounce(startX - 1, startY, "W") == false)
                {
                    target = maze[startX - 1, startY];
                }
            }
            //if target node is west(x-1) or east(x+1): drift can go north + south
            else if ((startNode.Direction == "W") || (startNode.Direction == "E"))
            {
                //drifts north if no bounceback
                if ((randomProb >= 1) && (randomProb <= 15) && CheckBounce(startX, startY - 1, "N") == false)
                {
                    target = maze[startX, startY - 1];
                }
                //drifts south 
                else if ((randomProb >= 16) && (randomProb <= 30) && CheckBounce(startX, startY + 1, "S") == false)
                {
                    target = maze[startX, startY + 1];
                }
            }

            return target;
        }

        //Noel Code
        //checks if there is bounceback using the target node's X and Y maze coordinates
        //returns true if the node bounces back into orig place
        static bool CheckBounce(int targetX, int targetY, string targetDir)
        {
            bool isBounce = false;
            //if there's a wall/obstacle to the North
            if (targetDir == "N")
            {
                if ((targetY - 1) < 0 || maze[targetX, targetY] == null)
                {
                    isBounce = true;
                }
            }
            //if there's a wall/obstacle to the south
            else if (targetDir == "S")
            {
                if ((targetY + 1) > 5 || maze[targetX, targetY] == null)
                {
                    isBounce = true;
                }
            }
            //if there's a wall/obstacle to the West
            else if (targetDir == "W")
            {
                if ((targetX - 1) < 0 || maze[targetX, targetY] == null)
                {
                    isBounce = true;
                }
            }
            //if there's a wall/obstacle to the East
            else if (targetDir == "E")
            {
                if ((targetX + 1) > 4 || maze[targetX, targetY] == null)
                {
                    isBounce = true;
                }
            }

            return isBounce;
        }

        //Noel and Curtis Code
        //Checks the direction the current node is going in in order to designate the correct values
        //updates the access maze and q-value maze values at the placement that was passed in via a Node 
        static void QLearning(Tuple tuple, Node targetNode)
        {
            double maxTarget = new List<double> { targetNode.North, targetNode.East, targetNode.West, targetNode.South }.Max();
            if (tuple.Direction == "N")
            {
                var frequency = (accessMaze[tuple.Node.Position.X, tuple.Node.Position.Y].North += 1);
                tuple.Node.North = tuple.Node.North + (1 / frequency) * (tuple.Reward + 0.9 * maxTarget - tuple.Node.North);
            }
            else if (tuple.Direction == "E")
            {
                var frequency = (accessMaze[tuple.Node.Position.X, tuple.Node.Position.Y].East += 1);
                tuple.Node.East = tuple.Node.East + (1 / frequency) * (tuple.Reward + 0.9 * maxTarget - tuple.Node.East);
            }
            else if (tuple.Direction == "S")
            {
                var frequency = (accessMaze[tuple.Node.Position.X, tuple.Node.Position.Y].South += 1);
                tuple.Node.South = tuple.Node.South + (1 / frequency) * (tuple.Reward + 0.9 * maxTarget - tuple.Node.South);
            }
            else
            {
                var frequency = (accessMaze[tuple.Node.Position.X, tuple.Node.Position.Y].West += 1);
                tuple.Node.West = tuple.Node.West + (1 / frequency) * (tuple.Reward + 0.9 * maxTarget - tuple.Node.West);
            }
        }

        //Noel Code
        //for each node in the maze, this function checks the direction that has the max q-value assigned to it
        //then prints out the correct corresponding set of arrows (and spaces for formatting purposes)
        static void PrintOptDirection()
        {
            string[,] arrowMaze = new string[5, 6];
            double arrowChoice = 0.0;
            int lineCount = 0;
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (maze[j, i] != null)
                    {
                        List<double> choices = new List<double> { maze[j, i].North, maze[j, i].East, maze[j, i].South, maze[j, i].West };
                        arrowChoice = choices.Max();
                        if (arrowChoice == choices[0])
                        {
                            arrowMaze[j, i] = "^^^^^        ";
                        }
                        else if (arrowChoice == choices[1])
                        {
                            arrowMaze[j, i] = ">>>>>        ";
                        }
                        else if (arrowChoice == choices[2])
                        {
                            arrowMaze[j, i] = "VVVVV        ";
                        }
                        else if (arrowChoice == choices[3])
                        {
                            arrowMaze[j, i] = "<<<<<        ";
                        }
                    }
                    else
                    {
                        arrowMaze[j, i] = "#####        ";
                    }
                    Console.Write(arrowMaze[j, i]);
                    lineCount++;
                    if (lineCount % 5 == 0)
                    {
                        Console.Write("\n");
                    }

                }
            }
        }
    }
}
