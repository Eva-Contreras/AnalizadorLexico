using System.Text.RegularExpressions;

namespace AnalizadorLexico
{
    public class Cuadruplo
    {
        public int Numero { get; set; }
        public string Operador { get; set; } = string.Empty;
        public string Argumento1 { get; set; } = string.Empty;
        public string Argumento2 { get; set; } = string.Empty;
        public string Resultado { get; set; } = string.Empty;
    }

    public class CodigoIntermedio
    {
        private readonly List<Cuadruplo> _cuadruplos = new();
        private int _contadorTemporales;
        private int _contadorEtiquetas;

        public List<string> Errores { get; } = new();
        public List<Cuadruplo> Cuadruplos => new(_cuadruplos);

        public List<Cuadruplo> GenerarDesdeTexto(string codigoFuente)
        {
            _cuadruplos.Clear();
            Errores.Clear();
            _contadorTemporales = 0;
            _contadorEtiquetas = 0;

            if (string.IsNullOrWhiteSpace(codigoFuente))
                return new List<Cuadruplo>();

            foreach (var linea in codigoFuente.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None))
            {
                string texto = linea.Trim();
                if (string.IsNullOrWhiteSpace(texto) || texto.StartsWith("//") || texto.StartsWith("#"))
                    continue;

                if (texto == "{" || texto == "}" ||
                    texto.StartsWith("PARA ", StringComparison.OrdinalIgnoreCase) || texto.StartsWith("PARA(", StringComparison.OrdinalIgnoreCase) ||
                    texto.StartsWith("LEER ", StringComparison.OrdinalIgnoreCase) || texto.StartsWith("LEER(", StringComparison.OrdinalIgnoreCase) ||
                    texto.StartsWith("IMPRIMIR ", StringComparison.OrdinalIgnoreCase) || texto.StartsWith("IMPRIMIR(", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                var match = Regex.Match(texto, @"^(?:(?<tipo>ENT|DEC|CAD)\s+)?(?<dest>[A-Za-z_][A-Za-z0-9_]*)\s*=\s*(?<expr>.+?)(?:;)?\s*$");
                if (!match.Success)
                {
                    if (!Regex.IsMatch(texto, @"^(?:\{|\}|LEER\s*\(|IMPRIMIR\s*\(|PARA\s*\()") &&
                        !Regex.IsMatch(texto, @"^(?:LEER|IMPRIMIR|PARA|ENT|DEC|CAD)\b", RegexOptions.IgnoreCase))
                    {
                        Errores.Add($"Error de generación: sentencia no soportada en la línea '{texto}'.");
                    }
                    continue;
                }

                string destino = match.Groups["dest"].Value;
                string expresion = match.Groups["expr"].Value.Trim();

                if (string.IsNullOrWhiteSpace(expresion))
                {
                    Errores.Add($"Error de generación: expresión vacía en la asignación a '{destino}'.");
                    continue;
                }

                string resultado = ResolverExpresion(expresion);
                if (string.IsNullOrWhiteSpace(resultado))
                {
                    Errores.Add($"Error de generación: no se pudo generar la expresión '{expresion}'.");
                    continue;
                }

                _cuadruplos.Add(new Cuadruplo
                {
                    Numero = _cuadruplos.Count + 1,
                    Operador = "=",
                    Argumento1 = resultado,
                    Argumento2 = string.Empty,
                    Resultado = destino
                });
            }

            return new List<Cuadruplo>(_cuadruplos);
        }

        public string NuevaTemporal()
        {
            _contadorTemporales++;
            return $"t{_contadorTemporales}";
        }

        public string NuevaEtiqueta()
        {
            _contadorEtiquetas++;
            return $"L{_contadorEtiquetas}";
        }

        private string ResolverExpresion(string expresion)
        {
            expresion = expresion.Trim();
            if (string.IsNullOrWhiteSpace(expresion))
                return string.Empty;

            if (Regex.IsMatch(expresion, @"^[A-Za-z_][A-Za-z0-9_]*$"))
                return expresion;

            if (Regex.IsMatch(expresion, @"^-?\d+(\.\d+)?$"))
                return expresion;

            if (expresion.StartsWith('"') && expresion.EndsWith('"'))
                return expresion;

            var tokens = TokenizarExpresion(expresion);
            if (tokens.Count == 0)
            {
                Errores.Add($"Error de generación: expresión vacía o no válida '{expresion}'.");
                return string.Empty;
            }

            int indice = 0;
            string resultado = ParseSumaResta(tokens, ref indice);

            if (indice < tokens.Count)
            {
                Errores.Add($"Error de generación: expresión no soportada en la generación de cuádruplos: '{expresion}'.");
                return string.Empty;
            }

            return resultado;
        }

        private List<string> TokenizarExpresion(string expresion)
        {
            var tokens = new List<string>();
            int indice = 0;

            while (indice < expresion.Length)
            {
                char caracter = expresion[indice];

                if (char.IsWhiteSpace(caracter))
                {
                    indice++;
                    continue;
                }

                if (char.IsDigit(caracter) || caracter == '.')
                {
                    int inicio = indice;
                    bool tienePunto = caracter == '.';
                    indice++;

                    while (indice < expresion.Length)
                    {
                        char actual = expresion[indice];
                        if (char.IsDigit(actual))
                        {
                            indice++;
                            continue;
                        }

                        if (actual == '.' && !tienePunto)
                        {
                            tienePunto = true;
                            indice++;
                            continue;
                        }

                        break;
                    }

                    tokens.Add(expresion.Substring(inicio, indice - inicio));
                    continue;
                }

                if (char.IsLetter(caracter) || caracter == '_')
                {
                    int inicio = indice;
                    indice++;

                    while (indice < expresion.Length && (char.IsLetterOrDigit(expresion[indice]) || expresion[indice] == '_'))
                        indice++;

                    tokens.Add(expresion.Substring(inicio, indice - inicio));
                    continue;
                }

                if (caracter == '+' || caracter == '-' || caracter == '*' || caracter == '/' || caracter == '(' || caracter == ')')
                {
                    tokens.Add(caracter.ToString());
                    indice++;
                    continue;
                }

                indice++;
            }

            return tokens;
        }

        private string ParseSumaResta(List<string> tokens, ref int indice)
        {
            string izquierda = ParseMultiplicacion(tokens, ref indice);

            while (indice < tokens.Count && (tokens[indice] == "+" || tokens[indice] == "-"))
            {
                string operador = tokens[indice++];
                string derecha = ParseMultiplicacion(tokens, ref indice);
                string temporal = NuevaTemporal();

                _cuadruplos.Add(new Cuadruplo
                {
                    Numero = _cuadruplos.Count + 1,
                    Operador = operador,
                    Argumento1 = izquierda,
                    Argumento2 = derecha,
                    Resultado = temporal
                });

                izquierda = temporal;
            }

            return izquierda;
        }

        private string ParseMultiplicacion(List<string> tokens, ref int indice)
        {
            string izquierda = ParseFactor(tokens, ref indice);

            while (indice < tokens.Count && (tokens[indice] == "*" || tokens[indice] == "/"))
            {
                string operador = tokens[indice++];
                string derecha = ParseFactor(tokens, ref indice);
                string temporal = NuevaTemporal();

                _cuadruplos.Add(new Cuadruplo
                {
                    Numero = _cuadruplos.Count + 1,
                    Operador = operador,
                    Argumento1 = izquierda,
                    Argumento2 = derecha,
                    Resultado = temporal
                });

                izquierda = temporal;
            }

            return izquierda;
        }

        private string ParseFactor(List<string> tokens, ref int indice)
        {
            if (indice >= tokens.Count)
                return string.Empty;

            if (tokens[indice] == "(")
            {
                indice++;
                string valor = ParseSumaResta(tokens, ref indice);
                if (indice < tokens.Count && tokens[indice] == ")")
                    indice++;
                return valor;
            }

            if (tokens[indice] == "-")
            {
                indice++;
                string valor = ParseFactor(tokens, ref indice);
                string temporal = NuevaTemporal();
                _cuadruplos.Add(new Cuadruplo
                {
                    Numero = _cuadruplos.Count + 1,
                    Operador = "u-",
                    Argumento1 = valor,
                    Argumento2 = string.Empty,
                    Resultado = temporal
                });
                return temporal;
            }

            return tokens[indice++];
        }
    }
}
