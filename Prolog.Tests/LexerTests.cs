using Prolog;

namespace Prolog.Tests
{
    [TestFixture]
    public class LexerTests
    {
        [Test]
        public void Lexer_TokenizesAtoms()
        {
            var lexer = new Lexer("tom parent likes");
            var tokens = lexer.Tokenize();

            // Filter out whitespace tokens for easier testing
            var nonWhitespace = tokens.Where(t => t.Type != TokenType.Whitespace && t.Type != TokenType.EndOfFile).ToList();

            Assert.That(nonWhitespace.Count, Is.EqualTo(3));
            Assert.That(nonWhitespace[0].Type, Is.EqualTo(TokenType.Atom));
            Assert.That(nonWhitespace[0].Value, Is.EqualTo("tom"));
            Assert.That(nonWhitespace[1].Type, Is.EqualTo(TokenType.Atom));
            Assert.That(nonWhitespace[1].Value, Is.EqualTo("parent"));
            Assert.That(nonWhitespace[2].Type, Is.EqualTo(TokenType.Atom));
            Assert.That(nonWhitespace[2].Value, Is.EqualTo("likes"));
        }

        [Test]
        public void Lexer_TokenizesVariables()
        {
            var lexer = new Lexer("X Person _ _var");
            var tokens = lexer.Tokenize();

            var nonWhitespace = tokens.Where(t => t.Type != TokenType.Whitespace && t.Type != TokenType.EndOfFile).ToList();

            Assert.That(nonWhitespace.Count, Is.EqualTo(4));
            Assert.That(nonWhitespace.All(token => token.Type == TokenType.Variable), Is.True);
            Assert.That(nonWhitespace[0].Value, Is.EqualTo("X"));
            Assert.That(nonWhitespace[1].Value, Is.EqualTo("Person"));
            Assert.That(nonWhitespace[2].Value, Is.EqualTo("_"));
            Assert.That(nonWhitespace[3].Value, Is.EqualTo("_var"));
        }

        [Test]
        public void Lexer_TokenizesPunctuation()
        {
            var lexer = new Lexer(".,()");
            var tokens = lexer.Tokenize();

            var nonWhitespace = tokens.Where(t => t.Type != TokenType.Whitespace && t.Type != TokenType.EndOfFile).ToList();

            Assert.That(nonWhitespace.Count, Is.EqualTo(4));
            Assert.That(nonWhitespace[0].Type, Is.EqualTo(TokenType.Dot));
            Assert.That(nonWhitespace[1].Type, Is.EqualTo(TokenType.Comma));
            Assert.That(nonWhitespace[2].Type, Is.EqualTo(TokenType.LeftParen));
            Assert.That(nonWhitespace[3].Type, Is.EqualTo(TokenType.RightParen));
        }

        [Test]
        public void Lexer_TokenizesOperators()
        {
            var lexer = new Lexer(":- ?-");
            var tokens = lexer.Tokenize();

            var nonWhitespace = tokens.Where(t => t.Type != TokenType.Whitespace && t.Type != TokenType.EndOfFile).ToList();

            Assert.That(nonWhitespace.Count, Is.EqualTo(2));
            Assert.That(nonWhitespace[0].Type, Is.EqualTo(TokenType.Rule));
            Assert.That(nonWhitespace[0].Value, Is.EqualTo(":-"));
            Assert.That(nonWhitespace[1].Type, Is.EqualTo(TokenType.Query));
            Assert.That(nonWhitespace[1].Value, Is.EqualTo("?-"));
        }

        [Test]
        public void Lexer_TokenizesNotEqualOperator()
        {
            var lexer = new Lexer("X \\= Y");
            var tokens = lexer.Tokenize();

            var nonWhitespace = tokens.Where(t => t.Type != TokenType.Whitespace && t.Type != TokenType.EndOfFile).ToList();

            Assert.That(nonWhitespace.Count, Is.EqualTo(3));
            Assert.That(nonWhitespace[0].Type, Is.EqualTo(TokenType.Variable));
            Assert.That(nonWhitespace[0].Value, Is.EqualTo("X"));
            Assert.That(nonWhitespace[1].Type, Is.EqualTo(TokenType.NotEqual));
            Assert.That(nonWhitespace[1].Value, Is.EqualTo("\\="));
            Assert.That(nonWhitespace[2].Type, Is.EqualTo(TokenType.Variable));
            Assert.That(nonWhitespace[2].Value, Is.EqualTo("Y"));
        }

