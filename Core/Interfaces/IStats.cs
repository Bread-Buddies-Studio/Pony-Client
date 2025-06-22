using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Extensions;

namespace Core.Interfaces;

public interface IStats
{
    public abstract int Health { get; set; }
    public abstract int MaxHealth { get; set; }

    public abstract int Mana { get; set; }
    public abstract int MaxMana { get; set; }
}
