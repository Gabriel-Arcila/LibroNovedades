﻿using System;
using System.Collections.Generic;

namespace LibroNovedades.Models
{
    public partial class EquipoEam
    {
        public int IdEquipo { get; set; }
        public int IdLinea { get; set; }
        public string EcodEquiEam { get; set; } = null!;
        public string EnombreEam { get; set; } = null!;
        public string? EdescriEam { get; set; }

        public virtual Linea IdLineaNavigation { get; set; } = null!;
    }
}
