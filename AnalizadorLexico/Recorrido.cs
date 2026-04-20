using Microsoft.Data.SqlClient;

namespace AnalizadorLexico
{
    public class Recorrido
    {
        // Cadena de conexión
        protected string connectionString = "Server=Anapaula;Database=AnalizadorLexico;Trusted_Connection=True;TrustServerCertificate=True;";

        private static readonly Dictionary<char, string> simbolos = new()
        {
            {'>', "MAYOR"},
            {'<', "MENOR"},
            {'=', "IGUAL"},
            {'*', "ASTERISCO"},
            {'/', "SLASH"},
            {'+', "MAS"},
            {'-', "MENOS"},
            {'{', "LLAVE_ABRE"},
            {'}', "LLAVE_CIERRA"},
            {'#', "NUMERAL"},
            {'"', "COMILLA_DOBLE"},
            {'(', "PAR_ABRE"},
            {')', "PAR_CIERRA"},
            {';', "PUNTO_COMA"},
            {',', "COMA"},
            {':', "DOS_PUNTOS"},
            {'!', "ADMIRACION"},
            {'@', "ARROBA"},
            {'$', "DOLAR"},
            {'%', "PORCENTAJE"},
            {'^', "CIRCUNFLEJO"},
            {'&', "AMPERSAND"},
            {'?', "INTERROGACION"},
            {'.', "PUNTO"},
            {'\'', "COMILLA_SIMPLE"},
            {'[', "CORCHETE_ABRE"},
            {']', "CORCHETE_CIERRA"},
            {'~', "VIRGULILLA"},
            {'\\', "BACKSLASH"}
        };
        private static readonly Dictionary<string, string> palabrasReservadas = new()
    {
        {"LEER", "PR1"},
        {"leer", "PR1"},
        {"IMPRIMIR", "PR2"},
        {"imprimir", "PR2"},
        {"RETORNAR", "PR3"},
        {"retornar", "PR3"},
        {"SI", "PR4"},
        {"si", "PR4"},
        {"SINO", "PR5"},
        {"sino", "PR5"},
        {"CASOS", "PR6"},
        {"casos", "PR6"},
        {"OPCION", "PR7"},
        {"opcion", "PR7"},
        {"PREDEFINIDO", "PR8"},
        {"predefinido", "PR8"},
        {"MIENTRAS", "PR9"},
        {"mientras", "PR9"},
        {"HACER", "PR10"},
        {"hacer", "PR10"},
        {"PARA", "PR11"},
        {"para", "PR11"},
        {"LIMPIAR", "PR12"},
        {"limpiar", "PR12"},
        {"UBICAR", "PR13"},
        {"ubicar", "PR13"},
        {"Y", "OL1"},
        {"NO", "OL2"},
        {"O", "OL3"},
        {"TERMINAR", "CD10"}
    };
        private Dictionary<(int estado, string columna), int> _tabla = new();
        private Dictionary<int, string> _categoriaAceptacion = new();
        private HashSet<int> _estadosFinalCadena = new();
        private bool TablaCargada = false;
        private void CargarTabla()
        {
            if (TablaCargada) return;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query = "SELECT * FROM AnalizadorLexico";
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader reader = cmd.ExecuteReader();

                var columnas = Enumerable.Range(0, reader.FieldCount)
                                         .Select(reader.GetName)
                                         .ToList();

                var columnasIgnorar = new HashSet<string> { "ESTADO", "CAT", "FDC" };

                while (reader.Read())
                {
                    int estado = Convert.ToInt32(reader["ESTADO"]);

                    foreach (var col in columnas)
                    {
                        if (col == "CAT")
                        {
                            string? cat = reader[col]?.ToString()?.Trim();
                            if (!string.IsNullOrEmpty(cat))
                                _categoriaAceptacion[estado] = cat;
                            continue;
                        }

                        if (col == "FDC")
                        {
                            string? fdcVal = reader[col]?.ToString()?.Trim();

                            if (!string.IsNullOrEmpty(fdcVal) && fdcVal != "-")
                            {
                                if (fdcVal == "1")
                                {
                                    _estadosFinalCadena.Add(estado);
                                }
                                else if (int.TryParse(fdcVal, out int estadoDestino))
                                {
                                    _estadosFinalCadena.Add(estado);
                                    _tabla[(estado, "FDC")] = estadoDestino; 
                                }
                            }
                            continue;
                        }

                        if (columnasIgnorar.Contains(col))
                            continue;

                        string? val = reader[col]?.ToString()?.Trim();
                        if (!string.IsNullOrEmpty(val) && val != "-")
                            _tabla[(estado, col)] = Convert.ToInt32(val);
                    }
                }
                TablaCargada = true;
            }
        }
        public (bool esValido, string token, string recorrido) RecorrerCadena(string cadena)
        {

            if (cadena.StartsWith('#'))
            {
                return (true, "COM", $"(0) # -> COM");
            }

            if (cadena.StartsWith('"') && cadena.EndsWith('"'))
            {
                return (true, "CAD", $"(0) \"...\" -> CAD");
            }

            if (palabrasReservadas.TryGetValue(cadena, out string? tokenReservado))
            {
                CargarTabla();
                int estado = 0;
                string recorrido = $"({estado})";

                for (int x = 0; x < cadena.Length; x++)
                {
                    char caracter = cadena[x];
                    int siguienteEstado = Validar(estado, caracter);

                    if (siguienteEstado == -1)
                    {
                        recorrido += $" {caracter} -> (ERROR)";
                        string mensajeError = _categoriaAceptacion.TryGetValue(estado, out string? errorMsg)
                            ? errorMsg
                            : $"Error: carácter no válido '{caracter}' en posición {x}";
                        return (false, mensajeError, recorrido);
                    }

                    recorrido += $" {caracter} -> ({siguienteEstado})";
                    estado = siguienteEstado;
                }

                recorrido += $" ⊣ (FDC) -> Token: {tokenReservado}";
                return (true, tokenReservado, recorrido);
            }

            CargarTabla();
            int estadoNormal = 0;
            string recorridoNormal = $"({estadoNormal})";

            for (int x = 0; x < cadena.Length; x++)
            {
                char caracter = cadena[x];
                int siguienteEstado = Validar(estadoNormal, caracter);

                if (siguienteEstado == -1)
                {
                    recorridoNormal += $" {caracter} -> (ERROR)";
                    string mensajeError = _categoriaAceptacion.TryGetValue(estadoNormal, out string? errorMsg)
                        ? errorMsg
                        : $"Error: carácter no válido '{caracter}' en posición {x}";
                    return (false, mensajeError, recorridoNormal);
                }

                recorridoNormal += $" {caracter} -> ({siguienteEstado})";
                estadoNormal = siguienteEstado;
            }

            if (_estadosFinalCadena.Contains(estadoNormal))
            {
                if (_tabla.TryGetValue((estadoNormal, "FDC"), out int estadoFinal))
                {
                    recorridoNormal += $" ⊣ -> ({estadoFinal})";
                    estadoNormal = estadoFinal;
                }
                else if (_categoriaAceptacion.TryGetValue(estadoNormal, out string? tokenDirecto))
                {
                    recorridoNormal += $" ⊣ (FDC)";
                    return (true, tokenDirecto, recorridoNormal);
                }
            }

            if (_categoriaAceptacion.TryGetValue(estadoNormal, out string? tokenBD))
            {
                if (tokenBD.StartsWith("Error"))
                {
                    return (false, tokenBD, recorridoNormal);
                }

                recorridoNormal += $" ⊣ (Token BD)";
                return (true, tokenBD, recorridoNormal);
            }

            if (cadena.Length > 0 && char.IsLetter(cadena[0]))
            {
                bool esIdentificadorValido = true;
                for (int x = 0; x < cadena.Length; x++)
                {
                    if (!char.IsLetterOrDigit(cadena[x]))
                    {
                        esIdentificadorValido = false;
                        break;
                    }
                }

                if (esIdentificadorValido)
                {
                    recorridoNormal += $" ⊣ (FDC) -> Token: IDV";
                    return (true, "IDV", recorridoNormal);
                }
            }

            string mensajeErrorFinal = _categoriaAceptacion.TryGetValue(estadoNormal, out string? errorFinal)
                ? errorFinal
                : "Cadena no aceptada (estado final sin categoría).";

            return (false, mensajeErrorFinal, recorridoNormal);
        }
        public int Validar(int estado, char caracter)
        {
            string? columna = ObtenerColumna(caracter);

            if (columna == null)
                return -1;

            if (_tabla.TryGetValue((estado, columna), out int siguiente))
                return siguiente;

            return -1;
        }
        public string? ObtenerColumna(char c)
        {
            if (char.IsLetter(c))
                return char.ToUpper(c).ToString();

            if (char.IsDigit(c))
                return "_" + c.ToString();
            //return c.ToString(); 

            if (simbolos.TryGetValue(c, out string? columna))
                return columna;

            return null;
        }
        public (List<(int linea, string valor, string token)> tokens,List<(int linea, string valor, string error)> errores,List<(int id, string nombre)> simbolos) AnalizarPrograma(string textoCompleto)
        {
            CargarTabla();
            var tokens = new List<(int, string, string)>();
            var errores = new List<(int, string, string)>();
            var simbolos = new List<(int id, string nombre)>();
            var diccionarioSimbolos = new Dictionary<string, int>();
            int contadorIDV = 1;

            var lineas = textoCompleto.Split('\n');

            for (int numLinea = 0; numLinea < lineas.Length; numLinea++)
            {
                string linea = lineas[numLinea].Trim();
                if (string.IsNullOrEmpty(linea)) continue;

                var tokensDeLinea = SepararEnTokens(linea);

                foreach (string tokenCrudo in tokensDeLinea)
                {
                    if (string.IsNullOrEmpty(tokenCrudo)) continue;

                    var (esValido, tokenTipo, _) = RecorrerCadena(tokenCrudo);

                    if (esValido)
                    {
                        if (tokenTipo == "IDV")
                        {
                            if (!diccionarioSimbolos.ContainsKey(tokenCrudo))
                            {
                                diccionarioSimbolos[tokenCrudo] = contadorIDV;
                                simbolos.Add((contadorIDV, tokenCrudo));
                                contadorIDV++;
                            }

                            int id = diccionarioSimbolos[tokenCrudo];
                            tokens.Add((numLinea + 1, tokenCrudo, $"IDV{id:D2}"));
                        }
                        else
                        {
                            tokens.Add((numLinea + 1, tokenCrudo, tokenTipo));
                        }
                    }
                    else
                    {
                        errores.Add((numLinea + 1, tokenCrudo, tokenTipo));
                        tokens.Add((numLinea + 1, tokenCrudo, "ERROR"));
                    }
                }
            }

            return (tokens, errores, simbolos);
        }
        private List<string> SepararEnTokens(string linea)
        {
            var tokens = new List<string>();
            int i = 0;

            while (i < linea.Length)
            {
                char c = linea[i];

                if (char.IsWhiteSpace(c)) { i++; continue; }

                if (c == '#')
                {
                    string comentario = "";
                    while (i < linea.Length)
                    {
                        comentario += linea[i];
                        i++;
                    }
                    tokens.Add(comentario);
                    continue;
                }
                if (c == '"')
                {
                    string cadena = "\"";
                    i++;

                    while (i < linea.Length && linea[i] != '"')
                    {
                        cadena += linea[i];
                        i++;
                    }

                    if (i < linea.Length && linea[i] == '"')
                    {
                        cadena += '"';
                        i++;
                    }

                    tokens.Add(cadena);
                    continue;
                }

                if (EsDelimitadorOOperador(c))
                {
                    if (i + 1 < linea.Length)
                    {
                        string doble = "" + c + linea[i + 1];
                        if (doble == "==" || doble == "<>" || doble == "<=" ||
                            doble == ">=" || doble == "><" || doble == "**")
                        {
                            tokens.Add(doble);
                            i += 2;
                            continue;
                        }
                    }
                    tokens.Add(c.ToString());
                    i++;
                    continue;
                }

                if (EsCaracterEspecial(c))
                {
                    string tokenInvalido = "";
                    while (i < linea.Length)
                    {
                        char actual = linea[i];
                        if (!char.IsWhiteSpace(actual) && !EsDelimitadorOOperador(actual))
                        {
                            tokenInvalido += actual;
                            i++;
                        }
                        else
                        {
                            break;
                        }
                    }
                    tokens.Add(tokenInvalido);
                    continue;
                }

                if (char.IsLetter(c))
                {
                    string palabra = "";
                    while (i < linea.Length)
                    {
                        char actual = linea[i];
                        if (char.IsLetterOrDigit(actual))
                        {
                            palabra += actual;
                            i++;
                        }
                        else if (EsCaracterEspecial(actual) && !char.IsWhiteSpace(actual))
                        {
                            palabra += actual;
                            i++;
                        }
                        else
                        {
                            break;
                        }
                    }
                    tokens.Add(palabra);
                    continue;
                }

                if ((c == '+' || c == '-') && i + 1 < linea.Length && char.IsDigit(linea[i + 1]))
                {
                    string numero = CapturarNumeroCompleto(linea, ref i, true);
                    tokens.Add(numero);
                    continue;
                }

                if (char.IsDigit(c))
                {
                    string numero = CapturarNumeroCompleto(linea, ref i, false);
                    if (i < linea.Length && !char.IsWhiteSpace(linea[i]) && !EsDelimitadorOOperador(linea[i]))
                    {
                        while (i < linea.Length && !char.IsWhiteSpace(linea[i]) && !EsDelimitadorOOperador(linea[i]))
                        {
                            numero += linea[i];
                            i++;
                        }
                    }
                    tokens.Add(numero);
                    continue;
                }
                tokens.Add(c.ToString());
                i++;
            }

            return tokens;
        }
        private bool EsDelimitadorOOperador(char c)
        {
            return c == '=' || c == ';' || c == ',' || c == '(' || c == ')' ||
                   c == '{' || c == '}' || c == ':' || c == '<' || c == '>' ;
        }
        private bool EsCaracterEspecial(char c)
        {
            return simbolos.ContainsKey(c) &&
                   c != '"' && c != '#' &&
                   !EsDelimitadorOOperador(c);
        }
        private string CapturarNumeroCompleto(string linea, ref int i, bool tieneSigno)
        {
            string numero = "";

            if (tieneSigno)
            {
                numero += linea[i];
                i++; 
            }

            bool tienePunto = false;
            bool tieneExponente = false;

            while (i < linea.Length)
            {
                char actual = linea[i];

                if (char.IsDigit(actual))
                {
                    numero += actual;
                    i++;
                }
                else if (actual == '.' && !tienePunto && !tieneExponente)
                {
                    if (i + 1 < linea.Length && char.IsDigit(linea[i + 1]))
                    {
                        numero += actual;
                        tienePunto = true;
                        i++;
                    }
                    else
                    {
                        break;
                    }
                }
                else if (actual == '*' && !tieneExponente)
                {
                    if (i + 1 < linea.Length && linea[i + 1] == '*')
                    {
                        int j = i + 2;
                        if (j < linea.Length)
                        {
                            char siguiente = linea[j];
                            if (char.IsDigit(siguiente) || siguiente == '+' || siguiente == '-')
                            {
                                numero += "**";
                                tieneExponente = true;
                                i += 2;

                                if (i < linea.Length && (linea[i] == '+' || linea[i] == '-'))
                                {
                                    numero += linea[i];
                                    i++;
                                }

                                while (i < linea.Length && char.IsDigit(linea[i]))
                                {
                                    numero += linea[i];
                                    i++;
                                }

                                break;
                            }
                        }
                    }
                    break;
                }
                else
                {
                    break;
                }
            }

            return numero;
        }
    }
}