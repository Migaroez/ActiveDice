using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public struct DieRollResult
    {
        public readonly int Value;
        public readonly int Count;

        public DieRollResult(int value, int count)
        {
            Value = value;
            Count = count;
        }
    }
}
