using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrutasApp.Models
{
    public class Categoria
    {
        public int Id { get; set; } // Chave primária

        public string Nome { get; set; }
        public string Descricao { get; set; }


        public ICollection<Fruta> Frutas { get; set; } // Navegação para as frutas da categoria
    }
}
