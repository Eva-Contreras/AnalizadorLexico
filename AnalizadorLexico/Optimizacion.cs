using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

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
            AplicarOptimizacionesLocales(codigo);
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
        //hola
        // ========================================================
        // 1. OPTIMIZACIÓN LOCAL (Plegado y Simplificación)
        // ========================================================
        private void AplicarOptimizacionesLocales(List<Cuadruplo> codigo)
        {
            Dictionary<string, string> constantes = new(StringComparer.Ordinal);

            for (int i = 0; i < codigo.Count; i++)
            {
                Cuadruplo instruccion = codigo[i];
                if (instruccion.Operador == "LABEL") constantes.Clear();
                bool operandosEranLiterales = EsNumero(instruccion.Argumento1) &&
                    (UsaArgumento2(instruccion.Operador) ? EsNumero(instruccion.Argumento2) : true);

                SustituirConstantesEnUsos(instruccion, constantes);
                bool plegada = TryEvaluarConstante(instruccion, operandosEranLiterales, out string resultado);
                if (plegada)
                {
                    string expresion = FormatearExpresion(instruccion);
                    instruccion.Operador = "=";
                    instruccion.Argumento1 = resultado;
                    instruccion.Argumento2 = string.Empty;
                    LogOptimizacion.Add($"[Local] Plegado de constantes: {expresion} → {resultado}.");
                }
                else
                {
                    AplicarIdentidadAlgebraica(instruccion);
                }

                if (instruccion.Operador == "READ")
                {
                    constantes.Remove(instruccion.Argumento1);
                }
                else if (instruccion.Operador == "CALL")
                {
                    constantes.Clear();
                }
                else if (instruccion.Operador == "=")
                {
                    if (EsNumero(instruccion.Argumento1))
                        constantes[instruccion.Resultado] = instruccion.Argumento1;
                    else
                        constantes.Remove(instruccion.Resultado);
                }
                else if (DefineResultado(instruccion.Operador))
                {
                    constantes.Remove(instruccion.Resultado);
                }

                if (instruccion.Operador is "GOTO" or "IF_FALSE" or "IF_TRUE" or "RETURN")
                    constantes.Clear();
            }
        }

        private static void SustituirConstantesEnUsos(Cuadruplo instruccion, Dictionary<string, string> constantes)
        {
            if (UsaArgumento1(instruccion.Operador) && constantes.TryGetValue(instruccion.Argumento1, out string? constante1))
                instruccion.Argumento1 = constante1;
            if (UsaArgumento2(instruccion.Operador) && constantes.TryGetValue(instruccion.Argumento2, out string? constante2))
                instruccion.Argumento2 = constante2;
        }

        private static bool TryEvaluarConstante(Cuadruplo instruccion, bool operandosEranLiterales, out string resultado)
        {
            resultado = string.Empty;
            bool operacionAritmetica = instruccion.Operador is "+" or "-" or "*" or "/" or "%" or "^" or "NEG" or "POS";
            if (operacionAritmetica && !operandosEranLiterales) return false;
            if (!TryParseNumero(instruccion.Argumento1, out double izquierda) || !double.IsFinite(izquierda))
                return false;

            double valor;
            switch (instruccion.Operador)
            {
                case "NEG": valor = -izquierda; break;
                case "POS": valor = izquierda; break;
                case "NOT": valor = izquierda == 0 ? 1 : 0; break;
                default:
                    if (!TryParseNumero(instruccion.Argumento2, out double derecha) || !double.IsFinite(derecha))
                        return false;
                    switch (instruccion.Operador)
                    {
                        case "+": valor = izquierda + derecha; break;
                        case "-": valor = izquierda - derecha; break;
                        case "*": valor = izquierda * derecha; break;
                        case "/":
                            if (derecha == 0 || EsEnteroLiteral(instruccion.Argumento1) && EsEnteroLiteral(instruccion.Argumento2) && izquierda % derecha != 0)
                                return false;
                            valor = izquierda / derecha;
                            break;
                        case "%": if (derecha == 0) return false; valor = izquierda % derecha; break;
                        case "^": valor = Math.Pow(izquierda, derecha); break;
                        case ">": valor = izquierda > derecha ? 1 : 0; break;
                        case "<": valor = izquierda < derecha ? 1 : 0; break;
                        case ">=": valor = izquierda >= derecha ? 1 : 0; break;
                        case "<=": valor = izquierda <= derecha ? 1 : 0; break;
                        case "==": valor = izquierda == derecha ? 1 : 0; break;
                        case "!=": valor = izquierda != derecha ? 1 : 0; break;
                        case "AND": valor = izquierda != 0 && derecha != 0 ? 1 : 0; break;
                        case "OR": valor = izquierda != 0 || derecha != 0 ? 1 : 0; break;
                        default: return false;
                    }
                    break;
            }

            if (!double.IsFinite(valor)) return false;
            resultado = valor.ToString("G", CultureInfo.InvariantCulture);
            return true;
        }

        private static string FormatearExpresion(Cuadruplo instruccion) =>
            instruccion.Operador is "NEG" or "POS" or "NOT"
                ? $"{instruccion.Operador} {instruccion.Argumento1}"
                : $"{instruccion.Argumento1} {instruccion.Operador} {instruccion.Argumento2}";

        private void AplicarIdentidadAlgebraica(Cuadruplo instruccion)
        {
            bool identidadDerecha = instruccion.Operador == "*" && instruccion.Argumento2 == "1" ||
                (instruccion.Operador is "+" or "-") && instruccion.Argumento2 == "0";
            if (identidadDerecha)
            {
                string operacion = $"{instruccion.Argumento1} {instruccion.Operador} {instruccion.Argumento2}";
                instruccion.Operador = "=";
                instruccion.Argumento2 = string.Empty;
                LogOptimizacion.Add($"[Local] Simplificación algebraica: {operacion} → {instruccion.Argumento1}.");
            }
            else if (instruccion.Operador == "*" && instruccion.Argumento1 == "1")
            {
                string operacion = $"1 * {instruccion.Argumento2}";
                instruccion.Operador = "=";
                instruccion.Argumento1 = instruccion.Argumento2;
                instruccion.Argumento2 = string.Empty;
                LogOptimizacion.Add($"[Local] Simplificación algebraica: {operacion} → {instruccion.Argumento1}.");
            }
        }

        private void EliminarTemporalesMuertos(List<Cuadruplo> codigo)
        {
            bool seElimino;
            do
            {
                HashSet<string> usados = new(StringComparer.Ordinal);
                foreach (Cuadruplo instruccion in codigo)
                    foreach (string uso in ObtenerUsos(instruccion))
                        usados.Add(uso);

                seElimino = false;
                for (int i = codigo.Count - 1; i >= 0; i--)
                {
                    Cuadruplo instruccion = codigo[i];
                    if (EsTemporal(instruccion.Resultado) && EsOperacionPuraEliminable(instruccion.Operador) &&
                        !usados.Contains(instruccion.Resultado))
                    {
                        LogOptimizacion.Add($"[Global] Se eliminó la definición muerta del temporal {instruccion.Resultado}.");
                        codigo.RemoveAt(i);
                        seElimino = true;
                    }
                }
            } while (seElimino);
        }

        private static IEnumerable<string> ObtenerUsos(Cuadruplo instruccion)
        {
            if (UsaArgumento1(instruccion.Operador) && !string.IsNullOrEmpty(instruccion.Argumento1))
                yield return instruccion.Argumento1;
            if (UsaArgumento2(instruccion.Operador) && !string.IsNullOrEmpty(instruccion.Argumento2))
                yield return instruccion.Argumento2;
            if (instruccion.Operador == "LOCATE")
            {
                foreach (Match coincidencia in Regex.Matches(instruccion.Argumento1, @"(?<![A-Za-z0-9_])t\d+(?![A-Za-z0-9_])", RegexOptions.IgnoreCase))
                    yield return coincidencia.Value;
            }
        }

        private static bool UsaArgumento1(string operador) =>
            operador is "=" or "NEG" or "POS" or "NOT" or "+" or "-" or "*" or "/" or "%" or "^" or
                ">" or "<" or ">=" or "<=" or "==" or "!=" or "AND" or "OR" or
                "IF_FALSE" or "IF_TRUE" or "PRINT" or "RETURN" or "PARAM";

        private static bool UsaArgumento2(string operador) =>
            operador is "+" or "-" or "*" or "/" or "%" or "^" or ">" or "<" or ">=" or "<=" or "==" or "!=" or "AND" or "OR";

        private static bool DefineResultado(string operador) =>
            operador is "=" or "NEG" or "POS" or "NOT" or "+" or "-" or "*" or "/" or "%" or "^" or
                ">" or "<" or ">=" or "<=" or "==" or "!=" or "AND" or "OR" or "CALL";

        private static bool EsOperacionPuraEliminable(string operador) =>
            operador is "=" or "NEG" or "POS" or "NOT" or "+" or "-" or "*" or ">" or "<" or ">=" or "<=" or "==" or "!=" or "AND" or "OR";

        private static bool EsVariable(string valor) =>
            Regex.IsMatch(valor, @"^[A-Za-z_][A-Za-z0-9_]*$") || EsTemporal(valor);

        private static bool EsTemporal(string valor) =>
            Regex.IsMatch(valor, @"^t\d+$", RegexOptions.IgnoreCase);

        private static bool EsEscrituraDe(Cuadruplo instruccion, string variable) =>
            instruccion.Operador == "READ" && instruccion.Argumento1 == variable ||
            DefineResultado(instruccion.Operador) && instruccion.Resultado == variable;

        private static bool EsBarreraDePropagacion(string operador) =>
            operador is "LABEL" or "GOTO" or "IF_FALSE" or "IF_TRUE" or "READ" or "CALL" or "RETURN" or "LOCATE";

        private static bool SustituirUso(Cuadruplo instruccion, string destino, string origen)
        {
            bool sustituido = false;
            if (UsaArgumento1(instruccion.Operador) && instruccion.Argumento1 == destino)
            {
                instruccion.Argumento1 = origen;
                sustituido = true;
            }
            if (UsaArgumento2(instruccion.Operador) && instruccion.Argumento2 == destino)
            {
                instruccion.Argumento2 = origen;
                sustituido = true;
            }
            return sustituido;
        }

        private static bool TryParseNumero(string valor, out double numero) =>
            double.TryParse(valor, NumberStyles.Float, CultureInfo.InvariantCulture, out numero);

        private static bool EsNumero(string valor) =>
            TryParseNumero(valor, out double numero) && double.IsFinite(numero);

        private static bool EsEnteroLiteral(string valor) =>
            Regex.IsMatch(valor, @"^-?\d+$");

        // ========================================================
        // 2. OPTIMIZACIÓN DE MIRILLA (Peephole - Saltos redundantes)
        // ========================================================
        private void AplicarOptimizacionesMirilla(List<Cuadruplo> codigo)
        {
            for (int i = 0; i < codigo.Count; i++)
            {
                var actual = codigo[i];

                if ((actual.Operador is "IF_TRUE" or "IF_FALSE") && TryParseNumero(actual.Argumento1, out double condicion) && double.IsFinite(condicion))
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
        // 3. PROPAGACIÓN DE COPIAS EN SECUENCIAS SIN SALTOS
        // ========================================================
        private void AplicarOptimizacionesGlobales(List<Cuadruplo> codigo)
        {
            for (int i = 0; i < codigo.Count; i++)
            {
                Cuadruplo asignacion = codigo[i];
                if (asignacion.Operador != "=" || !EsVariable(asignacion.Resultado) ||
                    !EsVariable(asignacion.Argumento1) || asignacion.Resultado == asignacion.Argumento1)
                    continue;

                string destino = asignacion.Resultado;
                string origen = asignacion.Argumento1;
                for (int j = i + 1; j < codigo.Count; j++)
                {
                    Cuadruplo siguiente = codigo[j];
                    if (EsBarreraDePropagacion(siguiente.Operador)) break;
                    if (EsEscrituraDe(siguiente, destino) || EsEscrituraDe(siguiente, origen)) break;

                    bool sustituido = SustituirUso(siguiente, destino, origen);
                    if (sustituido)
                        LogOptimizacion.Add($"[Global] Propagación de copias: '{destino}' se sustituyó por '{origen}'.");

                    if (siguiente.Operador is "IF_TRUE" or "IF_FALSE" or "GOTO" or "RETURN") break;
                }
            }
        }

        // ========================================================
        // 4. MOVIMIENTO CONSERVADOR DE INVARIANTES DE CICLO
        // ========================================================
        private void AplicarOptimizacionesBucles(List<Cuadruplo> codigo)
        {
            for (int inicio = 0; inicio < codigo.Count; inicio++)
            {
                if (codigo[inicio].Operador != "LABEL") continue;
                string etiquetaInicio = codigo[inicio].Resultado;
                List<int> regresos = codigo
                    .Select((instruccion, indice) => (instruccion, indice))
                    .Where(x => x.indice > inicio && (x.instruccion.Operador is "GOTO" or "IF_TRUE" or "IF_FALSE"))
                    .Where(x => x.instruccion.Resultado == etiquetaInicio)
                    .Select(x => x.indice)
                    .ToList();
                if (regresos.Count != 1) continue;

                int fin = regresos[0];
                if (inicio + 1 >= fin) continue;

                Cuadruplo candidato = codigo[inicio + 1];
                if (candidato.Operador is not "+" and not "-" and not "*") continue;
                if (!EsTemporal(candidato.Resultado) ||
                    (!EsNumero(candidato.Argumento1) && !EsVariable(candidato.Argumento1)) ||
                    (!EsNumero(candidato.Argumento2) && !EsVariable(candidato.Argumento2)))
                    continue;
                if (codigo.Skip(inicio + 1).Take(fin - inicio - 1).Any(c => c.Operador == "CALL")) continue;
                if (codigo.Count(c => c.Resultado == candidato.Resultado && DefineResultado(c.Operador)) != 1) continue;

                bool operandoModificado = codigo.Skip(inicio + 1).Take(fin - inicio - 1)
                    .Any(c => EsEscrituraDe(c, candidato.Argumento1) || EsEscrituraDe(c, candidato.Argumento2));
                if (operandoModificado) continue;

                bool usoDentro = false;
                bool usoFuera = false;
                for (int indice = 0; indice < codigo.Count; indice++)
                {
                    if (!ObtenerUsos(codigo[indice]).Contains(candidato.Resultado)) continue;
                    if (indice > inicio + 1 && indice < fin) usoDentro = true;
                    else usoFuera = true;
                }
                if (!usoDentro || usoFuera) continue;

                codigo.RemoveAt(inicio + 1);
                codigo.Insert(inicio, candidato);
                LogOptimizacion.Add($"[Bucles] Se movió '{candidato.Argumento1} {candidato.Operador} {candidato.Argumento2}' fuera del ciclo {etiquetaInicio}; se limitó a suma, resta o multiplicación invariantes.");
                inicio++;
            }
        }


    }
}