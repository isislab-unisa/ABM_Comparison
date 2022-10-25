using System;
using System.Data.Common;
using System.Text;
using static System.Windows.Forms.LinkLabel;

//pub static COHESION: f32 = 0.8;
//pub static AVOIDANCE: f32 = 1.0;
//pub static RANDOMNESS: f32 = 1.1;
//pub static CONSISTENCY: f32 = 0.7;
//pub static MOMENTUM: f32 = 1.0;
//pub static JUMP: f32 = 0.7;
//pub static DISCRETIZATION: f32 = 10.0 / 1.5;
//pub static TOROIDAL: bool = true;

namespace PredatorPrey
{
    public enum CellState { Empty, Ant};

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
        public Cell[,] _map;
        private int _currentId;

        public World()
        {
            _map = new Cell[Settings.GridSize, Settings.GridSize];
            for (int i = 0; i < Settings.GridSize; i++)
                for (int j = 0; j < Settings.GridSize; j++)
                    _map[i, j] = new Cell();

            _currentId = 0;
        }

        public bool AddAgentToMap(InsectAgent a, int line, int column)
        {
            //Console.WriteLine($"%% {line} {column}");
            if (_map[line, column].State == CellState.Ant)
                return false;

            if (a.GetType().Name == "AntAgent")
                _map[line, column].State = CellState.Ant;

            a.Line = line; a.Column = column;
            _map[line, column].AgentInCell = a;
            return true;
        }

        public void AddAgentToMap(InsectAgent a, int vectorPosition)
        {
            int line = vectorPosition / Settings.GridSize;
            int column = vectorPosition % Settings.GridSize;

            AddAgentToMap(a, line, column);
        }

        public string CreateName(InsectAgent a)
        {
            if (a.GetType().Name == "AntAgent")
                return $"a{_currentId++}";

            throw new Exception($"Unknown agent type: {a.GetType()}");
        }

        public void CountInsects(out int noAnts)
        {
            noAnts = 0;

            for (int i = 0; i < Settings.GridSize; i++)
                for (int j = 0; j < Settings.GridSize; j++)
                {
                    if (_map[i, j].State == CellState.Ant)
                        noAnts++;
                }
        }

        public bool Move(InsectAgent a, int newLine, int newColumn)
        {
            // moving the agent
            if (newLine < 0 || newLine >= Settings.GridSize
                || newColumn < 0 || newLine >= Settings.GridSize)
                Console.WriteLine($"move {newLine} {newColumn}");

            if (_map[newLine, newColumn].State == CellState.Ant)
                return false;

            _map[newLine, newColumn].State = _map[a.Line, a.Column].State;
            _map[newLine, newColumn].AgentInCell = _map[a.Line, a.Column].AgentInCell;

            _map[a.Line, a.Column].State = CellState.Empty;
            _map[a.Line, a.Column].AgentInCell = null;

            // updating agent position

            a.Line = newLine;
            a.Column = newColumn;
            return true;
        }

        //public InsectAgent Breed(InsectAgent a, int newLine, int newColumn)
        //{
        //    InsectAgent offspring = null;

        //    if (a.GetType().Name == "AntAgent")
        //        offspring = new AntAgent();

        //    string name = CreateName(offspring);
        //    offspring.Name = name;
        //    AddAgentToMap(offspring, newLine, newColumn);

        //    if (Settings.Verbose)
        //        Console.WriteLine($"Breeding {offspring.Name}");

        //    return offspring;
        //}

        //public AntAgent Eat(DoodlebugAgent da, int newLine, int newColumn)
        //{
        //    var ant = (AntAgent)_map[newLine, newColumn].AgentInCell;

        //    if (Settings.Verbose)
        //        Console.WriteLine($"Removing {ant.Name}");

        //    // moving the doodlebug

        //    if (Settings.Verbose)
        //        Console.WriteLine($"Moving {da.Name}");

        //    _map[newLine, newColumn].State = CellState.Doodlebug;
        //    _map[newLine, newColumn].AgentInCell = _map[da.Line, da.Column].AgentInCell;

        //    _map[da.Line, da.Column].State = CellState.Empty;
        //    _map[da.Line, da.Column].AgentInCell = null;

        //    // updating doodlebug position

        //    da.Line = newLine;
        //    da.Column = newColumn;

        //    return ant;
        //}

        //public void Die(DoodlebugAgent da)
        //{
        //    _map[da.Line, da.Column].State = CellState.Empty;
        //    _map[da.Line, da.Column].AgentInCell = null;
        //}

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
                            sb.Append("x");
                            break;

                        case CellState.Ant:
                            sb.Append("O");
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