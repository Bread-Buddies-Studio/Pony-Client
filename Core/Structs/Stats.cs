using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Extensions;

namespace Core.Structs;

public struct Stats
{
    public int Health
    {
        readonly get => field;
        set => field = value.Clamp0(MaxHealth);
    }
    public int MaxHealth { get; set; }

    public int Mana
    {
        readonly get => field;
        set => field = value.Clamp0(MaxMana);
    }
    public int MaxMana { get; set; }
}
