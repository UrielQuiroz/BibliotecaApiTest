using BibliotecaAPI.Validaciones;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestingBiblioteca.PruebasUnitarias.Validaciones
{
    [TestClass]
    public class PrimeraLetraMayusculaAttributePruebas
    {
        [TestMethod]
        [DataRow("")]
        [DataRow("    ")]
        [DataRow(null)]
        [DataRow("Uriel")]
        public void IsValid_RetornaExitoso_SiValueNoTieneLaPrimeraLetraMayuscula(string value) 
        {
            //Preparacion
            var primeraLetraMayusuculaAttribute = new PrimeraLetraMayusculaAttribute();
            var validationContext = new ValidationContext(new object());

            //Prueba
            var resultado = primeraLetraMayusuculaAttribute.GetValidationResult(value, validationContext);

            //Verificacion
            Assert.AreEqual(expected: ValidationResult.Success, actual: resultado);
        }

        [TestMethod]
        [DataRow("uriel")]
        public void IsValid_RetornaError_SiValueTieneLaPrimeraLetraMinuscula(string value)
        { 
            //Preparacion
            var primeraLetraMayusuculaAttribute = new PrimeraLetraMayusculaAttribute();
            var validationContext = new ValidationContext(new object());

            //Prueba
            var resultado = primeraLetraMayusuculaAttribute.GetValidationResult(value, validationContext);

            //Verificacion
            Assert.AreEqual(expected: "La primera letra debe ser mayuscula", actual: resultado!.ErrorMessage);
        }
    }
}
