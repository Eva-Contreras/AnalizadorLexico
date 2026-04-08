using Microsoft.Data.SqlClient;

namespace AnalizadorLexico
{
    public abstract class Recorrido
    {
        // Cadena de conexión
        protected string connectionString = "Server=EVA;Database=AnalizadorLexico;Trusted_Connection=True;TrustServerCertificate=True;";

        private Dictionary<(int estado, string columna), int> _tabla = new();
        private bool TablaCargada = false;
        public abstract bool Aceptacion(int estado);
        public abstract string NombreTabla { get; }
        public abstract string ObtenerColumna(char caracter);
        public abstract string ObtenerMensajeError(int estado, char? caracter);
        public abstract bool EstadoError(int estado);
        public abstract int ObtenerEstadoError();
        private void CargarTabla()
        {
            if (TablaCargada) return;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = $"SELECT * FROM {NombreTabla}";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataReader reader = cmd.ExecuteReader();

                    var columnas = new List<string>();
                    for (int i = 0; i < reader.FieldCount; i++)
                        columnas.Add(reader.GetName(i));

                    while (reader.Read())
                    {
                        int estado = Convert.ToInt32(reader["Estado"]);
                        foreach (var col in columnas)
                        {
                            if (col == "Estado") continue;
                            var val = reader[col];
                            if (val == null || val == DBNull.Value)
                                continue;

                            string valStr = val?.ToString()?.Trim() ?? string.Empty;
                            if (!string.IsNullOrEmpty(valStr))
                                _tabla[(estado, col)] = Convert.ToInt32(valStr);
                        }
                    }
                    TablaCargada = true;
                }
                catch (SqlException ex)
                {
                    MessageBox.Show($"Error al conectar con la base de datos:\n{ex.Message}",
                                    "Error de conexión",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error inesperado al cargar la tabla:\n{ex.Message}",
                                    "Error",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                }
            }
        }
        public (bool esValido, string mensajeError, string recorrido) RecorrerCadena(string cadena)
        {
            CargarTabla();
            int estado = 0;
            string recorrido = $"({estado})";

            for (int x = 0; x < cadena.Length; x++)
            {
                char caracter = cadena[x];
                int siguienteEstado = Validar(estado, caracter);

                recorrido += $" {caracter} -> ({siguienteEstado})";

                if (EstadoError(siguienteEstado))
                    return (false, ObtenerMensajeError(estado, caracter), recorrido);

                estado = siguienteEstado;
            }

            if (_tabla.TryGetValue((estado, "FDC"), out int estadoFinal))
            {
                recorrido += $" FDC -> ({estadoFinal})";

                if (Aceptacion(estadoFinal))
                    return (true, "", recorrido);
                else
                    return (false, ObtenerMensajeError(estadoFinal, null), recorrido);
            }

            return (false, ObtenerMensajeError(estado, null), recorrido);
        }
        public int Validar(int estado, char caracter)
        {
            string columna = ObtenerColumna(caracter);

            if (_tabla.TryGetValue((estado, columna), out int siguiente))
                return siguiente;

            return ObtenerEstadoError();
        }
    }
}