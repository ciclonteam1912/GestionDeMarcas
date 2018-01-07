using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TelerikMvcApp4.Models
{
    public class EmpresaBusquedaModel : ModelBase
    {
        public EmpresaBusquedaModel(int codEmpresa, string descripcionEmpresa)
        {
            CodEmpresa = codEmpresa;
            DescripcionEmpresa = descripcionEmpresa;
        }

        public EmpresaBusquedaModel(string descripcionEmpresa)
        {
            
            DescripcionEmpresa = descripcionEmpresa;
        }

        public EmpresaBusquedaModel()
        {

        }

        public EmpresaBusquedaModel(string descEmpresa, string ruc, string direccion, string tel, 
            int paisCodigo, string pais, string correo, int tipoPers)
        {
            DescripcionEmpresa = descEmpresa;
            RUC = ruc;
            Direccion = direccion;
            //ProductoServicio = prod;
            Telefono = tel;
            PaisCodigo = paisCodigo;
            PaisDesc = pais;
            Correo = correo;
            //Titulares = tit;
            TipoPersona = tipoPers;
        }

        public int CodEmpresa { get; set; }

        public string DescripcionEmpresa { get; set; }
        public string RUC { get; set; }
        public string Direccion { get; set; }
        public string ProductoServicio { get; set; }
        public string Telefono { get; set; }
        public int PaisCodigo { get; set; }
        public string PaisDesc { get; set; }
        public string Correo { get; set; }
        public string Titulares { get; set; }
        public int TipoPersona { get; set; }

    }

    public class ListEmpresaBusqueda : List<EmpresaBusquedaModel> { }
}