This implementation was **Vibe Coded** using KIRO

- This is all [PLASTIC CODE](PLASTIC.md)

# Prolog Interpreter

A minimum Prolog compiler/interpreter/query runner implemented in C# that can parse Prolog programs, build a knowledge base, and execute queries against that knowledge base with unification and backtracking.

## Features

- ✅ **Term System**: Complete representation of Prolog data structures (atoms, variables, compound terms)
- ✅ **Lexical Analysis**: Tokenization of Prolog source code with proper syntax recognition
- ✅ **Parser**: Recursive descent parser for facts, rules, and queries
- ✅ **Pretty Printer**: Round-trip consistent formatting back to valid Prolog syntax
- ✅ **Knowledge Base**: Storage and retrieval of facts and rules with efficient indexing
- ✅ **Unification Engine**: Pattern matching with variable binding and occurs check
- ✅ **Query Engine**: Complete query resolution with unification and backtracking
- ✅ **Interactive Shell**: REPL interface for interactive Prolog programming
- ✅ **Sample Programs**: Comprehensive examples demonstrating interpreter capabilities
- ✅ **Integration Tests**: End-to-end testing with realistic Prolog programs

## Project Structure

```
├── Prolog/                   # Core interpreter library (class library)
│   ├── Term.cs               # Abstract base class for all Prolog terms
│   ├── Atom.cs               # Prolog atoms (constants)
│   ├── Variable.cs           # Prolog variables
│   ├── Compound.cs           # Compound terms with functors and arguments
│   ├── Token.cs              # Lexical tokens
│   ├── Lexer.cs              # Lexical analyzer
│   ├── Clause.cs             # Prolog clauses (facts and rules)
│   ├── Parser.cs             # Recursive descent parser
│   ├── ParseResult.cs        # Parser result wrapper
│   ├── PrettyPrinter.cs      # Formats internal structures back to Prolog
│   ├── IKnowledgeBase.cs     # Knowledge base interface
│   ├── KnowledgeBase.cs      # Knowledge base implementation
│   ├── IUnificationEngine.cs # Unification engine interface
│   ├── UnificationEngine.cs  # Unification algorithm implementation
│   ├── UnificationResult.cs  # Unification result wrapper
│   ├── IQueryEngine.cs       # Query engine interface
│   ├── QueryEngine.cs        # Query resolution with backtracking
│   ├── Solution.cs           # Query solution wrapper
│   └── PrologShell.cs        # Interactive shell implementation
├── Prolog.Console/           # Interactive Prolog shell application
│   └── Program.cs            # Console application entry point
├── Prolog.Samples/           # Sample programs demonstrator
│   ├── Program.cs            # Sample program runner with interactive menu
│   └── SamplePrograms.cs     # Collection of demonstration programs
├── Prolog.Tests/             # Comprehensive test suite
│   ├── TermTests.cs          # Tests for term system
│   ├── LexerTests.cs         # Tests for lexical analysis
│   ├── ParserTests.cs        # Tests for parsing
│   ├── PrettyPrinterTests.cs # Tests for pretty printing
│   ├── KnowledgeBaseTests.cs # Tests for knowledge base
│   ├── UnificationEngineTests.cs # Tests for unification
│   ├── QueryEngineTests.cs   # Tests for query resolution
│   ├── TestPrograms.cs       # Sample programs for testing
│   └── IntegrationTests.cs   # End-to-end integration tests
└── README.md                 # This file
```

## Getting Started

### Prerequisites

- .NET 10.0 or later
- Visual Studio 2022 or VS Code (optional)

### Building

```bash
# Clone the repository
git clone https://github.com/khurram-uworx/prolog.git
cd prolog

# Build the solution
dotnet build

# Run tests
dotnet test

# Run the interactive Prolog shell
dotnet run --project Prolog.Console

# Run the sample programs demonstrator
dotnet run --project Prolog.Samples
```

### Usage

#### Interactive Shell

Run the interactive Prolog shell for a full REPL experience:

```bash
dotnet run --project Prolog.Console
```

This provides an interactive environment where you can:
- Load Prolog programs from text input
- Execute queries and step through solutions
- Get help and exit gracefully

#### Sample Programs

Explore the interpreter capabilities with pre-built sample programs:

```bash
dotnet run --project Prolog.Samples
```

Available sample programs:
1. **Family Tree** - Parent-child relationships and derived family rules
2. **Simple Rules** - Basic facts and logical derivations  
3. **Logic Puzzle** - Object properties and reasoning
4. **Math Relations** - Number relationships and basic arithmetic

#### Programmatic Usage

Use the Prolog library in your own applications:

