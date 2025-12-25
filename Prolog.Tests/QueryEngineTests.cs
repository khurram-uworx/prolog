using NUnit.Framework;
using Prolog;

namespace Prolog.Tests
{
    [TestFixture]
    public class QueryEngineTests
    {
        IKnowledgeBase knowledgeBase;
        IUnificationEngine unificationEngine;
        QueryEngine queryEngine;

        [SetUp]
        public void SetUp()
        {
            knowledgeBase = new KnowledgeBase();
            unificationEngine = new UnificationEngine();
            queryEngine = new QueryEngine(knowledgeBase, unificationEngine);
        }

        [Test]
        public void Solve_SimpleFactQuery_ReturnsSolution()
        {
            // Arrange
            var fact = new Clause(new Compound("parent", new Atom("tom"), new Atom("bob")));
            knowledgeBase.AddClause(fact);
            
            var query = new Compound("parent", new Atom("tom"), new Atom("bob"));

            // Act
            var solutions = queryEngine.Solve(query).ToList();

            // Assert
            Assert.That(solutions.Count, Is.EqualTo(1));
            Assert.That(solutions[0].IsSuccess, Is.True);
            Assert.That(solutions[0].Bindings.Count, Is.EqualTo(0)); // No variables to bind
        }

        [Test]
        public void Solve_QueryWithVariable_ReturnsBindings()
        {
            // Arrange
            var fact = new Clause(new Compound("parent", new Atom("tom"), new Atom("bob")));
            knowledgeBase.AddClause(fact);
            
            var query = new Compound("parent", new Variable("X"), new Atom("bob"));

            // Act
            var solutions = queryEngine.Solve(query).ToList();

            // Assert
            Assert.That(solutions.Count, Is.EqualTo(1));
            Assert.That(solutions[0].IsSuccess, Is.True);
            Assert.That(solutions[0].Bindings.Count, Is.EqualTo(1));
            Assert.That(solutions[0].Bindings["X"], Is.EqualTo(new Atom("tom")));
        }

        [Test]
        public void Solve_MultipleMatchingFacts_ReturnsMultipleSolutions()
        {
            // Arrange
            knowledgeBase.AddClause(new Clause(new Compound("parent", new Atom("tom"), new Atom("bob"))));
            knowledgeBase.AddClause(new Clause(new Compound("parent", new Atom("tom"), new Atom("alice"))));
            
            var query = new Compound("parent", new Atom("tom"), new Variable("X"));

            // Act
            var solutions = queryEngine.Solve(query).ToList();

            // Assert
            Assert.That(solutions.Count, Is.EqualTo(2));
            Assert.That(solutions.All(s => s.IsSuccess), Is.True);
            
            var bindings = solutions.Select(s => s.Bindings["X"]).ToList();
            Assert.That(bindings, Contains.Item(new Atom("bob")));
            Assert.That(bindings, Contains.Item(new Atom("alice")));
        }

        [Test]
        public void Solve_NoMatchingFacts_ReturnsNoSolutions()
        {
            // Arrange
            var fact = new Clause(new Compound("parent", new Atom("tom"), new Atom("bob")));
            knowledgeBase.AddClause(fact);
            
            var query = new Compound("parent", new Atom("alice"), new Atom("bob"));

            // Act
            var solutions = queryEngine.Solve(query).ToList();

            // Assert
            Assert.That(solutions.Count, Is.EqualTo(0));
        }

        [Test]
        public void Solve_SimpleRule_ReturnsCorrectSolution()
        {
            // Arrange
            // Add facts: parent(tom, bob), parent(bob, alice)
            knowledgeBase.AddClause(new Clause(new Compound("parent", new Atom("tom"), new Atom("bob"))));
            knowledgeBase.AddClause(new Clause(new Compound("parent", new Atom("bob"), new Atom("alice"))));
            
            // Add rule: grandparent(X, Z) :- parent(X, Y), parent(Y, Z)
            var rule = new Clause(
                new Compound("grandparent", new Variable("X"), new Variable("Z")),
                new List<Term> 
                {
                    new Compound("parent", new Variable("X"), new Variable("Y")),
                    new Compound("parent", new Variable("Y"), new Variable("Z"))
                }
            );
            knowledgeBase.AddClause(rule);
            
            var query = new Compound("grandparent", new Atom("tom"), new Variable("Z"));

            // Act
            var solutions = queryEngine.Solve(query).ToList();

            // Assert
            Assert.That(solutions.Count, Is.EqualTo(1));
            Assert.That(solutions[0].IsSuccess, Is.True);
            Assert.That(solutions[0].Bindings["Z"], Is.EqualTo(new Atom("alice")));
        }

        [Test]
        public void Solve_CompoundQuery_ReturnsCorrectSolution()
        {
            // Arrange
            knowledgeBase.AddClause(new Clause(new Compound("parent", new Atom("tom"), new Atom("bob"))));
            knowledgeBase.AddClause(new Clause(new Compound("parent", new Atom("bob"), new Atom("alice"))));
            
            // Query: parent(tom, X), parent(X, alice)
            var query = new Compound(",", 
                new Compound("parent", new Atom("tom"), new Variable("X")),
                new Compound("parent", new Variable("X"), new Atom("alice"))
            );

            // Act
            var solutions = queryEngine.Solve(query).ToList();

            // Assert
            Assert.That(solutions.Count, Is.EqualTo(1));
            Assert.That(solutions[0].IsSuccess, Is.True);
            Assert.That(solutions[0].Bindings["X"], Is.EqualTo(new Atom("bob")));
        }

        [Test]
        public void Constructor_NullKnowledgeBase_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new QueryEngine(null!, unificationEngine));
        }

        [Test]
        public void Constructor_NullUnificationEngine_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new QueryEngine(knowledgeBase, null!));
        }

        [Test]
        public void Solve_NullQuery_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => queryEngine.Solve(null!));
        }
    }
}