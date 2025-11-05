// <copyright file="Instruction.cs" company="MjrTom">
// Copyright (c) Joseph Bridgewater. All rights reserved.
// </copyright>

namespace MiscUtil.Compression.Vcdiff
{
    /// <summary>
    /// Contains the information for a single instruction
    /// </summary>
    internal readonly struct Instruction
    {
        private readonly InstructionType type;
        internal readonly InstructionType Type => this.type;

        private readonly byte size;
        internal readonly byte Size => this.size;

        private readonly byte mode;
        internal readonly byte Mode => this.mode;

        internal Instruction(InstructionType type, byte size, byte mode)
        {
            this.type = type;
            this.size = size;
            this.mode = mode;
        }

    }
}

