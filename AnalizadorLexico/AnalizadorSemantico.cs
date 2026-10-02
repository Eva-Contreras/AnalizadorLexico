namespace AnalizadorLexico
{
    public class AnalizadorSemantico
    {
        private List<(int linea, string valor, string token)> _tokens = new();
        private int _pos;
        private int _ultimaLineaConsumida = 0;
        private Dictionary<(int ambitoId, string nombre), Simbolo> _tablaSimbolos = new();
        private int _stackPointer = 0; 
        private Stack<Ambito> _ambitos = new();
        private Ambito? _ambitoActual;
        private int _siguienteIdAmbito;
        private int _siguienteIdSimbolo;
        public List<string> Errores { get; private set; } = new();
        public List<string> DebugLog { get; private set; } = new();
        public List<Simbolo> TablaSimbolosFinal { get; private set; } = new();
        public ProgramNode? AstRoot { get; set; }

        private (int linea, string valor, string token) TokenActual => _pos < _tokens.Count ? _tokens[_pos] : (-1, "EOF", "EOF");

        public bool Analizar(List<(int linea, string valor, string token)> tokens)
        {
            _tokens = tokens.Where(t => t.token != "COM").ToList();
            _pos = 0;
            _ultimaLineaConsumida = 0;
            Errores.Clear();
            DebugLog.Clear();
            DebugLog.Add("Analizar: inicio");
            _tablaSimbolos.Clear();
            TablaSimbolosFinal.Clear();
            _ambitos.Clear();
            _ambitoActual = null;
            _siguienteIdAmbito = 0;
            _siguienteIdSimbolo = 0;
            _stackPointer = 0;

            EnterScope();

            try
            {
                while (TokenActual.token != "EOF")
                    ParseS();

                return Errores.Count == 0;
            }
            catch (Exception ex)
            {
                Errores.Add($"Error interno: {ex.Message}");
                return false;
            }
        }

        private void ParseS()
        {
            DebugLog.Add($"ParseS: token {_pos}: {TokenActual.token} '{TokenActual.valor}' (line {TokenActual.linea})");
            switch (TokenActual.token)
            {
                case "ENT": ParseDeclaracionENT(); break;
                case "DEC": ParseDeclaracionDEC(); break;
                case "CAD": ParseDeclaracionCAD(); break;
                case "PR1": ParseLeer(); break;
                case "PR2": ParseImprimir(); break;
                case "PR3": ParseRetornar(); break;
                case "PR4": ParseSentenciaIf(); break;
                case "PR6": ParseCasos(); break;
                case "PR9": ParseMientras(); break;
                case "PR10": ParseHacer(); break;
                case "PR11": ParsePara(); break;
                case "PR12": ParseLimpiar(); break;
                case "PR13": ParseUbicar(); break;
                default:
                    if (EsID(TokenActual.token))
                        ParseAsignacion();
                    else
                    {
                        Error($"Sentencia no reconocida '{TokenActual.valor}' en línea {TokenActual.linea}");
                        Avanzar();
                    }
                    break;
            }
        }

        private void ParseDeclaracionENT()
        {
            DebugLog.Add($"ParseDeclaracionENT at pos {_pos} token {TokenActual.token} '{TokenActual.valor}'");
            Consumir("ENT", "Se esperaba 'ENT'");
            string id = TokenActual.valor;
            int lineaDeclaracion = TokenActual.linea;
            if (!EsID(TokenActual.token))
            {
                Error($"Se esperaba identificador en declaración ENT en línea {TokenActual.linea}");
                return;
            }
            Consumir(TokenActual.token, "Se esperaba identificador");

            Consumir("OPA", "Se esperaba '=' después del identificador");

            string tokenInicio = TokenActual.token;
            string valorInicio = TokenActual.valor;

            string tipoInit = ParseARG2();

            Consumir("CD5", "Se esperaba ';' al final de declaración ENT");

            if (tipoInit == "ERROR") return;

            if (tipoInit != "ENT")
            {
                Error($"Tipo incompatible en inicialización de ENT: {tipoInit} en línea {TokenActual.linea}");
                return;
            }

            string valorRegistrar = "";
            if (tokenInicio == "CNU") 
                valorRegistrar = valorInicio;
            else if (EsID(tokenInicio)) 
                valorRegistrar = valorInicio;

            RegistrarSimbolo(id, "ENT", valorRegistrar, lineaDeclaracion);
        }

        private void ParseDeclaracionDEC()
        {
            DebugLog.Add($"ParseDeclaracionDEC at pos {_pos} token {TokenActual.token} '{TokenActual.valor}'");
            Consumir("DEC", "Se esperaba 'DEC'");
            string id = TokenActual.valor;
            int lineaDeclaracion = TokenActual.linea;
            if (!EsID(TokenActual.token))
            {
                Error($"Se esperaba identificador en declaración DEC en línea {TokenActual.linea}");
                return;
            }
            Consumir(TokenActual.token, "Se esperaba identificador");

            Consumir("OPA", "Se esperaba '=' después del identificador");

            string tokenInicio = TokenActual.token;
            string valorInicio = TokenActual.valor;

            string tipoInit = ParseARG2();

            Consumir("CD5", "Se esperaba ';' al final de declaración DEC");

            if (tipoInit == "ERROR") return;

            if (!EsNumerico(tipoInit))
            {
                Error($"Tipo incompatible en inicialización de DEC: {tipoInit} en línea {TokenActual.linea}");
                return;
            }

            string valorRegistrar = "";
            if (tokenInicio == "CNU" || tokenInicio == "CNR")
                valorRegistrar = valorInicio;
            else if (EsID(tokenInicio))
                valorRegistrar = valorInicio;

            RegistrarSimbolo(id, "DEC", valorRegistrar, lineaDeclaracion);
        }

        private void ParseDeclaracionCAD()
        {
            DebugLog.Add($"ParseDeclaracionCAD at pos {_pos} token {TokenActual.token} '{TokenActual.valor}'");
            Consumir("CAD", "Se esperaba 'CAD'");
            string id = TokenActual.valor;
            int lineaDeclaracion = TokenActual.linea;
            if (!EsID(TokenActual.token))
            {
                Error($"Se esperaba identificador en declaración CAD en línea {TokenActual.linea}");
                return;
            }
            Consumir(TokenActual.token, "Se esperaba identificador");

            Consumir("OPA", "Se esperaba '=' después del identificador");

            string tokenInicio = TokenActual.token;
            string valorInicio = TokenActual.valor;

            string tipoInit = ParseARG2();

            Consumir("CD5", "Se esperaba ';' al final de declaración CAD");

            if (tipoInit == "ERROR") return;

            if (tipoInit != "CAD")
            {
                Error($"Tipo incompatible en inicialización de CAD: {tipoInit} en línea {TokenActual.linea}");
                return;
            }

            string valorRegistrar = "";
            if (tokenInicio == "CAD")
            {
                valorRegistrar = valorInicio;
            }
            else if (EsID(tokenInicio))
            {
                var simboloInicial = BuscarSimbolo(valorInicio);
                if (simboloInicial != null)
                    valorRegistrar = simboloInicial.Valor ?? "";
            }

            RegistrarSimbolo(id, "CAD", valorRegistrar, lineaDeclaracion);
        }

        private void ParseAsignacion()
        {
            DebugLog.Add($"ParseAsignacion at pos {_pos} token {TokenActual.token} '{TokenActual.valor}'");
            string id = TokenActual.valor;
            if (!EsID(TokenActual.token))
            {
                Error($"Se esperaba identificador en asignación en línea {TokenActual.linea}");
                return;
            }
            Consumir(TokenActual.token, "Se esperaba identificador");

            Consumir("OPA", "Se esperaba '=' en asignación");

            string tokenInicio = TokenActual.token;
            string valorInicio = TokenActual.valor;

            string tipoRHS = ParseARG2();

            Consumir("CD5", "Se esperaba ';' al final de asignación");

            var simbolo = BuscarSimbolo(id);
            if (simbolo == null)
            {
                Error($"Variable '{id}' no declarada en línea {TokenActual.linea}");
                return;
            }

            if (!TiposCompatibles(simbolo.Tipo, tipoRHS))
            {
                Error($"Tipo incompatible en asignación a '{id}'. Esperado: {simbolo.Tipo}, obtenido: {tipoRHS} en línea {TokenActual.linea}");
                return;
            }

            if (tipoRHS != "ERROR")
            {
                string valorRegistrar = "";
                if (tokenInicio == "CNU" || tokenInicio == "CNR")
                {
                    valorRegistrar = valorInicio;
                }
                else if (EsID(tokenInicio))
                {
                    string idInit = valorInicio;
                    var simInit = BuscarSimbolo(idInit);
                    if (simInit != null)
                    {
                        valorRegistrar = simInit.Valor ?? "";
                    }
                }

                if (!string.IsNullOrEmpty(valorRegistrar))
                    simbolo.Valor = valorRegistrar;
            }
        }

        private void ParseLeer()
        {
            DebugLog.Add($"ParseLeer at pos {_pos} token {TokenActual.token} '{TokenActual.valor}'");
            Consumir("PR1", "Se esperaba 'leer'");
            Consumir("CD3", "Se esperaba '(' después de 'leer'");

            string id = TokenActual.valor;
            if (!EsID(TokenActual.token))
            {
                Error($"Se esperaba identificador en 'leer' en línea {TokenActual.linea}");
                return;
            }
            Consumir(TokenActual.token, "Se esperaba identificador");

            Consumir("CD4", "Se esperaba ')' en 'leer'");
            Consumir("CD5", "Se esperaba ';' al final de 'leer'");

            if (BuscarSimbolo(id) == null)
            {
                Error($"Variable '{id}' no declarada en línea {TokenActual.linea}");
            }
        }

        private void ParseImprimir()
        {
            DebugLog.Add($"ParseImprimir at pos {_pos} token {TokenActual.token} '{TokenActual.valor}'");
            Consumir("PR2", "Se esperaba 'imprimir'");
            Consumir("CD3", "Se esperaba '(' después de 'imprimir'");

            ParseARG2(); 

            Consumir("CD4", "Se esperaba ')' en 'imprimir'");
            Consumir("CD5", "Se esperaba ';' al final de 'imprimir'");
        }

        private void ParseRetornar()
        {
            DebugLog.Add($"ParseRetornar at pos {_pos} token {TokenActual.token} '{TokenActual.valor}'");
            Consumir("PR3", "Se esperaba 'retornar'");
            ParseARG2(); 
            Consumir("CD5", "Se esperaba ';' al final de 'retornar'");
        }

        private void ParseSentenciaIf()
        {
            DebugLog.Add($"ParseSentenciaIf at pos {_pos} token {TokenActual.token} '{TokenActual.valor}'");
            Consumir("PR4", "Se esperaba 'si'");
            Consumir("CD3", "Se esperaba '(' después de 'si'");

            string tipoCond = ParseCondic();

            Consumir("CD4", "Se esperaba ')' después de la condición");
            Consumir("CD1", "Se esperaba '{' para abrir bloque SI");

            if (tipoCond != "BOOL")
            {
                Error($"Se esperaba condición booleana en línea {TokenActual.linea}");
            }

            ParseBloque();

            Consumir("CD2", "Se esperaba '}' para cerrar bloque SI");

            if (TokenActual.token == "PR5")
            {
                Consumir("PR5", "Se esperaba 'sino'");
                Consumir("CD1", "Se esperaba '{' para abrir bloque SINO");
                ParseBloque();
                Consumir("CD2", "Se esperaba '}' para cerrar bloque SINO");
            }
        }

        private void ParseCasos()
        {
            DebugLog.Add($"ParseCasos at pos {_pos} token {TokenActual.token} '{TokenActual.valor}'");
            Consumir("PR6", "Se esperaba 'casos'");
            Consumir("CD3", "Se esperaba '(' después de 'casos'");
            ParseARG1();
            Consumir("CD4", "Se esperaba ')' en 'casos'");
            Consumir("CD1", "Se esperaba '{' para abrir CASOS");
            ParseOpciones();
            ParsePred();
            Consumir("CD2", "Se esperaba '}' para cerrar CASOS");
        }

        private void ParseOpciones()
        {
            ParseOpcion();
            while (TokenActual.token == "PR7")
                ParseOpcion();
        }

        private void ParseOpcion()
        {
            Consumir("PR7", "Se esperaba 'opcion'");
            ParseARG3();
            Consumir("CD9", "Se esperaba ':' después de ARG3 en opcion");
            ParseS();
            Consumir("CD11", "Se esperaba 'terminar' al final de opcion");
            Consumir("CD5", "Se esperaba ';' después de 'terminar'");
        }

        private void ParsePred()
        {
            Consumir("PR8", "Se esperaba 'predefinido'");
            Consumir("CD9", "Se esperaba ':' después de 'predefinido'");
            ParseS();
            Consumir("CD11", "Se esperaba 'terminar' al final de predefinido");
            Consumir("CD5", "Se esperaba ';' después de 'terminar'");
        }

        private void ParseMientras()
        {
            DebugLog.Add($"ParseMientras at pos {_pos} token {TokenActual.token} '{TokenActual.valor}'");
            Consumir("PR9", "Se esperaba 'mientras'");
            Consumir("CD3", "Se esperaba '(' después de 'mientras'");

            string tipoCond = ParseCondic();

            Consumir("CD4", "Se esperaba ')' después de la condición en 'mientras'");
            Consumir("CD1", "Se esperaba '{' para abrir bloque 'mientras'");

            if (tipoCond != "BOOL")
            {
                Error($"Se esperaba condición booleana en 'mientras' en línea {TokenActual.linea}");
            }

            ParseBloque();
            Consumir("CD2", "Se esperaba '}' para cerrar bloque 'mientras'");
        }

        private void ParseHacer()
        {
            DebugLog.Add($"ParseHacer at pos {_pos} token {TokenActual.token} '{TokenActual.valor}'");
            Consumir("PR10", "Se esperaba 'hacer'");
            Consumir("CD1", "Se esperaba '{' para abrir bloque 'hacer'");
            ParseBloque();
            Consumir("CD2", "Se esperaba '}' para cerrar bloque 'hacer'");
            Consumir("PR9", "Se esperaba 'mientras' después del bloque 'hacer'");
            Consumir("CD3", "Se esperaba '(' después de 'mientras' en 'hacer'");

            string tipoCond = ParseCondic();

            Consumir("CD4", "Se esperaba ')' después de la condición en 'hacer'");
            Consumir("CD5", "Se esperaba ';' al final de 'hacer-mientras'");

            if (tipoCond != "ERROR" && tipoCond != "BOOL")
            {
                Error($"Se esperaba condición booleana en 'hacer' en línea {TokenActual.linea}");
            }
        }

        private void ParsePara()
        {
            DebugLog.Add($"ParsePara at pos {_pos} token {TokenActual.token} '{TokenActual.valor}'");
            Consumir("PR11", "Se esperaba 'para'");
            Consumir("CD3", "Se esperaba '(' después de 'para'");
            Consumir("ENT", "Se esperaba 'ENT' en inicialización de 'para'");

            string id = TokenActual.valor;
            int lineaDeclaracion = TokenActual.linea;
            if (!EsID(TokenActual.token))
            {
                Error($"Se esperaba identificador en inicialización de 'para' en línea {TokenActual.linea}");
                SkipToEndOfStatement();
                return;
            }
            Consumir(TokenActual.token, "Se esperaba identificador");

            Consumir("OPA", "Se esperaba '=' en inicialización de 'para'");

            string initValor = TokenActual.valor;
            if (TokenActual.token != "CNU")
            {
                Error($"Se esperaba constante entera (CNU) en 'para', se encontró '{TokenActual.valor}' en línea {TokenActual.linea}");
                SkipToEndOfStatement();
                return;
            }
            Consumir("CNU", "Se esperaba constante entera");

            Consumir("CD5", "Se esperaba ';' después de inicialización en 'para'");

            if (BuscarSimbolo(id) == null)
                RegistrarSimbolo(id, "ENT", initValor, lineaDeclaracion);

            string tipoCond = ParseCondic();
            Consumir("CD5", "Se esperaba ';' después de condición en 'para'");

            string idIncremento = TokenActual.valor;
            if (!EsID(TokenActual.token))
            {
                Error($"Se esperaba identificador en incremento de 'para' en línea {TokenActual.linea}");
                SkipToEndOfStatement();
                return;
            }
            Consumir(TokenActual.token, "Se esperaba identificador");

            Consumir("OPA", "Se esperaba '=' en incremento de 'para'");
            string tipoIncremento = ParseARG2();

            Consumir("CD4", "Se esperaba ')' para cerrar cabecera de 'para'");

            if (tipoIncremento != "ERROR")
            {
                var simboloIncremento = BuscarSimbolo(idIncremento);
                if (simboloIncremento == null)
                {
                    Error($"Variable '{idIncremento}' no declarada en 'para' en línea {TokenActual.linea}");
                }
                else if (!TiposCompatibles(simboloIncremento.Tipo, tipoIncremento))
                {
                    Error($"Tipo incompatible en incremento de 'para' en línea {TokenActual.linea}");
                }
            }

            if (tipoCond != "ERROR" && tipoCond != "BOOL")
            {
                Error($"Se esperaba condición booleana en 'para' en línea {TokenActual.linea}");
            }

            Consumir("CD1", "Se esperaba '{' para abrir bloque 'para'");
            ParseBloque();
            Consumir("CD2", "Se esperaba '}' para cerrar bloque 'para'");
        }

        private void ParseLimpiar()
        {
            DebugLog.Add($"ParseLimpiar at pos {_pos} token {TokenActual.token} '{TokenActual.valor}'");
            Consumir("PR12", "Se esperaba 'limpiar'");
            Consumir("CD5", "Se esperaba ';' después de 'limpiar'");
        }

        private void ParseUbicar()
        {
            DebugLog.Add($"ParseUbicar at pos {_pos} token {TokenActual.token} '{TokenActual.valor}'");
            Consumir("PR13", "Se esperaba 'ubicar'");
            Consumir("CD3", "Se esperaba '(' en ARG5");
            ParseARG6();
            Consumir("CD6", "Se esperaba ',' entre argumentos de 'ubicar'");
            ParseARG6();
            Consumir("CD4", "Se esperaba ')' para cerrar ARG5");
            Consumir("CD5", "Se esperaba ';' al final de 'ubicar'");
        }

        private void ParseARG6()
        {
            DebugLog.Add($"ParseARG6 at pos {_pos} token {TokenActual.token} '{TokenActual.valor}'");
            if (EsID(TokenActual.token) || TokenActual.token == "CNU")
            {
                Consumir(TokenActual.token, "Se esperaba identificador o constante entera");
            }
            else
            {
                Error($"Se esperaba identificador o constante entera en ARG6, se encontró '{TokenActual.valor}' en línea {TokenActual.linea}");
            }
        }

        private void ParseBloque()
        {
            DebugLog.Add($"ParseBloque at pos {_pos} token {TokenActual.token} '{TokenActual.valor}'");
            EnterScope();
            while (TokenActual.token != "CD2" && TokenActual.token != "EOF")
                ParseS();
            ExitScope();
        }

        private string ParseCondic()
        {
            return ParseOPL();
        }

        private string ParseOPL()
        {
            string tipoIzquierdo = ParseCondicionBase();
            bool tieneOperadorLogico = false;
            bool hayError = tipoIzquierdo == "ERROR";

            while (EsOLBinario(TokenActual.token))
            {
                tieneOperadorLogico = true;
                Consumir(TokenActual.token, "Se esperaba operador lógico");
                string tipoDerecho = ParseCondicionBase();

                if (tipoIzquierdo == "ERROR" || tipoDerecho == "ERROR")
                {
                    hayError = true;
                    continue;
                }

                if (tipoIzquierdo != "BOOL" || tipoDerecho != "BOOL")
                {
                    Error($"Operación lógica requiere operandos booleanos en línea {TokenActual.linea}");
                    hayError = true;
                }

                tipoIzquierdo = "BOOL";
            }

            if (hayError) return "ERROR";
            return tieneOperadorLogico ? "BOOL" : tipoIzquierdo;
        }

        private string ParseCondicionBase()
        {
            if (TokenActual.token == "OL2")
            {
                Consumir("OL2", "Se esperaba operador NO");
                string tipoNegado = ParseCondicionBase();

                if (tipoNegado == "ERROR") return "ERROR";
                if (tipoNegado != "BOOL")
                {
                    Error($"El operador 'NO' requiere una condición booleana en línea {TokenActual.linea}");
                    return "ERROR";
                }

                return "BOOL";
            }

            int posTemp = _pos;
            AvanzarARG7Lookahead(ref posTemp);
            string operador = posTemp < _tokens.Count ? _tokens[posTemp].token : "EOF";
            return EsOR(operador) ? ParseOPR() : ParseARG7();
        }

        private string ParseOPR()
        {
            string tipo1 = ParseARG7();
            if (!EsOR(TokenActual.token))
            {
                Error($"Se esperaba operador relacional en línea {TokenActual.linea}, se encontró '{TokenActual.valor}'");
                return "ERROR";
            }
            Consumir(TokenActual.token, "Se esperaba operador relacional");
            string tipo2 = ParseARG7();
            if (tipo1 == "ERROR" || tipo2 == "ERROR") return "ERROR";

            if (!TiposCompatibles(tipo1, tipo2))
            {
                Error($"Tipos incompatibles en operación relacional: {tipo1} y {tipo2} en línea {TokenActual.linea}");
                return "ERROR";
            }

            return "BOOL";
        }

        private string ParseARG7()
        {
            DebugLog.Add($"ParseARG7 at pos {_pos} token {TokenActual.token} '{TokenActual.valor}'");
            if (TokenActual.token == "CD3")
            {
                Consumir("CD3", "Se esperaba '('");
                string tipoResultado = ParseCondic();
                Consumir("CD4", "Se esperaba ')' para cerrar ARG7");
                return tipoResultado;
            }
            else if (EsID(TokenActual.token))
            {
                string id = TokenActual.valor;
                Consumir(TokenActual.token, "Se esperaba identificador");

                var simbolo = BuscarSimbolo(id);
                if (simbolo == null)
                {
                    Error($"Variable '{id}' no declarada en línea {TokenActual.linea}");
                    if (_pos < _tokens.Count && EsOA(TokenActual.token))
                    {
                        Avanzar();
                        ParseARG4();
                    }
                    return "ERROR";
                }

                string tipoLeft = simbolo.Tipo;
                while (_pos < _tokens.Count && EsOA(TokenActual.token))
                {
                    string opToken = TokenActual.token;
                    string opLexema = TokenActual.valor;
                    Avanzar(); 
                    string tipoRight = ParseARG4();
                    if (tipoLeft == "ERROR" || tipoRight == "ERROR") return "ERROR";

                    bool esMas = opToken == "MAS" || opLexema == "+" || opToken == "OA1";
                    if (esMas)
                    {
                        if (tipoLeft == "CAD" && tipoRight == "CAD")
                        {
                            tipoLeft = "CAD";
                            continue;
                        }
                        if (EsNumerico(tipoLeft) && EsNumerico(tipoRight))
                        {
                            tipoLeft = (tipoLeft == "DEC" || tipoRight == "DEC") ? "DEC" : "ENT";
                            continue;
                        }
                        Error($"Operador '+' no admite operandos de tipos {tipoLeft} y {tipoRight} en línea {TokenActual.linea}");
                        return "ERROR";
                    }

                    if (!EsNumerico(tipoLeft) || !EsNumerico(tipoRight))
                    {
                        Error($"Operación aritmética requiere operandos numéricos en línea {TokenActual.linea}. Tipos: {tipoLeft} y {tipoRight}");
                        return "ERROR";
                    }

                    tipoLeft = (tipoLeft == "DEC" || tipoRight == "DEC") ? "DEC" : "ENT";
                }

                return tipoLeft;
            }
            else if (EsCN(TokenActual.token))
            {
                string tipo = TokenActual.token == "CNU" ? "ENT" : "DEC";
                Consumir(TokenActual.token, "Se esperaba constante numérica");
                return tipo;
            }
            else
            {
                Error($"Se esperaba identificador, número o expresión en línea {TokenActual.linea}, se encontró '{TokenActual.valor}'");
                Avanzar();
                return "ERROR";
            }
        }

        private string ParseARG2()
        {
            DebugLog.Add($"ParseARG2 at pos {_pos} token {TokenActual.token} '{TokenActual.valor}'");
            if (TokenActual.token == "CAD")
            {
                string literal = TokenActual.valor;
                Consumir("CAD", "Se esperaba literal de cadena");

                if (_pos < _tokens.Count && EsOA(TokenActual.token))
                {
                    string opToken = TokenActual.token;
                    string opValor = TokenActual.valor;
                    Consumir(TokenActual.token, "Se esperaba operador aritmético");

                    string tipoRight = ParseARG4();
                    if (tipoRight == "ERROR") return "ERROR";

                    bool esMas = opValor == "+" || opToken == "OA1" || opToken == "MAS";
                    if (esMas)
                    {
                        if (tipoRight == "CAD" || EsNumerico(tipoRight))
                            return "CAD";
                        Error($"Operador '+' no admite operandos de tipo {tipoRight} en línea {TokenActual.linea}");
                        return "ERROR";
                    }

                    Error($"Operación aritmética requiere operandos numéricos en línea {TokenActual.linea}. Tipos: CAD y {tipoRight}");
                    return "ERROR";
                }

                return "CAD";
            }
            else if (EsID(TokenActual.token))
            {
                string id = TokenActual.valor;
                Consumir(TokenActual.token, "Se esperaba identificador");

                var simbolo = BuscarSimbolo(id);
                if (simbolo == null)
                {
                    Error($"Variable '{id}' no declarada en línea {TokenActual.linea}");
                    if (_pos < _tokens.Count && EsOA(TokenActual.token))
                    {
                        Avanzar();
                        ParseARG4();
                    }
                    return "ERROR";
                }

                string tipoLeft = simbolo.Tipo;
                while (_pos < _tokens.Count && EsOA(TokenActual.token))
                {
                    string opToken = TokenActual.token;
                    string opLexema = TokenActual.valor;
                    Consumir(TokenActual.token, "Se esperaba operador aritmético");

                    string tipoRight = ParseARG4();
                    if (tipoLeft == "ERROR" || tipoRight == "ERROR") return "ERROR";

                    bool esMas = opToken == "MAS" || opLexema == "+" || opToken == "OA1";
                    if (esMas)
                    {
                        if (tipoLeft == "CAD" && (tipoRight == "CAD" || EsNumerico(tipoRight)))
                        {
                            tipoLeft = "CAD";
                            continue;
                        }
                        if (EsNumerico(tipoLeft) && EsNumerico(tipoRight))
                        {
                            tipoLeft = (tipoLeft == "DEC" || tipoRight == "DEC") ? "DEC" : "ENT";
                            continue;
                        }

                        Error($"Operador '+' no admite operandos de tipos {tipoLeft} y {tipoRight} en línea {TokenActual.linea}");
                        return "ERROR";
                    }

                    if (!EsNumerico(tipoLeft) || !EsNumerico(tipoRight))
                    {
                        Error($"Operación aritmética requiere operandos numéricos en línea {TokenActual.linea}. Tipos: {tipoLeft} y {tipoRight}");
                        return "ERROR";
                    }

                    tipoLeft = (tipoLeft == "DEC" || tipoRight == "DEC") ? "DEC" : "ENT";
                }

                return tipoLeft;
            }
            else if (EsCN(TokenActual.token))
            {
                string tipo = TokenActual.token == "CNU" ? "ENT" : "DEC";
                Consumir(TokenActual.token, "Se esperaba constante numérica");
                return tipo;
            }
            else if (TokenActual.token == "CD3")
            {
                return ParseOPA();
            }
            else
            {
                Error($"Se esperaba argumento válido en ARG2, se encontró '{TokenActual.valor}' en línea {TokenActual.linea}");
                Avanzar();
                return "ERROR";
            }
        }

        private void ParseARG3()
        {
            if (TokenActual.token == "CAD")
            {
                Consumir("CAD", "Se esperaba literal de cadena");
                return;
            }

            if (EsID(TokenActual.token) || EsCN(TokenActual.token) || TokenActual.token == "CD3")
            {
                int posTemp = _pos;
                AvanzarARG7Lookahead(ref posTemp);
                string opSig = posTemp < _tokens.Count ? _tokens[posTemp].token : "EOF";

                if (EsOL(opSig) || EsOR(opSig))
                    ParseCondic();
                else if (EsID(TokenActual.token))
                    ParseARG1();
                else
                    Consumir(TokenActual.token, "Se esperaba constante");
            }
            else
            {
                Error($"Se esperaba argumento válido en ARG3, se encontró '{TokenActual.valor}' en línea {TokenActual.linea}");
            }
        }

        private void ParseARG1()
        {
            if (EsID(TokenActual.token))
            {
                string id = TokenActual.valor;
                Consumir(TokenActual.token, "Se esperaba identificador");

                if (BuscarSimbolo(id) == null)
                {
                    Error($"Variable '{id}' no declarada en línea {TokenActual.linea}");
                }
            }
            else
            {
                Error($"Se esperaba identificador (ARG1) en línea {TokenActual.linea}, se encontró '{TokenActual.valor}'");
            }
        }

        private string ParseOPA()
        {
            if (TokenActual.token == "CD3")
            {
                Consumir("CD3", "Se esperaba '('");
                string tipoResult = ParseOPA();
                Consumir("CD4", "Se esperaba ')' después de OPA");
                return tipoResult;
            }
            else
            {
                string tipo1 = ParseARG4();
                if (EsOA(TokenActual.token))
                {
                    string opToken = TokenActual.token;
                    string opValor = TokenActual.valor;
                    Consumir(TokenActual.token, "Se esperaba operador aritmético");
                    string tipo2 = ParseARG4();

                    if (tipo1 == "ERROR" || tipo2 == "ERROR") return "ERROR";

                    bool esMas = opValor == "+" || opToken == "OA1" || opToken == "MAS";
                    if (esMas && (tipo1 == "CAD" || tipo2 == "CAD"))
                    {
                        return "CAD";
                    }

                    if (!EsNumerico(tipo1) || !EsNumerico(tipo2))
                    {
                        Error($"Operación aritmética requiere operandos numéricos en línea {TokenActual.linea}. Tipos: {tipo1} y {tipo2}");
                        return "ERROR";
                    }

                    if (tipo1 == "DEC" || tipo2 == "DEC")
                        return "DEC";
                    return "ENT";
                }
                return tipo1;
            }
        }

        private string ParseARG4()
        {
            if (TokenActual.token == "CD3")
            {
                Consumir("CD3", "Se esperaba '('");
                string tipoResult = ParseOPA();
                Consumir("CD4", "Se esperaba ')' después de OPA");
                return tipoResult;
            }
            else if (EsID(TokenActual.token))
            {
                string id = TokenActual.valor;
                Consumir(TokenActual.token, "Se esperaba identificador");

                var simbolo = BuscarSimbolo(id);
                if (simbolo == null)
                {
                    Error($"Variable '{id}' no declarada en línea {TokenActual.linea}");
                    int posTemp = _pos;
                    if (posTemp < _tokens.Count && EsOA(_tokens[posTemp].token))
                        ParseOPA();
                    return "ERROR";
                }
                return simbolo.Tipo;
            }
            else if (EsCN(TokenActual.token))
            {
                string tipo = TokenActual.token == "CNU" ? "ENT" : "DEC";
                Consumir(TokenActual.token, "Se esperaba constante numérica");
                return tipo;
            }
            else if (TokenActual.token == "CAD")
            {
                Consumir("CAD", "Se esperaba literal de cadena");
                return "CAD";
            }
            else
            {
                Error($"Se esperaba identificador, número, cadena o expresión aritmética en línea {TokenActual.linea}, se encontró '{TokenActual.valor}'");
                Avanzar();
                return "ERROR";
            }
        }

        private void RegistrarSimbolo(string id, string tipo, string valor, int linea)
        {
            if (_ambitoActual != null && _tablaSimbolos.ContainsKey((_ambitoActual.Id, id)))
            {
                Error($"Variable '{id}' ya declarada en línea {linea}");
                return;
            }

            int size = SizeOfType(tipo);
            int address = _stackPointer;
            _stackPointer += size;

            var simbolo = new Simbolo
            {
                Id = ++_siguienteIdSimbolo,
                Nombre = id,
                Tipo = tipo,
                Valor = valor,
                Linea = linea,
                Address = address,
                Size = size,
                Ambito = _ambitoActual?.Padre is null ? "Global" : "Local"
            };

            _tablaSimbolos[(_ambitoActual?.Id ?? 0, id)] = simbolo;
            TablaSimbolosFinal.Add(simbolo);

            _ambitoActual?.Simbolos.Add(id);

            DebugLog.Add($"RegistrarSimbolo: '{id}' tipo={tipo} addr={address} size={size} line={linea}");

            AnnotateDeclarationInAst(id, address, size);
        }

        private int SizeOfType(string tipo)
        {
            return tipo switch
            {
                "ENT" => 4,
                "DEC" => 8,
                "CAD" => 8, 
                _ => 4
            };
        }

        private void AnnotateDeclarationInAst(string id, int address, int size)
        {
            if (AstRoot == null) return;

            foreach (var stmt in AstRoot.Statements.OfType<DeclarationNode>())
            {
                if (stmt.Identificador == id && stmt.MemoryAddress == null)
                {
                    stmt.MemoryAddress = address;
                    stmt.Size = size;
                    DebugLog.Add($"AST annotated: decl {id} -> addr={address} size={size}");
                    return;
                }
            }
        }

        private void EnterScope()
        {
            int id = ++_siguienteIdAmbito;
            var ambito = new Ambito
            {
                Id = id,
                NombreCompleto = _ambitoActual == null
                    ? "Global"
                    : $"{_ambitoActual.NombreCompleto}/Bloque{id}",
                Padre = _ambitoActual,
                StackPointerGuardado = _stackPointer
            };
            _ambitos.Push(ambito);
            _ambitoActual = ambito;
            DebugLog.Add($"EnterScope: {_ambitoActual.NombreCompleto}, sp={_stackPointer}");
        }

        private void ExitScope()
        {
            if (_ambitos.Count > 0)
            {
                var ambito = _ambitos.Pop();
                DebugLog.Add($"ExitScope: {ambito.NombreCompleto}, sp={_stackPointer}");
                foreach (var name in ambito.Simbolos)
                {
                    if (_tablaSimbolos.Remove((ambito.Id, name)))
                    {
                        DebugLog.Add($"Free symbol: {name}");
                    }
                }
                _stackPointer = ambito.StackPointerGuardado;
                _ambitoActual = ambito.Padre;
                DebugLog.Add($"Restore sp={_stackPointer}");
            }
        }

        private Simbolo? BuscarSimbolo(string nombre)
        {
            for (var ambito = _ambitoActual; ambito != null; ambito = ambito.Padre)
            {
                if (_tablaSimbolos.TryGetValue((ambito.Id, nombre), out var simbolo))
                    return simbolo;
            }

            return null;
        }

        private sealed class Ambito
        {
            public int Id { get; init; }
            public string NombreCompleto { get; init; } = "";
            public Ambito? Padre { get; init; }
            public int StackPointerGuardado { get; init; }
            public List<string> Simbolos { get; } = new();
        }

        private bool TiposCompatibles(string tipo1, string tipo2)
        {
            if (tipo1 == tipo2) return true;
            if (tipo1 == "ENT" && tipo2 == "DEC") return true;
            if (tipo1 == "DEC" && tipo2 == "ENT") return true;
            return false;
        }

        private bool EsNumerico(string tipo)
        {
            return tipo == "ENT" || tipo == "DEC";
        }

        private bool EsID(string token) => token != null && token.StartsWith("IDV");
        private bool EsCN(string token) => token == "CNU" || token == "CNR";
        private bool EsOL(string token) => token == "OL1" || token == "OL2" || token == "OL3";
        private bool EsOLBinario(string token) => token == "OL1" || token == "OL3";
        private bool EsOR(string token)
        {
            if (string.IsNullOrEmpty(token)) return false;
            if (token.StartsWith("OR")) return true;

            switch (token)
            {
                case "MAYOR":
                case "MENOR":
                case "IGUAL":
                case "MAYOR_IGUAL":
                case "MENOR_IGUAL":
                case "DISTINTO":
                case ">=":
                case "<=":
                case "<>":
                case ">":
                case "<":
                    return true;
                default:
                    return false;
            }
        }
        private bool EsOA(string token) =>
            token == "OA1" || token == "OA2" || token == "OA3" || token == "OA4" || token == "OA5"
            || token == "MAS" || token == "MENOS" || token == "ASTERISCO" || token == "SLASH";

        private void Consumir(string tokenEsperado, string mensajeError)
        {
            if (TokenActual.token == tokenEsperado)
            {
                _ultimaLineaConsumida = TokenActual.linea;
                Avanzar();
            }
            else
            {
                Error($"{mensajeError} — línea {TokenActual.linea}, se encontró '{TokenActual.valor}' ({TokenActual.token})");
                Avanzar();
            }
        }

        private void Avanzar()
        {
            if (_pos < _tokens.Count)
            {
                _ultimaLineaConsumida = TokenActual.linea;
                _pos++;
            }
        }

        private void Error(string mensaje)
        {
            Errores.Add($"ERROR SEMÁNTICO → {mensaje}");
        }

        private void AvanzarARG7Lookahead(ref int pos)
        {
            if (pos >= _tokens.Count) return;
            string tok = _tokens[pos].token;

            if (tok == "CD3")
            {
                int depth = 1;
                pos++;
                while (pos < _tokens.Count && depth > 0)
                {
                    if (_tokens[pos].token == "CD3") depth++;
                    else if (_tokens[pos].token == "CD4") depth--;
                    pos++;
                }
            }
            else if (EsID(tok) || EsCN(tok))
            {
                pos++;
                if (pos < _tokens.Count && EsOA(_tokens[pos].token))
                {
                    pos++;
                    if (pos < _tokens.Count && (EsID(_tokens[pos].token) || EsCN(_tokens[pos].token)))
                        pos++;
                }
            }
        }

        private void SkipToEndOfStatement()
        {
            while (_pos < _tokens.Count && TokenActual.token != "CD5" && TokenActual.token != "CD2" && TokenActual.token != "CD4" && TokenActual.token != "EOF")
            {
                _pos++;
            }

            if (TokenActual.token == "CD5" || TokenActual.token == "CD2" || TokenActual.token == "CD4")
                Avanzar();
        }

    }

    public class Simbolo
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = "";
        public string Tipo { get; set; } = "";
        public string Valor { get; set; } = "";
        public int Linea { get; set; }
        public int Address { get; set; }
        public int Size { get; set; }
        public string Ambito { get; set; } = "Global";
    }
}