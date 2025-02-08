using System;
using System.Drawing;

namespace AciertalaV3
{
    internal class ButtonConfig
    {
        public string Texto { get; }
        public string Icono { get; }
        public EventHandler EventoClick { get; }

        public ButtonConfig(string texto, string icono, EventHandler eventoClick)
        {
            Texto = texto;
            Icono = icono;
            EventoClick = eventoClick;
        }
    }
}
