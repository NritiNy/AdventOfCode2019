using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AdventOfCode
{
    namespace Intcodes
    {
        public enum InstructionType { Addition = 1, Multiplication = 2, Input = 3, Output = 4, JumpIfTrue = 5, JumpIfFalse = 6, LessThan = 7, Equals = 8, AdjustRelativeBase = 9, Stop = 99 };
        public enum ParameterMode { Position = 0, Immediate = 1, Relative = 2};
        public struct InstructionResult
        {
            public bool Result { get; private set; }

            public string Message { get; private set; }

            public InstructionResult(bool result, string msg)
            {
                Result = result;
                Message = msg;
            }

            public static implicit operator bool(InstructionResult result) => result.Result;
        }

        public struct Parameter
        {
            public int Adress { get; private set; }

            public int RelativeBase { get; private set; }

            public ParameterMode Mode { get; private set; }

            public Parameter(int adress, int relBase, ParameterMode mode)
            {
                Adress = adress;
                RelativeBase = relBase;
                Mode = mode;
            }

            public int GetValue(int[] memory)
            {
                if (Mode == ParameterMode.Immediate)
                {
                    return memory[Adress];
                }
                else if(Mode == ParameterMode.Position || Mode == ParameterMode.Relative)
                {
                    return memory[GetPosition(memory)];
                }
                else
                {
                    throw new NotImplementedException($"Unexpected Parameter Mode: {Mode}");
                }
            }

            public int GetPosition(int[] memory)
            {
                if (Mode == ParameterMode.Position)
                    return memory[Adress];
                else if (Mode == ParameterMode.Relative)
                    return memory[RelativeBase + memory[Adress]];
                else
                    throw new NotImplementedException($"Parameter Mode must be {ParameterMode.Position}");
            }

            public override string ToString()
            {
                return $"Parameter[Mode: {Mode}, Adress: {Adress}]";
            }
        }

        public abstract class IInstruction
        {
            public static Dictionary<int, Type> AvailableInstructions { get; } = InitializeAvailableInstructions();

            private static Dictionary<int, Type> InitializeAvailableInstructions()
            {
                var ret = new Dictionary<int, Type>();

                foreach (InstructionType type in Enum.GetValues(typeof(InstructionType)))
                {
                    ret.Add((int)type, Assembly.GetExecutingAssembly().GetTypes().First(t => t.Name == type.ToString() + "Instruction" && t.IsSubclassOf(typeof(IInstruction))));
                }

                return ret;
            }


            public int Adress { get; private set; }

            public abstract InstructionType InstructionType { get; }

            public abstract int Size { get; }

            protected int? _next = null;
            public int Next { get => _next ?? Adress + Size; }

            public Parameter[] Parameters { get; private set; }

            protected IInstruction(ref IntcodeComputer computer, List<int> parameterModes)
            {
                Adress = computer.InstructionPointer;
                Parameters = new Parameter[Size - 1];

                parameterModes = parameterModes ?? new List<int>(Size);
                for (int i = 1; i < Size; ++i)
                {
                    if (i <= parameterModes.Count)
                        Parameters[i - 1] = new Parameter(Adress + i, computer.RelativeBase, (ParameterMode)parameterModes[i - 1]);
                    else
                        Parameters[i - 1] = new Parameter(Adress + i, computer.RelativeBase, ParameterMode.Position);
                }
            }

            public abstract InstructionResult Execute(IntcodeComputer computer, int[] buffer);


            public static IInstruction GetInstruction(int instructionOpCode, int position)
            {
                var code = instructionOpCode % 100;
                instructionOpCode = (instructionOpCode - code) / 100;

                Type instruction = AvailableInstructions[code];

                var paramModes = ("" + instructionOpCode).ToCharArray().ToList().ConvertAll(c => int.Parse("" + c));
                paramModes.Reverse();

                object[] args = { position, paramModes };
                return (IInstruction)Activator.CreateInstance(instruction, args);
            }
        }

        public class AdditionInstruction : IInstruction
        {
            public override InstructionType InstructionType { get => InstructionType.Addition; }

            public override int Size => 4;

            public AdditionInstruction(ref IntcodeComputer computer, List<int> parameterModes) : base(ref computer, parameterModes)
            {
            }

            public override InstructionResult Execute(IntcodeComputer computer, int[] buffer)
            {
                try
                {
                    var memory = computer.CurrentMemoryState;
                    memory[Parameters[2].GetPosition(memory)] = Parameters[0].GetValue(memory) + Parameters[1].GetValue(memory);

                    return new InstructionResult(true, "AdditionInstruction passed.");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return new InstructionResult(false, "");
                }
            }
        }

        public class MultiplicationInstruction : IInstruction
        {
            public override InstructionType InstructionType { get => InstructionType.Multiplication; }

            public override int Size => 4;

            public MultiplicationInstruction(ref IntcodeComputer computer, List<int> parameterModes) : base(ref computer, parameterModes)
            {
            }

            public override InstructionResult Execute(IntcodeComputer computer, int[] buffer)
            {
                try
                {
                    var memory = computer.CurrentMemoryState;
                    memory[Parameters[2].GetPosition(memory)] = Parameters[0].GetValue(memory) * Parameters[1].GetValue(memory);

                    return new InstructionResult(true, "MiltiplicationInstruction passed.");
                }
                catch (Exception e)
                {
                    return new InstructionResult(false, e.Message);
                }
            }
        }

        public class InputInstruction : IInstruction
        {
            public override InstructionType InstructionType { get => InstructionType.Input; }

            public InputInstruction(ref IntcodeComputer computer, List<int> parameterModes) : base(ref computer, parameterModes)
            {
            }

            public override int Size => 2;

            public override InstructionResult Execute(IntcodeComputer computer, int[] buffer)
            {
                try
                {
                    var memory = computer.CurrentMemoryState;

                    var input = buffer[0];
                    memory[Parameters[0].GetPosition(memory)] = input;

                    return new InstructionResult(true, "InputInstruction passed.");
                }
                catch (Exception e)
                {
                    return new InstructionResult(false, e.Message);
                }
            }
        }

        public class OutputInstruction : IInstruction
        {
            public override InstructionType InstructionType { get => InstructionType.Output; }

            public OutputInstruction(ref IntcodeComputer computer, List<int> parameterModes) : base(ref computer, parameterModes)
            {
            }

            public override int Size => 2;

            public override InstructionResult Execute(IntcodeComputer computer, int[] buffer)
            {
                try
                {
                    var memory = computer.CurrentMemoryState;
                    buffer[0] = Parameters[0].GetValue(memory);

                    return new InstructionResult(true, "OutputInstruction passed.");
                }
                catch (Exception e)
                {
                    return new InstructionResult(false, e.Message);
                }
            }
        }

        public class JumpIfTrueInstruction : IInstruction
        {
            public JumpIfTrueInstruction(ref IntcodeComputer computer, List<int> parameterModes) : base(ref computer, parameterModes)
            {
            }

            public override InstructionType InstructionType => InstructionType.JumpIfTrue;

            public override int Size => 3;

            public override InstructionResult Execute(IntcodeComputer computer, int[] buffer)
            {
                try
                {
                    var memory = computer.CurrentMemoryState;
                    if (Parameters[0].GetValue(memory) != 0)
                        _next = Parameters[1].GetValue(memory);

                    return new InstructionResult(true, "JumpIfTrueInstruction passed.");
                }
                catch (Exception e)
                {
                    return new InstructionResult(false, e.Message);
                }
            }
        }

        public class JumpIfFalseInstruction : IInstruction
        {
            public JumpIfFalseInstruction(ref IntcodeComputer computer, List<int> parameterModes) : base(ref computer, parameterModes)
            {
            }

            public override InstructionType InstructionType => InstructionType.JumpIfFalse;

            public override int Size => 3;

            public override InstructionResult Execute(IntcodeComputer computer, int[] buffer)
            {
                try
                {
                    var memory = computer.CurrentMemoryState;
                    if (Parameters[0].GetValue(memory) == 0)
                        _next = Parameters[1].GetValue(memory);

                    return new InstructionResult(true, "JumpIfFalseInstruction passed.");
                }
                catch (Exception e)
                {
                    return new InstructionResult(false, e.Message);
                }
            }
        }

        public class LessThanInstruction : IInstruction
        {
            public LessThanInstruction(ref IntcodeComputer computer, List<int> parameterModes) : base(ref computer, parameterModes)
            {
            }

            public override InstructionType InstructionType => InstructionType.LessThan;

            public override int Size => 4;

            public override InstructionResult Execute(IntcodeComputer computer, int[] buffer)
            {
                try
                {
                    var memory = computer.CurrentMemoryState;
                    bool less = Parameters[0].GetValue(memory) < Parameters[1].GetValue(memory);
                    memory[Parameters[2].GetPosition(memory)] = less ? 1 : 0;

                    return new InstructionResult(true, "LessThanInstruction passed.");
                }
                catch (Exception e)
                {
                    return new InstructionResult(false, e.Message);
                }
            }
        }

        public class EqualsInstruction : IInstruction
        {
            public EqualsInstruction(ref IntcodeComputer computer, List<int> parameterModes) : base(ref computer, parameterModes)
            {
            }

            public override InstructionType InstructionType => InstructionType.Equals;

            public override int Size => 4;

            public override InstructionResult Execute(IntcodeComputer computer, int[] buffer)
            {
                try
                {
                    var memory = computer.CurrentMemoryState;
                    bool less = Parameters[0].GetValue(memory) == Parameters[1].GetValue(memory);
                    memory[Parameters[2].GetPosition(memory)] = less ? 1 : 0;

                    return new InstructionResult(true, "EqualsInstruction passed.");
                }
                catch (Exception e)
                {
                    return new InstructionResult(false, e.Message);
                }
            }
        }

        public class AdjustRelativeBaseInstruction : IInstruction
        {
            public AdjustRelativeBaseInstruction(ref IntcodeComputer computer, List<int> parameterModes) : base(ref computer, parameterModes)
            {
            }

            public override InstructionType InstructionType => InstructionType.AdjustRelativeBase;

            public override int Size => 2;

            public override InstructionResult Execute(IntcodeComputer computer, int[] buffer)
            {
                try
                {
                    computer.RelativeBase = Parameters[0].GetValue(computer.CurrentMemoryState);
                    return new InstructionResult(true, "AdjustRelativeBaseInstruction passed.");
                } catch (Exception e)
                {
                    return new InstructionResult(false, e.Message);
                }
                
            }
        }

        public class StopInstruction : IInstruction
        {
            public override InstructionType InstructionType { get => InstructionType.Stop; }

            public override int Size => 1;

            public StopInstruction(ref IntcodeComputer computer, List<int> parameterModes) : base(ref computer, parameterModes)
            {
            }

            public override InstructionResult Execute(IntcodeComputer computer, int[] buffer)
            {
                return new InstructionResult(false, "StopInstruction passed.");
            }
        }
    }
}