```csharp
using Prolog;

// Create components
var parser = new Parser();
var knowledgeBase = new KnowledgeBase();
var unificationEngine = new UnificationEngine();
var queryEngine = new QueryEngine(knowledgeBase, unificationEngine);

// Parse and load a Prolog program
var program = @"
    parent(tom, bob).
    parent(bob, alice).
    grandparent(X, Z) :- parent(X, Y), parent(Y, Z).
";

var parseResult = parser.ParseProgram(program);
if (parseResult.Success)
{
    foreach (var clause in parseResult.Clauses)
    {
        knowledgeBase.AddClause(clause);
    }
}

// Execute a query
var queryResult = parser.ParseQuery("?- grandparent(tom, X).");
if (queryResult.Success)
{
    var solutions = queryEngine.Solve(queryResult.Query);
    foreach (var solution in solutions)
    {
        if (solution.IsSuccess)
        {
            Console.WriteLine($"X = {solution.Bindings["X"]}");
        }
    }
}
```

## Supported Prolog Syntax

### Facts
```prolog
parent(tom, bob).
likes(mary, chocolate).
```

### Rules
```prolog
grandparent(X, Z) :- parent(X, Y), parent(Y, Z).
ancestor(X, Z) :- parent(X, Z).
ancestor(X, Z) :- parent(X, Y), ancestor(Y, Z).
```

### Queries
```prolog
?- parent(X, bob).
?- parent(X, Y), parent(Y, Z).
```

### Terms
- **Atoms**: `tom`, `parent`, `likes` (start with lowercase)
- **Variables**: `X`, `Person`, `_` (start with uppercase or underscore)
- **Compound Terms**: `parent(tom, bob)`, `likes(X, Y)`
- **Nested Terms**: `likes(person(tom), food(pizza))`

## Architecture

The interpreter follows a classic compiler architecture with complete query resolution:

```
Prolog Source → Lexer → Tokens → Parser → AST → Pretty Printer → Prolog Source
                                    ↓
                              Knowledge Base ← Unification Engine
                                    ↓              ↓
                              Query Engine → Solutions (with backtracking)
                                    ↓
                            Interactive Shell / Sample Programs
```

### Components

1. **Lexer**: Converts source text into tokens, handling atoms, variables, operators, and punctuation
2. **Parser**: Recursive descent parser that builds an Abstract Syntax Tree (AST) from tokens
3. **Term System**: Polymorphic hierarchy representing all Prolog data structures
4. **Pretty Printer**: Formats internal structures back to valid Prolog syntax with round-trip consistency
5. **Knowledge Base**: Efficient storage and retrieval of clauses with functor/arity indexing
6. **Unification Engine**: Implements Prolog unification algorithm with occurs check and variable binding
7. **Query Engine**: Complete query resolution with backtracking for finding all solutions
8. **Interactive Shell**: REPL interface for interactive Prolog programming

## Testing

The project uses NUnit for testing with comprehensive coverage:

- **116 total tests** covering all components
- **Property-based testing** with FsCheck for comprehensive validation
- **Round-trip tests** ensuring parse → print → parse consistency
- **Error handling tests** for robust error reporting
- **Unification tests** validating pattern matching correctness
- **Query engine tests** verifying backtracking and solution finding
- **Integration tests** with realistic Prolog programs testing end-to-end functionality

Run tests with:
```bash
dotnet test
```

### Sample Programs Tested

The integration tests include comprehensive sample programs:

1. **Family Tree Program**
   ```prolog
   parent(tom, bob).
   parent(bob, charlie).
   grandparent(X, Z) :- parent(X, Y), parent(Y, Z).
   ancestor(X, Y) :- parent(X, Y).
   ancestor(X, Y) :- parent(X, Z), ancestor(Z, Y).
   ```

2. **Logic Rules Program**
   ```prolog
   likes(mary, wine).
   likes(john, wine).
   happy(X) :- likes(X, wine).
   ```

3. **Object Properties Program**
   ```prolog
   object(ball, red, circle).
   red_object(X) :- object(X, red, _).
   ```

## Development Status

### Completed ✅
- [x] Core term representations (atoms, variables, compounds)
- [x] Lexical analysis with full Prolog syntax support
- [x] Recursive descent parser for facts, rules, and queries
- [x] Pretty printer with round-trip consistency
- [x] Knowledge base with efficient functor/arity indexing
- [x] Unification engine with occurs check and variable binding
- [x] Query engine with complete backtracking algorithm
- [x] Interactive shell interface with REPL functionality
- [x] Sample programs demonstrator with 4 comprehensive examples
- [x] Comprehensive test suite (116 tests including integration tests)
- [x] Error handling and reporting
- [x] End-to-end integration testing with realistic Prolog programs

### Future Enhancements 📋
- [ ] Built-in predicates (arithmetic, comparison, type checking)
- [ ] Cut operator (!) for controlling backtracking
- [ ] List syntax support ([H|T] notation)
- [ ] Arithmetic operations and evaluation
- [ ] File I/O for loading programs from files
- [ ] Debugging and tracing capabilities
- [ ] Performance optimizations for large programs

## Contributing

1. Fork the repository
2. Create a feature branch
3. Add tests for new functionality
4. Ensure all tests pass
5. Submit a pull request

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Acknowledgments

- Built following classic Prolog interpreter design patterns
- Implements standard Prolog syntax and semantics
- Uses property-based testing for comprehensive validation