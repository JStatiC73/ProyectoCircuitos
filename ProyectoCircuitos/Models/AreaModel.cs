using System;
using System.Collections.Generic;
using System.Text;

namespace ProyectoCircuitos.Models
{
    public class AreaModel : NotificationObject
    {
        private string noArea;
        private bool iluminado;

        private string nombre;
        
        public string NoArea
        {
            get => noArea;
            set
            {
                noArea = value;
                OnPropertyChanged();
            }
        }

        public bool Iluminado
        {
            get => iluminado;
            set
            {
                iluminado = value;
                OnPropertyChanged();
            }
        }

        public string Nombre
        {
            get => nombre;
            set
            {
                nombre = value;
                OnPropertyChanged();
            }
        }

        public AreaModel(string noArea, string nombre)
        {
            NoArea = noArea;
            Nombre = nombre;
            Iluminado = false;
        }
    }
}
