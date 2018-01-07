using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TelerikMvcApp4.Models
{
    public class BusquedaMarca : ModelBase
    {
        public BusquedaMarca(int cod, string empresa, string expediente, string fechaExpèdiente, string status, string registro, string fechaInicioRegistro, string fechaVencimientoRegistro, string marcaDescrpcion, string clase, string edNiza, string tipo, string titular, string direccion, string poder, string agente, string productoServicio, string denominacion)
        {
            this.cod = cod;
            Empresa = empresa;
            Expediente = expediente;
            FechaExpèdiente = fechaExpèdiente;
            Status = status;
            Registro = registro;
            FechaInicioRegistro = fechaInicioRegistro;
            FechaVencimientoRegistro = fechaVencimientoRegistro;
            MarcaDescrpcion = marcaDescrpcion;
            Clase = clase;
            EdNiza = edNiza;
            Tipo = tipo;
            Titular = titular;
            Direccion = direccion;
            Poder = poder;
            Agente = agente;
            ProductoServicio = productoServicio;
            Denominacion = denominacion;
        }

        public BusquedaMarca(int cod, string empresa, string expediente, string fechaExpèdiente, string status, string registro, string fechaInicioRegistro, string fechaVencimientoRegistro, string marcaDescrpcion,  string edNiza, string tipo, string titular, string direccion, string poder,  string productoServicio, string denominacion)
        {
            this.cod = cod;
            Empresa = empresa;
            Expediente = expediente;
            FechaExpèdiente = fechaExpèdiente;
            Status = status;
            Registro = registro;
            FechaInicioRegistro = fechaInicioRegistro;
            FechaVencimientoRegistro = fechaVencimientoRegistro;
            MarcaDescrpcion = marcaDescrpcion;
            //Clase = clase;
            EdNiza = edNiza;
            Tipo = tipo;
            Titular = titular;
            Direccion = direccion;
            Poder = poder;
            //Agente = agente;
            ProductoServicio = productoServicio;
            Denominacion = denominacion;
        }

        public BusquedaMarca(int cod, string empresa, string expediente, string fechaExpèdiente, string status, string registro, string fechaInicioRegistro, string fechaVencimientoRegistro, string marcaDescrpcion,  string tipo, string direccion, string poder, string productoServicio, string denominacion)
        {
            this.cod = cod;
            Empresa = empresa;
            Expediente = expediente;
            FechaExpèdiente = fechaExpèdiente;
            Status = status;
            Registro = registro;
            FechaInicioRegistro = fechaInicioRegistro;
            FechaVencimientoRegistro = fechaVencimientoRegistro;
            MarcaDescrpcion = marcaDescrpcion;
            Tipo = tipo;
            Direccion = direccion;
            Poder = poder;
            ProductoServicio = productoServicio;
            Denominacion = denominacion;
        }


        public int cod { get; set; }
        public string Empresa { get; set; }
        public string  Expediente { get; set; }
        public string FechaExpèdiente { get; set; }
        public string Status { get; set; }
        public string  Registro { get; set; }
        public string  FechaInicioRegistro { get; set; }
        public string FechaVencimientoRegistro { get; set; }
        public string MarcaDescrpcion { get; set; }
        public string Clase { get; set; }
        public string  EdNiza { get; set; }
        public string Tipo { get; set; }
        public string Titular { get; set; }
        public string  Direccion { get; set; }
        public string  Poder { get; set; }
        public string  Agente { get; set; }
        public string ProductoServicio { get; set; }
        public string Denominacion { get; set; }
    }

    public class BusquedaMarcaList : List<BusquedaMarca>
    {

    }
}