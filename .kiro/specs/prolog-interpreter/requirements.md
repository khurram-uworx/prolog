# Requirements Document

## Introduction

A minimum Prolog compiler/interpreter/query runner implemented in C# that can parse Prolog programs, build a knowledge base, and execute queries against that knowledge base. The system will support basic Prolog syntax including facts, rules, and queries with unification and backtracking.

## Glossary

- **Prolog_System**: The complete interpreter system including parser, knowledge base, and query engine
- **Knowledge_Base**: Collection of facts and rules that form the program's database
- **Query_Engine**: Component that processes queries and finds solutions through unification and backtracking
- **Parser**: Component that converts Prolog source text into internal data structures
- **Unification**: Process of matching terms and binding variables to make them identical
- **Backtracking**: Search strategy that explores alternative solutions when current path fails
- **Fact**: Simple statement that is unconditionally true (e.g., "parent(tom, bob).")
- **Rule**: Conditional statement with head and body (e.g., "grandparent(X, Z) :- parent(X, Y), parent(Y, Z).")
- **Term**: Basic data structure including atoms, variables, and compound terms
- **Variable**: Placeholder that can be bound to values during unification (starts with uppercase)
- **Atom**: Constant symbol (starts with lowercase)

## Requirements

### Requirement 1: Parse Prolog Programs

**User Story:** As a developer, I want to load Prolog programs from text, so that I can build a knowledge base for querying.

#### Acceptance Criteria

1. WHEN a valid Prolog program is provided, THE Parser SHALL convert it into internal data structures
2. WHEN parsing facts (e.g., "parent(tom, bob)."), THE Parser SHALL create fact entries in the knowledge base
3. WHEN parsing rules (e.g., "grandparent(X, Z) :- parent(X, Y), parent(Y, Z)."), THE Parser SHALL create rule entries with head and body
4. WHEN parsing invalid syntax, THE Parser SHALL return descriptive error messages
5. THE Pretty_Printer SHALL format internal structures back into valid Prolog syntax
6. FOR ALL valid Prolog programs, parsing then printing then parsing SHALL produce equivalent internal structures (round-trip property)

### Requirement 2: Execute Queries

**User Story:** As a user, I want to execute queries against the knowledge base, so that I can find solutions and verify relationships.

#### Acceptance Criteria

1. WHEN a query is submitted (e.g., "?- parent(X, bob)."), THE Query_Engine SHALL find all matching solutions
2. WHEN multiple solutions exist, THE Query_Engine SHALL return all valid bindings through backtracking
3. WHEN no solutions exist, THE Query_Engine SHALL indicate failure appropriately
4. WHEN variables are used in queries, THE Query_Engine SHALL bind them to concrete values in solutions
5. THE Query_Engine SHALL support compound queries with multiple goals (e.g., "?- parent(X, Y), parent(Y, Z).")

### Requirement 3: Perform Unification

**User Story:** As the system, I want to unify terms with variables, so that I can match patterns and bind variables during query resolution.

#### Acceptance Criteria

1. WHEN unifying identical atoms, THE Prolog_System SHALL succeed with no bindings
2. WHEN unifying a variable with any term, THE Prolog_System SHALL bind the variable to that term
3. WHEN unifying compound terms, THE Prolog_System SHALL recursively unify all arguments
4. WHEN unification fails, THE Prolog_System SHALL backtrack to find alternative solutions
5. THE Prolog_System SHALL maintain variable bindings consistently throughout query resolution

### Requirement 4: Support Basic Prolog Syntax

**User Story:** As a Prolog programmer, I want to use standard Prolog syntax, so that I can write familiar programs.

#### Acceptance Criteria

1. THE Parser SHALL recognize atoms starting with lowercase letters (e.g., "tom", "parent")
2. THE Parser SHALL recognize variables starting with uppercase letters (e.g., "X", "Person")
3. THE Parser SHALL parse compound terms with functors and arguments (e.g., "parent(tom, bob)")
4. THE Parser SHALL distinguish between facts ending with "." and rules containing ":-"
5. THE Parser SHALL handle whitespace and comments appropriately

### Requirement 5: Manage Knowledge Base

**User Story:** As a developer, I want to build and query a knowledge base, so that I can store and retrieve Prolog facts and rules.

#### Acceptance Criteria

1. WHEN facts are added, THE Knowledge_Base SHALL store them for later retrieval
2. WHEN rules are added, THE Knowledge_Base SHALL store them with proper head-body structure
3. WHEN querying, THE Knowledge_Base SHALL provide relevant facts and rules for unification
4. THE Knowledge_Base SHALL support multiple facts and rules with the same functor
5. THE Knowledge_Base SHALL maintain insertion order for deterministic behavior

### Requirement 6: Handle Interactive Queries

**User Story:** As a user, I want to interactively query the system, so that I can explore the knowledge base and test hypotheses.

#### Acceptance Criteria

1. WHEN the system starts, THE Prolog_System SHALL provide a query prompt
2. WHEN a query is entered, THE Prolog_System SHALL process it and display results
3. WHEN multiple solutions exist, THE Prolog_System SHALL allow stepping through them
4. WHEN the user requests more solutions, THE Prolog_System SHALL continue backtracking
5. THE Prolog_System SHALL handle exit commands gracefully