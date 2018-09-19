grammar S_Script;

// ------------------------------------------ Parser ------------------------------------------ //
compileUnit
	: expression
	| (statement SEMI_COLON)+
	| EOF
	;

// Statements
statement
	: name (LBRAC expression RBRAC)* assignment expression															# STMR_Assign
	| name (LBRAC expression RBRAC)* increment																		# STMR_Increment
	| name (LBRAC expression RBRAC)* OP_APPEND expression															# STMR_Append

	| K_PRINT expression																							# STMR_Print
	| name LPAREN ( expression (COMMA expression)* )? RPAREN	 													# STMR_Exec
	| K_INLINE expression (expression COMMA expression)?															# STMR_Inline
	| K_VOID name LPAREN (name (COMMA name)*)? RPAREN LCURL (statement SEMI_COLON)+ RCURL							# STMR_CreateStatement
	| K_FUNCTION name LPAREN (name (COMMA name)*)? RPAREN LCURL (statement SEMI_COLON)+ RCURL						# STMR_CreateFunction

	| LCURL (statement SEMI_COLON)+ RCURL																			# STMR_Set				
	| K_DO LCURL (statement SEMI_COLON)+ RCURL																		# STMR_Do
	
	| (K_BREAK | K_RETURN | K_EXIT)																					# STMR_Break

	| K_FOR_EACH LPAREN unit_name K_IN expression RPAREN LCURL (statement SEMI_COLON)+ RCURL							# STMR_ForEach
	| K_FOR_EACH LPAREN unit_name COMMA unit_name K_IN expression (AND | OR | XOR) expression join_predicate RPAREN 
		 LCURL (statement SEMI_COLON)+ RCURL																						# STMR_ForEachJoin
	| K_FOR LPAREN statement SEMI_COLON expression SEMI_COLON statement RPAREN LCURL (statement SEMI_COLON)+ RCURL																						# STMR_For		
	| K_WHILE LPAREN expression RPAREN LCURL (statement SEMI_COLON)+ RCURL															# STMR_While
	| K_IF LPAREN expression RPAREN statement (K_ELSE K_IF LPAREN expression RPAREN statement)* (K_ELSE statement)?					# STMR_If
	;

join_predicate : join_predicate_unit (AND join_predicate_unit)*;
join_predicate_unit : name (ASSIGN | EQ) name;


// Expressions
expression
	: expression LBRAC expression RBRAC															# EXPR_Indexer			// M[]
	| op=(NOT | PLUS | MINUS | QUESTION) expression												# EXPR_Uniary			// -X
	| expression POW expression																	# EXPR_Power			// X ^ Y
	| expression op=(MUL | DIV | MOD | DIV2| MOD2) expression									# EXPR_MultDivMod		// X / Y
	| expression op=(PLUS | MINUS) expression													# EXPR_AddSub			// X + Y
	| expression op=(GT | GTE | LT | LTE) expression											# EXPR_GreaterLesser	// X < Y
	| expression op=(EQ | NEQ | SEQ | SNEQ) expression											# EXPR_Equality			// X == Y
	| expression AND expression																	# EXPR_LogicalAnd		// X && Y
	| expression op=(OR | XOR) expression														# EXPR_LogicalOr		// X || Y
	| expression op=(L_SHIFT | R_SHIFT | L_ROTATE | R_ROTATE) expression						# EXPR_BitShiftRotate	// X << Y
	
	| type																						# EXPR_ExpressionType
	| unit_name DOT MUL																			# EXPR_WildCard			// X.*
	| name																						# EXPR_VarName			// X or X.Y
	
	| sliteral																					# EXPR_Literal					
	| LCURL (expression (COMMA expression)*)? RCURL												# EXPR_ArrayLiteral
	 
	| expression (IFNULL | NULLIF) expression													# EXPR_IfNull
	| expression IF_OP expression (COLON expression)?											# EXPR_If
	| name LPAREN ( expression (COMMA expression)*)? RPAREN										# EXPR_Function
	| AT name LPAREN ( expression (COMMA expression)*)? RPAREN									# EXPR_AggFunction
	| expression COLON unit_name																# EXPR_Alias

	| MUL LPAREN expression RPAREN 																# EXPR_Equation
	| MUL LPAREN expression RPAREN ARROW 
		LPAREN unit_name COLON unit_name (COMMA unit_name COLON unit_name)* RPAREN				# EXPR_BoundEquation
	| AMPER LPAREN expression RPAREN															# EXPR_Collapse

	| LPAREN expression RPAREN																	# EXPR_Parens
	;
	
