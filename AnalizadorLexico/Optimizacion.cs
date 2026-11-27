using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace AnalizadorLexico
{
    public class Optimizacion
    {
        private readonly List<string> _informeOptimizacion = new();
        public List<string> LogOptimizacion { get; } = new();

        public int InstruccionesAntesOptimizacion { get; private set; }
        public int InstruccionesDespuesOptimizacion { get; private set; }
        public int BloquesBasicos { get; private set; }
        public int CostoAntesOptimizacion { get; private set; }
        public int CostoDespuesOptimizacion { get; private set; }
        public int TemporalesAntesOptimizacion { get; private set; }
        public int TemporalesDespuesOptimizacion { get; private set; }
        public int PilaEstimadaAntesOptimizacion { get; private set; }
        public int PilaEstimadaDespuesOptimizacion { get; private set; }
        public int RegistrosEstimadosAntesOptimizacion { get; private set; }
        public int RegistrosEstimadosDespuesOptimizacion { get; private set; }
        public int MemoriaEstimadaAntesOptimizacion { get; private set; }
        public int MemoriaEstimadaDespuesOptimizacion { get; private set; }
        public string InformeOptimizacion => string.Join(Environment.NewLine, _informeOptimizacion);
        public List<Cuadruplo> Optimizar(List<Cuadruplo> originales)
        {
            ArgumentNullException.ThrowIfNull(originales);
            List<Cuadruplo> optimizado = originales.Select(ClonarCuadruplo).ToList();
            ValidarOperadores(optimizado);
            ValidarEtiquetas(optimizado);

            _informeOptimizacion.Clear();
            LogOptimizacion.Clear();
            InstruccionesAntesOptimizacion = optimizado.Count;
            BloquesBasicos = ConstruirBloques(optimizado).Count;
            CostoAntesOptimizacion = CalcularCostoEstructural(optimizado);
            TemporalesAntesOptimizacion = ContarTemporales(optimizado);
            PilaEstimadaAntesOptimizacion = EstimarPicoPila(optimizado);
            RegistrosEstimadosAntesOptimizacion = ContarTemporales(optimizado);
            MemoriaEstimadaAntesOptimizacion = EstimarMemoria(optimizado);
            AplicarOptimizacionesLocales(optimizado);
            AplicarOptimizacionesGlobales(optimizado);
            AplicarOptimizacionesBucles(optimizado);
            AplicarOptimizacionesMirilla(optimizado);
            RenumerarCuadruplos(optimizado);

            ValidarOperadores(optimizado);
            ValidarEtiquetas(optimizado);
            InstruccionesDespuesOptimizacion = optimizado.Count;
            CostoDespuesOptimizacion = CalcularCostoEstructural(optimizado);
            TemporalesDespuesOptimizacion = ContarTemporales(optimizado);
            PilaEstimadaDespuesOptimizacion = EstimarPicoPila(optimizado);
            RegistrosEstimadosDespuesOptimizacion = ContarTemporales(optimizado);
            MemoriaEstimadaDespuesOptimizacion = EstimarMemoria(optimizado);
            if (LogOptimizacion.Count == 0)
                LogOptimizacion.Add("No se encontraron optimizaciones aplicables.");
            _informeOptimizacion.AddRange(LogOptimizacion);
            return optimizado;
        }

        private void AplicarOptimizacionesLocales(List<Cuadruplo> instrucciones) => OptimizarConstantesLocales(instrucciones);

        private void AplicarOptimizacionesGlobales(List<Cuadruplo> instrucciones)
        {
            AplicarOptimizacionGlobal(instrucciones);
            EliminarTemporalesMuertos(instrucciones);
        }

        private void AplicarOptimizacionesBucles(List<Cuadruplo> instrucciones) => AplicarOptimizacionDeBucles(instrucciones);

        private void AplicarOptimizacionesMirilla(List<Cuadruplo> instrucciones) => AplicarOptimizacionDeMirilla(instrucciones);

        private static bool CuadruplosIguales(IReadOnlyList<Cuadruplo> izquierda, IReadOnlyList<Cuadruplo> derecha) =>
            izquierda.Count == derecha.Count && izquierda.Zip(derecha).All(par =>
                par.First.Numero == par.Second.Numero &&
                par.First.Operador == par.Second.Operador &&
                par.First.Argumento1 == par.Second.Argumento1 &&
                par.First.Argumento2 == par.Second.Argumento2 &&
                par.First.Resultado == par.Second.Resultado);

        public string AnalizarFlujoDeDatos(IReadOnlyCollection<Cuadruplo> codigo)
        {
            ArgumentNullException.ThrowIfNull(codigo);
            List<Cuadruplo> instrucciones = codigo.Select(ClonarCuadruplo).ToList();
            ValidarOperadores(instrucciones);
            ValidarEtiquetas(instrucciones);
            var bloques = ConstruirBloques(instrucciones);
            CalcularVariablesVivas(instrucciones, bloques, out HashSet<string>[] vivasEntrada, out HashSet<string>[] vivasSalida);
            List<string> lineas = new();

            CalcularExpresionesDisponibles(instrucciones, bloques, out HashSet<string>[] disponiblesEntrada, out HashSet<string>[] disponiblesSalida);

            for (int i = 0; i < bloques.Count; i++)
            {
                var bloque = bloques[i];
                string sucesores = string.Join(", ", bloque.Sucesores.OrderBy(x => x));
                lineas.Add($"B{i}: instrucciones {bloque.Inicio + 1}-{bloque.Fin + 1}; sucesores [{sucesores}]");
                lineas.Add($"  Vivas entrada: {{{string.Join(", ", vivasEntrada[i].OrderBy(x => x, StringComparer.Ordinal))}}}");
                lineas.Add($"  Vivas salida: {{{string.Join(", ", vivasSalida[i].OrderBy(x => x, StringComparer.Ordinal))}}}");
                lineas.Add($"  Expresiones disponibles entrada: {{{string.Join(", ", disponiblesEntrada[i].OrderBy(x => x, StringComparer.Ordinal))}}}");
                lineas.Add($"  Expresiones disponibles salida: {{{string.Join(", ", disponiblesSalida[i].OrderBy(x => x, StringComparer.Ordinal))}}}");
            }

            return string.Join(Environment.NewLine, lineas);
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

        private static bool EsTemporal(string nombre) =>
            Regex.IsMatch(nombre, @"^t\d+$", RegexOptions.IgnoreCase);

        private static bool EsLiteral(string valor) =>
            EsNumero(valor) || (valor.Length >= 2 && valor.StartsWith("\"") && valor.EndsWith("\""));

        private static bool EsIdentificador(string valor) =>
            Regex.IsMatch(valor, @"^[A-Za-z_][A-Za-z0-9_]*$") && !EsTemporal(valor);

        private static List<(int Inicio, int Fin, HashSet<int> Predecesores, HashSet<int> Sucesores)> ConstruirBloques(
            IReadOnlyList<Cuadruplo> instrucciones)
        {
            List<(int Inicio, int Fin, HashSet<int> Predecesores, HashSet<int> Sucesores)> bloques = new();
            if (instrucciones.Count == 0) return bloques;

            Dictionary<string, int> indicesEtiquetas = instrucciones
                .Select((instruccion, indice) => (instruccion, indice))
                .Where(x => x.instruccion.Operador == "LABEL")
                .ToDictionary(x => x.instruccion.Resultado, x => x.indice, StringComparer.Ordinal);
            SortedSet<int> lideres = new() { 0 };

            for (int i = 0; i < instrucciones.Count; i++)
            {
                Cuadruplo instruccion = instrucciones[i];
                if (instruccion.Operador == "LABEL") lideres.Add(i);
                if (instruccion.Operador is "GOTO" or "IF_FALSE" or "IF_TRUE" or "RETURN")
                {
                    if (i + 1 < instrucciones.Count) lideres.Add(i + 1);
                    if (instruccion.Operador is "GOTO" or "IF_FALSE" or "IF_TRUE")
                        lideres.Add(indicesEtiquetas[instruccion.Resultado]);
                }
            }

            int[] inicios = lideres.ToArray();
            int[] indiceABloque = new int[instrucciones.Count];
            for (int bloqueId = 0; bloqueId < inicios.Length; bloqueId++)
            {
                int fin = bloqueId + 1 < inicios.Length ? inicios[bloqueId + 1] - 1 : instrucciones.Count - 1;
                bloques.Add((inicios[bloqueId], fin, new HashSet<int>(), new HashSet<int>()));
                for (int i = inicios[bloqueId]; i <= fin; i++) indiceABloque[i] = bloqueId;
            }

            for (int bloqueId = 0; bloqueId < bloques.Count; bloqueId++)
            {
                var bloque = bloques[bloqueId];
                Cuadruplo ultima = instrucciones[bloque.Fin];
                if (ultima.Operador is "GOTO" or "IF_FALSE" or "IF_TRUE")
                    AgregarArista(indiceABloque[indicesEtiquetas[ultima.Resultado]], bloqueId, bloques);
                if (ultima.Operador is not "GOTO" and not "RETURN" && bloqueId + 1 < bloques.Count)
                    AgregarArista(bloqueId + 1, bloqueId, bloques);
            }

            return bloques;
        }

        private static void AgregarArista(
            int sucesor,
            int predecesor,
            List<(int Inicio, int Fin, HashSet<int> Predecesores, HashSet<int> Sucesores)> bloques)
        {
            bloques[predecesor].Sucesores.Add(sucesor);
            bloques[sucesor].Predecesores.Add(predecesor);
        }

        private void OptimizarConstantesLocales(List<Cuadruplo> instrucciones)
        {
            var bloques = ConstruirBloques(instrucciones);

            foreach (var bloque in bloques)
            {
                Dictionary<string, string> constantes = new(StringComparer.Ordinal);
                Dictionary<string, string> expresiones = new(StringComparer.Ordinal);
                for (int i = bloque.Inicio; i <= bloque.Fin; i++)
                {
                    Cuadruplo instruccion = instrucciones[i];
                    int numero = instruccion.Numero;
                    string operadorOriginal = instruccion.Operador;
                    string argumento1Original = instruccion.Argumento1;
                    string argumento2Original = instruccion.Argumento2;
                    if (UsaArgumento1(instruccion.Operador) && constantes.TryGetValue(instruccion.Argumento1, out string? constante1))
                        instruccion.Argumento1 = constante1;
                    if (UsaArgumento2(instruccion.Operador) && constantes.TryGetValue(instruccion.Argumento2, out string? constante2))
                        instruccion.Argumento2 = constante2;

                    if (EsOperacionAritmetica(instruccion.Operador) &&
                        TryPlegarConstantes(instruccion, out string valorPlegado))
                    {
                        string argumento1Plegado = instruccion.Argumento1;
                        string argumento2Plegado = instruccion.Argumento2;
                        instruccion.Operador = "=";
                        instruccion.Argumento1 = valorPlegado;
                        instruccion.Argumento2 = string.Empty;
                        constantes[instruccion.Resultado] = valorPlegado;
                        LogOptimizacion.Add($"Se aplicó Plegado de Constantes en cuádruplo {numero}: {argumento1Plegado} {operadorOriginal} {argumento2Plegado} = {valorPlegado}.");
                        continue;
                    }

                    if (AplicarIdentidadAlgebraica(instruccion))
                    {
                        constantes.Remove(instruccion.Resultado);
                        LogOptimizacion.Add($"Se aplicó Simplificación algebraica en cuádruplo {numero}: {operadorOriginal} {argumento1Original} {argumento2Original} → = {instruccion.Argumento1}.");
                    }
                    else if (EsOperacionAritmetica(instruccion.Operador))
                    {
                        string clave = ConstruirClaveExpresion(instruccion);
                        if (expresiones.TryGetValue(clave, out string? temporal))
                        {
                            instruccion.Operador = "=";
                            instruccion.Argumento1 = temporal;
                            instruccion.Argumento2 = string.Empty;
                            LogOptimizacion.Add($"Se aplicó Eliminación de Subexpresión Común en cuádruplo {numero}: {operadorOriginal} {argumento1Original} {argumento2Original} reutiliza {temporal}.");
                        }
                        else
                        {
                            expresiones[clave] = instruccion.Resultado;
                        }
                    }

                    if (instruccion.Operador == "READ")
                    {
                        constantes.Remove(instruccion.Argumento1);
                    }
                    else if (instruccion.Operador == "=")
                    {
                        if (EsLiteral(instruccion.Argumento1)) constantes[instruccion.Resultado] = instruccion.Argumento1;
                        else constantes.Remove(instruccion.Resultado);
                    }
                    else if (DefineResultado(instruccion.Operador))
                    {
                        constantes.Remove(instruccion.Resultado);
                    }

                    foreach (string definicion in ObtenerDefiniciones(instruccion))
                    {
                        foreach (string clave in expresiones.Keys
                            .Where(clave => clave.Contains($"|{definicion}|", StringComparison.Ordinal))
                            .ToList())
                            expresiones.Remove(clave);
                    }
                }
            }
        }

        private static bool TryPlegarConstantes(Cuadruplo instruccion, out string resultado)
        {
            resultado = string.Empty;
            if (instruccion.Operador is not "+" and not "-" and not "*" and not "/" ||
                !decimal.TryParse(instruccion.Argumento1, NumberStyles.Float, CultureInfo.InvariantCulture, out decimal izquierda) ||
                !decimal.TryParse(instruccion.Argumento2, NumberStyles.Float, CultureInfo.InvariantCulture, out decimal derecha))
                return false;
            if (instruccion.Operador == "/" && derecha == 0)
                return false;

            try
            {
                decimal valor = instruccion.Operador switch
                {
                    "+" => izquierda + derecha,
                    "-" => izquierda - derecha,
                    "*" => izquierda * derecha,
                    "/" when derecha != 0 => izquierda / derecha,
                    _ => throw new InvalidOperationException()
                };
                resultado = valor.ToString(CultureInfo.InvariantCulture);
                return true;
            }
            catch (OverflowException)
            {
                return false;
            }
            catch (DivideByZeroException)
            {
                return false;
            }
        }

        private void AplicarOptimizacionDeBucles(List<Cuadruplo> instrucciones)
        {
            for (int saltoIndice = instrucciones.Count - 1; saltoIndice >= 0; saltoIndice--)
            {
                Cuadruplo salto = instrucciones[saltoIndice];
                if (salto.Operador != "GOTO") continue;
                int inicio = instrucciones.FindIndex(c => c.Operador == "LABEL" && c.Resultado == salto.Resultado);
                if (inicio < 0 || inicio >= saltoIndice) continue;

                bool entradaExterna = instrucciones
                    .Select((c, i) => (Cuadruplo: c, Indice: i))
                    .Any(x => x.Indice != saltoIndice &&
                        (x.Cuadruplo.Operador is "GOTO" or "IF_FALSE" or "IF_TRUE") &&
                        x.Cuadruplo.Resultado == salto.Resultado);
                if (entradaExterna) continue;

                HashSet<string> etiquetasCuerpo = instrucciones
                    .Skip(inicio + 1).Take(saltoIndice - inicio - 1)
                    .Where(c => c.Operador is "IF_FALSE" or "IF_TRUE")
                    .Select(c => c.Resultado)
                    .ToHashSet(StringComparer.Ordinal);
                int etiquetaCuerpo = instrucciones.FindIndex(inicio + 1, saltoIndice - inicio - 1,
                    c => c.Operador == "LABEL" && etiquetasCuerpo.Contains(c.Resultado));
                if (etiquetaCuerpo < 0) continue;

                HashSet<string> definidasEnBucle = instrucciones
                    .Skip(inicio + 1).Take(saltoIndice - inicio - 1)
                    .SelectMany(ObtenerDefiniciones)
                    .ToHashSet(StringComparer.Ordinal);

                for (int i = etiquetaCuerpo + 1; i < saltoIndice; i++)
                {
                    Cuadruplo candidato = instrucciones[i];
                    if (candidato.Operador is not "+" and not "-" and not "*") break;
                    string destino = candidato.Resultado;
                    if (definidasEnBucle.Contains(candidato.Argumento1) || definidasEnBucle.Contains(candidato.Argumento2)) break;

                    int definicionesDestino = instrucciones
                        .Skip(inicio + 1).Take(saltoIndice - inicio - 1)
                        .SelectMany(ObtenerDefiniciones)
                        .Count(definicion => definicion == destino);
                    bool resultadoUsadoAntes = instrucciones
                        .Select((c, indice) => (Cuadruplo: c, Indice: indice))
                        .Any(x => x.Indice > inicio && x.Indice < i && ObtenerUsos(x.Cuadruplo).Contains(destino, StringComparer.Ordinal));
                    if (definicionesDestino != 1 || resultadoUsadoAntes) break;

                    bool resultadoUsadoEnBucle = instrucciones
                        .Select((c, indice) => (Cuadruplo: c, Indice: indice))
                        .Any(x => x.Indice > i && x.Indice < saltoIndice && ObtenerUsos(x.Cuadruplo).Contains(destino, StringComparer.Ordinal));
                    bool resultadoUsadoFuera = instrucciones
                        .Select((c, indice) => (Cuadruplo: c, Indice: indice))
                        .Any(x => (x.Indice < inicio || x.Indice > saltoIndice) && ObtenerUsos(x.Cuadruplo).Contains(destino, StringComparer.Ordinal));
                    if (!resultadoUsadoEnBucle || resultadoUsadoFuera) continue;

                    int numero = candidato.Numero;
                    string expresion = $"{candidato.Argumento1} {candidato.Operador} {candidato.Argumento2}";
                    instrucciones.RemoveAt(i);
                    instrucciones.Insert(inicio, candidato);
                    LogOptimizacion.Add($"Se sacó operación invariante del bucle {salto.Resultado} en cuádruplo {numero}: {expresion} → {destino}.");
                    saltoIndice--;
                    break;
                }
            }
        }

        private void AplicarOptimizacionGlobal(List<Cuadruplo> instrucciones)
        {
            var bloques = ConstruirBloques(instrucciones);

            foreach (var bloque in bloques)
            {
                Dictionary<string, string> copias = new(StringComparer.Ordinal);

                for (int i = bloque.Inicio; i <= bloque.Fin; i++)
                {
                    Cuadruplo instruccion = instrucciones[i];
                    int numero = instruccion.Numero;

                    if (UsaArgumento1(instruccion.Operador) && copias.TryGetValue(instruccion.Argumento1, out string? copia1))
                    {
                        string original = instruccion.Argumento1;
                        instruccion.Argumento1 = copia1;
                        LogOptimizacion.Add($"Se aplicó Propagación de Copias en cuádruplo {numero}: {original} → {copia1}.");
                    }
                    if (UsaArgumento2(instruccion.Operador) && copias.TryGetValue(instruccion.Argumento2, out string? copia2))
                    {
                        string original = instruccion.Argumento2;
                        instruccion.Argumento2 = copia2;
                        LogOptimizacion.Add($"Se aplicó Propagación de Copias en cuádruplo {numero}: {original} → {copia2}.");
                    }

                    List<string> definiciones = ObtenerDefiniciones(instruccion).ToList();
                    foreach (string definicion in definiciones)
                    {
                        copias.Remove(definicion);
                        foreach (string alias in copias
                            .Where(copia => copia.Value == definicion)
                            .Select(copia => copia.Key)
                            .ToList())
                            copias.Remove(alias);
                    }

                    if (instruccion.Operador == "=")
                    {
                        if ((EsIdentificador(instruccion.Argumento1) || EsTemporal(instruccion.Argumento1)) &&
                                 instruccion.Argumento1 != instruccion.Resultado)
                            copias[instruccion.Resultado] = instruccion.Argumento1;
                    }
                }
            }
        }

        private void EliminarTemporalesMuertos(List<Cuadruplo> instrucciones)
        {
            var bloques = ConstruirBloques(instrucciones);
            CalcularVariablesVivas(instrucciones, bloques, out _, out HashSet<string>[] vivasSalida);
            HashSet<int> indicesEliminar = new();

            for (int bloqueId = 0; bloqueId < bloques.Count; bloqueId++)
            {
                var bloque = bloques[bloqueId];
                HashSet<string> vivas = new(vivasSalida[bloqueId], StringComparer.Ordinal);
                for (int i = bloque.Fin; i >= bloque.Inicio; i--)
                {
                    Cuadruplo instruccion = instrucciones[i];
                    if (EsTemporal(instruccion.Resultado) &&
                        (instruccion.Operador is "=" or "+" or "-" or "*" or ">" or "<" or ">=" or "<=" or "==" or "!=" or "AND" or "OR") &&
                        !vivas.Contains(instruccion.Resultado))
                    {
                        indicesEliminar.Add(i);
                        LogOptimizacion.Add($"Se aplicó Eliminación de Código Muerto en cuádruplo {instruccion.Numero}: {instruccion.Operador} {instruccion.Argumento1} {instruccion.Argumento2} → {instruccion.Resultado} no tiene usos.");
                        continue;
                    }

                    ActualizarVivas(instruccion, vivas);
                }
            }

            if (indicesEliminar.Count > 0)
            {
                List<Cuadruplo> conservadas = instrucciones.Where((_, indice) => !indicesEliminar.Contains(indice)).ToList();
                instrucciones.Clear();
                instrucciones.AddRange(conservadas);
            }
        }

        private static void CalcularExpresionesDisponibles(
            IReadOnlyList<Cuadruplo> instrucciones,
            List<(int Inicio, int Fin, HashSet<int> Predecesores, HashSet<int> Sucesores)> bloques,
            out HashSet<string>[] disponiblesEntrada,
            out HashSet<string>[] disponiblesSalida)
        {
            disponiblesEntrada = Enumerable.Range(0, bloques.Count).Select(_ => new HashSet<string>(StringComparer.Ordinal)).ToArray();
            disponiblesSalida = Enumerable.Range(0, bloques.Count).Select(_ => new HashSet<string>(StringComparer.Ordinal)).ToArray();

            bool cambio;
            do
            {
                cambio = false;
                for (int bloqueId = 0; bloqueId < bloques.Count; bloqueId++)
                {
                    HashSet<string>? entrada;
                    if (bloques[bloqueId].Predecesores.Count == 0)
                        entrada = new HashSet<string>(StringComparer.Ordinal);
                    else
                    {
                        entrada = null;
                        foreach (int predecesor in bloques[bloqueId].Predecesores)
                        {
                            if (entrada == null) entrada = new HashSet<string>(disponiblesSalida[predecesor], StringComparer.Ordinal);
                            else entrada.IntersectWith(disponiblesSalida[predecesor]);
                        }
                        entrada ??= new HashSet<string>(StringComparer.Ordinal);
                    }

                    HashSet<string> salida = TransferirExpresionesDisponibles(entrada, instrucciones, bloques[bloqueId]);
                    if (!entrada.SetEquals(disponiblesEntrada[bloqueId]) || !salida.SetEquals(disponiblesSalida[bloqueId]))
                    {
                        disponiblesEntrada[bloqueId] = entrada;
                        disponiblesSalida[bloqueId] = salida;
                        cambio = true;
                    }
                }
            } while (cambio);
        }

        private static HashSet<string> TransferirExpresionesDisponibles(
            HashSet<string> entrada,
            IReadOnlyList<Cuadruplo> instrucciones,
            (int Inicio, int Fin, HashSet<int> Predecesores, HashSet<int> Sucesores) bloque)
        {
            HashSet<string> salida = new(entrada, StringComparer.Ordinal);
            for (int i = bloque.Inicio; i <= bloque.Fin; i++)
            {
                Cuadruplo instruccion = instrucciones[i];
                if (EsOperacionAritmetica(instruccion.Operador) || instruccion.Operador is ">" or "<" or ">=" or "<=")
                    salida.Add(ConstruirClaveExpresion(instruccion));

                foreach (string definicion in ObtenerDefiniciones(instruccion))
                {
                    foreach (string expresion in salida
                        .Where(expresion => expresion.Contains($"|{definicion}|", StringComparison.Ordinal))
                        .ToList())
                        salida.Remove(expresion);
                }
            }
            return salida;
        }

        private static void CalcularVariablesVivas(
            IReadOnlyList<Cuadruplo> instrucciones,
            List<(int Inicio, int Fin, HashSet<int> Predecesores, HashSet<int> Sucesores)> bloques,
            out HashSet<string>[] vivasEntrada,
            out HashSet<string>[] vivasSalida)
        {
            vivasEntrada = Enumerable.Range(0, bloques.Count).Select(_ => new HashSet<string>(StringComparer.Ordinal)).ToArray();
            vivasSalida = Enumerable.Range(0, bloques.Count).Select(_ => new HashSet<string>(StringComparer.Ordinal)).ToArray();
            HashSet<string>[] usos = Enumerable.Range(0, bloques.Count).Select(_ => new HashSet<string>(StringComparer.Ordinal)).ToArray();
            HashSet<string>[] definiciones = Enumerable.Range(0, bloques.Count).Select(_ => new HashSet<string>(StringComparer.Ordinal)).ToArray();

            for (int bloqueId = 0; bloqueId < bloques.Count; bloqueId++)
            {
                var bloque = bloques[bloqueId];
                for (int i = bloque.Inicio; i <= bloque.Fin; i++)
                {
                    foreach (string uso in ObtenerUsos(instrucciones[i]))
                        if (!definiciones[bloqueId].Contains(uso)) usos[bloqueId].Add(uso);
                    foreach (string definicion in ObtenerDefiniciones(instrucciones[i]))
                        definiciones[bloqueId].Add(definicion);
                }
            }

            bool cambio;
            do
            {
                cambio = false;
                for (int bloqueId = bloques.Count - 1; bloqueId >= 0; bloqueId--)
                {
                    HashSet<string> salida = new(StringComparer.Ordinal);
                    foreach (int sucesor in bloques[bloqueId].Sucesores) salida.UnionWith(vivasEntrada[sucesor]);
                    HashSet<string> entrada = new(usos[bloqueId], StringComparer.Ordinal);
                    entrada.UnionWith(salida.Except(definiciones[bloqueId]));

                    if (!salida.SetEquals(vivasSalida[bloqueId]) || !entrada.SetEquals(vivasEntrada[bloqueId]))
                    {
                        vivasSalida[bloqueId] = salida;
                        vivasEntrada[bloqueId] = entrada;
                        cambio = true;
                    }
                }
            } while (cambio);
        }

        private static IEnumerable<string> ObtenerUsos(Cuadruplo instruccion)
        {
            if (UsaArgumento1(instruccion.Operador) && !string.IsNullOrEmpty(instruccion.Argumento1))
                yield return instruccion.Argumento1;
            if (UsaArgumento2(instruccion.Operador) && !string.IsNullOrEmpty(instruccion.Argumento2))
                yield return instruccion.Argumento2;
            if (instruccion.Operador == "LOCATE")
            {
                foreach (string argumento in instruccion.Argumento1.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                    yield return argumento;
            }
        }

        private static IEnumerable<string> ObtenerDefiniciones(Cuadruplo instruccion)
        {
            if (instruccion.Operador == "READ" && !string.IsNullOrEmpty(instruccion.Argumento1))
                yield return instruccion.Argumento1;
            else if (DefineResultado(instruccion.Operador) && !string.IsNullOrEmpty(instruccion.Resultado))
                yield return instruccion.Resultado;
        }

        private static void ActualizarVivas(Cuadruplo instruccion, HashSet<string> vivas)
        {
            foreach (string definicion in ObtenerDefiniciones(instruccion)) vivas.Remove(definicion);
            foreach (string uso in ObtenerUsos(instruccion)) vivas.Add(uso);
        }

        private static bool UsaArgumento1(string operador) =>
            operador is "=" or "NEG" or "POS" or "NOT" or "+" or "-" or "*" or "/" or "%" or "^" or ">" or "<" or ">=" or "<=" or "==" or "!=" or "AND" or "OR" or "IF_FALSE" or "IF_TRUE" or "PRINT" or "RETURN";

        private static bool UsaArgumento2(string operador) =>
            operador is "+" or "-" or "*" or "/" or "%" or "^" or ">" or "<" or ">=" or "<=" or "==" or "!=" or "AND" or "OR";

        private static bool DefineResultado(string operador) =>
            operador is "=" or "NEG" or "POS" or "NOT" or "+" or "-" or "*" or "/" or "%" or "^" or ">" or "<" or ">=" or "<=" or "==" or "!=" or "AND" or "OR";

        private static bool EsOperacionAritmetica(string operador) => operador is "+" or "-" or "*" or "/";

        private static string ConstruirClaveExpresion(Cuadruplo instruccion)
        {
            string izquierda = instruccion.Argumento1;
            string derecha = instruccion.Argumento2;
            if (instruccion.Operador == "*")
                (izquierda, derecha) = string.CompareOrdinal(izquierda, derecha) <= 0
                    ? (izquierda, derecha)
                    : (derecha, izquierda);
            return $"{instruccion.Operador}|{izquierda}|{derecha}";
        }

        private static int CalcularCostoEstructural(IReadOnlyCollection<Cuadruplo> instrucciones)
        {
            int costo = 0;
            foreach (Cuadruplo instruccion in instrucciones)
            {
                costo += instruccion.Operador switch
                {
                    "=" or "NEG" or "POS" or "NOT" => 1,
                    "+" or "-" or "*" or "/" or "%" or "^" => 3,
                    ">" or "<" or ">=" or "<=" or "==" or "!=" or "AND" or "OR" => 2,
                    "LABEL" => 0,
                    "GOTO" or "IF_FALSE" or "IF_TRUE" => 2,
                    "READ" => 5,
                    "PRINT" or "RETURN" => 3,
                    "CLEAR" or "LOCATE" => 2,
                    _ => 1
                };
            }
            return costo;
        }

        private static int ContarTemporales(IReadOnlyCollection<Cuadruplo> instrucciones) =>
            instrucciones.Where(i => EsTemporal(i.Resultado)).Select(i => i.Resultado).Distinct(StringComparer.Ordinal).Count();

        private static int EstimarMemoria(IReadOnlyCollection<Cuadruplo> instrucciones)
        {
            HashSet<string> variables = new(StringComparer.Ordinal);
            foreach (Cuadruplo instruccion in instrucciones)
            {
                if (EsIdentificador(instruccion.Resultado)) variables.Add(instruccion.Resultado);
                if (EsIdentificador(instruccion.Argumento1)) variables.Add(instruccion.Argumento1);
                if (EsIdentificador(instruccion.Argumento2)) variables.Add(instruccion.Argumento2);
            }
            return variables.Count * 8;
        }

        private static int EstimarPicoPila(IReadOnlyCollection<Cuadruplo> instrucciones)
        {
            int actual = 0;
            int pico = 0;
            foreach (Cuadruplo instruccion in instrucciones)
            {
                if (EsOperacionAritmetica(instruccion.Operador) || instruccion.Operador is "NEG" or "POS" or "NOT")
                {
                    actual++;
                    pico = Math.Max(pico, actual);
                    if (EsTemporal(instruccion.Resultado)) actual = Math.Max(0, actual - 1);
                }
                else if (instruccion.Operador == "=")
                {
                    actual = Math.Max(0, actual - 1);
                }
            }
            return pico;
        }

        private void AplicarOptimizacionDeMirilla(List<Cuadruplo> instrucciones)
        {
            bool cambio;
            do
            {
                cambio = false;
                for (int i = 0; i < instrucciones.Count; i++)
                {
                    Cuadruplo instruccion = instrucciones[i];

                    if (instruccion.Operador is "IF_FALSE" or "IF_TRUE")
                    {
                        bool constanteEncontrada = bool.TryParse(instruccion.Argumento1, out bool constante);
                        bool enteroEncontrado = int.TryParse(instruccion.Argumento1, NumberStyles.Integer, CultureInfo.InvariantCulture, out int entero);
                        bool realEncontrado = double.TryParse(instruccion.Argumento1, NumberStyles.Float, CultureInfo.InvariantCulture, out double real);
                        if (constanteEncontrada || enteroEncontrado || realEncontrado)
                        {
                            bool valor = constanteEncontrada ? constante : enteroEncontrado ? entero != 0 : real != 0.0;
                            bool saltar = instruccion.Operador == "IF_TRUE" ? valor : !valor;
                            int numero = instruccion.Numero;
                            string condicion = instruccion.Argumento1;
                            string destino = instruccion.Resultado;
                            if (saltar)
                            {
                                instruccion.Operador = "GOTO";
                                instruccion.Argumento1 = string.Empty;
                                LogOptimizacion.Add($"Se aplicó Simplificación de salto en cuádruplo {numero}: condición constante {condicion}; se conservó el salto a {destino}.");
                            }
                            else
                            {
                                instrucciones.RemoveAt(i);
                                i--;
                                LogOptimizacion.Add($"Se aplicó Eliminación de salto en cuádruplo {numero}: condición constante {condicion}; el salto a {destino} nunca se ejecuta.");
                            }
                            cambio = true;
                            continue;
                        }
                    }

                    if (instruccion.Operador is "GOTO" or "RETURN")
                    {
                        int siguienteEtiqueta = i + 1;
                        while (siguienteEtiqueta < instrucciones.Count && instrucciones[siguienteEtiqueta].Operador != "LABEL")
                            siguienteEtiqueta++;

                        if (siguienteEtiqueta > i + 1)
                        {
                            for (int muerto = i + 1; muerto < siguienteEtiqueta; muerto++)
                                LogOptimizacion.Add($"Se aplicó Eliminación de Código Inalcanzable en cuádruplo {instrucciones[muerto].Numero}: {instrucciones[muerto].Operador} {instrucciones[muerto].Argumento1} {instrucciones[muerto].Argumento2} → {instrucciones[muerto].Resultado}.");
                            instrucciones.RemoveRange(i + 1, siguienteEtiqueta - i - 1);
                            cambio = true;
                        }

                        if (instruccion.Operador == "GOTO" && i + 1 < instrucciones.Count &&
                            instrucciones[i + 1].Operador == "LABEL" && instrucciones[i + 1].Resultado == instruccion.Resultado)
                        {
                            LogOptimizacion.Add($"Se aplicó Eliminación de Salto Redundante en cuádruplo {instruccion.Numero}: GOTO {instruccion.Resultado} seguido de LABEL {instruccion.Resultado}.");
                            instrucciones.RemoveAt(i);
                            cambio = true;
                            i--;
                        }
                    }
                }
            } while (cambio);
        }

        private static bool AplicarIdentidadAlgebraica(Cuadruplo instruccion)
        {
            if (instruccion.Operador == "+")
            {
                if (instruccion.Argumento1 == "0") { instruccion.Operador = "="; instruccion.Argumento1 = instruccion.Argumento2; instruccion.Argumento2 = string.Empty; return true; }
                if (instruccion.Argumento2 == "0") { instruccion.Operador = "="; instruccion.Argumento2 = string.Empty; return true; }
            }
            if (instruccion.Operador == "-")
            {
                if (instruccion.Argumento2 == "0") { instruccion.Operador = "="; instruccion.Argumento2 = string.Empty; return true; }
                if (instruccion.Argumento1 == instruccion.Argumento2) { instruccion.Operador = "="; instruccion.Argumento1 = "0"; instruccion.Argumento2 = string.Empty; return true; }
            }
            if (instruccion.Operador == "*")
            {
                if (instruccion.Argumento1 == "1") { instruccion.Operador = "="; instruccion.Argumento1 = instruccion.Argumento2; instruccion.Argumento2 = string.Empty; return true; }
                if (instruccion.Argumento2 == "1") { instruccion.Operador = "="; instruccion.Argumento2 = string.Empty; return true; }
                if (instruccion.Argumento1 == "0" || instruccion.Argumento2 == "0") { instruccion.Operador = "="; instruccion.Argumento1 = "0"; instruccion.Argumento2 = string.Empty; return true; }
            }
            if (instruccion.Operador == "/" && instruccion.Argumento2 == "1")
            {
                instruccion.Operador = "=";
                instruccion.Argumento2 = string.Empty;
                return true;
            }
            return false;
        }

        private static void ValidarOperadores(IReadOnlyCollection<Cuadruplo> instrucciones)
        {
            HashSet<string> operadoresSoportados = new(StringComparer.Ordinal)
            {
                "=", "NEG", "NOT", "POS", "+", "-", "*", "/", "%", "^",
                ">", "<", ">=", "<=", "==", "!=", "AND", "OR",
                "LABEL", "GOTO", "IF_FALSE", "IF_TRUE", "READ", "PRINT", "RETURN", "CLEAR", "LOCATE"
            };

            foreach (Cuadruplo instruccion in instrucciones)
                if (!operadoresSoportados.Contains(instruccion.Operador))
                    throw new InvalidOperationException($"El IR contiene la instrucción no soportada '{instruccion.Operador}'.");
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

            foreach (Cuadruplo instruccion in instrucciones.Where(c => c.Operador is "GOTO" or "IF_FALSE" or "IF_TRUE"))
                if (!etiquetas.Contains(instruccion.Resultado))
                    throw new InvalidOperationException($"El salto {instruccion.Operador} apunta a la etiqueta inexistente '{instruccion.Resultado}'.");
        }

        private static bool EsNumero(string texto) =>
            Regex.IsMatch(texto, @"^(?:\d+(?:\.\d+)?|\.\d+)$");
    }
}