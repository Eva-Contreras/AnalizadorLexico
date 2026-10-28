
using System;
using System.Collections.Generic;
using System.Globalization;
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

    public class Triplo
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
        private readonly List<Triplo> _triplos = new();

        private readonly List<string> _codigoP = new();
        private readonly List<string> _notacionPostfija = new();
        private readonly List<string> _notacionPrefija = new();

        private readonly List<string> _codigoTresDireccionesCuadruplos = new();
        private readonly List<string> _codigoTresDireccionesTriplos = new();
        private readonly List<string> _resultadosCalculados = new();

        private readonly Dictionary<string, double> _valoresVariables = new();

        private int _contadorTemporales;
        private int _contadorEtiquetas;

        public List<Cuadruplo> Cuadruplos =>
            new List<Cuadruplo>(_cuadruplos);

        public List<Triplo> Triplos =>
            new List<Triplo>(_triplos);

        public List<string> CodigoP =>
            new List<string>(_codigoP);

        public string NotacionPostfija =>
            string.Join(Environment.NewLine, _notacionPostfija);

        public string NotacionPrefija =>
            string.Join(Environment.NewLine, _notacionPrefija);

        public string CodigoTresDireccionesCuadruplos =>
            string.Join(Environment.NewLine, _codigoTresDireccionesCuadruplos);

        public string CodigoTresDireccionesTriplos =>
            string.Join(Environment.NewLine, _codigoTresDireccionesTriplos);

        public string ResultadosCalculados =>
            string.Join(Environment.NewLine, _resultadosCalculados);

        // =====================================================
        // NODO DEL ARBOL DE EXPRESIONES
        // =====================================================
        private class NodoExpresion
        {
            public string Valor { get; set; } = string.Empty;
            public string Operador { get; set; } = string.Empty;
            public NodoExpresion? Izquierda { get; set; }
            public NodoExpresion? Derecha { get; set; }
            public bool EsOperando { get; set; }
            public bool EsUnario { get; set; }
        }

        // =====================================================
        // GENERAR REPRESENTACIONES
        // =====================================================
        public List<Cuadruplo> GenerarDesdeTexto(string codigoFuente)
        {
            LimpiarResultados();

            if (string.IsNullOrWhiteSpace(codigoFuente))
                return Cuadruplos;

            string[] lineas = codigoFuente.Split(
                new[] { "\r\n", "\n" },
                StringSplitOptions.None
            );

            foreach (string linea in lineas)
            {
                string texto = linea.Trim();

                if (string.IsNullOrWhiteSpace(texto))
                    continue;

                if (texto.StartsWith("//") || texto.StartsWith("#"))
                    continue;

                int comentario = texto.IndexOf("//");

                if (comentario >= 0)
                    texto = texto.Substring(0, comentario).Trim();

                if (string.IsNullOrWhiteSpace(texto))
                    continue;

                // Estas instrucciones todavía no se traducen.
                if (texto == "{" || texto == "}" ||
                    Regex.IsMatch(
                        texto,
                        @"^(PARA|LEER|IMPRIMIR|MIENTRAS|HACER|SI|SINO)\b",
                        RegexOptions.IgnoreCase))
                {
                    continue;
                }

                ProcesarAsignacion(texto);
            }

            GenerarTriplos();
            GenerarCodigoTresDireccionesCuadruplos();
            GenerarCodigoTresDireccionesTriplos();

            return Cuadruplos;
        }

        // =====================================================
        // LIMPIAR RESULTADOS ANTERIORES
        // =====================================================
        private void LimpiarResultados()
        {
            _cuadruplos.Clear();
            _triplos.Clear();

            _codigoP.Clear();
            _notacionPostfija.Clear();
            _notacionPrefija.Clear();

            _codigoTresDireccionesCuadruplos.Clear();
            _codigoTresDireccionesTriplos.Clear();
            _resultadosCalculados.Clear();

            _valoresVariables.Clear();

            _contadorTemporales = 0;
            _contadorEtiquetas = 0;
        }

        // =====================================================
        // PROCESAR ASIGNACION
        // =====================================================
        private void ProcesarAsignacion(string texto)
        {
            Match match = Regex.Match(
                texto,
                @"^(?:(?:ENT|DEC|CAD)\s+)?(?<dest>[A-Za-z_][A-Za-z0-9_]*)\s*=\s*(?<expr>.+?)\s*;?$",
                RegexOptions.IgnoreCase
            );

            if (!match.Success)
                return;

            string destino = match.Groups["dest"].Value;
            string expresion = match.Groups["expr"].Value.Trim();

            if (string.IsNullOrWhiteSpace(expresion))
                return;

            NodoExpresion? arbol = AnalizarExpresion(expresion);

            if (arbol == null)
                return;

            double? valorCalculado = EvaluarExpresion(arbol);

            string resultado = GenerarCuadruplos(arbol);

            if (string.IsNullOrWhiteSpace(resultado))
                return;

            AgregarCuadruplo(
                "=",
                resultado,
                string.Empty,
                destino
            );

            // Guardar el valor si puede calcularse.
            if (valorCalculado.HasValue)
            {
                _valoresVariables[destino] = valorCalculado.Value;

                _resultadosCalculados.Add(
                    $"{destino} = {FormatoNumero(valorCalculado.Value)}"
                );
            }
            else
            {
                _valoresVariables.Remove(destino);

                _resultadosCalculados.Add(
                    $"{destino} = valor no calculable"
                );
            }

            // Notacion postfija.
            List<string> postfija = new();
            GenerarPostfija(arbol, postfija);
            postfija.Add(destino);
            postfija.Add("=");

            _notacionPostfija.Add(string.Join(" ", postfija));

            // Notacion prefija.
            List<string> prefija = new()
            {
                "=",
                destino
            };

            GenerarPrefija(arbol, prefija);

            _notacionPrefija.Add(string.Join(" ", prefija));

            // Codigo P.
            GenerarCodigoP(arbol);

            if (valorCalculado.HasValue)
            {
                _codigoP.Add(
                    $"; Resultado de la operacion: {FormatoNumero(valorCalculado.Value)}"
                );
            }

            _codigoP.Add($"STO {destino}");

            if (_valoresVariables.TryGetValue(destino, out double valorFinal))
            {
                _codigoP.Add(
                    $"; {destino} = {FormatoNumero(valorFinal)}"
                );
            }
        }

        // =====================================================
        // ANALIZAR EXPRESION
        // =====================================================
        private NodoExpresion? AnalizarExpresion(string expresion)
        {
            List<string> tokens = TokenizarExpresion(expresion);

            if (tokens.Count == 0)
                return null;

            int indice = 0;

            NodoExpresion? arbol = ParseSumaResta(tokens, ref indice);

            if (arbol == null || indice != tokens.Count)
                return null;

            return arbol;
        }

        // =====================================================
        // TOKENIZAR EXPRESION
        // =====================================================
        private List<string> TokenizarExpresion(string expresion)
        {
            List<string> tokens = new();
            int indice = 0;

            while (indice < expresion.Length)
            {
                char caracter = expresion[indice];

                if (char.IsWhiteSpace(caracter))
                {
                    indice++;
                    continue;
                }

                // Cadenas entre comillas.
                if (caracter == '"')
                {
                    int inicio = indice;
                    indice++;

                    while (indice < expresion.Length &&
                           expresion[indice] != '"')
                    {
                        if (expresion[indice] == '\\' &&
                            indice + 1 < expresion.Length)
                        {
                            indice += 2;
                        }
                        else
                        {
                            indice++;
                        }
                    }

                    if (indice >= expresion.Length)
                        return new List<string>();

                    indice++;

                    tokens.Add(
                        expresion.Substring(inicio, indice - inicio)
                    );

                    continue;
                }

                // Numeros enteros y decimales.
                if (char.IsDigit(caracter) ||
                    (caracter == '.' &&
                     indice + 1 < expresion.Length &&
                     char.IsDigit(expresion[indice + 1])))
                {
                    int inicio = indice;
                    bool tienePunto = false;

                    while (indice < expresion.Length)
                    {
                        char actual = expresion[indice];

                        if (char.IsDigit(actual))
                        {
                            indice++;
                        }
                        else if (actual == '.' && !tienePunto)
                        {
                            tienePunto = true;
                            indice++;
                        }
                        else
                        {
                            break;
                        }
                    }

                    tokens.Add(
                        expresion.Substring(inicio, indice - inicio)
                    );

                    continue;
                }

                // Identificadores.
                if (char.IsLetter(caracter) || caracter == '_')
                {
                    int inicio = indice;
                    indice++;

                    while (indice < expresion.Length &&
                           (char.IsLetterOrDigit(expresion[indice]) ||
                            expresion[indice] == '_'))
                    {
                        indice++;
                    }

                    tokens.Add(
                        expresion.Substring(inicio, indice - inicio)
                    );

                    continue;
                }

                if ("+-*/()".Contains(caracter))
                {
                    tokens.Add(caracter.ToString());
                    indice++;
                    continue;
                }

                // Caracter no admitido.
                return new List<string>();
            }

            return tokens;
        }

        // =====================================================
        // SUMA Y RESTA
        // =====================================================
        private NodoExpresion? ParseSumaResta(
            List<string> tokens,
            ref int indice)
        {
            NodoExpresion? izquierda =
                ParseMultiplicacion(tokens, ref indice);

            if (izquierda == null)
                return null;

            while (indice < tokens.Count &&
                   (tokens[indice] == "+" ||
                    tokens[indice] == "-"))
            {
                string operador = tokens[indice++];

                NodoExpresion? derecha =
                    ParseMultiplicacion(tokens, ref indice);

                if (derecha == null)
                    return null;

                izquierda = new NodoExpresion
                {
                    Operador = operador,
                    Izquierda = izquierda,
                    Derecha = derecha
                };
            }

            return izquierda;
        }

        // =====================================================
        // MULTIPLICACION Y DIVISION
        // =====================================================
        private NodoExpresion? ParseMultiplicacion(
            List<string> tokens,
            ref int indice)
        {
            NodoExpresion? izquierda =
                ParseFactor(tokens, ref indice);

            if (izquierda == null)
                return null;

            while (indice < tokens.Count &&
                   (tokens[indice] == "*" ||
                    tokens[indice] == "/"))
            {
                string operador = tokens[indice++];

                NodoExpresion? derecha =
                    ParseFactor(tokens, ref indice);

                if (derecha == null)
                    return null;

                izquierda = new NodoExpresion
                {
                    Operador = operador,
                    Izquierda = izquierda,
                    Derecha = derecha
                };
            }

            return izquierda;
        }

        // =====================================================
        // FACTORES, PARENTESIS Y SIGNOS UNARIOS
        // =====================================================
        private NodoExpresion? ParseFactor(
            List<string> tokens,
            ref int indice)
        {
            if (indice >= tokens.Count)
                return null;

            if (tokens[indice] == "(")
            {
                indice++;

                NodoExpresion? expresion =
                    ParseSumaResta(tokens, ref indice);

                if (expresion == null ||
                    indice >= tokens.Count ||
                    tokens[indice] != ")")
                {
                    return null;
                }

                indice++;
                return expresion;
            }

            if (tokens[indice] == "-")
            {
                indice++;

                NodoExpresion? valor =
                    ParseFactor(tokens, ref indice);

                if (valor == null)
                    return null;

                return new NodoExpresion
                {
                    Operador = "NEG",
                    Izquierda = valor,
                    EsUnario = true
                };
            }

            if (tokens[indice] == "+")
            {
                indice++;
                return ParseFactor(tokens, ref indice);
            }

            string token = tokens[indice];

            if (token == "*" || token == "/" || token == ")")
                return null;

            indice++;

            return new NodoExpresion
            {
                Valor = token,
                EsOperando = true
            };
        }

        // =====================================================
        // GENERAR CUADRUPLOS
        // =====================================================
        private string GenerarCuadruplos(NodoExpresion nodo)
        {
            if (nodo.EsOperando)
                return nodo.Valor;

            string izquierda =
                GenerarCuadruplos(nodo.Izquierda!);

            if (nodo.EsUnario)
            {
                string temporalUnario = NuevaTemporal();

                AgregarCuadruplo(
                    "NEG",
                    izquierda,
                    string.Empty,
                    temporalUnario
                );

                return temporalUnario;
            }

            string derecha =
                GenerarCuadruplos(nodo.Derecha!);

            string temporal = NuevaTemporal();

            AgregarCuadruplo(
                nodo.Operador,
                izquierda,
                derecha,
                temporal
            );

            return temporal;
        }

        private void AgregarCuadruplo(
            string operador,
            string argumento1,
            string argumento2,
            string resultado)
        {
            _cuadruplos.Add(new Cuadruplo
            {
                Numero = _cuadruplos.Count + 1,
                Operador = operador,
                Argumento1 = argumento1,
                Argumento2 = argumento2,
                Resultado = resultado
            });
        }

        // =====================================================
        // GENERAR TRIPLOS
        // =====================================================
        private void GenerarTriplos()
        {
            _triplos.Clear();

            Dictionary<string, int> referencias = new();

            foreach (Cuadruplo cuadruplo in _cuadruplos)
            {
                string argumento1 = ConvertirReferenciaTriplo(
                    cuadruplo.Argumento1,
                    referencias
                );

                string argumento2 = ConvertirReferenciaTriplo(
                    cuadruplo.Argumento2,
                    referencias
                );

                int numeroTriplo = _triplos.Count;

                if (cuadruplo.Operador == "=")
                {
                    argumento2 = cuadruplo.Resultado;
                }

                _triplos.Add(new Triplo
                {
                    Numero = numeroTriplo,
                    Operador = cuadruplo.Operador,
                    Argumento1 = argumento1,
                    Argumento2 = argumento2,
                    Resultado = cuadruplo.Operador == "="
                        ? string.Empty
                        : cuadruplo.Resultado
                });

                if (cuadruplo.Resultado.StartsWith(
                        "t",
                        StringComparison.OrdinalIgnoreCase) &&
                    cuadruplo.Operador != "=")
                {
                    referencias[cuadruplo.Resultado] = numeroTriplo;
                }
            }
        }

        private string ConvertirReferenciaTriplo(
            string argumento,
            Dictionary<string, int> referencias)
        {
            if (string.IsNullOrWhiteSpace(argumento))
                return string.Empty;

            if (referencias.TryGetValue(argumento, out int numero))
                return $"({numero})";

            return argumento;
        }

        // =====================================================
        // CODIGO DE TRES DIRECCIONES DESDE CUADRUPLOS
        // =====================================================
        private void GenerarCodigoTresDireccionesCuadruplos()
        {
            _codigoTresDireccionesCuadruplos.Clear();

            foreach (Cuadruplo c in _cuadruplos)
            {
                if (c.Operador == "=")
                {
                    _codigoTresDireccionesCuadruplos.Add(
                        $"{c.Resultado} = {c.Argumento1}"
                    );
                }
                else if (c.Operador == "NEG")
                {
                    _codigoTresDireccionesCuadruplos.Add(
                        $"{c.Resultado} = -{c.Argumento1}"
                    );
                }
                else
                {
                    _codigoTresDireccionesCuadruplos.Add(
                        $"{c.Resultado} = {c.Argumento1} {c.Operador} {c.Argumento2}"
                    );
                }
            }
        }

        // =====================================================
        // CODIGO DE TRES DIRECCIONES DESDE TRIPLOS
        // =====================================================
        private void GenerarCodigoTresDireccionesTriplos()
        {
            _codigoTresDireccionesTriplos.Clear();

            foreach (Triplo t in _triplos)
            {
                string instruccion;

                if (t.Operador == "=")
                {
                    instruccion =
                        $"{t.Argumento2} = {t.Argumento1}";
                }
                else if (t.Operador == "NEG")
                {
                    instruccion =
                        $"t{t.Numero + 1} = -{t.Argumento1}";
                }
                else
                {
                    instruccion =
                        $"t{t.Numero + 1} = {t.Argumento1} {t.Operador} {t.Argumento2}";
                }

                _codigoTresDireccionesTriplos.Add(
                    $"{t.Numero}: {instruccion}"
                );
            }
        }

        // =====================================================
        // NOTACION POSTFIJA
        // =====================================================
        private void GenerarPostfija(
            NodoExpresion nodo,
            List<string> salida)
        {
            if (nodo.EsOperando)
            {
                salida.Add(nodo.Valor);
                return;
            }

            if (nodo.Izquierda != null)
                GenerarPostfija(nodo.Izquierda, salida);

            if (nodo.Derecha != null)
                GenerarPostfija(nodo.Derecha, salida);

            salida.Add(nodo.Operador);
        }

        // =====================================================
        // NOTACION PREFIJA
        // =====================================================
        private void GenerarPrefija(
            NodoExpresion nodo,
            List<string> salida)
        {
            if (nodo.EsOperando)
            {
                salida.Add(nodo.Valor);
                return;
            }

            salida.Add(nodo.Operador);

            if (nodo.Izquierda != null)
                GenerarPrefija(nodo.Izquierda, salida);

            if (nodo.Derecha != null)
                GenerarPrefija(nodo.Derecha, salida);
        }

        // =====================================================
        // CODIGO P
        // =====================================================
        private void GenerarCodigoP(NodoExpresion nodo)
        {
            if (nodo.EsOperando)
            {
                if (EsCadena(nodo.Valor) || EsNumero(nodo.Valor))
                    _codigoP.Add($"LDC {nodo.Valor}");
                else
                    _codigoP.Add($"LDV {nodo.Valor}");

                return;
            }

            if (nodo.Izquierda != null)
                GenerarCodigoP(nodo.Izquierda);

            if (nodo.Derecha != null)
                GenerarCodigoP(nodo.Derecha);

            switch (nodo.Operador)
            {
                case "+":
                    _codigoP.Add("ADD");
                    break;
                case "-":
                    _codigoP.Add("SUB");
                    break;
                case "*":
                    _codigoP.Add("MUL");
                    break;
                case "/":
                    _codigoP.Add("DIV");
                    break;
                case "NEG":
                    _codigoP.Add("NEG");
                    break;
            }
        }

        // =====================================================
        // EVALUAR EXPRESIONES NUMERICAS
        // =====================================================
        private double? EvaluarExpresion(NodoExpresion nodo)
        {
            if (nodo.EsOperando)
            {
                if (double.TryParse(
                    nodo.Valor,
                    NumberStyles.Any,
                    CultureInfo.InvariantCulture,
                    out double numero))
                {
                    return numero;
                }

                if (_valoresVariables.TryGetValue(
                    nodo.Valor, out double valorVariable))
                {
                    return valorVariable;
                }

                return null;
            }

            double? izquierda = nodo.Izquierda != null
                ? EvaluarExpresion(nodo.Izquierda)
                : null;

            if (nodo.EsUnario)
            {
                return izquierda.HasValue
                    ? -izquierda.Value
                    : null;
            }

            double? derecha = nodo.Derecha != null
                ? EvaluarExpresion(nodo.Derecha)
                : null;

            if (!izquierda.HasValue || !derecha.HasValue)
                return null;

            switch (nodo.Operador)
            {
                case "+":
                    return izquierda.Value + derecha.Value;
                case "-":
                    return izquierda.Value - derecha.Value;
                case "*":
                    return izquierda.Value * derecha.Value;
                case "/":
                    if (derecha.Value == 0)
                        return null;

                    return izquierda.Value / derecha.Value;
                default:
                    return null;
            }
        }

        // =====================================================
        // UTILIDADES
        // =====================================================
        private bool EsNumero(string texto)
        {
            return Regex.IsMatch(
                texto,
                @"^(?:\d+(?:\.\d+)?|\.\d+)$"
            );
        }

        private bool EsCadena(string texto)
        {
            return texto.Length >= 2 &&
                   texto.StartsWith("\"") &&
                   texto.EndsWith("\"");
        }

        private string FormatoNumero(double valor)
        {
            return valor.ToString("G", CultureInfo.InvariantCulture);
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
    }
}
