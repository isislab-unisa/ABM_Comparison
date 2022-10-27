using System;
using System.Text;

namespace PredatorPrey
{
    public enum CellState { Empty, Sheep, Doodlebug };

    public enum Direction { Up, Down, Left, Right };

    public class Cell
    {
        public CellState State;
        public InsectAgent AgentInCell;

        public Cell()
        {
            State = CellState.Empty;
            AgentInCell = null;
        }
    }

    public class World
    {
        private Cell[,] _map;
        private int[,] _grassfield;
        private int _currentId;
        private static Random _rand = new Random();

        public World()
        {
            _map = new Cell[Settings.GridSize, Settings.GridSize];
            _grassfield = new int[Settings.GridSize, Settings.GridSize];
            for (int i = 0; i < Settings.GridSize; i++)
                for (int j = 0; j < Settings.GridSize; j++)
                {
                    _map[i, j] = new Cell();
                    double r = _rand.NextDouble();
                    if (r < 0.5)
                        _grassfield[i, j] = 20;
                    else
                    {
                        _grassfield[i, j] = _rand.Next(20);
                    }

                }

            _currentId = 0;
        }

        public void AddAgentToMap(InsectAgent a, int line, int column)
        {
            if (a.GetType().Name == "Sheep")
                _map[line, column].State = CellState.Sheep;
            else if (a.GetType().Name == "DoodlebugAgent")
                _map[line, column].State = CellState.Doodlebug;

            a.Line = line; a.Column = column;
            _map[line, column].AgentInCell = a;
        }

        public bool CheckFood(int line, int column)
        {
            //true if there is food
            if (_grassfield[line, column] >= 20)
            {
                _grassfield[line, column] = 0;
                return true;
            }
            else
                return false;
        }

        public void AddAgentToMap(InsectAgent a, int vectorPosition)
        {
            int line = vectorPosition / Settings.GridSize;
            int column = vectorPosition % Settings.GridSize;

            AddAgentToMap(a, line, column);
        }

        public string CreateName(InsectAgent a)
        {
            if (a.GetType().Name == "Sheep")
                return $"a{_currentId++}";
            else if (a.GetType().Name == "DoodlebugAgent")
                return $"d{_currentId++}";

            throw new Exception($"Unknown agent type: {a.GetType()}");
        }

        public void CountInsects(out int noDoodlebugs, out int noSheeps)
        {
            noSheeps = 0;
            noDoodlebugs = 0;

            for (int i = 0; i < Settings.GridSize; i++)
                for (int j = 0; j < Settings.GridSize; j++)
                {
                    if (_map[i, j].State == CellState.Doodlebug)
                        noDoodlebugs++;
                    else if (_map[i, j].State == CellState.Sheep)
                        noSheeps++;
                }
        }

        public void Move(InsectAgent a, int newLine, int newColumn)
        {
            // moving the agent

            _map[newLine, newColumn].State = _map[a.Line, a.Column].State;
            _map[newLine, newColumn].AgentInCell = _map[a.Line, a.Column].AgentInCell;

            _map[a.Line, a.Column].State = CellState.Empty;
            _map[a.Line, a.Column].AgentInCell = null;

            // updating agent position

            a.Line = newLine;
            a.Column = newColumn;
        }

        public void UpdateGrass()
        {
            for (int i = 0; i < Settings.GridSize; i++)
                for (int j = 0; j < Settings.GridSize; j++)
                {
                    _grassfield[i, j] += 1;
                }
        }

        public InsectAgent Breed(InsectAgent a, int newLine, int newColumn, int e)
        {
            InsectAgent offspring = null;

            if (a.GetType().Name == "Sheep")
                offspring = new Sheep(e);
            else if (a.GetType().Name == "DoodlebugAgent")
                offspring = new DoodlebugAgent(e);

            string name = CreateName(offspring);
            offspring.Name = name;
            AddAgentToMap(offspring, newLine, newColumn);

            if (Settings.Verbose)
                Console.WriteLine($"Breeding {offspring.Name}");

            return offspring;
        }

        public Sheep Eat(DoodlebugAgent da, int newLine, int newColumn)
        {
            var sheep = (Sheep)_map[newLine, newColumn].AgentInCell;

            if (Settings.Verbose)
                Console.WriteLine($"Removing {sheep.Name}");

            // moving the doodlebug

            if (Settings.Verbose)
                Console.WriteLine($"Moving {da.Name}");

            _map[newLine, newColumn].State = CellState.Doodlebug;
            _map[newLine, newColumn].AgentInCell = _map[da.Line, da.Column].AgentInCell;

            _map[da.Line, da.Column].State = CellState.Empty;
            _map[da.Line, da.Column].AgentInCell = null;

            // updating doodlebug position

            da.Line = newLine;
            da.Column = newColumn;

            return sheep;
        }

        public void Die(DoodlebugAgent da)
        {
            _map[da.Line, da.Column].State = CellState.Empty;
            _map[da.Line, da.Column].AgentInCell = null;
        }

        public void Die(InsectAgent da)
        {
            _map[da.Line, da.Column].State = CellState.Empty;
            _map[da.Line, da.Column].AgentInCell = null;
        }

        public bool ValidMovement(InsectAgent a, Direction direction, CellState desiredState, out int newLine, out int newColumn)
        {
            int currentLine = a.Line; int currentColumn = a.Column;
            newLine = currentLine; newColumn = currentColumn;

            switch (direction)
            {
                case Direction.Up:
                    if (currentLine == 0) return false;
                    if (_map[currentLine - 1, currentColumn].State != desiredState) return false;
                    newLine = currentLine - 1;
                    return true;

                case Direction.Down:
                    if (currentLine == Settings.GridSize - 1) return false;
                    if (_map[currentLine + 1, currentColumn].State != desiredState) return false;
                    newLine = currentLine + 1;
                    return true;

                case Direction.Left:
                    if (currentColumn == 0) return false;
                    if (_map[currentLine, currentColumn - 1].State != desiredState) return false;
                    newColumn = currentColumn - 1;
                    return true;

                case Direction.Right:
                    if (currentColumn == Settings.GridSize - 1) return false;
                    if (_map[currentLine, currentColumn + 1].State != desiredState) return false;
                    newColumn = currentColumn + 1;
                    return true;

                default:
                    break;
            }

            throw new Exception("Invalid direction");
        }

        public string PrintMap()
        {
            var sb = new StringBuilder();

            for (int i = 0; i < Settings.GridSize; i++)
            {
                for (int j = 0; j < Settings.GridSize; j++)
                {
                    switch (_map[i, j].State)
                    {
                        case CellState.Empty:
                            sb.Append("-");
                            break;

                        case CellState.Sheep:
                            sb.Append("S");
                            break;

                        case CellState.Doodlebug:
                            sb.Append("D");
                            break;

                        default:
                            break;
                    }
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}