grammar Calc;

/*
 * Parser Rules
 */

expression
 : OpenParen expression CloseParen                                                     # ParenthesisExpression
 | <assoc=right> expression Power expression                                           # PowerExpression
 | Minus expression                                                                    # UnaryMinusExpression
 | expression (Multiply | Divide) expression                                           # MultiplicativeExpression
 | expression (Plus | Minus)      expression                                           # AdditiveExpression
 | expression (LessThanEquals | GreaterThanEquals | LessThan | GreaterThan) expression # RelationalExpression
 | expression (Equals_ | NotEquals) expression                                         # EqualityExpression
 | Abs OpenParen expression CloseParen                                                 # AbsExpression
 | Sqrt OpenParen expression CloseParen                                                # SqrtExpression
 | Sgn OpenParen expression CloseParen                                                 # SgnExpression
 | Var OpenParen expression CloseParen                                                 # VarExpression
 | Normd OpenParen expression CloseParen                                               # NormdExpression
 | Field OpenParen expression CloseParen                                               # FieldExpression
 | FldLen OpenParen expression CloseParen                                              # FldlenExpression
 | Identifier OpenParen expression CloseParen                                          # UnknownFunctionExpression
 | Identifier                                                                          # IdentifierExpression
 | (ConstPi | ConstE)                                                                  # ConstantExpression
 | literal                                                                             # LiteralExpression
 ;

literal
 : StringLiteral
 | IntegerLiteral
 | DecimalLiteral
 | DateLiteral
 ;

/*
 * Lexer Rules
 */

Var                    : DOLLAR V A R;
Normd                  : DOLLAR N O R M D;
Field                  : DOLLAR F I E L D;
FldLen                 : DOLLAR F L D L E N;

Abs                    : A B S;
Sqrt                   : S Q R T;
Sgn                    : S G N;

LessThanEquals         : '<=' ;
GreaterThanEquals      : '>=' ;
NotEquals              : '<>' ;
Equals_                : '=' ;
LessThan               : '<' ;
GreaterThan            : '>' ;

Plus                   : '+' ;
Minus                  : '-' ;
Multiply               : '*' ;
Divide                 : '/' ;
Power                  : '^' ;

OpenParen              : '(' ;
CloseParen             : ')' ;

IntegerLiteral         : [0-9]+ ;
DecimalLiteral         : [0-9]+ '.' [0-9]* | '.' [0-9]+ ;
DateLiteral            : '[' (~[\]\r\n])* ']' { Text = Text.Substring(1, Text.Length - 2); };

ConstE                 : E ;
ConstPi                : P I ;

Identifier             : [a-zA-Z_] [a-zA-Z_0-9]* ;

Whitespace             : [ \t\r\n] -> skip ;

StringLiteral          : '\'' STRING '\'';

/* case insensitive lexer matching */
fragment DOLLAR: '$';
fragment A:('a'|'A');
fragment B:('b'|'B');
fragment C:('c'|'C');
fragment D:('d'|'D');
fragment E:('e'|'E');
fragment F:('f'|'F');
fragment G:('g'|'G');
fragment H:('h'|'H');
fragment I:('i'|'I');
fragment J:('j'|'J');
fragment K:('k'|'K');
fragment L:('l'|'L');
fragment M:('m'|'M');
fragment N:('n'|'N');
fragment O:('o'|'O');
fragment P:('p'|'P');
fragment Q:('q'|'Q');
fragment R:('r'|'R');
fragment S:('s'|'S');
fragment T:('t'|'T');
fragment U:('u'|'U');
fragment V:('v'|'V');
fragment W:('w'|'W');
fragment X:('x'|'X');
fragment Y:('y'|'Y');
fragment Z:('z'|'Z');
fragment STRING: (~['\\\r\n] | '\'\'')*;