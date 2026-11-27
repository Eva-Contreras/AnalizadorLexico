namespace AnalizadorLexico
{
    public abstract class AstNode { }

    public abstract class ExpressionNode : AstNode { }

    public class ProgramNode : AstNode
    {
        public List<AstNode> Statements { get; } = new();
    }

    public class BlockNode : AstNode
    {
        public List<AstNode> Statements { get; } = new();
    }

    public class FunctionNode : AstNode
    {
        public string Nombre { get; set; } = "";
        public List<FunctionParameterNode> Parametros { get; } = new();
        public BlockNode Cuerpo { get; set; } = new();
    }

    public class FunctionParameterNode
    {
        public string Tipo { get; set; } = "";
        public string Nombre { get; set; } = "";
    }

    public class CallNode : ExpressionNode
    {
        public string NombreFuncion { get; set; } = "";
        public List<ExpressionNode> Argumentos { get; } = new();
    }

    public class DeclarationNode : AstNode
    {
        public string Tipo { get; set; } = "";
        public string Identificador { get; set; } = "";
        public AstNode? Valor { get; set; }
        public int? MemoryAddress { get; set; }
        public int? Size { get; set; }
    }

    public class AssignmentNode : AstNode
    {
        public string Identificador { get; set; } = "";
        public AstNode? Valor { get; set; }
    }

    public class LiteralNode : ExpressionNode
    {
        public string TipoLiteral { get; set; } = "";
        public string Valor { get; set; } = "";
    }

    public class IdentifierNode : ExpressionNode
    {
        public string Nombre { get; set; } = "";
        public int? MemoryAddress { get; set; }
    }

    public class BinaryExpressionNode : ExpressionNode
    {
        public string Operador { get; set; } = "";
        public ExpressionNode? Izquierda { get; set; }
        public ExpressionNode? Derecha { get; set; }
    }

    public class UnaryExpressionNode : ExpressionNode
    {
        public string Operador { get; set; } = "";
        public ExpressionNode? Operando { get; set; }
    }

    public class IfNode : AstNode
    {
        public ExpressionNode? Condicion { get; set; }
        public BlockNode BloqueVerdadero { get; set; } = new();
        public BlockNode? BloqueFalso { get; set; }
    }

    public class WhileNode : AstNode
    {
        public ExpressionNode? Condicion { get; set; }
        public BlockNode Cuerpo { get; set; } = new();
    }

    public class DoWhileNode : AstNode
    {
        public ExpressionNode? Condicion { get; set; }
        public BlockNode Cuerpo { get; set; } = new();
    }

    public class ForNode : AstNode
    {
        public AssignmentNode? Inicializacion { get; set; }
        public ExpressionNode? Condicion { get; set; }
        public AssignmentNode? Incremento { get; set; }
        public BlockNode Cuerpo { get; set; } = new();
    }

    public class ReadNode : AstNode
    {
        public string Identificador { get; set; } = "";
    }

    public class PrintNode : AstNode
    {
        public AstNode? Valor { get; set; }
    }

    public class ReturnNode : AstNode
    {
        public AstNode? Valor { get; set; }
    }

    public class SimpleInstructionNode : AstNode
    {
        public string Operacion { get; set; } = "";
        public List<string> Argumentos { get; } = new();
    }

    public class SwitchNode : AstNode
    {
        public string Selector { get; set; } = "";
        public List<SwitchCaseNode> Casos { get; } = new();
        public BlockNode? Predeterminado { get; set; }
    }

    public class SwitchCaseNode
    {
        public ExpressionNode? Valor { get; set; }
        public BlockNode Cuerpo { get; set; } = new();
    }

}
