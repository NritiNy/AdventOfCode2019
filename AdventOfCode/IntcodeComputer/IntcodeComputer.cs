using System;

namespace AdventOfCode
{
    namespace Intcodes
    {
        public class IntcodeComputer
        {
            public int[] Input { get; set; }

            public int[] CurrentMemoryState { get; private set; }

            public int Output { get => CurrentMemoryState[0]; }

            public IntcodeComputer(int[] input = null)
            {
                Input = (int[])input.Clone() ?? new int[0];
                Reset();
            }

            public void Run()
            {
                int instructionPointer = 0;
                for (; instructionPointer < CurrentMemoryState.Length;)
                {
                    var instruction = IInstruction.GetInstruction(instructionOpCode: CurrentMemoryState[instructionPointer], position: instructionPointer);

                    int[] buffer = new int[1] { 0 };
                    if (instruction.InstructionType == InstructionType.Input)
                    {
                        Console.WriteLine("Input: ");
                        try
                        {
                            buffer[0] = int.Parse(Console.ReadLine());
                        }
                        catch
                        {
                            Console.WriteLine("Failed to parse input.");
                            break;
                        }
                    }

                    var result = instruction.Execute(CurrentMemoryState, buffer);
                    if (!result)
                    {
                        if (instruction.InstructionType != InstructionType.Stop)
                        {
                            Console.WriteLine($"Instruction '{instruction.InstructionType}' failed.");
                            Console.WriteLine(result.Message);
                        }
                        break;
                    }

                    if (instruction.InstructionType == InstructionType.Output)
                        Console.WriteLine($"Output: {buffer[0]}");

                    instructionPointer = instruction.Next;
                }
            }

            public void Reset()
            {
                CurrentMemoryState = (int[])Input.Clone();
            }
        }
    }
}
