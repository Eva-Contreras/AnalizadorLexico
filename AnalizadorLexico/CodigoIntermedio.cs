
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

        // =====================================================
        // GENERAR REPRESENTACIONES
        // =====================================================
        public List<Cuadruplo> GenerarDesdeAst(ProgramNode programa)
        {
            ArgumentNullException.ThrowIfNull(programa);
            LimpiarResultados();
            List<FunctionNode> funciones = programa.Statements.OfType<FunctionNode>().ToList();
            EmitirSentencias(programa.Statements.Where(sentencia => sentencia is not FunctionNode));
            if (funciones.Count > 0)
            {
                string finFunciones = NuevaEtiqueta();
                AgregarCuadruplo("GOTO", string.Empty, string.Empty, finFunciones);
                foreach (FunctionNode funcion in funciones)
                {
                    AgregarCuadruplo("LABEL", string.Empty, string.Empty, funcion.Nombre);
                    EmitirSentencias(funcion.Cuerpo.Statements);
                    AgregarCuadruplo("RETURN", string.Empty, string.Empty, string.Empty);
                }
                AgregarCuadruplo("LABEL", string.Empty, string.Empty, finFunciones);
            }
            ValidarOperadores(_cuadruplos);
            ValidarEtiquetas(_cuadruplos);
            GenerarTriplos();
            GenerarCodigoTresDireccionesCuadruplos();
            GenerarCodigoTresDireccionesTriplos();
            GenerarCodigoPDesdeCuadruplos();
            GenerarNotacionesDesdeCuadruplos();
            return Cuadruplos;
        }

        public List<Cuadruplo> GenerarDesdeCuadruplos(IEnumerable<Cuadruplo> codigo)
        {
            ArgumentNullException.ThrowIfNull(codigo);
            LimpiarResultados();
            _cuadruplos.AddRange(codigo.Select(ClonarCuadruplo));
            RenumerarCuadruplos(_cuadruplos);
            ValidarOperadores(_cuadruplos);
            ValidarEtiquetas(_cuadruplos);
            GenerarTriplos();
            GenerarCodigoTresDireccionesCuadruplos();
            GenerarCodigoTresDireccionesTriplos();
            GenerarCodigoPDesdeCuadruplos();
            GenerarNotacionesDesdeCuadruplos();
            return Cuadruplos;
        }

        private static Cuadruplo ClonarCuadruplo(Cuadruplo instruccion) => new()
        {
            Numero = instruccion.Numero,
            Operador = instruccion.Operador,
            Argumento1 = instruccion.Argumento1,
            Argumento2 = instruccion.Argumento2,
            Resultado = instruccion.Resultado
        };

        private static void RenumerarCuadruplos(List<Cuadruplo> instrucciones)
        {
            for (int i = 0; i < instrucciones.Count; i++)
                instrucciones[i].Numero = i + 1;
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
                    case CallNode llamada:
                        EmitirExpresion(llamada);
                        break;
                    case FunctionNode funcion:
                        AgregarCuadruplo("LABEL", string.Empty, string.Empty, funcion.Nombre);
                        EmitirSentencias(funcion.Cuerpo.Statements);
                        AgregarCuadruplo("RETURN", string.Empty, string.Empty, string.Empty);
                        break;
                    default:
                        throw new InvalidOperationException($"No se puede generar IR para el nodo {sentencia.GetType().Name}.");
                }
            }
        }

        private void EmitirAsignacion(string destino, AstNode? valor)
        {
            string operando = EmitirExpresion(valor);
            AgregarCuadruplo("=", operando, string.Empty, destino);
        }

        private string EmitirExpresion(AstNode? expresion)
        {
            switch (expresion)
            {
                case IdentifierNode identificador:
                    return identificador.Nombre;
                case LiteralNode literal:
                    return literal.Valor;
                case UnaryExpressionNode unaria when unaria.Operador == "+":
                    return EmitirExpresion(unaria.Operando);
                case UnaryExpressionNode unaria when unaria.Operador == "-":
                    string operando = EmitirExpresion(unaria.Operando);
                    string temporalUnario = NuevaTemporal();
                    AgregarCuadruplo("NEG", operando, string.Empty, temporalUnario);
                    return temporalUnario;
                case BinaryExpressionNode binaria:
                    string izquierda = EmitirExpresion(binaria.Izquierda);
                    string derecha = EmitirExpresion(binaria.Derecha);
                    string temporal = NuevaTemporal();
                    AgregarCuadruplo(binaria.Operador, izquierda, derecha, temporal);
                    return temporal;
                case CallNode llamada:
                    for (int i = 0; i < llamada.Argumentos.Count; i++)
                    {
                        string argumento = EmitirExpresion(llamada.Argumentos[i]);
                        AgregarCuadruplo("PARAM", argumento, (i + 1).ToString(CultureInfo.InvariantCulture), string.Empty);
                    }
                    string resultadoLlamada = NuevaTemporal();
                    AgregarCuadruplo("CALL", llamada.NombreFuncion, llamada.Argumentos.Count.ToString(CultureInfo.InvariantCulture), resultadoLlamada);
                    return resultadoLlamada;
                default:
                    throw new InvalidOperationException($"No se puede generar IR para la expresión {expresion?.GetType().Name ?? "nula"}.");
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
                    case "+": case "-": case "*": case "/": case "%": case "^":
                    case ">": case "<": case ">=": case "<=": case "==": case "!=":
                    case "AND": case "OR":
                        CargarOperando(instruccion.Argumento1);
                        CargarOperando(instruccion.Argumento2);
                        _codigoP.Add(instruccion.Operador switch
                        {
                            "+" => "adi", "-" => "sbi", "*" => "mli", "/" => "dvi", "%" => "mod", "^" => "pow",
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
                    case "PARAM":
                        CargarOperando(instruccion.Argumento1);
                        _codigoP.Add($"par {instruccion.Argumento2}");
                        break;
                    case "CALL":
                        _codigoP.Add($"cal {instruccion.Argumento1}, {instruccion.Argumento2}");
                        _codigoP.Add($"sto {instruccion.Resultado}");
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
                    default:
                        throw new InvalidOperationException($"No se puede generar Código P para la instrucción '{instruccion.Operador}'.");
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
                bool binaria = instruccion.Operador is "+" or "-" or "*" or "/" or "%" or "^" or ">" or "<" or ">=" or "<=" or "==" or "!=" or "AND" or "OR";
                if (instruccion.Operador == "=")
                {
                    _notacionPrefija.Add($"= {instruccion.Resultado} {instruccion.Argumento1}");
                    _notacionPostfija.Add($"{instruccion.Argumento1} {instruccion.Resultado} =");
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
            string etiquetaVerdadera = NuevaEtiqueta();
            string etiquetaFalsa = NuevaEtiqueta();
            string etiquetaFin = NuevaEtiqueta();
            EmitirSaltoCondicional(nodo.Condicion, etiquetaVerdadera, etiquetaFalsa);
            AgregarCuadruplo("LABEL", string.Empty, string.Empty, etiquetaVerdadera);
            EmitirSentencias(nodo.BloqueVerdadero.Statements);
            if (nodo.BloqueFalso != null)
                AgregarCuadruplo("GOTO", string.Empty, string.Empty, etiquetaFin);
            AgregarCuadruplo("LABEL", string.Empty, string.Empty, etiquetaFalsa);
            if (nodo.BloqueFalso != null)
            {
                EmitirSentencias(nodo.BloqueFalso.Statements);
                AgregarCuadruplo("LABEL", string.Empty, string.Empty, etiquetaFin);
            }
        }

        private void EmitirWhile(WhileNode nodo)
        {
            string inicio = NuevaEtiqueta();
            string cuerpo = NuevaEtiqueta();
            string fin = NuevaEtiqueta();
            AgregarCuadruplo("LABEL", string.Empty, string.Empty, inicio);
            EmitirSaltoCondicional(nodo.Condicion, cuerpo, fin);
            AgregarCuadruplo("LABEL", string.Empty, string.Empty, cuerpo);
            EmitirSentencias(nodo.Cuerpo.Statements);
            AgregarCuadruplo("GOTO", string.Empty, string.Empty, inicio);
            AgregarCuadruplo("LABEL", string.Empty, string.Empty, fin);
        }

        private void EmitirDoWhile(DoWhileNode nodo)
        {
            string inicio = NuevaEtiqueta();
            string fin = NuevaEtiqueta();
            AgregarCuadruplo("LABEL", string.Empty, string.Empty, inicio);
            EmitirSentencias(nodo.Cuerpo.Statements);
            EmitirSaltoCondicional(nodo.Condicion, inicio, fin);
            AgregarCuadruplo("LABEL", string.Empty, string.Empty, fin);
        }

        private void EmitirFor(ForNode nodo)
        {
            if (nodo.Inicializacion != null)
                EmitirAsignacion(nodo.Inicializacion.Identificador, nodo.Inicializacion.Valor);
            string inicio = NuevaEtiqueta();
            string cuerpo = NuevaEtiqueta();
            string fin = NuevaEtiqueta();
            AgregarCuadruplo("LABEL", string.Empty, string.Empty, inicio);
            EmitirSaltoCondicional(nodo.Condicion, cuerpo, fin);
            AgregarCuadruplo("LABEL", string.Empty, string.Empty, cuerpo);
            EmitirSentencias(nodo.Cuerpo.Statements);
            if (nodo.Incremento != null)
                EmitirAsignacion(nodo.Incremento.Identificador, nodo.Incremento.Valor);
            AgregarCuadruplo("GOTO", string.Empty, string.Empty, inicio);
            AgregarCuadruplo("LABEL", string.Empty, string.Empty, fin);
        }

        private void EmitirSaltoCondicional(AstNode? condicion, string destinoVerdadero, string destinoFalso)
        {
            if (condicion == null)
                throw new InvalidOperationException("No se puede generar IR: falta una condición en el AST.");

            if (condicion is UnaryExpressionNode negacion && negacion.Operador == "NO")
            {
                EmitirSaltoCondicional(negacion.Operando, destinoFalso, destinoVerdadero);
                return;
            }

            if (condicion is BinaryExpressionNode logica && logica.Operador == "Y")
            {
                string siguiente = NuevaEtiqueta();
                EmitirSaltoCondicional(logica.Izquierda, siguiente, destinoFalso);
                AgregarCuadruplo("LABEL", string.Empty, string.Empty, siguiente);
                EmitirSaltoCondicional(logica.Derecha, destinoVerdadero, destinoFalso);
                return;
            }

            if (condicion is BinaryExpressionNode logicaO && logicaO.Operador == "O")
            {
                string siguiente = NuevaEtiqueta();
                EmitirSaltoCondicional(logicaO.Izquierda, destinoVerdadero, siguiente);
                AgregarCuadruplo("LABEL", string.Empty, string.Empty, siguiente);
                EmitirSaltoCondicional(logicaO.Derecha, destinoVerdadero, destinoFalso);
                return;
            }

            string resultado = EmitirExpresion(condicion);
            AgregarCuadruplo("IF_TRUE", resultado, string.Empty, destinoVerdadero);
            AgregarCuadruplo("GOTO", string.Empty, string.Empty, destinoFalso);
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

            _contadorTemporales = 0;
            _contadorEtiquetas = 0;
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
        private static void ValidarOperadores(IReadOnlyCollection<Cuadruplo> instrucciones)
        {
            HashSet<string> operadoresSoportados = new(StringComparer.Ordinal)
            {
                "=", "NEG", "NOT", "POS", "+", "-", "*", "/", "%", "^",
                ">", "<", ">=", "<=", "==", "!=", "AND", "OR",
                "LABEL", "GOTO", "IF_FALSE", "IF_TRUE", "READ", "PRINT", "PARAM", "CALL", "RETURN", "CLEAR", "LOCATE"
            };

            foreach (Cuadruplo instruccion in instrucciones)
            {
                if (!operadoresSoportados.Contains(instruccion.Operador))
                    throw new InvalidOperationException($"El IR contiene la instrucción no soportada '{instruccion.Operador}'.");
            }
        }

        private static void ValidarEtiquetas(IReadOnlyCollection<Cuadruplo> instrucciones)
        {
            HashSet<string> etiquetas = instrucciones
                .Where(c => c.Operador == "LABEL")
                .Select(c => c.Resultado)
                .ToHashSet(StringComparer.Ordinal);

            if (instrucciones.Where(c => c.Operador == "LABEL").Any(c => string.IsNullOrWhiteSpace(c.Resultado)) ||
                etiquetas.Count != instrucciones.Count(c => c.Operador == "LABEL"))
                throw new InvalidOperationException("El IR contiene etiquetas vacías o duplicadas.");

            foreach (Cuadruplo instruccion in instrucciones.Where(c => c.Operador == "GOTO" || c.Operador == "IF_FALSE" || c.Operador == "IF_TRUE"))
            {
                if (!etiquetas.Contains(instruccion.Resultado))
                    throw new InvalidOperationException($"El salto {instruccion.Operador} apunta a la etiqueta inexistente '{instruccion.Resultado}'.");
            }
        }

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
                else if (c.Operador == "PARAM")
                {
                    _codigoTresDireccionesCuadruplos.Add($"param {c.Argumento1} #{c.Argumento2}");
                }
                else if (c.Operador == "CALL")
                {
                    _codigoTresDireccionesCuadruplos.Add($"{c.Resultado} = call {c.Argumento1}, {c.Argumento2}");
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
                else if (t.Operador == "PARAM")
                {
                    instruccion = $"param {t.Argumento1} #{t.Argumento2}";
                }
                else if (t.Operador == "CALL")
                {
                    instruccion = $"{t.Resultado} = call {t.Argumento1}, {t.Argumento2}";
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

        private static bool EsNumero(string texto)
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

        private string NuevaTemporal()
        {
            _contadorTemporales++;
            return $"t{_contadorTemporales}";
        }

        private string NuevaEtiqueta()
        {
            _contadorEtiquetas++;
            return $"L{_contadorEtiquetas}";
        }
    }
}
