using Prolog;

namespace Prolog.Tests
{
    [TestFixture]
    public class KnowledgeBaseTests
    {
        KnowledgeBase knowledgeBase;

        [SetUp]
        public void SetUp()
        {
            knowledgeBase = new KnowledgeBase();
        }

        [Test]
        public void KnowledgeBase_StartsEmpty()
        {
            Assert.That(knowledgeBase.ClauseCount, Is.EqualTo(0));
            
            var stats = knowledgeBase.GetStats();
            Assert.That(stats.TotalClauses, Is.EqualTo(0));
            Assert.That(stats.FactCount, Is.EqualTo(0));
            Assert.That(stats.RuleCount, Is.EqualTo(0));
            Assert.That(stats.UniqueFunctors, Is.EqualTo(0));
        }

        [Test]
        public void KnowledgeBase_AddsFact()
        {
            var fact = new Clause(new Compound("parent", new Atom("tom"), new Atom("bob")));
            
            knowledgeBase.AddClause(fact);
            
            Assert.That(knowledgeBase.ClauseCount, Is.EqualTo(1));
            
            var stats = knowledgeBase.GetStats();
            Assert.That(stats.FactCount, Is.EqualTo(1));
            Assert.That(stats.RuleCount, Is.EqualTo(0));
        }

        [Test]
        public void KnowledgeBase_AddsRule()
        {
            var head = new Compound("grandparent", new Variable("X"), new Variable("Z"));
            var body = new List<Term>
            {
                new Compound("parent", new Variable("X"), new Variable("Y")),
                new Compound("parent", new Variable("Y"), new Variable("Z"))
            };
            var rule = new Clause(head, body);
            
            knowledgeBase.AddClause(rule);
            
            Assert.That(knowledgeBase.ClauseCount, Is.EqualTo(1));
            
            var stats = knowledgeBase.GetStats();
            Assert.That(stats.FactCount, Is.EqualTo(0));
            Assert.That(stats.RuleCount, Is.EqualTo(1));
        }

        [Test]
        public void KnowledgeBase_RetrievesMatchingClauses()
        {
            // Add some facts
            var fact1 = new Clause(new Compound("parent", new Atom("tom"), new Atom("bob")));
            var fact2 = new Clause(new Compound("parent", new Atom("bob"), new Atom("alice")));
            var fact3 = new Clause(new Compound("likes", new Atom("mary"), new Atom("chocolate")));
            
            knowledgeBase.AddClause(fact1);
            knowledgeBase.AddClause(fact2);
            knowledgeBase.AddClause(fact3);
            
            // Query for parent facts
            var goal = new Compound("parent", new Variable("X"), new Variable("Y"));
            var matches = knowledgeBase.GetMatchingClauses(goal).ToList();
            
            Assert.That(matches.Count, Is.EqualTo(2));
            Assert.That(matches[0], Is.EqualTo(fact1));
            Assert.That(matches[1], Is.EqualTo(fact2));
        }

        [Test]
        public void KnowledgeBase_RetrievesNoMatchesForUnknownFunctor()
        {
            var fact = new Clause(new Compound("parent", new Atom("tom"), new Atom("bob")));
            knowledgeBase.AddClause(fact);
            
            var goal = new Compound("unknown", new Variable("X"));
            var matches = knowledgeBase.GetMatchingClauses(goal).ToList();
            
            Assert.That(matches.Count, Is.EqualTo(0));
        }

        [Test]
        public void KnowledgeBase_MaintainsInsertionOrder()
        {
            var fact1 = new Clause(new Compound("parent", new Atom("tom"), new Atom("bob")));
            var fact2 = new Clause(new Compound("parent", new Atom("bob"), new Atom("alice")));
            var fact3 = new Clause(new Compound("parent", new Atom("alice"), new Atom("charlie")));
            
            knowledgeBase.AddClause(fact1);
            knowledgeBase.AddClause(fact2);
            knowledgeBase.AddClause(fact3);
            
            var goal = new Compound("parent", new Variable("X"), new Variable("Y"));
            var matches = knowledgeBase.GetMatchingClauses(goal).ToList();
            
            Assert.That(matches.Count, Is.EqualTo(3));
            Assert.That(matches[0], Is.EqualTo(fact1));
            Assert.That(matches[1], Is.EqualTo(fact2));
            Assert.That(matches[2], Is.EqualTo(fact3));
        }

        [Test]
        public void KnowledgeBase_SupportsMultipleFunctors()
        {
            var parentFact = new Clause(new Compound("parent", new Atom("tom"), new Atom("bob")));
            var likesFact = new Clause(new Compound("likes", new Atom("mary"), new Atom("chocolate")));
            var ageFact = new Clause(new Compound("age", new Atom("tom"), new Atom("30")));
            
            knowledgeBase.AddClause(parentFact);
            knowledgeBase.AddClause(likesFact);
            knowledgeBase.AddClause(ageFact);
            
            var stats = knowledgeBase.GetStats();
            Assert.That(stats.UniqueFunctors, Is.EqualTo(3));
            
            // Test each functor separately
            var parentMatches = knowledgeBase.GetMatchingClauses(new Compound("parent", new Variable("X"), new Variable("Y"))).ToList();
            Assert.That(parentMatches.Count, Is.EqualTo(1));
            
            var likesMatches = knowledgeBase.GetMatchingClauses(new Compound("likes", new Variable("X"), new Variable("Y"))).ToList();
            Assert.That(likesMatches.Count, Is.EqualTo(1));
            
            var ageMatches = knowledgeBase.GetMatchingClauses(new Compound("age", new Variable("X"), new Variable("Y"))).ToList();
            Assert.That(ageMatches.Count, Is.EqualTo(1));
        }

