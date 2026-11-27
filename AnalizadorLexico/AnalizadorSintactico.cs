namespace AnalizadorLexico
{
    public class AnalizadorSintactico
    {
        private List<(int linea, string valor, string token)> _tokens = new();
        private int _pos;
        private int _ultimaLineaConsumida = 0;
        private List<AstNode> _sentenciasActuales = new();
        public List<string> Pasos { get; private set; } = new();
        public List<string> Errores { get; private set; } = new();
        public bool EsValido => Errores.Count == 0;
        public ProgramNode AstRoot { get; private set; } = new ProgramNode();
        private (int linea, string valor, string token) TokenActual => _pos < _tokens.Count ? _tokens[_pos] : (-1, "EOF", "EOF");
        public bool Analizar(List<(int linea, string valor, string token)> tokens)
        {
            _tokens = tokens.Where(t => t.token != "COM").ToList();
            _pos = 0;
            _ultimaLineaConsumida = 0;
            Pasos.Clear();
            Errores.Clear();
            AstRoot = new ProgramNode();
            _sentenciasActuales = AstRoot.Statements;

            Pasos.Add("=== INICIO DEL ANÁLISIS SINTÁCTICO ===");

            while (TokenActual.token != "EOF")
                ParseS();

            Pasos.Add(EsValido
                ? "=== ANÁLISIS COMPLETADO: CADENA ACEPTADA ✔ ==="
                : "=== ANÁLISIS COMPLETADO: CADENA RECHAZADA ✘ ===");

            return EsValido;
        }
        private void ParseS()
        {
            Pasos.Add($"[S] línea {TokenActual.linea} — token: {TokenActual.token} '{TokenActual.valor}'");

            switch (TokenActual.token)
            {
                case "ENT": ParseDeclaracionENT(); break;
                case "DEC": ParseDeclaracionDEC(); break;
                case "CAD": ParseDeclaracionCAD(); break;
                case "PR1": ParseLeer(); break;
                case "PR2": ParseImprimir(); break;
                case "PR3": ParseRetornar(); break;
                case "PR14": ParseFuncion(); break;
                case "PR4": ParseSentenciaIf(); break;
                case "PR6": ParseCasos(); break;
                case "PR9": ParseMientras(); break;
                case "PR10": ParseHacer(); break;
                case "PR11": ParsePara(); break;
                case "PR12": ParseLimpiar(); break;
                case "PR13": ParseUbicar(); break;
                default:
                    if (EsID(TokenActual.token))
                    {
                        if (_pos + 1 < _tokens.Count && _tokens[_pos + 1].token == "CD3")
                            ParseLlamadaFuncion();
                        else
                            ParseAsignacion();
                    }
                    else
                    {
                        Error($"Sentencia no reconocida '{TokenActual.valor}' ({TokenActual.token}) en línea {_ultimaLineaConsumida}");
                        Avanzar();
                    }
                    break;
            }
        }

        private void ParseLlamadaFuncion()
        {
            int inicio = _pos;
            ParseLlamadaFuncionExpresion();
            CallNode? llamada = ConstruirExpresion(inicio, _pos) as CallNode;
            Consumir("CD5", "Se esperaba ';' al final de la llamada de función");
            if (llamada != null)
                _sentenciasActuales.Add(llamada);
        }

        private void ParseFuncion()
        {
            Consumir("PR14", "Se esperaba 'FUNCION'");
            string nombre = LeerIdentificador();
            Consumir("CD3", "Se esperaba '(' después del nombre de la función");
            FunctionNode funcion = new() { Nombre = nombre };

            if (TokenActual.token != "CD4")
            {
                do
                {
                    string tipo = TokenActual.token;
                    if (tipo != "ENT" && tipo != "DEC" && tipo != "CAD")
                    {
                        Error($"Se esperaba tipo de parámetro en función '{nombre}' en línea {TokenActual.linea}");
                        return;
                    }
                    Avanzar();
                    string id = LeerIdentificador();
                    funcion.Parametros.Add(new FunctionParameterNode { Tipo = tipo, Nombre = id });
                    if (TokenActual.token != "CD6") break;
                    Avanzar();
                } while (TokenActual.token != "CD4" && TokenActual.token != "EOF");
            }

            Consumir("CD4", "Se esperaba ')' después de los parámetros");
            Consumir("CD1", "Se esperaba '{' al iniciar el cuerpo de la función");
            funcion.Cuerpo = ParseBloque();
            Consumir("CD2", "Se esperaba '}' al cerrar el cuerpo de la función");
            _sentenciasActuales.Add(funcion);
        }

        private void ParseDeclaracionENT()
        {
            Pasos.Add("[ENT] Declaración entera...");
            Consumir("ENT", "Se esperaba 'ENT'");
            string id = LeerIdentificador();
            Consumir("OPA", "Se esperaba '=' después del identificador");
            int inicioExpresion = _pos;
            ParseARG2();
            AstNode? valorNode = ConstruirExpresion(inicioExpresion, _pos);
            Consumir("CD5", "Se esperaba ';' al final de declaración ENT");
            var decl = new DeclarationNode { Tipo = "ENT", Identificador = id, Valor = valorNode };
            _sentenciasActuales.Add(decl);
            Pasos.Add("[ENT] Declaración reconocida ✔");
        }

        private void ParseDeclaracionDEC()
        {
            Pasos.Add("[DEC] Declaración decimal...");
            Consumir("DEC", "Se esperaba 'DEC'");
            string id = LeerIdentificador();
            Consumir("OPA", "Se esperaba '=' después del identificador");
            int inicioExpresion = _pos;
            ParseARG2();
            AstNode? valorNode = ConstruirExpresion(inicioExpresion, _pos);
            Consumir("CD5", "Se esperaba ';' al final de declaración DEC");
            var decl = new DeclarationNode { Tipo = "DEC", Identificador = id, Valor = valorNode };
            _sentenciasActuales.Add(decl);
            Pasos.Add("[DEC] Declaración reconocida ✔");
        }

        private void ParseDeclaracionCAD()
        {
            Pasos.Add("[CAD] Declaración cadena...");
            Consumir("CAD", "Se esperaba 'CAD'");
            string id = LeerIdentificador();
            Consumir("OPA", "Se esperaba '=' después del identificador");
            int inicioExpresion = _pos;
            ParseARG2();
            AstNode? valorNode = ConstruirExpresion(inicioExpresion, _pos);
            Consumir("CD5", "Se esperaba ';' al final de declaración CAD");
            var decl = new DeclarationNode { Tipo = "CAD", Identificador = id, Valor = valorNode };
            _sentenciasActuales.Add(decl);
            Pasos.Add("[CAD] Declaración reconocida ✔");
        }

        private void ParseAsignacion()
        {
            Pasos.Add($"[ASIG] Asignación a '{TokenActual.valor}'...");
            string id = LeerIdentificador();
            Consumir("OPA", "Se esperaba '=' en asignación");
            int inicioExpresion = _pos;
            ParseARG2();
            AstNode? valorNode = ConstruirExpresion(inicioExpresion, _pos);
            Consumir("CD5", "Se esperaba ';' al final de asignación");
            var assign = new AssignmentNode { Identificador = id, Valor = valorNode };
            _sentenciasActuales.Add(assign);
            Pasos.Add("[ASIG] Asignación reconocida ✔");
        }

        private void ParseLeer()
        {
            Pasos.Add("[PR1] leer...");
            Consumir("PR1", "Se esperaba 'leer' (PR1)");
            Consumir("CD3", "Se esperaba '(' después de 'leer'");
            string identificador = TokenActual.valor;
            ParseARG1();
            Consumir("CD4", "Se esperaba ')' en 'leer'");
            Consumir("CD5", "Se esperaba ';' al final de 'leer'");
            _sentenciasActuales.Add(new ReadNode { Identificador = identificador });
            Pasos.Add("[PR1] leer reconocido ✔");
        }

        private void ParseImprimir()
        {
            Pasos.Add("[PR2] imprimir...");
            Consumir("PR2", "Se esperaba 'imprimir' (PR2)");
            Consumir("CD3", "Se esperaba '(' después de 'imprimir'");
            int inicioValor = _pos;
            ParseARG2();
            AstNode? valor = ConstruirExpresion(inicioValor, _pos);
            Consumir("CD4", "Se esperaba ')' en 'imprimir'");
            Consumir("CD5", "Se esperaba ';' al final de 'imprimir'");
            _sentenciasActuales.Add(new PrintNode { Valor = valor });
            Pasos.Add("[PR2] imprimir reconocido ✔");
        }

        private void ParseRetornar()
        {
            Pasos.Add("[PR3] retornar...");
            Consumir("PR3", "Se esperaba 'retornar' (PR3)");
            int inicioValor = _pos;
            ParseARG2();
            AstNode? valor = ConstruirExpresion(inicioValor, _pos);
            Consumir("CD5", "Se esperaba ';' al final de 'retornar'");
            _sentenciasActuales.Add(new ReturnNode { Valor = valor });
            Pasos.Add("[PR3] retornar reconocido ✔");
        }

        private void ParseSentenciaIf()
        {
            Pasos.Add("[SI] Analizando sentencia SI...");
            Consumir("PR4", "Se esperaba 'si' (PR4)");
            Consumir("CD3", "Se esperaba '(' después de 'si'");
            int inicioCondicion = _pos;
            ParseCondic();
            ExpressionNode? condicion = ConstruirCondicion(inicioCondicion, _pos);
            Consumir("CD4", "Se esperaba ')' después de la condición");
            Consumir("CD1", "Se esperaba '{' para abrir bloque SI");
            BlockNode bloqueVerdadero = ParseBloque();
            Consumir("CD2", "Se esperaba '}' para cerrar bloque SI");
            BlockNode? bloqueFalso = null;

            if (TokenActual.token == "PR5")
            {
                Pasos.Add("[SINO] Analizando bloque SINO...");
                Consumir("PR5", "Se esperaba 'sino' (PR5)");
                Consumir("CD1", "Se esperaba '{' para abrir bloque SINO");
                bloqueFalso = ParseBloque();
                Consumir("CD2", "Se esperaba '}' para cerrar bloque SINO");
            }
            _sentenciasActuales.Add(new IfNode
            {
                Condicion = condicion,
                BloqueVerdadero = bloqueVerdadero,
                BloqueFalso = bloqueFalso
            });
            Pasos.Add("[SI] Sentencia SI/SINO reconocida ✔");
        }

        private void ParseCasos()
        {
            Pasos.Add("[PR6] casos...");
            Consumir("PR6", "Se esperaba 'casos' (PR6)");
            Consumir("CD3", "Se esperaba '(' después de 'casos'");
            string selector = TokenActual.valor;
            ParseARG1();
            Consumir("CD4", "Se esperaba ')' en 'casos'");
            Consumir("CD1", "Se esperaba '{' para abrir CASOS");
            SwitchNode seleccion = new() { Selector = selector };
            ParseOpciones(seleccion);
            ParsePred(seleccion);
            Consumir("CD2", "Se esperaba '}' para cerrar CASOS");
            _sentenciasActuales.Add(seleccion);
            Pasos.Add("[PR6] casos reconocido ✔");
        }

        private void ParseOpciones(SwitchNode seleccion)
        {
            ParseOpcion(seleccion);
            while (TokenActual.token == "PR7")
                ParseOpcion(seleccion);
        }

        private void ParseOpcion(SwitchNode seleccion)
        {
            Pasos.Add("[PR7] opcion...");
            Consumir("PR7", "Se esperaba 'opcion' (PR7)");
            int inicioValor = _pos;
            ParseARG3();
            AstNode? valor = ConstruirExpresion(inicioValor, _pos);
            Consumir("CD9", "Se esperaba ':' después de ARG3 en opcion");
            BlockNode cuerpo = new();
            List<AstNode> anterior = _sentenciasActuales;
            _sentenciasActuales = cuerpo.Statements;
            ParseS();
            _sentenciasActuales = anterior;
            Consumir("CD10", "Se esperaba 'terminar' al final de opcion");
            Consumir("CD5", "Se esperaba ';' después de 'terminar'");
            seleccion.Casos.Add(new SwitchCaseNode { Valor = valor as ExpressionNode, Cuerpo = cuerpo });
            Pasos.Add("[PR7] opcion reconocida ✔");
        }

        private void ParsePred(SwitchNode seleccion)
        {
            Pasos.Add("[PR8] predefinido...");
            Consumir("PR8", "Se esperaba 'predefinido' (PR8)");
            Consumir("CD9", "Se esperaba ':' después de 'predefinido'");
            BlockNode cuerpo = new();
            List<AstNode> anterior = _sentenciasActuales;
            _sentenciasActuales = cuerpo.Statements;
            ParseS();
            _sentenciasActuales = anterior;
            Consumir("CD10", "Se esperaba 'terminar' al final de predefinido");
            Consumir("CD5", "Se esperaba ';' después de 'terminar'");
            seleccion.Predeterminado = cuerpo;
            Pasos.Add("[PR8] predefinido reconocido ✔");
        }

        private void ParseMientras()
        {
            Pasos.Add("[PR9] mientras...");
            Consumir("PR9", "Se esperaba 'mientras' (PR9)");
            Consumir("CD3", "Se esperaba '(' después de 'mientras'");
            int inicioCondicion = _pos;
            ParseCondic();
            ExpressionNode? condicion = ConstruirCondicion(inicioCondicion, _pos);
            Consumir("CD4", "Se esperaba ')' después de la condición en 'mientras'");
            Consumir("CD1", "Se esperaba '{' para abrir bloque 'mientras'");
            BlockNode cuerpo = ParseBloque();
            Consumir("CD2", "Se esperaba '}' para cerrar bloque 'mientras'");
            _sentenciasActuales.Add(new WhileNode { Condicion = condicion, Cuerpo = cuerpo });
            Pasos.Add("[PR9] mientras reconocido ✔");
        }

        private void ParseHacer()
        {
            Pasos.Add("[PR10] hacer...");
            Consumir("PR10", "Se esperaba 'hacer' (PR10)");
            Consumir("CD1", "Se esperaba '{' para abrir bloque 'hacer'");
            BlockNode cuerpo = ParseBloque();
            Consumir("CD2", "Se esperaba '}' para cerrar bloque 'hacer'");
            Consumir("PR9", "Se esperaba 'mientras' después del bloque 'hacer'");
            Consumir("CD3", "Se esperaba '(' después de 'mientras' en 'hacer'");
            int inicioCondicion = _pos;
            ParseCondic();
            ExpressionNode? condicion = ConstruirCondicion(inicioCondicion, _pos);
            Consumir("CD4", "Se esperaba ')' después de la condición en 'hacer'");
            Consumir("CD5", "Se esperaba ';' al final de 'hacer-mientras'");
            _sentenciasActuales.Add(new DoWhileNode { Condicion = condicion, Cuerpo = cuerpo });
            Pasos.Add("[PR10] hacer reconocido ✔");
        }

        private void ParsePara()
        {
            Pasos.Add("[PR11] para...");
            Consumir("PR11", "Se esperaba 'para' (PR11)");
            Consumir("CD3", "Se esperaba '(' después de 'para'");
            Consumir("ENT", "Se esperaba 'ENT' en inicialización de 'para'");
            string idInicial = TokenActual.valor;
            ParseARG1();
            Consumir("OPA", "Se esperaba '=' en inicialización de 'para'");
            string valorInicial = TokenActual.valor;
            if (TokenActual.token == "CNU")
                Avanzar();
            else
                Error($"Se esperaba constante entera (CNU) en 'para', se encontró '{TokenActual.valor}'");
            Consumir("CD5", "Se esperaba ';' después de inicialización en 'para'");
            AssignmentNode inicializacion = new()
            {
                Identificador = idInicial,
                Valor = new LiteralNode { TipoLiteral = "CNU", Valor = valorInicial }
            };
            int inicioCondicion = _pos;
            ParseCondic();
            ExpressionNode? condicion = ConstruirCondicion(inicioCondicion, _pos);
            Consumir("CD5", "Se esperaba ';' después de condición en 'para'");
            string idIncremento = TokenActual.valor;
            ParseARG1();
            Consumir("OPA", "Se esperaba '=' en incremento de 'para'");
            int inicioIncremento = _pos;
            ParseARG2();
            AstNode? valorIncremento = ConstruirExpresion(inicioIncremento, _pos);
            Consumir("CD4", "Se esperaba ')' para cerrar cabecera de 'para'");
            Consumir("CD1", "Se esperaba '{' para abrir bloque 'para'");
            BlockNode cuerpo = ParseBloque();
            Consumir("CD2", "Se esperaba '}' para cerrar bloque 'para'");
            _sentenciasActuales.Add(new ForNode
            {
                Inicializacion = inicializacion,
                Condicion = condicion,
                Incremento = new AssignmentNode { Identificador = idIncremento, Valor = valorIncremento },
                Cuerpo = cuerpo
            });
            Pasos.Add("[PR11] para reconocido ✔");
        }

        private void ParseLimpiar()
        {
            Pasos.Add("[PR12] limpiar...");
            Consumir("PR12", "Se esperaba 'limpiar' (PR12)");
            Consumir("CD5", "Se esperaba ';' después de 'limpiar'");
            _sentenciasActuales.Add(new SimpleInstructionNode { Operacion = "CLEAR" });
            Pasos.Add("[PR12] limpiar reconocido ✔");
        }

        private void ParseUbicar()
        {
            Pasos.Add("[PR13] ubicar...");
            Consumir("PR13", "Se esperaba 'ubicar' (PR13)");
            Consumir("CD3", "Se esperaba '(' en ARG5");
            string argumento1 = TokenActual.valor;
            ParseARG6();
            Consumir("CD6", "Se esperaba ',' entre argumentos de 'ubicar'");
            string argumento2 = TokenActual.valor;
            ParseARG6();
            Consumir("CD4", "Se esperaba ')' para cerrar ARG5");
            Consumir("CD5", "Se esperaba ';' al final de 'ubicar'");
            var instruccion = new SimpleInstructionNode { Operacion = "LOCATE" };
            instruccion.Argumentos.Add(argumento1);
            instruccion.Argumentos.Add(argumento2);
            _sentenciasActuales.Add(instruccion);
            Pasos.Add("[PR13] ubicar reconocido ✔");
        }

        private void ParseARG6()
        {
            if (EsID(TokenActual.token))
                ParseID();
            else if (TokenActual.token == "CNU")
                ParseCN();
            else
                Error($"Se esperaba identificador o constante entera en ARG6, se encontró '{TokenActual.valor}'");
        }

        private BlockNode ParseBloque()
        {
            Pasos.Add("[BLOQUE] Analizando cuerpo del bloque...");
            BlockNode bloque = new();
            List<AstNode> sentenciasAnteriores = _sentenciasActuales;
            _sentenciasActuales = bloque.Statements;

            if (TokenActual.token == "CD2" || TokenActual.token == "EOF")
            {
                Error($"Bloque vacío en línea {_ultimaLineaConsumida}, se esperaba al menos una sentencia");
                _sentenciasActuales = sentenciasAnteriores;
                return bloque;
            }

            while (TokenActual.token != "CD2"
                && TokenActual.token != "EOF")
                ParseS();

            _sentenciasActuales = sentenciasAnteriores;
            return bloque;
        }

        private void ParseCondic()
        {
            Pasos.Add($"[CONDIC] token: {TokenActual.token} '{TokenActual.valor}'");
            ParseOPL();
        }

        private void ParseOPL()
        {
            Pasos.Add("[OPL] Operación lógica...");
            ParseCondicionBase();
            while (EsOLBinario(TokenActual.token))
            {
                Pasos.Add($"[OL] '{TokenActual.valor}' ({TokenActual.token})");
                Avanzar();
                ParseCondicionBase();
            }
            Pasos.Add("[OPL] Operación lógica reconocida ✔");
        }

        private void ParseCondicionBase()
        {
            if (TokenActual.token == "OL2")
            {
                Avanzar();
                ParseCondicionBase();
                return;
            }

            int posTemp = _pos;
            AvanzarARG7Lookahead(ref posTemp);
            string operador = posTemp < _tokens.Count ? _tokens[posTemp].token : "EOF";

            if (EsOR(operador))
                ParseOPR();
            else
                ParseARG7();
        }

        private void ParseOPR()
        {
            Pasos.Add("[OPR] Operación relacional...");
            ParseARG7();
            if (EsOR(TokenActual.token))
            {
                Pasos.Add($"[OR] '{TokenActual.valor}' ({TokenActual.token})");
                Avanzar();
            }
            else
                Error($"Se esperaba operador relacional en línea {_ultimaLineaConsumida}, se encontró '{TokenActual.valor}'");
            ParseARG7();
            Pasos.Add("[OPR] Operación relacional reconocida ✔");
        }

        private void ParseARG7()
        {
            Pasos.Add($"[ARG7] token: {TokenActual.token} '{TokenActual.valor}'");

            if (TokenActual.token == "CD3")
            {
                Consumir("CD3", "Se esperaba '('");
                ParseCondic();
                Consumir("CD4", "Se esperaba ')' para cerrar ARG7");
            }
            else if (EsID(TokenActual.token) || EsCN(TokenActual.token))
            {
                if (EsID(TokenActual.token) && _pos + 1 < _tokens.Count && _tokens[_pos + 1].token == "CD3")
                {
                    ParseLlamadaFuncionExpresion();
                    return;
                }
                int posTemp = _pos + 1;
                bool esOpa = posTemp < _tokens.Count && EsOA(_tokens[posTemp].token);
                if (esOpa) ParseOPA();
                else if (EsID(TokenActual.token)) ParseID();
                else ParseCN();
            }
            else
                Error($"Se esperaba identificador, número o expresión en línea {_ultimaLineaConsumida}, se encontró '{TokenActual.valor}'");
        }

        private void ParseARG2()
        {
            Pasos.Add($"[ARG2] token: {TokenActual.token} '{TokenActual.valor}'");

            if (EsID(TokenActual.token) && _pos + 1 < _tokens.Count && _tokens[_pos + 1].token == "CD3")
            {
                ParseLlamadaFuncionExpresion();
                ParseOPARest();
                return;
            }

            if (EsCadeLiteral(TokenActual.token))
            {
                
                int posTemp = _pos + 1;
                if (posTemp < _tokens.Count && EsOA(_tokens[posTemp].token))
                    ParseOPA();
                else
                {
                    Pasos.Add($"[ARG2] Literal cadena: '{TokenActual.valor}'");
                    Avanzar();
                }
            }
            else if (EsID(TokenActual.token) || EsCN(TokenActual.token))
            {
                int posTemp = _pos + 1;
                bool esOpa = posTemp < _tokens.Count && EsOA(_tokens[posTemp].token);
                if (esOpa) ParseOPA();
                else if (EsID(TokenActual.token)) ParseID();
                else ParseCN();
            }
            else if (TokenActual.token == "CD3")
                ParseOPA();
            else
                Error($"Se esperaba argumento válido en ARG2, se encontró '{TokenActual.valor}'");
        }

        private void ParseLlamadaFuncionExpresion()
        {
            Avanzar();
            Consumir("CD3", "Se esperaba '(' en llamada de función");
            if (TokenActual.token == "CD4")
            {
                Avanzar();
                return;
            }

            ParseARG2();
            while (TokenActual.token == "CD6")
            {
                Avanzar();
                ParseARG2();
            }
            Consumir("CD4", "Se esperaba ')' al cerrar llamada de función");
        }

        private void ParseARG3()
        {
            Pasos.Add($"[ARG3] token: {TokenActual.token} '{TokenActual.valor}'");

            if (EsCadeLiteral(TokenActual.token))
            {
                Avanzar();
            }
            else if (EsID(TokenActual.token) || EsCN(TokenActual.token) || TokenActual.token == "CD3")
            {
                
                int posTemp = _pos;
                AvanzarARG7Lookahead(ref posTemp);
                string opSig = posTemp < _tokens.Count ? _tokens[posTemp].token : "EOF";

                if (EsOL(opSig) || EsOR(opSig))
                    ParseCondic();
                else if (EsID(TokenActual.token))
                    ParseID();
                else
                    ParseCN();
            }
            else
                Error($"Se esperaba argumento válido en ARG3, se encontró '{TokenActual.valor}'");
        }

        private void ParseARG1()
        {
            if (EsID(TokenActual.token))
                ParseID();
            else
                Error($"Se esperaba identificador (ARG1) en línea {_ultimaLineaConsumida}, se encontró '{TokenActual.valor}'");
        }

        private string LeerIdentificador()
        {
            if (EsID(TokenActual.token))
            {
                string id = TokenActual.valor;
                Pasos.Add($"[ID] '{id}' ({TokenActual.token}) línea {TokenActual.linea}");
                Avanzar();
                return id;
            }
            Error($"Se esperaba identificador (ARG1) en línea {_ultimaLineaConsumida}, se encontró '{TokenActual.valor}'");
            return "";
        }

        private AstNode? BuildSimpleExprNode()
        {
            if (TokenActual.token == "CAD")
            {
                var node = new LiteralNode { TipoLiteral = "CAD", Valor = TokenActual.valor };
                Avanzar();
                return node;
            }
            if (EsCN(TokenActual.token))
            {
                var tipoLit = TokenActual.token == "CNU" ? "CNU" : "CNR";
                var node = new LiteralNode { TipoLiteral = tipoLit, Valor = TokenActual.valor };
                Avanzar();
                return node;
            }
            if (EsID(TokenActual.token))
            {
                if (_pos + 1 < _tokens.Count && _tokens[_pos + 1].token == "CD3")
                    return null;
                var node = new IdentifierNode { Nombre = TokenActual.valor };
                Avanzar();
                return node;
            }
            if (TokenActual.token == "CD3")
                return null;

            return null;
        }

        private AstNode? ConstruirExpresion(int inicio, int fin)
        {
            if (inicio >= fin || inicio < 0 || fin > _tokens.Count)
            {
                Error($"Expresión vacía o inválida en línea {_ultimaLineaConsumida}");
                return null;
            }

            List<(int linea, string valor, string token)> tokens = _tokens.Skip(inicio).Take(fin - inicio).ToList();
            int indice = 0;
            ExpressionNode? expresion = ParseExpresionSuma(tokens, ref indice);
            if (expresion == null || indice != tokens.Count)
            {
                Error($"No se pudo construir la expresión del AST en línea {tokens[0].linea}");
                return null;
            }

            return expresion;
        }

        private ExpressionNode? ParseExpresionSuma(
            List<(int linea, string valor, string token)> tokens,
            ref int indice)
        {
            ExpressionNode? izquierda = ParseExpresionProducto(tokens, ref indice);
            while (izquierda != null && indice < tokens.Count && EsOperadorAditivo(tokens[indice]))
            {
                string operador = tokens[indice++].valor;
                ExpressionNode? derecha = ParseExpresionProducto(tokens, ref indice);
                if (derecha == null) return null;
                izquierda = CrearOperacion(operador, izquierda, derecha);
            }
            return izquierda;
        }

        private ExpressionNode? ParseExpresionProducto(
            List<(int linea, string valor, string token)> tokens,
            ref int indice)
        {
            ExpressionNode? izquierda = ParseExpresionUnaria(tokens, ref indice);
            while (izquierda != null && indice < tokens.Count && EsOperadorMultiplicativo(tokens[indice]))
            {
                string operador = tokens[indice++].valor;
                ExpressionNode? derecha = ParseExpresionUnaria(tokens, ref indice);
                if (derecha == null) return null;
                izquierda = CrearOperacion(operador, izquierda, derecha);
            }
            return izquierda;
        }

        private ExpressionNode? ParseExpresionUnaria(
            List<(int linea, string valor, string token)> tokens,
            ref int indice)
        {
            if (indice < tokens.Count && (tokens[indice].valor == "+" || tokens[indice].valor == "-"))
            {
                string operador = tokens[indice++].valor;
                ExpressionNode? operando = ParseExpresionUnaria(tokens, ref indice);
                return operando == null ? null : new UnaryExpressionNode { Operador = operador, Operando = operando };
            }

            return ParsePotencia(tokens, ref indice);
        }

        private ExpressionNode? ParsePotencia(
            List<(int linea, string valor, string token)> tokens,
            ref int indice)
        {
            ExpressionNode? izquierda = ParseFactor(tokens, ref indice);
            if (izquierda != null && indice < tokens.Count && tokens[indice].valor == "^")
            {
                indice++;
                ExpressionNode? derecha = ParseExpresionUnaria(tokens, ref indice);
                if (derecha == null) return null;
                izquierda = CrearOperacion("^", izquierda, derecha);
            }
            return izquierda;
        }

        private ExpressionNode? ParseFactor(
            List<(int linea, string valor, string token)> tokens,
            ref int indice)
        {
            if (indice >= tokens.Count) return null;

            string valor = tokens[indice].valor;
            string token = tokens[indice].token;
            if (token == "CD3")
            {
                indice++;
                ExpressionNode? agrupada = ParseExpresionSuma(tokens, ref indice);
                if (agrupada == null || indice >= tokens.Count || tokens[indice].token != "CD4") return null;
                indice++;
                return agrupada;
            }

            if (token.StartsWith("IDV", StringComparison.Ordinal))
            {
                indice++;
                if (indice < tokens.Count && tokens[indice].token == "CD3")
                {
                    indice++;
                    CallNode llamada = new() { NombreFuncion = valor };
                    if (indice < tokens.Count && tokens[indice].token != "CD4")
                    {
                        while (true)
                        {
                            ExpressionNode? argumento = ParseExpresionSuma(tokens, ref indice);
                            if (argumento == null) return null;
                            llamada.Argumentos.Add(argumento);
                            if (indice >= tokens.Count || tokens[indice].token != "CD6") break;
                            indice++;
                        }
                    }
                    if (indice >= tokens.Count || tokens[indice].token != "CD4") return null;
                    indice++;
                    return llamada;
                }
                return new IdentifierNode { Nombre = valor };
            }
            if (token == "CAD" || token == "CNU" || token == "CNR")
            {
                indice++;
                return new LiteralNode { TipoLiteral = token, Valor = valor };
            }
            return null;
        }

        private static bool EsOperadorAditivo((int linea, string valor, string token) token)
        {
            return token.valor == "+" || token.valor == "-";
        }

        private static bool EsOperadorMultiplicativo((int linea, string valor, string token) token)
        {
            return token.valor == "*" || token.valor == "/" || token.valor == "%";
        }

        private ExpressionNode? ConstruirCondicion(int inicio, int fin)
        {
            if (inicio >= fin || inicio < 0 || fin > _tokens.Count) return null;

            List<(int linea, string valor, string token)> tokens = _tokens.Skip(inicio).Take(fin - inicio).ToList();
            ExpressionNode? condicion = ParseCondicion(tokens);
            if (condicion == null)
                Error($"No se pudo construir la condición del AST en línea {tokens[0].linea}");
            return condicion;
        }

        private ExpressionNode? ParseCondicion(List<(int linea, string valor, string token)> tokens)
        {
            if (tokens.Count == 0) return null;

            if (EstaEnmarcadaPorParentesis(tokens))
                return ParseCondicion(tokens.Skip(1).Take(tokens.Count - 2).ToList());

            int operador = BuscarOperadorLogico(tokens, "OL3");
            if (operador >= 0)
            {
                ExpressionNode? izquierda = ParseCondicion(tokens.Take(operador).ToList());
                ExpressionNode? derecha = ParseCondicion(tokens.Skip(operador + 1).ToList());
                return izquierda == null || derecha == null ? null : CrearOperacion("O", izquierda, derecha);
            }

            operador = BuscarOperadorLogico(tokens, "OL1");
            if (operador >= 0)
            {
                ExpressionNode? izquierda = ParseCondicion(tokens.Take(operador).ToList());
                ExpressionNode? derecha = ParseCondicion(tokens.Skip(operador + 1).ToList());
                return izquierda == null || derecha == null ? null : CrearOperacion("Y", izquierda, derecha);
            }

            if (tokens[0].token == "OL2")
            {
                ExpressionNode? operando = ParseCondicion(tokens.Skip(1).ToList());
                return operando == null ? null : new UnaryExpressionNode { Operador = "NO", Operando = operando };
            }

            operador = BuscarOperadorRelacional(tokens);
            if (operador >= 0)
            {
                ExpressionNode? izquierda = ParseExpresionCompleta(tokens.Take(operador).ToList());
                ExpressionNode? derecha = ParseExpresionCompleta(tokens.Skip(operador + 1).ToList());
                return izquierda == null || derecha == null
                    ? null
                    : CrearOperacion(tokens[operador].valor, izquierda, derecha);
            }

            return ParseExpresionCompleta(tokens);
        }

        private ExpressionNode? ParseExpresionCompleta(List<(int linea, string valor, string token)> tokens)
        {
            int indice = 0;
            ExpressionNode? expresion = ParseExpresionSuma(tokens, ref indice);
            return expresion != null && indice == tokens.Count ? expresion : null;
        }

        private static bool EstaEnmarcadaPorParentesis(List<(int linea, string valor, string token)> tokens)
        {
            if (tokens.Count < 2 || tokens[0].token != "CD3" || tokens[^1].token != "CD4") return false;
            int nivel = 0;
            for (int i = 0; i < tokens.Count; i++)
            {
                if (tokens[i].token == "CD3") nivel++;
                else if (tokens[i].token == "CD4" && --nivel == 0) return i == tokens.Count - 1;
            }
            return false;
        }

        private static int BuscarOperadorLogico(
            List<(int linea, string valor, string token)> tokens,
            string operador)
        {
            int nivel = 0;
            for (int i = 0; i < tokens.Count; i++)
            {
                if (tokens[i].token == "CD3") nivel++;
                else if (tokens[i].token == "CD4") nivel--;
                else if (nivel == 0 && tokens[i].token == operador) return i;
            }
            return -1;
        }

        private int BuscarOperadorRelacional(List<(int linea, string valor, string token)> tokens)
        {
            int nivel = 0;
            for (int i = 0; i < tokens.Count; i++)
            {
                if (tokens[i].token == "CD3") nivel++;
                else if (tokens[i].token == "CD4") nivel--;
                else if (nivel == 0 && EsOR(tokens[i].token)) return i;
            }
            return -1;
        }

        private static BinaryExpressionNode CrearOperacion(
            string operador,
            ExpressionNode izquierda,
            ExpressionNode derecha)
        {
            return new BinaryExpressionNode { Operador = operador, Izquierda = izquierda, Derecha = derecha };
        }

        private void ParseOPA()
        {
            Pasos.Add("[OPA] Operación aritmética...");

            if (TokenActual.token == "CD3")
            {
                Consumir("CD3", "Se esperaba '('");
                ParseOPA();
                Consumir("CD4", "Se esperaba ')' después de OPA");
            }
            else
            {
                ParseARG4();
            }

            while (EsOA(TokenActual.token))
            {
                Pasos.Add($"[OA] '{TokenActual.valor}' ({TokenActual.token})");
                Avanzar();
                if (TokenActual.token == "CD3")
                {
                    Consumir("CD3", "Se esperaba '('");
                    ParseOPA();
                    Consumir("CD4", "Se esperaba ')' después de OPA");
                }
                else
                {
                    ParseARG4();
                }
            }

            Pasos.Add("[OPA] Operación aritmética reconocida ✔");
        }

        private void ParseOPARest()
        {
            while (EsOA(TokenActual.token))
            {
                Pasos.Add($"[OPA-REST] '{TokenActual.valor}' ({TokenActual.token})");
                Avanzar();
                if (TokenActual.token == "CD3")
                {
                    Consumir("CD3", "Se esperaba '('");
                    ParseOPA();
                    Consumir("CD4", "Se esperaba ')' después de OPA");
                }
                else
                {
                    ParseARG4();
                }
            }
            Pasos.Add("[OPA-REST] Operación aritmética reconocida ✔");
        }

        private void ParseARG4()
        {
            Pasos.Add($"[ARG4] token: {TokenActual.token}");
            if (EsOA(TokenActual.token))
            {
                if (TokenActual.token == "MAS" || TokenActual.token == "MENOS")
                {
                    Pasos.Add($"[ARG4] unary '{TokenActual.valor}'");
                    Avanzar();
                    if (TokenActual.token == "CD3")
                    {
                        Consumir("CD3", "Se esperaba '('");
                        ParseOPA();
                        Consumir("CD4", "Se esperaba ')' después de OPA");
                        return;
                    }
                    else if (EsID(TokenActual.token)) { ParseID(); return; }
                    else if (EsCN(TokenActual.token)) { ParseCN(); return; }
                    else if (EsCadeLiteral(TokenActual.token)) { Pasos.Add($"[ARG4] Literal cadena: '{TokenActual.valor}'"); Avanzar(); return; }
                    else { Error($"Se esperaba identificador, número, cadena o expresión aritmética en línea {_ultimaLineaConsumida}, se encontró '{TokenActual.valor}'"); return; }
                }

                Error($"Se esperaba identificador, número, cadena o expresión aritmética en línea {_ultimaLineaConsumida}, se encontró '{TokenActual.valor}'");
                Avanzar();
                if (TokenActual.token == "CD3")
                {
                    Consumir("CD3", "Se esperaba '('");
                    ParseOPA();
                    Consumir("CD4", "Se esperaba ')' después de OPA");
                    return;
                }
                else if (EsID(TokenActual.token)) { ParseID(); return; }
                else if (EsCN(TokenActual.token)) { ParseCN(); return; }
                else if (EsCadeLiteral(TokenActual.token)) { Pasos.Add($"[ARG4] Literal cadena: '{TokenActual.valor}'"); Avanzar(); return; }
                else return;
            }

            if (TokenActual.token == "CD3")
            {
                Consumir("CD3", "Se esperaba '('");
                ParseOPA();
                Consumir("CD4", "Se esperaba ')' después de OPA");
            }
            else if (EsID(TokenActual.token) && _pos + 1 < _tokens.Count && _tokens[_pos + 1].token == "CD3")
                ParseLlamadaFuncionExpresion();
            else if (EsID(TokenActual.token))
                ParseID();
            else if (EsCN(TokenActual.token))
                ParseCN();
            else if (EsCadeLiteral(TokenActual.token))
            {
                Pasos.Add($"[ARG4] Literal cadena: '{TokenActual.valor}'");
                Avanzar();
            }
            else
                Error($"Se esperaba identificador, número, cadena o expresión aritmética en línea {_ultimaLineaConsumida}, se encontró '{TokenActual.valor}'");
        }

        private void ParseID()
        {
            Pasos.Add($"[ID] '{TokenActual.valor}' ({TokenActual.token}) línea {TokenActual.linea}");
            Avanzar();
        }

        private void ParseCN()
        {
            Pasos.Add($"[CN] '{TokenActual.valor}' ({TokenActual.token}) línea {TokenActual.linea}");
            Avanzar();
        }

        private void Consumir(string tokenEsperado, string mensajeError)
        {
            if (TokenActual.token == tokenEsperado)
            {
                _ultimaLineaConsumida = TokenActual.linea;
                Pasos.Add($"[✔] '{TokenActual.valor}' ({tokenEsperado}) línea {_ultimaLineaConsumida}");
                Avanzar();
            }
            else
                Error($"{mensajeError} — línea {_ultimaLineaConsumida}, se encontró '{TokenActual.valor}' ({TokenActual.token})");
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
            string msg = $"ERROR SINTÁCTICO → {mensaje}";
            Errores.Add(msg);
            Pasos.Add(msg);
        }

        private bool EsID(string token) => token != null && token.StartsWith("IDV");
        private bool EsCN(string token) => token == "CNU" || token == "CNR";
        private bool EsOL(string token) => token == "OL1" || token == "OL2" || token == "OL3";
        private bool EsOLBinario(string token) => token == "OL1" || token == "OL3";
        private bool EsOR(string token) => token == "OR1" || token == "OR2" || token == "OR3"
                                                  || token == "OR4" || token == "OR5" || token == "OR6";
        private bool EsOA(string token) => token == "OA1" || token == "OA2" || token == "OA3"
                                                  || token == "OA4" || token == "OA5"
                                                  || token == "MAS" || token == "MENOS" || token == "ASTERISCO" || token == "SLASH";
        private bool EsCadeLiteral(string token) => token == "CAD";

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
    }
}