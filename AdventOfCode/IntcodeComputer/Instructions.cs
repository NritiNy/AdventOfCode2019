using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AdventOfCode
{
    namespace Intcodes
    {
        public enum InstructionType { Addition = 1, Multiplication = 2, Input = 3, Output = 4, Stop = 99 };
        public enum ParameterMode { Position, Immediate };
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

            public ParameterMode Mode { get; private set; }

            public Parameter(int adress, ParameterMode mode)
            {
                Adress = adress;
                Mode = mode;
            }

            public int GetValue(int[] memory)
            {
                if(Mode == ParameterMode.Immediate)
                {
                    return memory[Adress];
                } else if (Mode == ParameterMode.Position)
                {
                    return memory[memory[Adress]];
                } else
                {
                    throw new NotImplementedException($"Unexpected Parameter Mode: {Mode}");
                }
            }

            public int GetPosition(int[] memory)
            {
                if(Mode == ParameterMode.Position)
                    return memory[Adress];
                else
                    throw new NotImplementedException($"Parameter Mode must be {ParameterMode.Position}");
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
                    ret.Add((int)type, Assembly.GetExecutingAssembly().GetTypes().First(t => t.Name == type.ToString()+"Instruction"));
                }

                return ret;
            }


            public int Adress { get; private set; }

            public abstract InstructionType InstructionType { get; }

            public abstract int Size { get; }

            public Parameter[] Parameters { get; private set; }

            protected IInstruction(int position, List<int> parameterModes)
            {
                Adress = position;
                Parameters = new Parameter[Size-1];

                parameterModes = parameterModes ?? new List<int>(Size);
                for(int i = 1; i < Size; ++i)
                {
                    if (i < parameterModes.Count)
                        Parameters[i-1] = new Parameter(Adress + i, (ParameterMode)parameterModes[i]);
                    else
                        Parameters[i-1] = new Parameter(Adress + i, 0);
                    
                }
            }

            public abstract InstructionResult Execute(int[] memory, int[] buffer);


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

            public AdditionInstruction(int position, List<int> parameterModes) : base(position, parameterModes)
            {
            }

            public override InstructionResult Execute(int[] memory, int[] buffer)
            {
                try
                {
                    //memory[Parameters[2].GetPosition(memory)] = Parameters[0].GetValue(memory) + Parameters[1].GetValue(memory);
                    memory[memory[Adress + 3]] = (Parameters[0].Mode == ParameterMode.Immediate ? memory[Adress + 1] : memory[memory[Adress + 1]])
                        + (Parameters[1].Mode == ParameterMode.Immediate ? memory[Adress + 2] : memory[memory[Adress + 2]]);

                    return new InstructionResult(true, "AdditionInstruction passed.");
                }
                catch(Exception e)
                {
                    Console.WriteLine(e);
                    return new InstructionResult(false, e.Message);
                }
            }
        }

        public class MultiplicationInstruction : IInstruction
        {
            public override InstructionType InstructionType { get => InstructionType.Multiplication; }

            public override int Size => 4;

            public MultiplicationInstruction(int position, List<int> parameterModes) : base(position, parameterModes)
            {
            }

            public override InstructionResult Execute(int[] memory, int[] buffer)
            {
                try
                {
                    var input1 = memory[Adress + 1];
                    var input2 = memory[Adress + 2];
                    var destination = memory[Adress + 3];

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

            public InputInstruction(int position, List<int> parameterModes) : base(position, parameterModes)
            {
            }

            public override int Size => 2;

            public override InstructionResult Execute(int[] memory, int[] buffer)
            {
                try
                {
                    var input = buffer[0];
                    memory[Parameters[0].GetPosition(memory)] = input;

                    return new InstructionResult(true, "InputInstruction passed.");
                } catch (Exception e)
                {
                    return new InstructionResult(false, e.Message);
                }
            }
        }

        public class OutputInstruction : IInstruction
        {
            public override InstructionType InstructionType { get => InstructionType.Output; }

            public OutputInstruction(int position, List<int> parameterModes) : base(position, parameterModes)
            {
            }

            public override int Size => 2;

            public override InstructionResult Execute(int[] memory, int[] buffer)
            {
                try
                {
                    buffer[0] = Parameters[0].GetPosition(memory);

                    return new InstructionResult(true, "OutputInstruction passed.");
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

            public StopInstruction(int position, List<int> parameterModes) : base(position, parameterModes)
            {
            }

            public override InstructionResult Execute(int[] memory, int[] buffer)
            {
                return new InstructionResult(false, "StopInstruction passed.");
            }
        }
    }
}
