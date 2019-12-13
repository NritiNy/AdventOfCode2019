using System;

namespace AdventOfCode
{
    public abstract class IInstruction
    {
        public int Adress { get; private set; }

        public int Size { get; protected set; }

        protected IInstruction(int position)
        {
            Adress = position;
        }

        public static IInstruction GetInstruction(int instructionOpCode, int position)
        {
            switch(instructionOpCode) {
                case 1:
                    return new AdditionInstruction(position);
                case 2:
                    return new MultiplikationInstruction(position);
                case 99:
                    return new StopInstruction(position);
                default:
                    throw new NotImplementedException($"An operation for code {instructionOpCode} is not implemented yet.");
            }
        }

        public abstract bool Execute(int[] memory);
    }

    public class AdditionInstruction : IInstruction
    {
        public AdditionInstruction(int position) : base(position)
        {
            Size = 4;
        }

        public override bool Execute(int[] memory)
        {
            try
            {
                var input1 = memory[Adress + 1];
                var input2 = memory[Adress + 2];
                var destination = memory[Adress + 3];

                memory[destination] = memory[input1] + memory[input2];

                return true;
            } catch
            {
                return false;
            }
        }
    }

    public class MultiplikationInstruction : IInstruction
    {
        public MultiplikationInstruction(int position) : base(position)
        {
            Size = 4;
        }

        public override bool Execute(int[] memory)
        {
            try
            {
                var input1 = memory[Adress + 1];
                var input2 = memory[Adress + 2];
                var destination = memory[Adress + 3];

                memory[destination] = memory[input1] * memory[input2];

                return true;
            }
            catch
            {
                return false;
            }
        }
    }

    public class StopInstruction : IInstruction
    {
        public StopInstruction(int position) : base(position)
        {
            Size = 1;
        }

        public override bool Execute(int[] memory)
        {
            return false;
        }
    }
}