        [Test]
        public void KnowledgeBase_HandlesAtomGoals()
        {
            var atomFact = new Clause(new Atom("sunny"));
            knowledgeBase.AddClause(atomFact);
            
            var goal = new Atom("sunny");
            var matches = knowledgeBase.GetMatchingClauses(goal).ToList();
            
            Assert.That(matches.Count, Is.EqualTo(1));
            Assert.That(matches[0], Is.EqualTo(atomFact));
        }

        [Test]
        public void KnowledgeBase_HandlesZeroArityCompounds()
        {
            var zeroArityFact = new Clause(new Compound("fact", new List<Term>()));
            knowledgeBase.AddClause(zeroArityFact);
            
            var goal = new Compound("fact", new List<Term>());
            var matches = knowledgeBase.GetMatchingClauses(goal).ToList();
            
            Assert.That(matches.Count, Is.EqualTo(1));
            Assert.That(matches[0], Is.EqualTo(zeroArityFact));
        }

        [Test]
        public void KnowledgeBase_DistinguishesByArity()
        {
            var fact1 = new Clause(new Compound("test", new List<Term>())); // test/0
            var fact2 = new Clause(new Compound("test", new Atom("arg"))); // test/1
            var fact3 = new Clause(new Compound("test", new Atom("arg1"), new Atom("arg2"))); // test/2
            
            knowledgeBase.AddClause(fact1);
            knowledgeBase.AddClause(fact2);
            knowledgeBase.AddClause(fact3);
            
            // Query for test/1
            var goal = new Compound("test", new Variable("X"));
            var matches = knowledgeBase.GetMatchingClauses(goal).ToList();
            
            Assert.That(matches.Count, Is.EqualTo(1));
            Assert.That(matches[0], Is.EqualTo(fact2));
        }

        [Test]
        public void KnowledgeBase_ClearsCorrectly()
        {
            var fact = new Clause(new Compound("parent", new Atom("tom"), new Atom("bob")));
            knowledgeBase.AddClause(fact);
            
            Assert.That(knowledgeBase.ClauseCount, Is.EqualTo(1));
            
            knowledgeBase.Clear();
            
            Assert.That(knowledgeBase.ClauseCount, Is.EqualTo(0));
            
            var goal = new Compound("parent", new Variable("X"), new Variable("Y"));
            var matches = knowledgeBase.GetMatchingClauses(goal).ToList();
            Assert.That(matches.Count, Is.EqualTo(0));
        }

        [Test]
        public void KnowledgeBase_GetAllClauses()
        {
            var fact1 = new Clause(new Compound("parent", new Atom("tom"), new Atom("bob")));
            var fact2 = new Clause(new Compound("likes", new Atom("mary"), new Atom("chocolate")));
            
            knowledgeBase.AddClause(fact1);
            knowledgeBase.AddClause(fact2);
            
            var allClauses = knowledgeBase.GetAllClauses().ToList();
            
            Assert.That(allClauses.Count, Is.EqualTo(2));
            Assert.That(allClauses[0], Is.EqualTo(fact1));
            Assert.That(allClauses[1], Is.EqualTo(fact2));
        }

        [Test]
        public void KnowledgeBase_GetClausesByFunctor()
        {
            var parentFact1 = new Clause(new Compound("parent", new Atom("tom"), new Atom("bob")));
            var parentFact2 = new Clause(new Compound("parent", new Atom("bob"), new Atom("alice")));
            var likesFact = new Clause(new Compound("likes", new Atom("mary"), new Atom("chocolate")));
            
            knowledgeBase.AddClause(parentFact1);
            knowledgeBase.AddClause(parentFact2);
            knowledgeBase.AddClause(likesFact);
            
            var parentClauses = knowledgeBase.GetClausesByFunctor("parent/2").ToList();
            Assert.That(parentClauses.Count, Is.EqualTo(2));
            
            var likesClauses = knowledgeBase.GetClausesByFunctor("likes/2").ToList();
            Assert.That(likesClauses.Count, Is.EqualTo(1));
            
            var unknownClauses = knowledgeBase.GetClausesByFunctor("unknown/1").ToList();
            Assert.That(unknownClauses.Count, Is.EqualTo(0));
        }

        [Test]
        public void KnowledgeBase_ThrowsOnNullClause()
        {
            Assert.Throws<ArgumentNullException>(() => knowledgeBase.AddClause(null!));
        }

        [Test]
        public void KnowledgeBase_ThrowsOnNullGoal()
        {
            Assert.Throws<ArgumentNullException>(() => knowledgeBase.GetMatchingClauses(null!));
        }
    }
}