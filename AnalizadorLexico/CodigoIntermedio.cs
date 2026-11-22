
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
        public List<Cuadruplo> GenerarDesdeAst(ProgramNode programa)
        {
            LimpiarResultados();
            EmitirSentencias(programa.Statements);
            GenerarTriplos();
            GenerarCodigoTresDireccionesCuadruplos();
            GenerarCodigoTresDireccionesTriplos();
            GenerarCodigoPDesdeCuadruplos();
            GenerarNotacionesDesdeCuadruplos();
            return Cuadruplos;
        }

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

        private void EmitirSentencias(IEnumerable<AstNode> sentencias)
        {
            foreach (AstNode sentencia in sentencias)
            {
                switch (sentencia)
                {
                    case DeclarationNode declaracion:
                        EmitirAsignacion(declaracion.Identificador, declaracion.Valor);
                        break;
                    case AssignmentNode asignacion:
                        EmitirAsignacion(asignacion.Identificador, asignacion.Valor);
                        break;
                    case BlockNode bloque:
                        EmitirSentencias(bloque.Statements);
                        break;
                    case IfNode condicional:
                        EmitirIf(condicional);
                        break;
                    case WhileNode mientras:
                        EmitirWhile(mientras);
                        break;
                    case DoWhileNode hacerMientras:
                        EmitirDoWhile(hacerMientras);
                        break;
                    case ForNode para:
                        EmitirFor(para);
                        break;
                    case ReadNode lectura:
                        AgregarCuadruplo("READ", lectura.Identificador, string.Empty, string.Empty);
                        break;
                    case PrintNode imprimir:
                        AgregarCuadruplo("PRINT", EmitirExpresion(imprimir.Valor), string.Empty, string.Empty);
                        break;
                    case ReturnNode retornar:
                        AgregarCuadruplo("RETURN", EmitirExpresion(retornar.Valor), string.Empty, string.Empty);
                        break;
                    case SimpleInstructionNode instruccion:
                        AgregarCuadruplo(instruccion.Operacion, string.Join(", ", instruccion.Argumentos), string.Empty, string.Empty);
                        break;
                    case SwitchNode seleccion:
                        EmitirSwitch(seleccion);
                        break;
                }
            }
        }

        private void EmitirAsignacion(string destino, AstNode? valor)
        {
            string operando = EmitirExpresion(valor);
            AgregarCuadruplo("=", operando, string.Empty, destino);
        }

        private void AgregarRepresentacionesDeExpresion(string destino, AstNode? expresion)
        {
            List<string> postfija = new() { destino };
            GenerarPostfijaAst(expresion, postfija);
            postfija.Add("=");
            _notacionPostfija.Add(string.Join(" ", postfija));

            List<string> prefija = new() { "=", destino };
            GenerarPrefijaAst(expresion, prefija);
            _notacionPrefija.Add(string.Join(" ", prefija));

            GenerarCodigoPAst(expresion);
            _codigoP.Add($"sto {destino}");
        }

        private void GenerarPostfijaAst(AstNode? expresion, List<string> salida)
        {
            if (expresion is BinaryExpressionNode binaria)
            {
                GenerarPostfijaAst(binaria.Izquierda, salida);
                GenerarPostfijaAst(binaria.Derecha, salida);
                salida.Add(binaria.Operador);
            }
            else if (expresion is UnaryExpressionNode unaria)
            {
                GenerarPostfijaAst(unaria.Operando, salida);
                salida.Add(unaria.Operador == "-" ? "NEG" : "POS");
            }
            else
            {
                salida.Add(EmitirExpresionLiteral(expresion));
            }
        }

        private void GenerarPrefijaAst(AstNode? expresion, List<string> salida)
        {
            if (expresion is BinaryExpressionNode binaria)
            {
                salida.Add(binaria.Operador);
                GenerarPrefijaAst(binaria.Izquierda, salida);
                GenerarPrefijaAst(binaria.Derecha, salida);
            }
            else if (expresion is UnaryExpressionNode unaria)
            {
                salida.Add(unaria.Operador == "-" ? "NEG" : "POS");
                GenerarPrefijaAst(unaria.Operando, salida);
            }
            else
            {
                salida.Add(EmitirExpresionLiteral(expresion));
            }
        }

        private void GenerarCodigoPAst(AstNode? expresion)
        {
            if (expresion is BinaryExpressionNode binaria)
            {
                GenerarCodigoPAst(binaria.Izquierda);
                GenerarCodigoPAst(binaria.Derecha);
                string? operacion = binaria.Operador switch
                {
                    "+" => "adi",
                    "-" => "sbi",
                    "*" => "mli",
                    "/" => "dvi",
                    _ => null
                };
                if (operacion != null) _codigoP.Add(operacion);
            }
            else if (expresion is UnaryExpressionNode unaria)
            {
                GenerarCodigoPAst(unaria.Operando);
                if (unaria.Operador == "-") _codigoP.Add("ngi");
            }
            else
            {
                string operando = EmitirExpresionLiteral(expresion);
                _codigoP.Add(EsNumero(operando) || EsCadena(operando) ? $"ldc {operando}" : $"lod {operando}");
            }
        }

        private string EmitirExpresionLiteral(AstNode? expresion)
        {
            return expresion switch
            {
                IdentifierNode identificador => identificador.Nombre,
                LiteralNode literal => literal.Valor,
                RawExpressionNode raw => raw.Texto,
                _ => string.Empty
            };
        }

        private string EmitirCondicion(AstNode? condicion)
        {
            if (condicion == null)
                throw new InvalidOperationException("No se pudo construir una expresión booleana válida desde el AST.");

            if (condicion is UnaryExpressionNode unaria && unaria.Operador == "NO")
            {
                string operandoNegado = EmitirCondicion(unaria.Operando);
                string resultadoNegacion = NuevaTemporal();
                AgregarCuadruplo("NOT", operandoNegado, string.Empty, resultadoNegacion);
                return resultadoNegacion;
            }

            if (condicion is BinaryExpressionNode binaria &&
                (binaria.Operador == "Y" || binaria.Operador == "O"))
            {
                string izquierdaLogica = EmitirCondicion(binaria.Izquierda);
                string derechaLogica = EmitirCondicion(binaria.Derecha);
                string resultadoLogico = NuevaTemporal();
                AgregarCuadruplo(binaria.Operador == "Y" ? "AND" : "OR",
                    izquierdaLogica, derechaLogica, resultadoLogico);
                return resultadoLogico;
            }

            return EmitirExpresion(condicion);
        }

        private string EmitirExpresion(AstNode? expresion)
        {
            switch (expresion)
            {
                case IdentifierNode identificador:
                    return identificador.Nombre;
                case LiteralNode literal:
                    return literal.Valor;
                case RawExpressionNode raw:
                    return raw.Texto;
                case UnaryExpressionNode unaria:
                    string operando = EmitirExpresion(unaria.Operando);
                    string temporalUnario = NuevaTemporal();
                    AgregarCuadruplo(unaria.Operador == "-" ? "NEG" : "POS",
                        operando, string.Empty, temporalUnario);
                    return temporalUnario;
                case BinaryExpressionNode binaria:
                    string izquierda = EmitirExpresion(binaria.Izquierda);
                    string derecha = EmitirExpresion(binaria.Derecha);
                    string temporal = NuevaTemporal();
                    AgregarCuadruplo(binaria.Operador, izquierda, derecha, temporal);
                    return temporal;
                default:
                    return string.Empty;
            }
        }

        private void GenerarCodigoPDesdeCuadruplos()
        {
            _codigoP.Clear();
            foreach (Cuadruplo instruccion in _cuadruplos)
            {
                switch (instruccion.Operador)
                {
                    case "=":
                        CargarOperando(instruccion.Argumento1);
                        _codigoP.Add($"sto {instruccion.Resultado}");
                        break;
                    case "NEG":
                    case "NOT":
                    case "POS":
                        CargarOperando(instruccion.Argumento1);
                        _codigoP.Add(instruccion.Operador switch { "NEG" => "ngi", "NOT" => "not", _ => "nop" });
                        _codigoP.Add($"sto {instruccion.Resultado}");
                        break;
                    case "+": case "-": case "*": case "/":
                    case ">": case "<": case ">=": case "<=": case "==": case "!=":
                    case "AND": case "OR":
                        CargarOperando(instruccion.Argumento1);
                        CargarOperando(instruccion.Argumento2);
                        _codigoP.Add(instruccion.Operador switch
                        {
                            "+" => "adi", "-" => "sbi", "*" => "mli", "/" => "dvi",
                            ">" => "grt", "<" => "les", ">=" => "geq", "<=" => "leq",
                            "==" => "equ", "!=" => "neq", "AND" => "and", _ => "or"
                        });
                        _codigoP.Add($"sto {instruccion.Resultado}");
                        break;
                    case "LABEL":
                        _codigoP.Add($"{instruccion.Resultado}:");
                        break;
                    case "GOTO":
                        _codigoP.Add($"jmp {instruccion.Resultado}");
                        break;
                    case "IF_FALSE": case "IF_TRUE":
                        CargarOperando(instruccion.Argumento1);
                        _codigoP.Add($"{(instruccion.Operador == "IF_FALSE" ? "jmf" : "jmt")} {instruccion.Resultado}");
                        break;
                    case "READ":
                        _codigoP.Add($"inp {instruccion.Argumento1}");
                        break;
                    case "PRINT":
                        CargarOperando(instruccion.Argumento1);
                        _codigoP.Add("out");
                        break;
                    case "RETURN":
                        if (!string.IsNullOrEmpty(instruccion.Argumento1)) CargarOperando(instruccion.Argumento1);
                        _codigoP.Add("ret");
                        break;
                    case "CLEAR":
                        _codigoP.Add("clr");
                        break;
                    case "LOCATE":
                        _codigoP.Add($"loc {instruccion.Argumento1}");
                        break;
                }
            }
        }

        private void CargarOperando(string operando)
        {
            _codigoP.Add(EsNumero(operando) || EsCadena(operando) ? $"ldc {operando}" : $"lod {operando}");
        }

        private void GenerarNotacionesDesdeCuadruplos()
        {
            _notacionPrefija.Clear();
            _notacionPostfija.Clear();
            foreach (Cuadruplo instruccion in _cuadruplos)
            {
                bool binaria = instruccion.Operador is "+" or "-" or "*" or "/" or ">" or "<" or ">=" or "<=" or "==" or "!=" or "AND" or "OR";
                if (instruccion.Operador == "=")
                {
                    _notacionPrefija.Add($"= {instruccion.Resultado} {instruccion.Argumento1}");
                    _notacionPostfija.Add($"{instruccion.Resultado} {instruccion.Argumento1} =");
                }
                else if (binaria)
                {
                    _notacionPrefija.Add($"{instruccion.Operador} {instruccion.Argumento1} {instruccion.Argumento2} {instruccion.Resultado}");
                    _notacionPostfija.Add($"{instruccion.Argumento1} {instruccion.Argumento2} {instruccion.Operador} {instruccion.Resultado}");
                }
                else if (instruccion.Operador == "NEG" || instruccion.Operador == "NOT")
                {
                    _notacionPrefija.Add($"{instruccion.Operador} {instruccion.Argumento1} {instruccion.Resultado}");
                    _notacionPostfija.Add($"{instruccion.Argumento1} {instruccion.Operador} {instruccion.Resultado}");
                }
                else if (instruccion.Operador == "LABEL")
                {
                    _notacionPrefija.Add($"LABEL {instruccion.Resultado}");
                    _notacionPostfija.Add($"{instruccion.Resultado} LABEL");
                }
                else if (instruccion.Operador == "GOTO")
                {
                    _notacionPrefija.Add($"GOTO {instruccion.Resultado}");
                    _notacionPostfija.Add($"{instruccion.Resultado} GOTO");
                }
                else if (instruccion.Operador == "IF_FALSE" || instruccion.Operador == "IF_TRUE")
                {
                    _notacionPrefija.Add($"{instruccion.Operador} {instruccion.Argumento1} {instruccion.Resultado}");
                    _notacionPostfija.Add($"{instruccion.Argumento1} {instruccion.Resultado} {instruccion.Operador}");
                }
                else
                {
                    _notacionPrefija.Add($"{instruccion.Operador} {instruccion.Argumento1}".TrimEnd());
                    _notacionPostfija.Add($"{instruccion.Argumento1} {instruccion.Operador}".Trim());
                }
            }
        }

        private void EmitirIf(IfNode nodo)
        {
            string etiquetaFalsa = NuevaEtiqueta();
            string etiquetaFin = NuevaEtiqueta();
            AgregarCuadruplo("IF_FALSE", EmitirCondicion(nodo.Condicion), string.Empty, etiquetaFalsa);
            EmitirSentencias(nodo.BloqueVerdadero.Statements);
            if (nodo.BloqueFalso != null)
            {
                AgregarCuadruplo("GOTO", string.Empty, string.Empty, etiquetaFin);
                AgregarCuadruplo("LABEL", string.Empty, string.Empty, etiquetaFalsa);
                EmitirSentencias(nodo.BloqueFalso.Statements);
                AgregarCuadruplo("LABEL", string.Empty, string.Empty, etiquetaFin);
            }
            else
            {
                AgregarCuadruplo("LABEL", string.Empty, string.Empty, etiquetaFalsa);
            }
        }

        private void EmitirWhile(WhileNode nodo)
        {
            string inicio = NuevaEtiqueta();
            string fin = NuevaEtiqueta();
            AgregarCuadruplo("LABEL", string.Empty, string.Empty, inicio);
            AgregarCuadruplo("IF_FALSE", EmitirCondicion(nodo.Condicion), string.Empty, fin);
            EmitirSentencias(nodo.Cuerpo.Statements);
            AgregarCuadruplo("GOTO", string.Empty, string.Empty, inicio);
            AgregarCuadruplo("LABEL", string.Empty, string.Empty, fin);
        }

        private void EmitirDoWhile(DoWhileNode nodo)
        {
            string inicio = NuevaEtiqueta();
            AgregarCuadruplo("LABEL", string.Empty, string.Empty, inicio);
            EmitirSentencias(nodo.Cuerpo.Statements);
            AgregarCuadruplo("IF_TRUE", EmitirCondicion(nodo.Condicion), string.Empty, inicio);
        }

        private void EmitirFor(ForNode nodo)
        {
            if (nodo.Inicializacion != null)
                EmitirAsignacion(nodo.Inicializacion.Identificador, nodo.Inicializacion.Valor);
            string inicio = NuevaEtiqueta();
            string fin = NuevaEtiqueta();
            AgregarCuadruplo("LABEL", string.Empty, string.Empty, inicio);
            AgregarCuadruplo("IF_FALSE", EmitirCondicion(nodo.Condicion), string.Empty, fin);
            EmitirSentencias(nodo.Cuerpo.Statements);
            if (nodo.Incremento != null)
                EmitirAsignacion(nodo.Incremento.Identificador, nodo.Incremento.Valor);
            AgregarCuadruplo("GOTO", string.Empty, string.Empty, inicio);
            AgregarCuadruplo("LABEL", string.Empty, string.Empty, fin);
        }

        private void EmitirSwitch(SwitchNode nodo)
        {
            string fin = NuevaEtiqueta();
            foreach (SwitchCaseNode caso in nodo.Casos)
            {
                string siguiente = NuevaEtiqueta();
                string comparacion = NuevaTemporal();
                AgregarCuadruplo("==", nodo.Selector, EmitirExpresion(caso.Valor), comparacion);
                AgregarCuadruplo("IF_FALSE", comparacion, string.Empty, siguiente);
                EmitirSentencias(caso.Cuerpo.Statements);
                AgregarCuadruplo("GOTO", string.Empty, string.Empty, fin);
                AgregarCuadruplo("LABEL", string.Empty, string.Empty, siguiente);
            }
            if (nodo.Predeterminado != null)
                EmitirSentencias(nodo.Predeterminado.Statements);
            AgregarCuadruplo("LABEL", string.Empty, string.Empty, fin);
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
            List<string> postfija = new()
            {
                destino
            };
            GenerarPostfija(arbol, postfija);
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

            

            GenerarCodigoP(arbol);

            _codigoP.Add($"sto {destino}");
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

            Dictionary<string, int> referencias = new(StringComparer.OrdinalIgnoreCase);

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

                if (cuadruplo.Operador == "=")
                {
                    _triplos.Add(new Triplo
                    {
                        Numero = _triplos.Count,
                        Operador = cuadruplo.Operador,
                        Argumento1 = argumento1,
                        Argumento2 = string.Empty,
                        Resultado = cuadruplo.Resultado
                    });

                    continue;
                }

                _triplos.Add(new Triplo
                {
                    Numero = _triplos.Count,
                    Operador = cuadruplo.Operador,
                    Argumento1 = argumento1,
                    Argumento2 = argumento2,
                    Resultado = cuadruplo.Resultado
                });

                if (Regex.IsMatch(cuadruplo.Resultado, @"^t\d+$", RegexOptions.IgnoreCase))
                {
                    referencias[cuadruplo.Resultado] = _triplos.Count - 1;
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
                else if (c.Operador == "NEG" || c.Operador == "POS" || c.Operador == "NOT")
                {
                    string operador = c.Operador switch { "NEG" => "-", "POS" => "+", _ => "NOT " };
                    _codigoTresDireccionesCuadruplos.Add($"{c.Resultado} = {operador}{c.Argumento1}");
                }
                else if (c.Operador == "LABEL")
                {
                    _codigoTresDireccionesCuadruplos.Add($"{c.Resultado}:");
                }
                else if (c.Operador == "GOTO")
                {
                    _codigoTresDireccionesCuadruplos.Add($"goto {c.Resultado}");
                }
                else if (c.Operador == "IF_FALSE" || c.Operador == "IF_TRUE")
                {
                    string condicion = c.Operador == "IF_FALSE" ? "ifFalse" : "ifTrue";
                    _codigoTresDireccionesCuadruplos.Add($"{condicion} {c.Argumento1} goto {c.Resultado}");
                }
                else if (c.Operador == "READ" || c.Operador == "PRINT" || c.Operador == "RETURN" || c.Operador == "CLEAR" || c.Operador == "LOCATE")
                {
                    _codigoTresDireccionesCuadruplos.Add($"{c.Operador.ToLowerInvariant()} {c.Argumento1}".TrimEnd());
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
                        $"{t.Resultado} = {t.Argumento1}";
                }
                else if (t.Operador == "NEG" || t.Operador == "POS" || t.Operador == "NOT")
                {
                    instruccion = $"{(t.Operador == "NEG" ? "-" : t.Operador == "POS" ? "+" : "NOT ")}{t.Argumento1}";
                }
                else if (t.Operador == "LABEL")
                {
                    instruccion = $"{t.Resultado}:";
                }
                else if (t.Operador == "GOTO")
                {
                    instruccion = $"goto {t.Resultado}";
                }
                else if (t.Operador == "IF_FALSE" || t.Operador == "IF_TRUE")
                {
                    instruccion = $"{(t.Operador == "IF_FALSE" ? "ifFalse" : "ifTrue")} {t.Argumento1} goto {t.Resultado}";
                }
                else if (t.Operador == "READ" || t.Operador == "PRINT" || t.Operador == "RETURN" || t.Operador == "CLEAR" || t.Operador == "LOCATE")
                {
                    instruccion = $"{t.Operador.ToLowerInvariant()} {t.Argumento1}".TrimEnd();
                }
                else
                {
                    instruccion = $"{t.Argumento1} {t.Operador} {t.Argumento2}".TrimEnd();
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

                {

                    _codigoP.Add($"ldc {nodo.Valor}");

                }

                else

                {

                    _codigoP.Add($"lod {nodo.Valor}");

                }

                return;

            }

            if (nodo.Izquierda != null)

                GenerarCodigoP(nodo.Izquierda);

            if (nodo.Derecha != null)

                GenerarCodigoP(nodo.Derecha);

            switch (nodo.Operador)

            {

                case "+":

                    _codigoP.Add("adi");

                    break;

                case "-":

                    _codigoP.Add("sbi");

                    break;

                case "*":

                    _codigoP.Add("mli");

                    break;

                case "/":

                    _codigoP.Add("dvi");

                    break;

                case "NEG":

                    _codigoP.Add("ngi");

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
