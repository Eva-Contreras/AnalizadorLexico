using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace AnalizadorLexico
{
    public class Optimizacion
    {
        public List<string> LogOptimizacion { get; } = new();

        public List<Cuadruplo> Optimizar(List<Cuadruplo> originales)
        {
            if (originales == null) return new List<Cuadruplo>();

            // 1. Clonamos la lista para no modificar la original
            List<Cuadruplo> codigo = originales.Select(c => new Cuadruplo
            {
                Numero = c.Numero,
                Operador = c.Operador,
                Argumento1 = c.Argumento1,
                Argumento2 = c.Argumento2,
                Resultado = c.Resultado
            }).ToList();

            LogOptimizacion.Clear();

            // 2. CAMBIO DE ORDEN: Primero Bucles, luego Locales
            AplicarOptimizacionesBucles(codigo);       // <-- Se mueve aquí
            AplicarOptimizacionesGlobales(codigo);
            AplicarOptimizacionesLocales(codigo);      // <-- Ahora se aplica después
            AplicarOptimizacionesMirilla(codigo);
            EliminarTemporalesMuertos(codigo);

            // 3. Renumeramos al final
            for (int i = 0; i < codigo.Count; i++)
            {
                codigo[i].Numero = i + 1;
            }

            if (LogOptimizacion.Count == 0)
            {
                LogOptimizacion.Add("No se encontraron patrones de optimización aplicables (el código ya es óptimo o no cumple las reglas).");
            }

            return codigo;
        }

        // ========================================================
        // 1. OPTIMIZACIÓN LOCAL (Plegado y Simplificación)
        // ========================================================
        private void AplicarOptimizacionesLocales(List<Cuadruplo> codigo)
        {
            Dictionary<string, string> constantes = new(StringComparer.Ordinal);
            Dictionary<string, string> origenes = new(StringComparer.Ordinal);

            for (int i = 0; i < codigo.Count; i++)
            {
                var c = codigo[i];

                if (c.Operador is "LABEL" or "GOTO" or "IF_FALSE" or "IF_TRUE")
                {
                    constantes.Clear();
                    origenes.Clear();
                }

                string argumentoOriginal1 = c.Argumento1;
                string argumentoOriginal2 = c.Argumento2;
                string expresionOriginal1 = ObtenerExpresion(argumentoOriginal1, origenes);
                string expresionOriginal2 = ObtenerExpresion(argumentoOriginal2, origenes);
                bool plegadoConstante = false;

                if (constantes.TryGetValue(c.Argumento1, out string? constante1))
                {
                    c.Argumento1 = constante1;
                    LogOptimizacion.Add($"[Local] Propagación de constantes: se reemplazó '{argumentoOriginal1}' por '{constante1}'.");
                }
                if (constantes.TryGetValue(c.Argumento2, out string? constante2))
                {
                    c.Argumento2 = constante2;
                    LogOptimizacion.Add($"[Local] Propagación de constantes: se reemplazó '{argumentoOriginal2}' por '{constante2}'.");
                }

                // Plegado de constantes (ej. 5 + 3 = 8)
                if (EsOperadorAritmetico(c.Operador) &&
                    TryParseNumero(c.Argumento1, out double val1) &&
                    TryParseNumero(c.Argumento2, out double val2))
                {
                    double resultado = c.Operador switch
                    {
                        "+" => val1 + val2,
                        "-" => val1 - val2,
                        "*" => val1 * val2,
                        "/" => val2 != 0 ? val1 / val2 : double.NaN,
                        _ => double.NaN
                    };

                    if (!double.IsNaN(resultado))
                    {
                        string resStr = resultado.ToString(CultureInfo.InvariantCulture);
                        string operadorOriginal = c.Operador;
                        string expresion = $"{expresionOriginal1} {operadorOriginal} {expresionOriginal2}";
                        LogOptimizacion.Add($"[Local] Se aplicó Plegado de Constantes: {expresion} = {resStr} (en {c.Resultado}).");
                        c.Operador = "=";
                        c.Argumento1 = resStr;
                        c.Argumento2 = "_";
                        constantes[c.Resultado] = resStr;
                        origenes[c.Resultado] = expresion;
                        plegadoConstante = true;
                    }
                }

                if (!plegadoConstante && c.Operador == "=")
                {
                    if (origenes.TryGetValue(argumentoOriginal1, out string? origenAsignado) &&
                        EsNumero(c.Argumento1))
                    {
                        LogOptimizacion.Add($"[Local] Se aplicó Plegado de Constantes: {origenAsignado} = {c.Argumento1} (asignado a {c.Resultado}).");
                        origenes[c.Resultado] = origenAsignado;
                    }
                    else if (!origenes.ContainsKey(c.Resultado))
                    {
                        origenes.Remove(c.Resultado);
                    }

                    if (EsNumero(c.Argumento1))
                        constantes[c.Resultado] = c.Argumento1;
                    else
                        constantes.Remove(c.Resultado);
                }
                else if (!plegadoConstante && !string.IsNullOrEmpty(c.Resultado))
                {
                    constantes.Remove(c.Resultado);
                    if (!origenes.ContainsKey(c.Resultado))
                        origenes.Remove(c.Resultado);
                }

                // Simplificación algebraica (ej. x * 1 = x, x + 0 = x)
                if (EsOperadorAritmetico(c.Operador))
                {
                    if (c.Operador == "*" && c.Argumento2 == "1")
                    {
                        LogOptimizacion.Add($"[Local] Simplificación algebraica: {c.Argumento1} * 1 → {c.Argumento1}");
                        c.Operador = "="; c.Argumento2 = "_";
                    }
                    else if (c.Operador == "*" && c.Argumento1 == "1")
                    {
                        LogOptimizacion.Add($"[Local] Simplificación algebraica: 1 * {c.Argumento2} → {c.Argumento2}");
                        c.Operador = "="; c.Argumento1 = c.Argumento2; c.Argumento2 = "_";
                    }
                    else if (c.Operador == "+" && c.Argumento2 == "0")
                    {
                        LogOptimizacion.Add($"[Local] Simplificación algebraica: {c.Argumento1} + 0 → {c.Argumento1}");
                        c.Operador = "="; c.Argumento2 = "_";
                    }
                    else if (c.Operador == "-" && c.Argumento2 == "0")
                    {
                        LogOptimizacion.Add($"[Local] Simplificación algebraica: {c.Argumento1} - 0 → {c.Argumento1}");
                        c.Operador = "="; c.Argumento2 = "_";
                    }
                }
            }
        }

        private void EliminarTemporalesMuertos(List<Cuadruplo> codigo)
        {
            bool seElimino;
            do
            {
                HashSet<string> usados = codigo
                    .SelectMany(c => new[] { c.Argumento1, c.Argumento2 })
                    .Where(argumento => !string.IsNullOrEmpty(argumento))
                    .ToHashSet(StringComparer.Ordinal);

                seElimino = false;
                for (int i = codigo.Count - 1; i >= 0; i--)
                {
                    Cuadruplo c = codigo[i];
                    bool esTemporal = c.Resultado.StartsWith("t", StringComparison.Ordinal);
                    bool esOperacionSinEfectos = c.Operador == "=" ||
                        EsOperadorAritmetico(c.Operador) || c.Operador is "NEG" or "POS" or "NOT";

                    if (esTemporal && esOperacionSinEfectos && !usados.Contains(c.Resultado))
                    {
                        LogOptimizacion.Add($"[Local] Código muerto eliminado: temporal {c.Resultado}.");
                        codigo.RemoveAt(i);
                        seElimino = true;
                    }
                }
            } while (seElimino);
        }

        private static string ObtenerExpresion(string operando, Dictionary<string, string> origenes)
        {
            if (origenes.TryGetValue(operando, out string? origen))
                return $"({origen})";
            return operando;
        }

        private static bool TryParseNumero(string valor, out double numero)
        {
            return double.TryParse(valor, NumberStyles.Float, CultureInfo.InvariantCulture, out numero);
        }

        // ========================================================
        // 2. OPTIMIZACIÓN DE MIRILLA (Peephole - Saltos redundantes)
        // ========================================================
        private void AplicarOptimizacionesMirilla(List<Cuadruplo> codigo)
        {
            for (int i = 0; i < codigo.Count; i++)
            {
                var actual = codigo[i];

                if ((actual.Operador is "IF_TRUE" or "IF_FALSE") && TryParseNumero(actual.Argumento1, out double condicion))
                {
                    bool esVerdadera = condicion != 0;
                    bool tomaSalto = actual.Operador == "IF_TRUE" ? esVerdadera : !esVerdadera;

                    if (tomaSalto)
                    {
                        actual.Operador = "GOTO";
                        actual.Argumento1 = string.Empty;
                        actual.Argumento2 = string.Empty;
                    }
                    else
                    {
                        codigo.RemoveAt(i);
                        i--;
                    }

                    LogOptimizacion.Add("[Mirilla] Se eliminó salto redundante por evaluación de condición constante.");
                    continue;
                }

                if (i >= codigo.Count - 1)
                    continue;

                var siguiente = codigo[i + 1];

                // Eliminar GOTO si salta a la etiqueta que está justo en la línea siguiente
                if (actual.Operador == "GOTO" && siguiente.Operador == "LABEL" && actual.Resultado == siguiente.Resultado)
                {
                    LogOptimizacion.Add($"[Mirilla] Salto redundante eliminado: GOTO {actual.Resultado} (la etiqueta está en la línea siguiente).");
                    codigo.RemoveAt(i);
                    i--; // Retrocedemos para volver a revisar el anterior
                }
            }
        }

        // ========================================================
        // 3. OPTIMIZACIÓN GLOBAL (Propagación de copias)
        // ========================================================
        private void AplicarOptimizacionesGlobales(List<Cuadruplo> codigo)
        {
            // Buscamos asignaciones tipo "var1 = var2" y reemplazamos los usos de var1 por var2 si es seguro
            for (int i = 0; i < codigo.Count; i++)
            {
                var c = codigo[i];
                if (c.Operador == "=" && !EsNumero(c.Argumento1) && !c.Argumento1.StartsWith("\"")) // Es una copia (ej: a = b)
                {
                    string origen = c.Argumento1;
                    string destino = c.Resultado;

                    // Verificar en las siguientes líneas si 'destino' se usa y no se modifica
                    for (int j = i + 1; j < codigo.Count; j++)
                    {
                        var sig = codigo[j];

                        // Si modifican 'destino', dejamos de propagar (por seguridad)
                        if (sig.Resultado == destino)
                            break;

                        // Si usa 'destino', lo cambiamos por 'origen'
                        if (sig.Argumento1 == destino)
                        {
                            sig.Argumento1 = origen;
                            LogOptimizacion.Add($"[Global] Propagación de copias: se reemplazó '{destino}' por '{origen}'.");
                        }
                        if (sig.Argumento2 == destino)
                        {
                            sig.Argumento2 = origen;
                            LogOptimizacion.Add($"[Global] Propagación de copias: se reemplazó '{destino}' por '{origen}'.");
                        }
                    }
                }
            }
        }

        // ========================================================
        // 4. OPTIMIZACIÓN DE BUCLES (Invariantes de bucle)
        // ========================================================
        private void AplicarOptimizacionesBucles(List<Cuadruplo> codigo)
        {
            for (int i = 0; i < codigo.Count; i++)
            {
                if (codigo[i].Operador == "LABEL")
                {
                    string etiquetaInicio = codigo[i].Resultado;
                    int posGOTO = -1;

                    // Buscar el GOTO que cierra el ciclo (ignorando los IF)
                    for (int j = i + 1; j < codigo.Count; j++)
                    {
                        if (codigo[j].Operador == "GOTO" && codigo[j].Resultado == etiquetaInicio)
                        {
                            posGOTO = j;
                            break;
                        }
                    }

                    if (posGOTO > i)
                    {
                        // Buscar operaciones aritméticas dentro del bucle
                        for (int j = i + 1; j < posGOTO; j++)
                        {
                            var op = codigo[j];
                            if (EsOperadorAritmetico(op.Operador))
                            {
                                // Revisar si los argumentos se modifican dentro del bucle
                                bool arg1SeModifica = false;
                                bool arg2SeModifica = false;

                                for (int k = i + 1; k < posGOTO; k++)
                                {
                                    if (codigo[k].Resultado == op.Argumento1) arg1SeModifica = true;
                                    if (codigo[k].Resultado == op.Argumento2) arg2SeModifica = true;
                                }

                                // Si NINGUNO se modifica, es invariante (aunque sean variables)
                                if (!arg1SeModifica && !arg2SeModifica)
                                {
                                    Cuadruplo instruccionInvariante = op;
                                    codigo.RemoveAt(j);

                                    // Insertar justo antes del LABEL de inicio
                                    codigo.Insert(i, instruccionInvariante);

                                    LogOptimizacion.Add($"[Bucles] Se sacó operación invariante del bucle {etiquetaInicio}: {instruccionInvariante.Argumento1} {instruccionInvariante.Operador} {instruccionInvariante.Argumento2} → {instruccionInvariante.Resultado}.");

                                    i--;
                                    posGOTO--;
                                    j--;
                                }
                            }
                        }
                    }
                }
            }
        }

        private bool EsOperadorAritmetico(string op) => op == "+" || op == "-" || op == "*" || op == "/";

        private bool EsNumero(string val)
        {
            return TryParseNumero(val, out _);
        }
    }
}