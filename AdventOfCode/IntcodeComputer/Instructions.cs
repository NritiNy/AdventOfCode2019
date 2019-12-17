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

            public long RelativeBase { get; private set; }

            public ParameterMode Mode { get; private set; }

            public Parameter(int adress, long relBase, ParameterMode mode)
            {
                Adress = adress;
                RelativeBase = relBase;
                Mode = mode;
            }

            public long GetValue(ref IntcodeComputer computer)
            {
                if (Adress > computer.CurrentMemoryState().Length)
                    computer.ResizeMemory(Adress);

                if (Mode == ParameterMode.Immediate)
                {
                    return computer.CurrentMemoryState()[Adress];
                }
                else if(Mode == ParameterMode.Position || Mode == ParameterMode.Relative)
                {
                    var adress = GetPosition(ref computer);
                    return computer.CurrentMemoryState()[adress];
                }
                else
                {
                    throw new NotImplementedException($"Unexpected Parameter Mode: {Mode}");
                }
            }

            public long GetPosition(ref IntcodeComputer computer)
            {
                if (Adress > computer.CurrentMemoryState().Length)
                    computer.ResizeMemory(Adress);
                if (RelativeBase + computer.CurrentMemoryState()[Adress] > computer.CurrentMemoryState().Length)
                    computer.ResizeMemory((int)(RelativeBase + computer.CurrentMemoryState()[Adress]));

                if (Mode == ParameterMode.Position)
                {
                    var pos = computer.CurrentMemoryState()[Adress];
                    if (pos >= computer.CurrentMemoryState().Length)
                        computer.ResizeMemory((int)pos);
                    return pos;
                }
                else if (Mode == ParameterMode.Relative)
                {
                    var pos = RelativeBase + computer.CurrentMemoryState()[Adress];
                    if (pos >= computer.CurrentMemoryState().Length)
                        computer.ResizeMemory((int)pos);
                    return pos;
                }
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
            public static Dictionary<long, Type> AvailableInstructions { get; } = InitializeAvailableInstructions();

            private static Dictionary<long, Type> InitializeAvailableInstructions()
            {
                var ret = new Dictionary<long, Type>();

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

            public abstract InstructionResult Execute(IntcodeComputer computer, long[] buffer);
        }

        public class AdditionInstruction : IInstruction
        {
            public override InstructionType InstructionType { get => InstructionType.Addition; }

            public override int Size => 4;

            public AdditionInstruction(ref IntcodeComputer computer, List<int> parameterModes) : base(ref computer, parameterModes)
            {
            }

            public override InstructionResult Execute(IntcodeComputer computer, long[] buffer)
            {
                try
                {
                    var in1 = Parameters[0].GetValue(ref computer);
                    var in2 = Parameters[1].GetValue(ref computer);
                    var dest = Parameters[2].GetPosition(ref computer);

                    computer.CurrentMemoryState()[dest] = in1 + in2;

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

            public override InstructionResult Execute(IntcodeComputer computer, long[] buffer)
            {
                /*try
                {*/
                    var in1 = Parameters[0].GetValue(ref computer);
                    var in2 = Parameters[1].GetValue(ref computer);
                    var dest = Parameters[2].GetPosition(ref computer);

                    computer.CurrentMemoryState()[dest] = in1 * in2;

                    return new InstructionResult(true, "MiltiplicationInstruction passed.");
                /*}
                catch (Exception e)
                {
                    return new InstructionResult(false, e.Message);
                }*/
            }
        }

        public class InputInstruction : IInstruction
        {
            public override InstructionType InstructionType { get => InstructionType.Input; }

            public InputInstruction(ref IntcodeComputer computer, List<int> parameterModes) : base(ref computer, parameterModes)
            {
            }

            public override int Size => 2;

            public override InstructionResult Execute(IntcodeComputer computer, long[] buffer)
            {
                try
                {
                    var dest = Parameters[0].GetPosition(ref computer);

                    var input = buffer[0];
                    computer.CurrentMemoryState()[dest] = input;

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

            public override InstructionResult Execute(IntcodeComputer computer, long[] buffer)
            {
                try
                {
                    buffer[0] = Parameters[0].GetValue(ref computer);

                    return new InstructionResult(true, "OutputInstruction passed.");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
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

            public override InstructionResult Execute(IntcodeComputer computer, long[] buffer)
            {
                try
                {
                    if (Parameters[0].GetValue(ref computer) != 0)
                        _next = (int)Parameters[1].GetValue(ref computer);

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

            public override InstructionResult Execute(IntcodeComputer computer, long[] buffer)
            {
                try
                {
                    var memory = computer.CurrentMemoryState();
                    if (Parameters[0].GetValue(ref computer) == 0)
                        _next = (int)Parameters[1].GetValue(ref computer);

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

            public override InstructionResult Execute(IntcodeComputer computer, long[] buffer)
            {
                try
                {
                    bool less = Parameters[0].GetValue(ref computer) < Parameters[1].GetValue(ref computer);
                    var dest = Parameters[2].GetPosition(ref computer);

                    computer.CurrentMemoryState()[dest] = less ? 1 : 0;

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

            public override InstructionResult Execute(IntcodeComputer computer, long[] buffer)
            {
                try
                {
                    bool equal = Parameters[0].GetValue(ref computer) == Parameters[1].GetValue(ref computer);
                    var dest = Parameters[2].GetPosition(ref computer);

                    computer.CurrentMemoryState()[dest] = equal ? 1 : 0;

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

            public override InstructionResult Execute(IntcodeComputer computer, long[] buffer)
            {
                try
                {
                    computer.RelativeBase += (int)Parameters[0].GetValue(ref computer);
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

            public override InstructionResult Execute(IntcodeComputer computer, long[] buffer)
            {
                return new InstructionResult(false, "StopInstruction passed.");
            }
        }
    }
}
