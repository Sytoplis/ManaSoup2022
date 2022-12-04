using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManaSoup2022.Assets.Scripts.Systematic
{
    public interface IJumpingMovement
    {
        public float CurrentHorizontalMovement { get; set; }
        public bool NeedsJumping { get; set; }
    }
}