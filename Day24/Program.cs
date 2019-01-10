using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day24
{
    class Program
    {
        private const string inputFile = "..\\..\\..\\input24.txt";

        static void Main(string[] args)
        {
            Console.WriteLine("Day 24");
            Console.WriteLine("Star 1");
            Console.WriteLine("");

            //Each group also has an effective power:
            //the number of units in that group multiplied by their attack damage.

            //During the target selection phase, each group attempts to choose one target.
            //In decreasing order of effective power, groups choose their targets; 
            //in a tie, the group with the higher initiative chooses first.
            //The attacking group chooses to target the group in the enemy army to which 
            //it would deal the most damage
            //(after accounting for weaknesses and immunities, but not accounting for whether the
            //defending group has enough units to actually receive all of that damage).

            //If an attacking group is considering two defending groups to which it would deal 
            //equal damage, it chooses to target the defending group with the 
            //largest effective power;
            //if there is still a tie, it chooses the defending group with the highest initiative.
            //If it cannot deal any defending groups damage, it does not choose a target.
            //Defending groups can only be chosen as a target by one attacking group.


            //At the end of the target selection phase,
            //each group has selected zero or one groups to attack, 
            //and each group is being attacked by zero or one groups.

            //By default, an attacking group would deal damage equal to its effective power 
            //to the defending group. 
            //However, if the defending group is immune to the attacking group's attack type, 
            //the defending group instead takes no damage;
            //if the defending group is weak to the attacking group's attack type, 
            //the defending group instead takes double damage.

            string[] input = System.IO.File.ReadAllLines(inputFile);

            List<UnitGroup> rawUnits = new List<UnitGroup>();

            bool immuneSystem = true;

            foreach (string line in input)
            {
                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }
                else if (line == "Immune System:")
                {
                    immuneSystem = true;
                    continue;
                }
                else if (line == "Infection:")
                {
                    immuneSystem = false;
                    continue;
                }

                rawUnits.Add(new UnitGroup(line, immuneSystem));
            }

            List<UnitGroup> units = new List<UnitGroup>();

            foreach (UnitGroup unit in rawUnits)
            {
                units.Add(new UnitGroup(unit, 1570));
            }

            Comparer<UnitGroup> targettingComparer = new UnitGroupTargetSelectionOrder();
            Comparer<UnitGroup> turnComparer = new UnitGroupTurnSelectionOrder();

            Dictionary<UnitGroup, UnitGroup> targetDictionary = new Dictionary<UnitGroup, UnitGroup>();
            while (true)
            {
                units.Sort(targettingComparer);

                foreach (UnitGroup unit in units)
                {
                    UnitGroup selectedTarget = null;
                    int damage = 0;

                    foreach (UnitGroup target in units)
                    {
                        if (target.ImmuneSystem == unit.ImmuneSystem || targetDictionary.ContainsValue(target))
                        {
                            //Skip friendlies, self, and already-targeted units
                            continue;
                        }

                        int potentialDamage = unit.CalculateDamage(target);
                        if (potentialDamage > damage)
                        {
                            selectedTarget = target;
                            damage = potentialDamage;
                        }
                        else if (potentialDamage > 0 && potentialDamage == damage)
                        {
                            if (target.EffectivePower > selectedTarget.EffectivePower ||
                                (target.EffectivePower == selectedTarget.EffectivePower &&
                                target.Initiative > selectedTarget.Initiative))
                            {
                                selectedTarget = target;
                            }
                        }
                    }

                    if (selectedTarget != null)
                    {
                        targetDictionary.Add(unit, selectedTarget);
                    }
                }

                units.Sort(turnComparer);

                foreach (UnitGroup unit in units)
                {
                    if (!unit.Alive || !targetDictionary.ContainsKey(unit))
                    {
                        continue;
                    }

                    UnitGroup target = targetDictionary[unit];
                    target.TakeDamage(unit.CalculateDamage(target));
                }
                targetDictionary.Clear();

                units.RemoveAll(x => !x.Alive);

                int immuneSystemCount = units.Count(x => x.ImmuneSystem);
                if (immuneSystemCount == units.Count || immuneSystemCount == 0)
                {
                    //End when one side is dead
                    break;
                }

                //Console.WriteLine($"Immune:[{string.Join(",", units.Where(x => x.ImmuneSystem).Select(x => x.UnitCount.ToString()).ToArray())}]  Infection:[{string.Join(",", units.Where(x => !x.ImmuneSystem).Select(x => x.UnitCount.ToString()).ToArray())}]");

            }

            Console.WriteLine($"{(units[0].ImmuneSystem ? "ImmuneSystem" : "Infection")} wins with: {units.Sum(x => x.UnitCount)} units remaining.");
            Console.WriteLine("");

            Console.WriteLine("Star 2");
            Console.WriteLine("");

            bool growing = true;
            bool done = false;

            int boost = 2;
            int boostLB = 1;
            int boostUB = 2;

            while(!done)
            {
                Console.Write($"Immune Boost: {boost}");

                //Reset
                units.Clear();
                foreach (UnitGroup unit in rawUnits)
                {
                    units.Add(new UnitGroup(unit, boost));
                }

                string lastRound = "";

                //Simulate battle
                while (true)
                {
                    units.Sort(targettingComparer);

                    foreach (UnitGroup unit in units)
                    {
                        UnitGroup selectedTarget = null;
                        int damage = 0;

                        foreach (UnitGroup target in units)
                        {
                            if (target.ImmuneSystem == unit.ImmuneSystem || targetDictionary.ContainsValue(target))
                            {
                                //Skip friendlies, self, and already-targeted units
                                continue;
                            }

                            int potentialDamage = unit.CalculateDamage(target);
                            if (potentialDamage > damage)
                            {
                                selectedTarget = target;
                                damage = potentialDamage;
                            }
                            else if (potentialDamage > 0 && potentialDamage == damage)
                            {
                                if (target.EffectivePower > selectedTarget.EffectivePower ||
                                    (target.EffectivePower == selectedTarget.EffectivePower &&
                                    target.Initiative > selectedTarget.Initiative))
                                {
                                    selectedTarget = target;
                                }
                            }
                        }

                        if (selectedTarget != null)
                        {
                            targetDictionary.Add(unit, selectedTarget);
                        }
                    }

                    units.Sort(turnComparer);

                    foreach (UnitGroup unit in units)
                    {
                        if (!unit.Alive || !targetDictionary.ContainsKey(unit))
                        {
                            continue;
                        }

                        UnitGroup target = targetDictionary[unit];

                        if (!target.Alive)
                        {
                            continue;
                        }

                        target.TakeDamage(unit.CalculateDamage(target));
                    }
                    targetDictionary.Clear();

                    units.RemoveAll(x => !x.Alive);

                    int immuneSystemCount = units.Count(x => x.ImmuneSystem);
                    if (immuneSystemCount == units.Count || immuneSystemCount == 0)
                    {
                        //End when one side is dead
                        break;
                    }

                    string thisRound = $"Immune:[{string.Join(",", units.Where(x => x.ImmuneSystem).Select(x => x.UnitCount.ToString()).ToArray())}]  Infection:[{string.Join(",", units.Where(x => !x.ImmuneSystem).Select(x => x.UnitCount.ToString()).ToArray())}]";

                    if (thisRound == lastRound)
                    {
                        //End if there is a stalemate
                        break;
                    }

                    lastRound = thisRound;

                }

                bool immuneWon = units.Count(x => x.ImmuneSystem) == units.Count;

                Console.WriteLine(immuneWon ? " Won" : " Loss");

                if (growing)
                {
                    if (immuneWon)
                    {
                        growing = false;
                        //Move to binary search

                        boost = (boostUB + boostLB) / 2;
                    }
                    else
                    {
                        boostLB = boostUB;
                        boostUB *= 2;
                        boost = boostUB;
                    }
                }
                else
                {
                    if (immuneWon)
                    {
                        if (boostUB == boostLB)
                        {
                            break;
                        }

                        boostUB = boost;
                    }
                    else
                    {
                        boostLB = boost + 1;
                    }

                    boost = (boostUB + boostLB) / 2;
                }
            }


            Console.WriteLine($"{(units[0].ImmuneSystem ? "ImmuneSystem" : "Infection")} wins with: {units.Sum(x => x.UnitCount)} units remaining.");
            Console.WriteLine("");
            Console.ReadKey();

        }

        public class UnitGroupTargetSelectionOrder : Comparer<UnitGroup>
        {
            public override int Compare(UnitGroup x, UnitGroup y)
            {
                if (x.EffectivePower.CompareTo(y.EffectivePower) != 0)
                {
                    return -1 * x.EffectivePower.CompareTo(y.EffectivePower);
                }
                else if (x.Initiative.CompareTo(y.Initiative) != 0)
                {
                    //Higher initiative goes first if power is equal
                    return -1 * x.Initiative.CompareTo(y.Initiative);
                }

                return 0;
            }
        }
        public class UnitGroupTurnSelectionOrder : Comparer<UnitGroup>
        {
            public override int Compare(UnitGroup x, UnitGroup y)
            {
                if (x.Initiative.CompareTo(y.Initiative) != 0)
                {
                    //Higher initiative goes first
                    return -1 * x.Initiative.CompareTo(y.Initiative);
                }

                return 0;
            }
        }

        public class UnitGroup : IComparable<UnitGroup>
        {
            public int UnitCount { get; set; }
            public int HP { get; }
            public int Initiative { get; }
            public int Damage { get; }
            public string DamageType { get; }

            public Dictionary<string, int> DamageMultipliers { get; } = new Dictionary<string, int>();

            public bool ImmuneSystem { get; }

            public int EffectivePower => UnitCount * Damage;

            public bool Alive => UnitCount > 0;

            //Psuedo copy-constructor
            public UnitGroup(UnitGroup source, int boost = 0)
            {
                UnitCount = source.UnitCount;
                HP = source.HP;
                Initiative = source.Initiative;

                DamageType = source.DamageType;

                DamageMultipliers = source.DamageMultipliers;
                ImmuneSystem = source.ImmuneSystem;

                if (ImmuneSystem)
                {
                    Damage = source.Damage + boost;
                }
                else
                {
                    Damage = source.Damage;
                }
            }

            public UnitGroup(string line, bool immuneSystem)
            {
                string[] words = line.Split(' ');

                UnitCount = int.Parse(words[0]);
                HP = int.Parse(words[4]);
                Initiative = int.Parse(words[words.Length - 1]);
                Damage = int.Parse(words[words.Length - 6]);
                DamageType = words[words.Length - 5];

                ImmuneSystem = immuneSystem;

                int openIndex = line.IndexOf('(');
                int closeIndex = line.IndexOf(')');

                if (openIndex != -1)
                {
                    openIndex += 1;

                    string resistances = line.Substring(openIndex, closeIndex - openIndex);
                    string[] resistanceWords = resistances.Split(new char[] { ' ', ',', ';' }, StringSplitOptions.RemoveEmptyEntries);

                    bool immuneState = true;

                    for (int i = 0; i < resistanceWords.Length; i++)
                    {
                        switch (resistanceWords[i])
                        {
                            case "immune":
                                //Set state to Immunities
                                immuneState = true;
                                break;

                            case "weak":
                                //Set state to Weaknesses
                                immuneState = false;
                                break;

                            case "to":
                                //Skip meaningless word
                                break;

                            case "bludgeoning":
                            case "radiation":
                            case "slashing":
                            case "cold":
                            case "fire":
                                DamageMultipliers.Add(resistanceWords[i], immuneState ? 0 : 2);
                                break;

                            default: throw new Exception($"Unexpected Keyword: {resistanceWords[i]}");
                        }
                    }
                }


            }

            public int CalculateDamage(UnitGroup attacked) =>
                EffectivePower * attacked.GetDamageMultiplier(DamageType);

            public int GetDamageMultiplier(string damageType)
            {
                if (DamageMultipliers.TryGetValue(damageType, out int multiplier))
                {
                    return multiplier;
                }

                return 1;
            }

            public void TakeDamage(int damage)
            {
                UnitCount -= damage / HP;
            }

            int IComparable<UnitGroup>.CompareTo(UnitGroup other) =>
                EffectivePower.CompareTo(other.EffectivePower);
        }
    }
}
