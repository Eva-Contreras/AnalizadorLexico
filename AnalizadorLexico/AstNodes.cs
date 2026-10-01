namespace AnalizadorLexico
{
    public abstract class AstNode { }

    public class ProgramNode : AstNode
    {
        public List<AstNode> Statements { get; } = new List<AstNode>();
    }

    public class DeclarationNode : AstNode
    {
        public string Tipo { get; set; } = ""; // ENT, DEC, CAD
        public string Identificador { get; set; } = "";
        public AstNode? Valor { get; set; }
        // Memory information (assigned by semantic analyzer)
        public int? MemoryAddress { get; set; }
        public int? Size { get; set; }
    }

    public class AssignmentNode : AstNode
    {
        public string Identificador { get; set; } = "";
        public AstNode? Valor { get; set; }
    }

    public class LiteralNode : AstNode
    {
        public string TipoLiteral { get; set; } = ""; // CNU, CNR, CAD
        public string Valor { get; set; } = "";
    }

    public class IdentifierNode : AstNode
    {
        public string Nombre { get; set; } = "";
        // Optional memory address (filled by semantic analyzer when available)
        public int? MemoryAddress { get; set; }
    }
}
