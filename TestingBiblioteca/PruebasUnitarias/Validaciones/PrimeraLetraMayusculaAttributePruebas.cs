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
        public void IsValid_RetornaExitoso_SiValueEsVacio()
        {
            //Preparacion
            var primeraLetraMayusuculaAttribute = new PrimeraLetraMayusculaAttribute();
            var validationContext = new ValidationContext(new object());
            var value = string.Empty;

            //Prueba
            var resultado = primeraLetraMayusuculaAttribute.GetValidationResult(value, validationContext);

            //Verificacion
            Assert.AreEqual(expected: ValidationResult.Success, actual: resultado);
        }
    }
}