// Compound Opperators //
assignment : (ASSIGN | PLUS ASSIGN | MINUS ASSIGN | MUL ASSIGN | DIV ASSIGN | DIV2 ASSIGN | MOD ASSIGN | MOD2 ASSIGN);
increment : (PLUS PLUS | MINUS MINUS);
    
// Names 
name : unit_name (DOT unit_name)?;
unit_name : IDENTIFIER | T_BOOL | T_DATE | T_BYTE | T_SHORT | T_INT | T_LONG | T_SINGLE | T_DOUBLE | T_BINARY | T_BSTRING | T_CSTRING | T_TABLE | T_ARRAY | T_VARIANT;

// Types //
sliteral : (LITERAL_CSTRING | LITERAL_BSTRING | LITERAL_BINARY | LITERAL_DOUBLE | LITERAL_SINGLE | LITERAL_LONG | LITERAL_INT | LITERAL_SHORT | LITERAL_BYTE | LITERAL_DATE_TIME | LITERAL_BOOL | LITERAL_NULL);
type : (T_BOOL | T_DATE | T_BYTE | T_SHORT | T_INT | T_LONG | T_SINGLE | T_DOUBLE | T_BINARY | T_BSTRING | T_CSTRING | T_TABLE | T_ARRAY | T_VARIANT | T_EQUATION);


// ------------------------------------------ Lexer ------------------------------------------ //

// Keywods //
K_AS : A S;									
K_ASC : A S C;
K_BREAK : B R E A K;
K_BY : B Y;
K_DESC : D E S C;
K_DO : D O;
K_ELSE : E L S E;
K_EXIT : E X I T;
K_EXEC : E X E C | E X E C U T E;
K_FOR : F O R;
K_FOR_EACH : F O R E A C H;
K_FUNCTION : F U N C | F U N C T I O N;
K_IF : I F;
K_IN : I N;
K_INLINE : I N L I N E;
K_PRINT : P R I N T;
K_RETURN : R E T U R N;
K_THEN : T H E N;
K_TO : T O;
K_VOID : V O I D;
K_WHILE : W H I L E;

// Opperators //
OR : O R | PIPE PIPE;
AND : A N D | AMPER AMPER;
XOR : X O R | POW POW;
NOT : N O T | '!';
PLUS : '+';
MINUS : '-';
MUL : '*';
DIV : '/';
DIV2 : '/?';
MOD : '%';
MOD2 : '%?';
POW : '^';
EQ : '==';
SEQ : '===';
NEQ : '!=';
SNEQ : '!==';
LT : '<';
LTE : '<=';
GT : '>';
GTE : '>=';
IFNULL : '??';
NULLIF : '!?';
QUESTION : '?';
LPAREN : '(';
RPAREN : ')';
LBRAC : '[';
RBRAC : ']';
LCURL : '{';
RCURL : '}';
COMMA : ',';
SEMI_COLON : ';';
ARROW : '->';
ARROW2 : '=>';
DOT : '.';
ASSIGN : '=';
TILDA : '~';
PIPE : '|';
AMPER : '&';
AT : '@';
COLON : ':';
L_SHIFT : '<<';
L_ROTATE : '<<<';
R_SHIFT : '>>';
R_ROTATE : '>>>';
OP_APPEND : '+>';

