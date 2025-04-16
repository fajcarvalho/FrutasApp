using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrutasApp.Models
{
    public class Fruta
    {
        public int Id { get; set; } // Chave primária

        public string Nome { get; set; }
        public string Cor { get; set; }
        public double Peso { get; set; }

        public SaborFruta Sabor { get; set; } // Enum para o sabor da fruta

        public int CategoriaId { get; set; } // Chave estrangeira para a categoria

        public Categoria Categoria { get; set; } // Navegação para a categoria
    }
}
