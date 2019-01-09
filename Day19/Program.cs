﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day19
{
    class Program
    {
        private const string inputFile = "..\\..\\..\\input19.txt";

        static readonly Dictionary<string, Operation> opDict = new Dictionary<string, Operation>();
        static List<Instruction> instructions;

        static int instrPtr = 0;

        static void Main(string[] args)
        {
            Console.WriteLine("Day 19");
            Console.WriteLine("Star 1");
            Console.WriteLine("");

            List<Operation> operations = new List<Operation>()
            {
                new ADDR(),
                new ADDI(),
                new MULR(),
                new MULI(),
                new BANR(),
                new BANI(),
                new BORR(),
                new BORI(),
                new SETR(),
                new SETI(),
                new GTIR(),
                new GTRI(),
                new GTRR(),
                new EQIR(),
                new EQRI(),
                new EQRR()
            };

            foreach (Operation operation in operations)
            {
                opDict.Add(operation.Op, operation);
            }

            instructions = new List<Instruction>();

            int boundRegister = -1;
            foreach (string line in System.IO.File.ReadAllLines(inputFile))
            {
                if (line[0] == '#')
                {
                    boundRegister = int.Parse(line.Substring(4));
                    continue;
                }

                instructions.Add(new Instruction(line));
            }

            Registers reg = new Registers(0, 0, 0, 0, 0, 0);

            while (true)
            {
                instrPtr = reg[boundRegister];

                //Halt when instrPtr exceeds instructions
                if (instrPtr >= instructions.Count)
                {
                    break;
                }

                //Execute instruction
                reg = instructions[instrPtr].Execute(reg);

                //Increment Instr pointer
                reg = reg.SetReg(boundRegister, reg[boundRegister] + 1);
            }

            Console.WriteLine($"Register 0 At End: [{reg.A}]");
            Console.WriteLine("");
            Console.WriteLine("Star 2");
            Console.WriteLine("");
            Console.WriteLine("");

            Console.WriteLine("Printing out clearer instructions");
            for (instrPtr = 0; instrPtr < instructions.Count; instrPtr++)
            {
                instructions[instrPtr].Print();
            }

            Console.WriteLine("");
            Console.WriteLine("");

            instrPtr = 0;
            reg = new Registers(1, 0, 0, 0, 0, 0);

            for(int i = 0; i < 1000; i++)
            {
                instrPtr = reg[boundRegister];

                //Halt when instrPtr exceeds instructions
                if (instrPtr >= instructions.Count)
                {
                    break;
                }

                Console.Write(reg);
                //Execute instruction
                reg = instructions[instrPtr].Execute(reg, true);

                //Increment Instr pointer
                reg = reg.SetReg(boundRegister, reg[boundRegister] + 1);
            }
            Console.WriteLine($"B Reg: {reg.B}");
            Console.WriteLine("");

            //This program sums the divisors of the number that ends up in Reg 1.
            //In this case, 10,551,292
            int answer = 0;
            for(int i = 1; i <= reg.B; i++)
            {
                if (reg.B % i == 0)
                {
                    Console.WriteLine(i);
                    answer += i;
                }
            }
            Console.WriteLine("");


            Console.WriteLine($"Register 0 at the end: [{answer}]");
            Console.WriteLine("");
            Console.ReadKey();
        }

        public abstract class Operation
        {
            public abstract string Op { get; }

            public abstract Registers Invoke(Instruction instr, Registers input);

            public abstract void Print(Instruction instr);

            public static string RegName(int index)
            {
                switch (index)
                {
                    case 0: return "A";
                    case 1: return "B";
                    case 2: return "C";
                    case 3: return "D";
                    case 4: return "E";
                    case 5: return "F";
                    default: throw new Exception();
                }
            }
        }

        public class ADDR : Operation
        {
            public override string Op => "addr";

            public override Registers Invoke(Instruction instr, Registers input) =>
                input.SetReg(instr.C, input[instr.A] + input[instr.B]);

            public override void Print(Instruction instr) =>
                Console.WriteLine($"[{instrPtr:D2}] {RegName(instr.C)} = {RegName(instr.A)} + {RegName(instr.B)}");
        }

        public class ADDI : Operation
        {
            public override string Op => "addi";

            public override Registers Invoke(Instruction instr, Registers input) =>
                input.SetReg(instr.C, input[instr.A] + instr.B);

            public override void Print(Instruction instr) =>
                Console.WriteLine($"[{instrPtr:D2}] {RegName(instr.C)} = {RegName(instr.A)} + {instr.B}");
        }

        public class MULR : Operation
        {
            public override string Op => "mulr";

            public override Registers Invoke(Instruction instr, Registers input) =>
                input.SetReg(instr.C, input[instr.A] * input[instr.B]);

            public override void Print(Instruction instr) =>
                Console.WriteLine($"[{instrPtr:D2}] {RegName(instr.C)} = {RegName(instr.A)} * {RegName(instr.B)}");
        }

        public class MULI : Operation
        {
            public override string Op => "muli";

            public override Registers Invoke(Instruction instr, Registers input) =>
                input.SetReg(instr.C, input[instr.A] * instr.B);

            public override void Print(Instruction instr) =>
                Console.WriteLine($"[{instrPtr:D2}] {RegName(instr.C)} = {RegName(instr.A)} * {instr.B}");
        }

        public class BANR : Operation
        {
            public override string Op => "banr";

            public override Registers Invoke(Instruction instr, Registers input) =>
                input.SetReg(instr.C, input[instr.A] & input[instr.B]);

            public override void Print(Instruction instr) =>
                Console.WriteLine($"[{instrPtr:D2}] {RegName(instr.C)} = {RegName(instr.A)} & {RegName(instr.B)}");
        }

        public class BANI : Operation
        {
            public override string Op => "bani";

            public override Registers Invoke(Instruction instr, Registers input) =>
                input.SetReg(instr.C, input[instr.A] & instr.B);

            public override void Print(Instruction instr) =>
                Console.WriteLine($"[{instrPtr:D2}] {RegName(instr.C)} = {RegName(instr.A)} & {instr.B}");
        }

        public class BORR : Operation
        {
            public override string Op => "borr";

            public override Registers Invoke(Instruction instr, Registers input) =>
                input.SetReg(instr.C, input[instr.A] | input[instr.B]);

            public override void Print(Instruction instr) =>
                Console.WriteLine($"[{instrPtr:D2}] {RegName(instr.C)} = {RegName(instr.A)} | {RegName(instr.B)}");
        }

        public class BORI : Operation
        {
            public override string Op => "bori";

            public override Registers Invoke(Instruction instr, Registers input) =>
                input.SetReg(instr.C, input[instr.A] | instr.B);

            public override void Print(Instruction instr) =>
                Console.WriteLine($"[{instrPtr:D2}] {RegName(instr.C)} = {RegName(instr.A)} | {instr.B}");
        }

        public class SETR : Operation
        {
            public override string Op => "setr";

            public override Registers Invoke(Instruction instr, Registers input) =>
            input.SetReg(instr.C, input[instr.A]);

            public override void Print(Instruction instr) =>
                Console.WriteLine($"[{instrPtr:D2}] {RegName(instr.C)} = {RegName(instr.A)}");
        }

        public class SETI : Operation
        {
            public override string Op => "seti";

            public override Registers Invoke(Instruction instr, Registers input) =>
            input.SetReg(instr.C, instr.A);

            public override void Print(Instruction instr) =>
                Console.WriteLine($"[{instrPtr:D2}] {RegName(instr.C)} = {instr.A}");
        }

        public class GTIR : Operation
        {
            public override string Op => "gtir";

            public override Registers Invoke(Instruction instr, Registers input) =>
                input.SetReg(instr.C, instr.A > input[instr.B] ? 1 : 0);

            public override void Print(Instruction instr) =>
                Console.WriteLine($"[{instrPtr:D2}] {RegName(instr.C)} = {instr.A} > {RegName(instr.B)}");
        }

        public class GTRI : Operation
        {
            public override string Op => "gtri";

            public override Registers Invoke(Instruction instr, Registers input) =>
                input.SetReg(instr.C, input[instr.A] > instr.B ? 1 : 0);

            public override void Print(Instruction instr) =>
                Console.WriteLine($"[{instrPtr:D2}] {RegName(instr.C)} = {RegName(instr.A)} > {instr.B}");
        }

        public class GTRR : Operation
        {
            public override string Op => "gtrr";

            public override Registers Invoke(Instruction instr, Registers input) =>
                input.SetReg(instr.C, input[instr.A] > input[instr.B] ? 1 : 0);

            public override void Print(Instruction instr) =>
                Console.WriteLine($"[{instrPtr:D2}] {RegName(instr.C)} = {RegName(instr.A)} > {RegName(instr.B)}");
        }

        public class EQIR : Operation
        {
            public override string Op => "eqir";

            public override Registers Invoke(Instruction instr, Registers input) =>
                input.SetReg(instr.C, instr.A == input[instr.B] ? 1 : 0);

            public override void Print(Instruction instr) =>
                Console.WriteLine($"[{instrPtr:D2}] {RegName(instr.C)} = {instr.A} == {RegName(instr.B)}");
        }

        public class EQRI : Operation
        {
            public override string Op => "eqri";

            public override Registers Invoke(Instruction instr, Registers input) =>
                input.SetReg(instr.C, input[instr.A] == instr.B ? 1 : 0);

            public override void Print(Instruction instr) =>
                Console.WriteLine($"[{instrPtr:D2}] {RegName(instr.C)} = {RegName(instr.A)} == {instr.B}");
        }

        public class EQRR : Operation
        {
            public override string Op => "eqrr";

            public override Registers Invoke(Instruction instr, Registers input) =>
                input.SetReg(instr.C, input[instr.A] == input[instr.B] ? 1 : 0);

            public override void Print(Instruction instr) =>
                Console.WriteLine($"[{instrPtr:D2}] {RegName(instr.C)} = {RegName(instr.A)} == {RegName(instr.B)}");
        }



        public readonly struct Instruction
        {
            public readonly Operation Op;
            public readonly int A;
            public readonly int B;
            public readonly int C;

            public Instruction(string op, int a, int b, int c)
            {
                Op = opDict[op];
                A = a;
                B = b;
                C = c;
            }
            public Instruction(Operation op, int a, int b, int c)
            {
                Op = op;
                A = a;
                B = b;
                C = c;
            }

            public Instruction(string line)
            {
                string[] splitLine = line.Split(' ');
                int[] parsed = splitLine.Skip(1).Select(int.Parse).ToArray();

                Op = opDict[splitLine[0]];
                A = parsed[0];
                B = parsed[1];
                C = parsed[2];
            }

            public void Print() => Op.Print(this);

            public Registers Execute(Registers input, bool verbose = false)
            {
                if (verbose)
                {
                    Op.Print(this);
                }


                return Op.Invoke(this, input);
            }

            public override string ToString() => $"{Op.Op} {A:X1}{B:X1}{C:X1}";

        }

        public readonly struct Registers
        {
            public readonly int A;
            public readonly int B;
            public readonly int C;
            public readonly int D;
            public readonly int E;
            public readonly int F;

            public int this[int i]
            {
                get
                {
                    switch (i)
                    {
                        case 0: return A;
                        case 1: return B;
                        case 2: return C;
                        case 3: return D;
                        case 4: return E;
                        case 5: return F;
                        default: return 0;
                    }
                }
            }

            public Registers(int a, int b, int c, int d, int e, int f)
            {
                A = a;
                B = b;
                C = c;
                D = d;
                E = e;
                F = f;
            }

            public static bool operator ==(Registers reg1, Registers reg2)
            {
                return reg1.A == reg2.A && reg1.B == reg2.B && reg1.C == reg2.C &&
                    reg1.D == reg2.D && reg1.E == reg2.E && reg1.F == reg2.F;
            }

            public static bool operator !=(Registers reg1, Registers reg2)
            {
                return reg1.A != reg2.A || reg1.B != reg2.B || reg1.C != reg2.C ||
                    reg1.D != reg2.D || reg1.E != reg2.E || reg1.F != reg2.F;
            }

            public Registers SetReg(int reg, int val) =>
                new Registers(
                    a: reg == 0 ? val : A,
                    b: reg == 1 ? val : B,
                    c: reg == 2 ? val : C,
                    d: reg == 3 ? val : D,
                    e: reg == 4 ? val : E,
                    f: reg == 5 ? val : F);

            public override string ToString() => $"[{A}, {B}, {C}, {D}, {E}, {F}]";
        }
    }
}
