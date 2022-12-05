﻿using System;
using System.Collections.Generic;

namespace LibroNovedades.Models
{
    public partial class Rol
    {
        public Rol()
        {
            Nivels = new HashSet<Nivel>();
        }

        public int IdRol { get; set; }
        public string Rnombre { get; set; } = null!;
        public bool Restado { get; set; }
        public string? Rdescri { get; set; }

        public virtual ICollection<Nivel> Nivels { get; set; }
    }
}
