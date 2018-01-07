using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TelerikMvcApp4.Models
{
    public class LoginModel : ModelBase
    {
        [Required]
        [DataType(DataType.Text)]
        [StringLength(30)]
        [Display(Name = "Nombre Usuario")]
        public string NombreUsuario { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(16, MinimumLength = 8)]
        [Display(Name = "Contraseña")]
        public string ContraseñaUsuario { get; set; }

        [Required]
        [Display(Name = "Base de Datos")]
        public string BaseUsuario { get; set; }

        public LoginModel() : base()
        {
            this.NombreUsuario = string.Empty;
            this.ContraseñaUsuario = string.Empty;
            this.BaseUsuario = string.Empty;
        }
    }
}