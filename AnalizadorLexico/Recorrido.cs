using Microsoft.Data.SqlClient;

namespace AnalizadorLexico
{
    public class Recorrido
    {
        // Cadena de conexión
        protected string connectionString = "Server=EVA;Database=AnalizadorLexico;Trusted_Connection=True;TrustServerCertificate=True;";

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
                    return (false, $"Error: carácter no válido '{caracter}' en posición {x}", recorrido);
                }

                recorrido += $" {caracter} -> ({siguienteEstado})";
                estado = siguienteEstado;
            }

            if (_estadosFinalCadena.Contains(estado))
            {

                if (_tabla.TryGetValue((estado, "FDC"), out int estadoFinal))
                {
                    recorrido += $" ⊣ -> ({estadoFinal})";
                    estado = estadoFinal;
                }
            }

            if (_categoriaAceptacion.TryGetValue(estado, out string? token))
            {
                return (true, token, recorrido);
            }

            return (false, "Cadena no aceptada (estado final sin categoría).", recorrido);
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
                return c.ToString(); 

            if (simbolos.TryGetValue(c, out string? columna))
                return columna;

            return null;
        }
    }
}