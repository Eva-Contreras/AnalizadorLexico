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
        public List<Cuadruplo> Optimizar(IReadOnlyCollection<Cuadruplo> codigo)
        {
            ArgumentNullException.ThrowIfNull(codigo);
            List<Cuadruplo> optimizado = codigo.Select(ClonarCuadruplo).ToList();
            ValidarOperadores(optimizado);
            ValidarEtiquetas(optimizado);

            _informeOptimizacion.Clear();
            InstruccionesAntesOptimizacion = optimizado.Count;
            BloquesBasicos = ConstruirBloques(optimizado).Count;
            CostoAntesOptimizacion = CalcularCostoEstructural(optimizado);
            TemporalesAntesOptimizacion = ContarTemporales(optimizado);
            PilaEstimadaAntesOptimizacion = EstimarPicoPila(optimizado);
            RegistrosEstimadosAntesOptimizacion = ContarTemporales(optimizado);
            MemoriaEstimadaAntesOptimizacion = EstimarMemoria(optimizado);

            OptimizarConstantesLocales(optimizado);
            AplicarOptimizacionDeBucles(optimizado);
            AplicarOptimizacionGlobal(optimizado);
            EliminarTemporalesMuertos(optimizado);
            AplicarOptimizacionDeMirilla(optimizado);
            RenumerarCuadruplos(optimizado);

            ValidarOperadores(optimizado);
            ValidarEtiquetas(optimizado);
            InstruccionesDespuesOptimizacion = optimizado.Count;
            CostoDespuesOptimizacion = CalcularCostoEstructural(optimizado);
            TemporalesDespuesOptimizacion = ContarTemporales(optimizado);
            PilaEstimadaDespuesOptimizacion = EstimarPicoPila(optimizado);
            RegistrosEstimadosDespuesOptimizacion = ContarTemporales(optimizado);
            MemoriaEstimadaDespuesOptimizacion = EstimarMemoria(optimizado);
            _informeOptimizacion.Add($"Bloques básicos analizados: {BloquesBasicos}.");
            _informeOptimizacion.Add($"Instrucciones IR: {InstruccionesAntesOptimizacion} → {InstruccionesDespuesOptimizacion}.");
            _informeOptimizacion.Add($"Temporales distintos: {TemporalesAntesOptimizacion} → {TemporalesDespuesOptimizacion}.");
            _informeOptimizacion.Add($"Costo estructural: {CostoAntesOptimizacion} → {CostoDespuesOptimizacion}; pila: {PilaEstimadaAntesOptimizacion} → {PilaEstimadaDespuesOptimizacion}; registros: {RegistrosEstimadosAntesOptimizacion} → {RegistrosEstimadosDespuesOptimizacion}; memoria: {MemoriaEstimadaAntesOptimizacion} → {MemoriaEstimadaDespuesOptimizacion} bytes.");
            _informeOptimizacion.Add("Costo: estimación por pesos de IR, no medición real de registros, memoria, pila o tiempo de máquina.");
            return optimizado;
        }

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
            bool aplicoRegla = false;

            foreach (var bloque in bloques)
            {
                Dictionary<string, string> constantes = new(StringComparer.Ordinal);
                for (int i = bloque.Inicio; i <= bloque.Fin; i++)
                {
                    Cuadruplo instruccion = instrucciones[i];
                    if (UsaArgumento1(instruccion.Operador) && constantes.TryGetValue(instruccion.Argumento1, out string? constante1))
                        instruccion.Argumento1 = constante1;
                    if (UsaArgumento2(instruccion.Operador) && constantes.TryGetValue(instruccion.Argumento2, out string? constante2))
                        instruccion.Argumento2 = constante2;

                    if (EsOperacionAritmetica(instruccion.Operador) &&
                        TryPlegarConstantes(instruccion, out string valorPlegado))
                    {
                        instruccion.Operador = "=";
                        instruccion.Argumento1 = valorPlegado;
                        instruccion.Argumento2 = string.Empty;
                        constantes[instruccion.Resultado] = valorPlegado;
                        aplicoRegla = true;
                        continue;
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
                }
            }

            if (aplicoRegla) _informeOptimizacion.Add("Local: plegado de expresiones aritméticas constantes.");
        }

        private static bool TryPlegarConstantes(Cuadruplo instruccion, out string resultado)
        {
            resultado = string.Empty;
            if (!double.TryParse(instruccion.Argumento1, NumberStyles.Float, CultureInfo.InvariantCulture, out double izquierda) ||
                !double.TryParse(instruccion.Argumento2, NumberStyles.Float, CultureInfo.InvariantCulture, out double derecha))
                return false;

            double valor;
            switch (instruccion.Operador)
            {
                case "+": valor = izquierda + derecha; break;
                case "-": valor = izquierda - derecha; break;
                case "*": valor = izquierda * derecha; break;
                case "/":
                    if (derecha == 0) return false;
                    valor = izquierda / derecha;
                    break;
                default: return false;
            }

            if (!double.IsFinite(valor)) return false;
            resultado = valor.ToString("G", CultureInfo.InvariantCulture);
            return true;
        }

        private void AplicarOptimizacionDeBucles(List<Cuadruplo> instrucciones)
        {
            int movimientos = 0;
            bool huboMovimiento;
            do
            {
                huboMovimiento = false;
                Dictionary<string, int> indicesEtiquetas = instrucciones
                    .Select((instruccion, indice) => (instruccion, indice))
                    .Where(x => x.instruccion.Operador == "LABEL")
                    .ToDictionary(x => x.instruccion.Resultado, x => x.indice, StringComparer.Ordinal);

                for (int fin = 0; fin < instrucciones.Count; fin++)
                {
                    Cuadruplo salto = instrucciones[fin];
                    if (salto.Operador != "GOTO" || !indicesEtiquetas.TryGetValue(salto.Resultado, out int inicio) || inicio >= fin)
                        continue;

                    int saltosAlEncabezado = instrucciones.Count(c =>
                        (c.Operador is "GOTO" or "IF_FALSE" or "IF_TRUE") && c.Resultado == salto.Resultado);
                    if (saltosAlEncabezado != 1) continue;

                    HashSet<string> modificadas = new(StringComparer.Ordinal);
                    for (int i = inicio + 1; i < fin; i++)
                    {
                        Cuadruplo instruccion = instrucciones[i];
                        if (instruccion.Operador == "=" && !EsTemporal(instruccion.Resultado))
                            modificadas.Add(instruccion.Resultado);
                        else if (instruccion.Operador == "READ")
                            modificadas.Add(instruccion.Argumento1);
                    }

                    for (int i = inicio + 1; i < fin; i++)
                    {
                        Cuadruplo candidato = instrucciones[i];
                        if (candidato.Operador is not "-" and not "*" || !EsTemporal(candidato.Resultado))
                            continue;
                        if ((!EsNumero(candidato.Argumento1) && !EsIdentificador(candidato.Argumento1)) ||
                            (!EsNumero(candidato.Argumento2) && !EsIdentificador(candidato.Argumento2)))
                            continue;
                        if ((EsIdentificador(candidato.Argumento1) && modificadas.Contains(candidato.Argumento1)) ||
                            (EsIdentificador(candidato.Argumento2) && modificadas.Contains(candidato.Argumento2)))
                            continue;
                        if (instrucciones.Count(instruccion =>
                                ObtenerDefiniciones(instruccion).Contains(candidato.Resultado)) != 1)
                            continue;

                        int bloqueInicio = inicio + 1;
                        for (int j = inicio + 1; j < i; j++)
                            if (instrucciones[j].Operador == "LABEL") bloqueInicio = j + 1;
                        if (Enumerable.Range(bloqueInicio, i - bloqueInicio).Any(j =>
                                instrucciones[j].Operador is "GOTO" or "IF_FALSE" or "IF_TRUE" or "READ" or "PRINT" or "RETURN" or "CLEAR" or "LOCATE"))
                            continue;

                        bool usadoDentroDelCiclo = false;
                        bool usadoFueraDelCiclo = false;
                        for (int j = 0; j < instrucciones.Count; j++)
                        {
                            if (!ObtenerUsos(instrucciones[j]).Contains(candidato.Resultado)) continue;
                            if (j > i && j < fin) usadoDentroDelCiclo = true;
                            else if (j <= inicio || j >= fin) usadoFueraDelCiclo = true;
                        }
                        if (!usadoDentroDelCiclo || usadoFueraDelCiclo) continue;

                        instrucciones.RemoveAt(i);
                        instrucciones.Insert(inicio, candidato);
                        movimientos++;
                        huboMovimiento = true;
                        break;
                    }

                    if (huboMovimiento) break;
                }
            } while (huboMovimiento);

            if (movimientos > 0)
                _informeOptimizacion.Add($"Ciclos: se movieron {movimientos} expresiones invariantes seguras fuera de ciclos.");
        }

        private void AplicarOptimizacionGlobal(List<Cuadruplo> instrucciones)
        {
            var bloques = ConstruirBloques(instrucciones);
            int sustituciones = 0;
            int expresionesEliminadas = 0;

            foreach (var bloque in bloques)
            {
                Dictionary<string, string> copias = new(StringComparer.Ordinal);
                Dictionary<string, string> expresionesDisponibles = new(StringComparer.Ordinal);

                for (int i = bloque.Inicio; i <= bloque.Fin; i++)
                {
                    Cuadruplo instruccion = instrucciones[i];

                    if (UsaArgumento1(instruccion.Operador) && copias.TryGetValue(instruccion.Argumento1, out string? copia1))
                    {
                        instruccion.Argumento1 = copia1;
                        sustituciones++;
                    }
                    if (UsaArgumento2(instruccion.Operador) && copias.TryGetValue(instruccion.Argumento2, out string? copia2))
                    {
                        instruccion.Argumento2 = copia2;
                        sustituciones++;
                    }

                    if (EsOperacionAritmetica(instruccion.Operador))
                    {
                        string clave = ConstruirClaveExpresion(instruccion);
                        if (expresionesDisponibles.TryGetValue(clave, out string? temporalPrevio))
                        {
                            instruccion.Operador = "=";
                            instruccion.Argumento1 = temporalPrevio;
                            instruccion.Argumento2 = string.Empty;
                            expresionesEliminadas++;
                        }
                        else
                        {
                            expresionesDisponibles[clave] = instruccion.Resultado;
                        }
                    }

                    if (instruccion.Operador == "=")
                    {
                        copias.Remove(instruccion.Resultado);
                        if (!EsTemporal(instruccion.Argumento1) && EsLiteral(instruccion.Argumento1))
                            copias[instruccion.Resultado] = instruccion.Argumento1;
                        else if (EsIdentificador(instruccion.Argumento1) || EsTemporal(instruccion.Argumento1))
                            copias[instruccion.Resultado] = instruccion.Argumento1;
                    }
                    else if (DefineResultado(instruccion.Operador))
                    {
                        copias.Remove(instruccion.Resultado);
                        copias.Remove(instruccion.Resultado);
                    }
                    else if (instruccion.Operador == "READ")
                    {
                        copias.Remove(instruccion.Argumento1);
                    }

                    foreach (string operandoModificado in ObtenerDefiniciones(instruccion))
                    {
                        foreach (string clave in expresionesDisponibles.Keys
                            .Where(clave => clave.Contains($"|{operandoModificado}|", StringComparison.Ordinal))
                            .ToList())
                            expresionesDisponibles.Remove(clave);
                    }
                }
            }

            if (sustituciones > 0) _informeOptimizacion.Add($"Global: propagación de copias en {sustituciones} operandos.");
            if (expresionesEliminadas > 0) _informeOptimizacion.Add($"Global: {expresionesEliminadas} expresiones comunes eliminadas dentro de bloques básicos.");
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
                    if (EsTemporal(instruccion.Resultado) && EsOperacionPuraEliminable(instruccion.Operador) && !vivas.Contains(instruccion.Resultado))
                    {
                        indicesEliminar.Add(i);
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
                _informeOptimizacion.Add($"Global: se eliminaron {indicesEliminar.Count} temporales sin usos vivos.");
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
                    HashSet<string> entrada;
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

        private static bool EsOperacionPuraEliminable(string operador) =>
            operador is "=" or "NEG" or "POS" or "NOT" or "+" or "-" or "*" or "/" or "%" or "^" or ">" or "<" or ">=" or "<=" or "==" or "!=" or "AND" or "OR";

        private static string ConstruirClaveExpresion(Cuadruplo instruccion)
        {
            string izquierda = instruccion.Argumento1;
            string derecha = instruccion.Argumento2;
            if (instruccion.Operador is "+" or "*")
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
            int saltosEliminados = 0;
            int instruccionesInalcanzables = 0;
            int identidadesEliminadas = 0;
            bool cambio;
            do
            {
                cambio = false;
                for (int i = 0; i < instrucciones.Count; i++)
                {
                    Cuadruplo instruccion = instrucciones[i];

                    if (AplicarIdentidadAlgebraica(instruccion))
                    {
                        identidadesEliminadas++;
                        cambio = true;
                    }

                    if (instruccion.Operador is "IF_FALSE" or "IF_TRUE")
                    {
                        bool constanteEncontrada = bool.TryParse(instruccion.Argumento1, out bool constante);
                        bool enteroEncontrado = int.TryParse(instruccion.Argumento1, NumberStyles.Integer, CultureInfo.InvariantCulture, out int entero);
                        bool realEncontrado = double.TryParse(instruccion.Argumento1, NumberStyles.Float, CultureInfo.InvariantCulture, out double real);
                        if (constanteEncontrada || enteroEncontrado || realEncontrado)
                        {
                            bool valor = constanteEncontrada ? constante : enteroEncontrado ? entero != 0 : real != 0.0;
                            bool saltar = instruccion.Operador == "IF_TRUE" ? valor : !valor;
                            if (saltar)
                            {
                                instruccion.Operador = "GOTO";
                                instruccion.Argumento1 = string.Empty;
                            }
                            else
                            {
                                instrucciones.RemoveAt(i);
                                i--;
                            }
                            saltosEliminados++;
                            cambio = true;
                        }
                    }

                    if (instruccion.Operador is "GOTO" or "RETURN")
                    {
                        int siguienteEtiqueta = i + 1;
                        while (siguienteEtiqueta < instrucciones.Count && instrucciones[siguienteEtiqueta].Operador != "LABEL")
                            siguienteEtiqueta++;

                        if (siguienteEtiqueta > i + 1)
                        {
                            instruccionesInalcanzables += siguienteEtiqueta - i - 1;
                            instrucciones.RemoveRange(i + 1, siguienteEtiqueta - i - 1);
                            cambio = true;
                        }

                        if (instruccion.Operador == "GOTO" && i + 1 < instrucciones.Count &&
                            instrucciones[i + 1].Operador == "LABEL" && instrucciones[i + 1].Resultado == instruccion.Resultado)
                        {
                            instrucciones.RemoveAt(i);
                            saltosEliminados++;
                            cambio = true;
                            i--;
                        }
                    }
                }
            } while (cambio);

            if (identidadesEliminadas > 0)
                _informeOptimizacion.Add($"Mirilla: se simplificaron {identidadesEliminadas} identidades algebraicas.");
            if (saltosEliminados > 0 || instruccionesInalcanzables > 0)
                _informeOptimizacion.Add($"Mirilla: {saltosEliminados} saltos simplificados/eliminados y {instruccionesInalcanzables} instrucciones inalcanzables eliminados.");
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