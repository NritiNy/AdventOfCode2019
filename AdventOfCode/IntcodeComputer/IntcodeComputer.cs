using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode
{
    namespace Intcodes
    {
        public enum ExitCode { SUCCESS = 0, ERROR = 1};

        public enum InputMode { Manual, Static, Automatic };
        public enum OutputMode { Internal, External};

        public class IntcodeComputer
        {
            public int[] Programm { get; set; }

            public int[] CurrentMemoryState { get; private set; }
            public int InstructionPointer { get; private set; } = 0;
            public int RelativeBase { get; set; } = 0;

            public List<int> Input { get; set; } = new List<int>();
            private int InputPointer { get; set; } = 0;
            public InputMode InputMode { get; set; } = InputMode.Manual;

            public List<int> Output { get; private set; } = new List<int>();
            private int OutputPointer { get; set; } = 0;
            public OutputMode OutputMode { get; set; } = OutputMode.External;

            public IntcodeComputer(int[] input = null)
            {
                Programm = (int[])input.Clone() ?? new int[0];
                Reset();
            }

            public ExitCode Run()
            {
                for (; InstructionPointer < CurrentMemoryState.Length;)
                {
                    var instruction = IInstruction.GetInstruction(instructionOpCode: CurrentMemoryState[InstructionPointer], position: InstructionPointer);

                    int[] buffer = new int[1] { 0 };
                    if (instruction.InstructionType == InstructionType.Input)
                    {
                        if(InputMode == InputMode.Manual)
                        {
                            Console.WriteLine("Input: ");
                            try
                            {
                                buffer[0] = int.Parse(Console.ReadLine());
                            }
                            catch
                            {
                                Console.WriteLine("Failed to parse input.");
                                return ExitCode.ERROR;
                            }
                        }
                        else
                        {
                            try
                            {
                                buffer[0] = Input[InputPointer];
                                if (InputMode == InputMode.Automatic)
                                    ++InputPointer;
                            } catch
                            {
                                Console.WriteLine($"No more input provided. (Only {Input.Count} inputs were provided.)");
                                return ExitCode.ERROR;
                            }
                            
                        }
                    }

                    var result = instruction.Execute(this, buffer);
                    if (!result)
                    {
                        if (instruction.InstructionType != InstructionType.Stop)
                        {
                            Console.WriteLine($"Instruction '{instruction.InstructionType}' failed.");
                            Console.WriteLine(result.Message);
                            return ExitCode.ERROR;
                        }
                        else
                            return ExitCode.SUCCESS;
                    }

                    if (instruction.InstructionType == InstructionType.Output)
                    {
                        if (OutputMode == OutputMode.External)
                            Console.WriteLine($"Output: {buffer[0]}");
                        else
                            Output.Add(buffer[0]);
                    }


                    InstructionPointer = instruction.Next;
                }
                return ExitCode.SUCCESS;
            }

            public void Reset()
            {
                CurrentMemoryState = (int[])Programm.Clone();
                InstructionPointer = 0;
                Input = new List<int>();
                InputPointer = 0;
                Output = new List<int>();
                OutputPointer = 0;
            }


            public IInstruction GetInstruction(int instructionOpCode, int position)
            {
                var code = instructionOpCode % 100;
                instructionOpCode = (instructionOpCode - code) / 100;

                Type instruction = IInstruction.AvailableInstructions[code];

                var paramModes = ("" + instructionOpCode).ToCharArray().ToList().ConvertAll(c => int.Parse("" + c));
                paramModes.Reverse();

                object[] args = { this, paramModes };
                return (IInstruction)Activator.CreateInstance(instruction, args);
            }
        }
    }
}