        [Test]
        public void Lexer_TokenizesComments()
        {
            var lexer = new Lexer("% this is a comment\natom");
            var tokens = lexer.Tokenize();

            var commentToken = tokens.First(t => t.Type == TokenType.Comment);
            Assert.That(commentToken.Value, Is.EqualTo("% this is a comment"));

            var atomToken = tokens.First(t => t.Type == TokenType.Atom);
            Assert.That(atomToken.Value, Is.EqualTo("atom"));
        }

        [Test]
        public void Lexer_HandlesWhitespace()
        {
            var lexer = new Lexer("  \t\n  atom  \n");
            var tokens = lexer.Tokenize();

            var whitespaceTokens = tokens.Where(t => t.Type == TokenType.Whitespace).ToList();
            Assert.That(whitespaceTokens.Count, Is.GreaterThan(0));

            var atomToken = tokens.First(t => t.Type == TokenType.Atom);
            Assert.That(atomToken.Value, Is.EqualTo("atom"));
        }

        [Test]
        public void Lexer_TracksLineAndColumn()
        {
            var lexer = new Lexer("atom\n  Variable");
            var tokens = lexer.Tokenize();

            var atomToken = tokens.First(t => t.Type == TokenType.Atom);
            Assert.That(atomToken.Line, Is.EqualTo(1));
            Assert.That(atomToken.Column, Is.EqualTo(1));

            var variableToken = tokens.First(t => t.Type == TokenType.Variable);
            Assert.That(variableToken.Line, Is.EqualTo(2));
            Assert.That(variableToken.Column, Is.EqualTo(3));
        }

        [Test]
        public void Lexer_TokenizesCompleteFact()
        {
            var lexer = new Lexer("parent(tom, bob).");
            var tokens = lexer.Tokenize();

            var nonWhitespace = tokens.Where(t => t.Type != TokenType.Whitespace && t.Type != TokenType.EndOfFile).ToList();

            Assert.That(nonWhitespace.Count, Is.EqualTo(7));
            Assert.That(nonWhitespace[0].Type, Is.EqualTo(TokenType.Atom));
            Assert.That(nonWhitespace[0].Value, Is.EqualTo("parent"));
            Assert.That(nonWhitespace[1].Type, Is.EqualTo(TokenType.LeftParen));
            Assert.That(nonWhitespace[2].Type, Is.EqualTo(TokenType.Atom));
            Assert.That(nonWhitespace[2].Value, Is.EqualTo("tom"));
            Assert.That(nonWhitespace[3].Type, Is.EqualTo(TokenType.Comma));
            Assert.That(nonWhitespace[4].Type, Is.EqualTo(TokenType.Atom));
            Assert.That(nonWhitespace[4].Value, Is.EqualTo("bob"));
            Assert.That(nonWhitespace[5].Type, Is.EqualTo(TokenType.RightParen));
            Assert.That(nonWhitespace[6].Type, Is.EqualTo(TokenType.Dot));
        }

        [Test]
        public void Lexer_TokenizesCompleteRule()
        {
            var lexer = new Lexer("grandparent(X, Z) :- parent(X, Y), parent(Y, Z).");
            var tokens = lexer.Tokenize();

            var nonWhitespace = tokens.Where(t => t.Type != TokenType.Whitespace && t.Type != TokenType.EndOfFile).ToList();

            // Should contain: grandparent ( X , Z ) :- parent ( X , Y ) , parent ( Y , Z ) .
            Assert.That(nonWhitespace.Count, Is.GreaterThan(10));
            Assert.That(nonWhitespace.Any(t => t.Type == TokenType.Rule), Is.True);
            Assert.That(nonWhitespace.Any(t => t.Type == TokenType.Atom && t.Value == "grandparent"), Is.True);
            Assert.That(nonWhitespace.Any(t => t.Type == TokenType.Variable && t.Value == "X"), Is.True);
        }

        [Test]
        public void Lexer_HandlesInvalidCharacters()
        {
            var lexer = new Lexer("@#$");
            var tokens = lexer.Tokenize();

            var invalidTokens = tokens.Where(t => t.Type == TokenType.Invalid).ToList();
            Assert.That(invalidTokens.Count, Is.EqualTo(3));
        }

        [Test]
        public void Lexer_HandlesEmptyInput()
        {
            var lexer = new Lexer("");
            var tokens = lexer.Tokenize();

            Assert.That(tokens.Count, Is.EqualTo(1));
            Assert.That(tokens[0].Type, Is.EqualTo(TokenType.EndOfFile));
        }
    }
}