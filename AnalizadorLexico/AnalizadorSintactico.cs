namespace AnalizadorLexico
{
    public class AnalizadorSintactico
    {
        private List<(int linea, string valor, string token)> _tokens = new();
        private int _pos;
        private int _ultimaLineaConsumida = 0;
        public List<string> Pasos { get; private set; } = new();
        public List<string> Errores { get; private set; } = new();
        public bool EsValido => Errores.Count == 0;
        private (int linea, string valor, string token) TokenActual => _pos < _tokens.Count ? _tokens[_pos] : (-1, "EOF", "EOF");
        public bool Analizar(List<(int linea, string valor, string token)> tokens)
        {
            _tokens = tokens.Where(t => t.token != "COM").ToList();
            _pos = 0;
            _ultimaLineaConsumida = 0;
            Pasos.Clear();
            Errores.Clear();

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
                        Error($"Sentencia no reconocida '{TokenActual.valor}' ({TokenActual.token}) en línea {_ultimaLineaConsumida}");
                        Avanzar();
                    }
                    break;
            }
        }

        private void ParseDeclaracionENT()
        {
            Pasos.Add("[ENT] Declaración entera...");
            Consumir("ENT", "Se esperaba 'ENT'");
            ParseARG1();
            Consumir("OPA", "Se esperaba '=' después del identificador");
            if (TokenActual.token == "CNU")
                Avanzar();
            else
                Error($"Se esperaba constante entera (CNU) en línea {_ultimaLineaConsumida}, se encontró '{TokenActual.valor}'");
            Consumir("CD5", "Se esperaba ';' al final de declaración ENT");
            Pasos.Add("[ENT] Declaración reconocida ✔");
        }

        private void ParseDeclaracionDEC()
        {
            Pasos.Add("[DEC] Declaración decimal...");
            Consumir("DEC", "Se esperaba 'DEC'");
            ParseARG1();
            Consumir("OPA", "Se esperaba '=' después del identificador");
            if (TokenActual.token == "CNR")
                Avanzar();
            else
                Error($"Se esperaba constante real (CNR) en línea {_ultimaLineaConsumida}, se encontró '{TokenActual.valor}'");
            Consumir("CD5", "Se esperaba ';' al final de declaración DEC");
            Pasos.Add("[DEC] Declaración reconocida ✔");
        }

        private void ParseDeclaracionCAD()
        {
            Pasos.Add("[CAD] Declaración cadena...");
            Consumir("CAD", "Se esperaba 'CAD'");
            ParseARG1();
            Consumir("OPA", "Se esperaba '=' después del identificador");
            if (EsCadeLiteral(TokenActual.token))
                Avanzar();
            else
                Error($"Se esperaba literal de cadena en línea {_ultimaLineaConsumida}, se encontró '{TokenActual.valor}'");
            Consumir("CD5", "Se esperaba ';' al final de declaración CAD");
            Pasos.Add("[CAD] Declaración reconocida ✔");
        }
 
        private void ParseAsignacion()
        {
            Pasos.Add($"[ASIG] Asignación a '{TokenActual.valor}'...");
            ParseARG1();
            Consumir("OPA", "Se esperaba '=' en asignación");
            ParseARG2();
            Consumir("CD5", "Se esperaba ';' al final de asignación");
            Pasos.Add("[ASIG] Asignación reconocida ✔");
        }

        private void ParseLeer()
        {
            Pasos.Add("[PR1] leer...");
            Consumir("PR1", "Se esperaba 'leer' (PR1)");
            Consumir("CD3", "Se esperaba '(' después de 'leer'");
            ParseARG1();
            Consumir("CD4", "Se esperaba ')' en 'leer'");
            Consumir("CD5", "Se esperaba ';' al final de 'leer'");
            Pasos.Add("[PR1] leer reconocido ✔");
        }

        private void ParseImprimir()
        {
            Pasos.Add("[PR2] imprimir...");
            Consumir("PR2", "Se esperaba 'imprimir' (PR2)");
            Consumir("CD3", "Se esperaba '(' después de 'imprimir'");
            ParseARG2();
            Consumir("CD4", "Se esperaba ')' en 'imprimir'");
            Consumir("CD5", "Se esperaba ';' al final de 'imprimir'");
            Pasos.Add("[PR2] imprimir reconocido ✔");
        }

        private void ParseRetornar()
        {
            Pasos.Add("[PR3] retornar...");
            Consumir("PR3", "Se esperaba 'retornar' (PR3)");
            ParseARG2();
            Consumir("CD5", "Se esperaba ';' al final de 'retornar'");
            Pasos.Add("[PR3] retornar reconocido ✔");
        }

        private void ParseSentenciaIf()
        {
            Pasos.Add("[SI] Analizando sentencia SI...");
            Consumir("PR4", "Se esperaba 'si' (PR4)");
            Consumir("CD3", "Se esperaba '(' después de 'si'");
            ParseCondic();
            Consumir("CD4", "Se esperaba ')' después de la condición");
            Consumir("CD1", "Se esperaba '{' para abrir bloque SI");
            ParseBloque();
            Consumir("CD2", "Se esperaba '}' para cerrar bloque SI");

            if (TokenActual.token == "PR5")
            {
                Pasos.Add("[SINO] Analizando bloque SINO...");
                Consumir("PR5", "Se esperaba 'sino' (PR5)");
                Consumir("CD1", "Se esperaba '{' para abrir bloque SINO");
                ParseBloque();
                Consumir("CD2", "Se esperaba '}' para cerrar bloque SINO");
            }
            Pasos.Add("[SI] Sentencia SI/SINO reconocida ✔");
        }

        private void ParseCasos()
        {
            Pasos.Add("[PR6] casos...");
            Consumir("PR6", "Se esperaba 'casos' (PR6)");
            Consumir("CD3", "Se esperaba '(' después de 'casos'");
            ParseARG1();
            Consumir("CD4", "Se esperaba ')' en 'casos'");
            Consumir("CD1", "Se esperaba '{' para abrir CASOS");
            ParseOpciones();
            ParsePred();
            Consumir("CD2", "Se esperaba '}' para cerrar CASOS");
            Pasos.Add("[PR6] casos reconocido ✔");
        }

        private void ParseOpciones()
        {
            ParseOpcion();
            while (TokenActual.token == "PR7")
                ParseOpcion();
        }

        private void ParseOpcion()
        {
            Pasos.Add("[PR7] opcion...");
            Consumir("PR7", "Se esperaba 'opcion' (PR7)");
            ParseARG3();
            Consumir("CD9", "Se esperaba ':' después de ARG3 en opcion");
            ParseS();
            Consumir("CD11", "Se esperaba 'terminar' al final de opcion");
            Consumir("CD5", "Se esperaba ';' después de 'terminar'");
            Pasos.Add("[PR7] opcion reconocida ✔");
        }

        private void ParsePred()
        {
            Pasos.Add("[PR8] predefinido...");
            Consumir("PR8", "Se esperaba 'predefinido' (PR8)");
            Consumir("CD9", "Se esperaba ':' después de 'predefinido'");
            ParseS();
            Consumir("CD11", "Se esperaba 'terminar' al final de predefinido");
            Consumir("CD5", "Se esperaba ';' después de 'terminar'");
            Pasos.Add("[PR8] predefinido reconocido ✔");
        }

        private void ParseMientras()
        {
            Pasos.Add("[PR9] mientras...");
            Consumir("PR9", "Se esperaba 'mientras' (PR9)");
            Consumir("CD3", "Se esperaba '(' después de 'mientras'");
            ParseCondic();
            Consumir("CD4", "Se esperaba ')' después de la condición en 'mientras'");
            Consumir("CD1", "Se esperaba '{' para abrir bloque 'mientras'");
            ParseBloque();
            Consumir("CD2", "Se esperaba '}' para cerrar bloque 'mientras'");
            Consumir("CD5", "Se esperaba ';' al final de 'mientras'");
            Pasos.Add("[PR9] mientras reconocido ✔");
        }

        private void ParseHacer()
        {
            Pasos.Add("[PR10] hacer...");
            Consumir("PR10", "Se esperaba 'hacer' (PR10)");
            Consumir("CD1", "Se esperaba '{' para abrir bloque 'hacer'");
            ParseBloque();
            Consumir("CD2", "Se esperaba '}' para cerrar bloque 'hacer'");
            Consumir("PR9", "Se esperaba 'mientras' después del bloque 'hacer'");
            Consumir("CD3", "Se esperaba '(' después de 'mientras' en 'hacer'");
            ParseCondic();
            Consumir("CD4", "Se esperaba ')' después de la condición en 'hacer'");
            Consumir("CD5", "Se esperaba ';' al final de 'hacer-mientras'");
            Pasos.Add("[PR10] hacer reconocido ✔");
        }

        private void ParsePara()
        {
            Pasos.Add("[PR11] para...");
            Consumir("PR11", "Se esperaba 'para' (PR11)");
            Consumir("CD3", "Se esperaba '(' después de 'para'");
            Consumir("ENT", "Se esperaba 'ENT' en inicialización de 'para'");
            ParseARG1();
            Consumir("OPA", "Se esperaba '=' en inicialización de 'para'");
            if (TokenActual.token == "CNU")
                Avanzar();
            else
                Error($"Se esperaba constante entera (CNU) en 'para', se encontró '{TokenActual.valor}'");
            Consumir("CD5", "Se esperaba ';' después de inicialización en 'para'");
            ParseCondic();
            Consumir("CD5", "Se esperaba ';' después de condición en 'para'");
            // Incremento: IDV OPA ARG2  (asignación sin CD5)
            ParseARG1();
            Consumir("OPA", "Se esperaba '=' en incremento de 'para'");
            ParseARG2();
            Consumir("CD4", "Se esperaba ')' para cerrar cabecera de 'para'");
            Consumir("CD1", "Se esperaba '{' para abrir bloque 'para'");
            ParseBloque();
            Consumir("CD2", "Se esperaba '}' para cerrar bloque 'para'");
            Pasos.Add("[PR11] para reconocido ✔");
        }

        private void ParseLimpiar()
        {
            Pasos.Add("[PR12] limpiar...");
            Consumir("PR12", "Se esperaba 'limpiar' (PR12)");
            Consumir("CD5", "Se esperaba ';' después de 'limpiar'");
            Pasos.Add("[PR12] limpiar reconocido ✔");
        }

        private void ParseUbicar()
        {
            Pasos.Add("[PR13] ubicar...");
            Consumir("PR13", "Se esperaba 'ubicar' (PR13)");
            Consumir("CD3", "Se esperaba '(' en ARG5");
            ParseARG6();
            Consumir("CD6", "Se esperaba ',' entre argumentos de 'ubicar'");
            ParseARG6();
            Consumir("CD4", "Se esperaba ')' para cerrar ARG5");
            Consumir("CD5", "Se esperaba ';' al final de 'ubicar'");
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

        private void ParseBloque()
        {
            Pasos.Add("[BLOQUE] Analizando cuerpo del bloque...");

            if (TokenActual.token == "CD2" || TokenActual.token == "EOF")
            {
                Error($"Bloque vacío en línea {_ultimaLineaConsumida}, se esperaba al menos una sentencia");
                return;
            }

            while (TokenActual.token != "CD2"
                && TokenActual.token != "EOF")
                ParseS();
        }

        private void ParseCondic()
        {
            Pasos.Add($"[CONDIC] token: {TokenActual.token} '{TokenActual.valor}'");

            int posTemp = _pos;
            AvanzarARG7Lookahead(ref posTemp);
            string opSiguiente = posTemp < _tokens.Count ? _tokens[posTemp].token : "EOF";

            if (EsOL(opSiguiente))
                ParseOPL();
            else if (EsOR(opSiguiente))
                ParseOPR();
            else
                Error($"Se esperaba operador lógico o relacional en línea {_ultimaLineaConsumida}, se encontró '{TokenActual.valor}'");
        }

        private void ParseOPL()
        {
            Pasos.Add("[OPL] Operación lógica...");
            ParseARG7();
            if (EsOL(TokenActual.token))
            {
                Pasos.Add($"[OL] '{TokenActual.valor}' ({TokenActual.token})");
                Avanzar();
            }
            else
                Error($"Se esperaba operador lógico (Y/O/NO) en línea {_ultimaLineaConsumida}, se encontró '{TokenActual.valor}'");
            ParseARG7();
            Pasos.Add("[OPL] Operación lógica reconocida ✔");
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
                int posTemp = _pos + 1;
                AvanzarARG7Lookahead(ref posTemp);
                string opDentro = posTemp < _tokens.Count ? _tokens[posTemp].token : "EOF";

                Consumir("CD3", "Se esperaba '('");
                if (EsOL(opDentro)) ParseOPL();
                else if (EsOR(opDentro)) ParseOPR();
                else ParseOPA();
                Consumir("CD4", "Se esperaba ')' para cerrar ARG7");
            }
            else if (EsID(TokenActual.token) || EsCN(TokenActual.token))
            {
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

            if (EsCadeLiteral(TokenActual.token))
            {
                // Puede ser solo un literal o el inicio de una OPA: CAD OA IDV
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

        private void ParseARG3()
        {
            Pasos.Add($"[ARG3] token: {TokenActual.token} '{TokenActual.valor}'");

            if (EsCadeLiteral(TokenActual.token))
            {
                Avanzar();
            }
            else if (EsID(TokenActual.token) || EsCN(TokenActual.token) || TokenActual.token == "CD3")
            {
                // Lookahead para ver si es CONDIC
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
                if (EsOA(TokenActual.token))
                {
                    Pasos.Add($"[OA] '{TokenActual.valor}' ({TokenActual.token})");
                    Avanzar();
                    ParseARG4();
                }
            }
            Pasos.Add("[OPA] Operación aritmética reconocida ✔");
        }

        private void ParseARG4()
        {
            Pasos.Add($"[ARG4] token: {TokenActual.token}");

            if (TokenActual.token == "CD3")
            {
                Consumir("CD3", "Se esperaba '('");
                ParseOPA();
                Consumir("CD4", "Se esperaba ')' después de OPA");
            }
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
        private bool EsOR(string token) => token == "OR1" || token == "OR2" || token == "OR3"
                                                  || token == "OR4" || token == "OR5" || token == "OR6";
        private bool EsOA(string token) => token == "OA1" || token == "OA2" || token == "OA3"
                                                  || token == "OA4" || token == "OA5";
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