namespace AnalizadorLexico
{
    public class IdentificadorValido : Recorrido
    {
        public override string NombreTabla => "IdentificadorValido";
        public override string ObtenerMensajeError(int ultimoEstado, char? ultimoCaracter)
        {
            if (ultimoEstado == 0)
                return $"El identificador debe comenzar con una letra (a-z, A-Z).\n" +
                       $"Se encontró: '{ultimoCaracter}'";

            if (ultimoCaracter == null)
                return "La cadena terminó en un estado no válido.";

            return $"Caracter no permitido encontrado: '{ultimoCaracter}'";
        }
        public override bool EstadoError(int estado)
        {
            return estado == 3;
        }
        public override int ObtenerEstadoError()
        {
            return 3;
        }
        public override string ObtenerColumna(char caracter)
        {
            if (char.IsLetter(caracter))
            {
                if (char.IsUpper(caracter))
                    return "C" + caracter;
                else
                    return caracter.ToString();
            }
            return caracter.ToString();
        }
        public override bool Aceptacion(int estado)
        {
            return estado == 2;
        }
    }
}
