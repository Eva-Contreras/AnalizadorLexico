using System;
using System.Collections.Generic;
using System.Text;

namespace AnalizadorLexico
{
    public class AnalizadorSintactico
    {
        private List<(int linea, string valor, string token)> _tokens = new();
        private int _pos;
        public List<string> Pasos { get; private set; } = new();
        public List<string> Errores { get; private set; } = new();
        public bool EsValido => Errores.Count == 0;
        private (int linea, string valor, string token) TokenActual
            => _pos < _tokens.Count ? _tokens[_pos] : (-1, "EOF", "EOF");
        public bool Analizar(List<(int linea, string valor, string token)> tokens)
        {
            _tokens = tokens.Where(t => t.token != "COM").ToList();
            _pos = 0;
            Pasos.Clear();
            Errores.Clear();

            Pasos.Add("=== INICIO DEL ANÁLISIS SINTÁCTICO ===");

            ParseS();

            if (_pos < _tokens.Count)
            {
                string sobrante = string.Join(" ", _tokens
                    .GetRange(_pos, _tokens.Count - _pos)
                    .Select(t => $"'{t.valor}'({t.token})"));
                Error($"Tokens inesperados al final: {sobrante}");
            }

            Pasos.Add(EsValido
                ? "=== ANÁLISIS COMPLETADO: CADENA ACEPTADA ✔ ==="
                : "=== ANÁLISIS COMPLETADO: CADENA RECHAZADA ✘ ===");

            return EsValido;
        }
        private void ParseS()
        {
            Pasos.Add($"[S] línea {TokenActual.linea} — token: {TokenActual.token} '{TokenActual.valor}'");

            if (TokenActual.token == "PR4")
                ParseSentenciaIf();
            else
                ParseSentenciaGenerica();
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
        private void ParseBloque()
        {
            Pasos.Add("[BLOQUE] Analizando cuerpo del bloque...");

            if (TokenActual.token == "CD2" || TokenActual.token == "EOF")
            {
                Error($"Bloque vacío en línea {TokenActual.linea}, se esperaba al menos una sentencia");
                return;
            }

            while (TokenActual.token != "CD2" && TokenActual.token != "EOF")
                ParseS();
        }
        private void ParseCondic()
        {
            Pasos.Add($"[CONDIC] Determinando tipo de condición — token: {TokenActual.token} '{TokenActual.valor}'");

            int posTemp = _pos;
            AvanzarARG7Lookahead(ref posTemp);
            string opSiguiente = posTemp < _tokens.Count ? _tokens[posTemp].token : "EOF";

            if (EsOL(opSiguiente))
                ParseOPL();
            else if (EsOR(opSiguiente))
                ParseOPR();
            else
                Error($"Se esperaba operador lógico (Y/O/NO) o relacional (>/</==/>=/<=/><) en línea {TokenActual.linea}, se encontró '{TokenActual.valor}'");
        }
        private void ParseOPL()
        {
            Pasos.Add("[OPL] Analizando operación lógica...");

            ParseARG7();

            if (EsOL(TokenActual.token))
            {
                Pasos.Add($"[OL] Operador lógico: '{TokenActual.valor}' ({TokenActual.token})");
                Avanzar();
            }
            else
                Error($"Se esperaba operador lógico (Y/O/NO) en línea {TokenActual.linea}, se encontró '{TokenActual.valor}'");

            ParseARG7();
            Pasos.Add("[OPL] Operación lógica reconocida ✔");
        }
        private void ParseOPR()
        {
            Pasos.Add("[OPR] Analizando operación relacional...");

            ParseARG7();

            if (EsOR(TokenActual.token))
            {
                Pasos.Add($"[OR] Operador relacional: '{TokenActual.valor}' ({TokenActual.token})");
                Avanzar();
            }
            else
                Error($"Se esperaba operador relacional (>/</==/>=/<=/><) en línea {TokenActual.linea}, se encontró '{TokenActual.valor}'");

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

                if (EsOL(opDentro))
                    ParseOPL();
                else if (EsOR(opDentro))
                    ParseOPR();
                else
                    ParseOPA();

                Consumir("CD4", "Se esperaba ')' para cerrar ARG7");
            }
            else if (EsID(TokenActual.token) || EsCN(TokenActual.token))
            {
                int posTemp = _pos + 1;
                bool esOpa = posTemp < _tokens.Count && EsOA(_tokens[posTemp].token);

                if (esOpa)
                    ParseOPA();
                else if (EsID(TokenActual.token))
                    ParseID();
                else
                    ParseCN();
            }
            else
                Error($"Se esperaba identificador, número o expresión en línea {TokenActual.linea}, se encontró '{TokenActual.valor}'");
        }
        private void ParseOPA()
        {
            Pasos.Add("[OPA] Analizando operación aritmética...");

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
                    Pasos.Add($"[OA] Operador aritmético: '{TokenActual.valor}' ({TokenActual.token})");
                    Avanzar();
                    ParseARG4();
                }
            }

            Pasos.Add("[OPA] Operación aritmética reconocida ✔");
        }
        private void ParseARG4()
        {
            Pasos.Add($"[ARG4] token: {TokenActual.token} '{TokenActual.valor}'");

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
            else
                Error($"Se esperaba identificador, número o expresión aritmética en línea {TokenActual.linea}, se encontró '{TokenActual.valor}'");
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
        private void ParseSentenciaGenerica()
        {
            Pasos.Add($"[S_GEN] Consumiendo sentencia genérica en línea {TokenActual.linea}...");

            while (TokenActual.token != "CD5"
                && TokenActual.token != "CD2"
                && TokenActual.token != "EOF")
                Avanzar();

            if (TokenActual.token == "CD5")
                Avanzar(); 
        }
        private void Consumir(string tokenEsperado, string mensajeError)
        {
            if (TokenActual.token == tokenEsperado)
            {
                Pasos.Add($"[✔] '{TokenActual.valor}' ({tokenEsperado}) línea {TokenActual.linea}");
                Avanzar();
            }
            else
                Error($"{mensajeError} — línea {TokenActual.linea}, se encontró '{TokenActual.valor}' ({TokenActual.token})");
        }
        private void Avanzar()
        {
            if (_pos < _tokens.Count) _pos++;
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