// Core types //
T_BOOL : B O O L;
T_DATETIME : D A T E T I M E;
T_BYTE : B Y T E;
T_SHORT : S H O R T;
T_INT : I N T;
T_LONG : L O N G;
T_SINGLE : S I N G L E;
T_DOUBLE : D O U B L E;
T_BINARY : B I N A R Y;
T_BSTRING : B S T R I N G;
T_CSTRING : C S T R I N G;
T_TABLE : T A B L E;
T_ARRAY : A R R A Y;
T_EQUATION : E Q U A T I O N;

// Literals //
LITERAL_NULL
	: N U L L
	; 
LITERAL_BOOL 
	: T R U E 
	| F A L S E
	;
LITERAL_DATE_TIME 
	: '\'' DIGIT+ '-' DIGIT+ '-' DIGIT+ '\'' D T 												// 'YYYY-MM-DD'DT
	| '\'' DIGIT+ '-' DIGIT+ '-' DIGIT+ ':' DIGIT+ ':' DIGIT+ ':' DIGIT+ '\'' D T				// 'YYYY-MM-DD:HH:MM:SS'DT
	| '\'' DIGIT+ '-' DIGIT+ '-' DIGIT+ ':' DIGIT+ ':' DIGIT+ ':' DIGIT+ '.' DIGIT+ '\'' D T	// 'YYYY-MM-DD:HH:MM:SS.LLLLLLLL'DT
	;
LITERAL_BYTE 
	: DIGIT+ B
	;
LITERAL_SHORT 
	: DIGIT+ S
	;
LITERAL_INT 
	: DIGIT+ (I)?
	;
LITERAL_LONG
	: DIGIT+ L 
	;
LITERAL_SINGLE
	: DIGIT+ '.' DIGIT+ F 
	| (DIGIT+) F		
	;
LITERAL_DOUBLE 
	: DIGIT+ '.' DIGIT+ (D)? 
	| (DIGIT+) D
	;
LITERAL_BINARY
	: '0' X (HEX HEX)*
	;
LITERAL_BSTRING
	: LITERAL_STRING B?
	;
LITERAL_CSTRING
	: LITERAL_STRING C
	;
LITERAL_STRING 
	: '\'' ( ~'\'' | '\'\'' )* '\'' // 'abcdef'
	| '"' ( ~'"' | '""')* '"'		// "ABCDEF"
	| SLIT .*? SLIT					// $$ABCDEF$$
	| '\'\''						// ''
	| '\"\"'						// ""
	;

// Base Token //
IDENTIFIER
	: [a-zA-Z_] [a-zA-Z_0-9]*
	;

// Comments and whitespace //
SINGLE_LINE_COMMENT : '//' ~[\r\n]* -> channel(HIDDEN);
MULTILINE_COMMENT : '/*' .*? ( '*/' | EOF ) -> channel(HIDDEN);
WS : ( ' ' | '\t' |'\r' | '\n' | '\r\n')* -> channel(HIDDEN);

fragment SLIT : '$$';
fragment DIGIT : [0-9];
fragment HEX : [aAbBcCdDeEfF0123456789];
fragment A : [aA];
fragment B : [bB];
fragment C : [cC];
fragment D : [dD];
fragment E : [eE];
fragment F : [fF];
fragment G : [gG];
fragment H : [hH];
fragment I : [iI];
fragment J : [jJ];
fragment K : [kK];
fragment L : [lL];
fragment M : [mM];
fragment N : [nN];
fragment O : [oO];
fragment P : [pP];
fragment Q : [qQ];
fragment R : [rR];
fragment S : [sS];
fragment T : [tT];
fragment U : [uU];
fragment V : [vV];
fragment W : [wW];
fragment X : [xX];
fragment Y : [yY];
fragment Z : [zZ];