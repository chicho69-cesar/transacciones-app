using ManejoPresupuestos.Validations;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ManejoPresupuestos.Models {
    public class TipoCuenta /*: IValidatableObject*/ {
        public int Id { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Display(Name = "Nombre: ")]
        [FirstCapitalLetter] // Validacion de propiedad
        [Remote(action: "VerificarExisteTipoCuenta", controller: "TipoCuenta", AdditionalFields = nameof(Id))]
        public string Nombre { get; set; }

        public int UsuarioId { get; set; }

        public int Orden { get; set; }

        // Validacion de modelo
        /*public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
            if (Nombre != null && Nombre.Length > 0) {
                var firstLetter = Nombre[0].ToString();

                if (firstLetter != firstLetter.ToUpper()) {
                    yield return new ValidationResult (
                        "La primera letra debe de ser mayuscula", 
                        new[] { nameof(Nombre) } Aqui especificamos la propiedad
                        del error, pero si no la especificaramos el error
                        pasaria como error de modelonly, y esto es muy util
                        cuando queremos hacer validaciones que no incluyan a
                        una sola propiedad, sino a varias en conjunto
                    );
                }
            }
        }*/



        /*Pruebas de otras validaciones por defecto*/
        /*[Required(ErrorMessage = "El campo {0} es requerido")]
        [EmailAddress(ErrorMessage = "El campo {0} debe ser un correo electronico valido")]
        public string Email { get; set; }
        [Range(minimum: 18, maximum: 100, ErrorMessage = "El valor de la edad debe estar entre {1} y {2} años")]
        public int Edad { get; set; }
        [Url(ErrorMessage = "El campo debe ser una URL valida")]
        public string URL { get; set; }
        [CreditCard(ErrorMessage = "La tarjeta de credito no es valida")]
        [Display(Name = "Tarjeta de credito")]
        public string TarjetaDeCredito { get; set; }*/
    }
}