# Prolog Interpreter

A minimum Prolog compiler/interpreter/query runner implemented in C# that can parse Prolog programs, build a knowledge base, and execute queries against that knowledge base.

## Features

- ✅ **Term System**: Complete representation of Prolog data structures (atoms, variables, compound terms)
- ✅ **Lexical Analysis**: Tokenization of Prolog source code with proper syntax recognition
- ✅ **Parser**: Recursive descent parser for facts, rules, and queries
- ✅ **Pretty Printer**: Round-trip consistent formatting back to valid Prolog syntax
- 🚧 **Knowledge Base**: Storage and retrieval of facts and rules (in progress)
- 🚧 **Query Engine**: Unification and backtracking for query resolution (planned)
- 🚧 **Interactive Shell**: REPL interface for interactive Prolog programming (planned)

## Project Structure

```
├── Prolog/                 # Main interpreter library
│   ├── Term.cs            # Abstract base class for all Prolog terms
│   ├── Atom.cs            # Prolog atoms (constants)
│   ├── Variable.cs        # Prolog variables
│   ├── Compound.cs        # Compound terms with functors and arguments
│   ├── Token.cs           # Lexical tokens
│   ├── Lexer.cs           # Lexical analyzer
│   ├── Clause.cs          # Prolog clauses (facts and rules)
│   ├── Parser.cs          # Recursive descent parser
│   ├── PrettyPrinter.cs   # Formats internal structures back to Prolog
│   └── Program.cs         # Main console application
├── Prolog.Tests/          # NUnit test suite
│   ├── TermTests.cs       # Tests for term system
│   ├── LexerTests.cs      # Tests for lexical analysis
│   ├── ParserTests.cs     # Tests for parsing
│   └── PrettyPrinterTests.cs # Tests for pretty printing
└── README.md              # This file
```

## Getting Started

### Prerequisites

- .NET 10.0 or later
- Visual Studio 2022 or VS Code (optional)

### Building

```bash
# Clone the repository
git clone <repository-url>
cd prolog

# Build the solution
dotnet build

# Run tests
dotnet test

# Run the interpreter
dotnet run --project Prolog
```

### Usage

Currently, the interpreter supports parsing and pretty-printing Prolog programs:

```csharp
var parser = new Parser();
var prettyPrinter = new PrettyPrinter();

// Parse a Prolog program
var result = parser.ParseProgram("parent(tom, bob). grandparent(X, Z) :- parent(X, Y), parent(Y, Z).");

if (result.Success)
{
    // Pretty print the parsed clauses
    var formatted = prettyPrinter.FormatProgram(result.Clauses);
    Console.WriteLine(formatted);
}

// Parse a query
var queryResult = parser.ParseQuery("?- parent(X, bob).");
if (queryResult.Success)
{
    var formattedQuery = prettyPrinter.FormatQuery(queryResult.Query);
    Console.WriteLine(formattedQuery);
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

The interpreter follows a classic compiler architecture:

```
Prolog Source → Lexer → Tokens → Parser → AST → Pretty Printer → Prolog Source
                                    ↓
                              Knowledge Base → Query Engine → Solutions
```

### Components

1. **Lexer**: Converts source text into tokens, handling atoms, variables, operators, and punctuation
2. **Parser**: Recursive descent parser that builds an Abstract Syntax Tree (AST) from tokens
3. **Term System**: Polymorphic hierarchy representing all Prolog data structures
4. **Pretty Printer**: Formats internal structures back to valid Prolog syntax with round-trip consistency

## Testing

The project uses NUnit for testing with comprehensive coverage:

- **53 total tests** covering all components
- **Property-based testing** with FsCheck for comprehensive validation
- **Round-trip tests** ensuring parse → print → parse consistency
- **Error handling tests** for robust error reporting

Run tests with:
```bash
dotnet test
```

## Development Status

### Completed ✅
- [x] Core term representations (atoms, variables, compounds)
- [x] Lexical analysis with full Prolog syntax support
- [x] Recursive descent parser for facts, rules, and queries
- [x] Pretty printer with round-trip consistency
- [x] Comprehensive test suite (53 tests)
- [x] Error handling and reporting

### In Progress 🚧
- [ ] Knowledge base implementation
- [ ] Unification engine
- [ ] Query engine with backtracking
- [ ] Interactive shell interface

### Planned 📋
- [ ] Built-in predicates
- [ ] Cut operator (!)
- [ ] List syntax support
- [ ] Arithmetic operations
- [ ] File I/O for loading programs

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