using System;
using System.Collections.Generic;

namespace AdventOfCode
{ 
    public class IntcodeComputer
    {
        private int[] _input;
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
                if (!instruction.Execute(CurrentMemoryState))
                    break;

                instructionPointer += instruction.Size;
            }
        }

        public void Reset()
        {
            CurrentMemoryState = (int[])Input.Clone();
        }
    }
}
