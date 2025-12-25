using Prolog;

namespace Prolog.Tests
{
    [TestFixture]
    public class UnificationEngineTests
    {
        UnificationEngine unificationEngine;

        [SetUp]
        public void SetUp()
        {
            unificationEngine = new UnificationEngine();
        }

        [Test]
        public void UnificationEngine_UnifiesIdenticalAtoms()
        {
            var atom1 = new Atom("tom");
            var atom2 = new Atom("tom");
            
            var result = unificationEngine.Unify(atom1, atom2);
            
            Assert.That(result.Success, Is.True);
            Assert.That(result.Bindings.Count, Is.EqualTo(0));
        }

        [Test]
        public void UnificationEngine_FailsOnDifferentAtoms()
        {
            var atom1 = new Atom("tom");
            var atom2 = new Atom("bob");
            
            var result = unificationEngine.Unify(atom1, atom2);
            
            Assert.That(result.Success, Is.False);
        }

        [Test]
        public void UnificationEngine_UnifiesVariableWithAtom()
        {
            var variable = new Variable("X");
            var atom = new Atom("tom");
            
            var result = unificationEngine.Unify(variable, atom);
            
            Assert.That(result.Success, Is.True);
            Assert.That(result.Bindings.Count, Is.EqualTo(1));
            Assert.That(result.Bindings["X"], Is.EqualTo(atom));
        }

        [Test]
        public void UnificationEngine_UnifiesAtomWithVariable()
        {
            var atom = new Atom("tom");
            var variable = new Variable("X");
            
            var result = unificationEngine.Unify(atom, variable);
            
            Assert.That(result.Success, Is.True);
            Assert.That(result.Bindings.Count, Is.EqualTo(1));
            Assert.That(result.Bindings["X"], Is.EqualTo(atom));
        }

        [Test]
        public void UnificationEngine_UnifiesTwoVariables()
        {
            var var1 = new Variable("X");
            var var2 = new Variable("Y");
            
            var result = unificationEngine.Unify(var1, var2);
            
            Assert.That(result.Success, Is.True);
            Assert.That(result.Bindings.Count, Is.EqualTo(1));
            // One variable should be bound to the other
            Assert.That(result.Bindings.ContainsKey("X") || result.Bindings.ContainsKey("Y"), Is.True);
        }

        [Test]
        public void UnificationEngine_UnifiesIdenticalCompounds()
        {
            var compound1 = new Compound("parent", new Atom("tom"), new Atom("bob"));
            var compound2 = new Compound("parent", new Atom("tom"), new Atom("bob"));
            
            var result = unificationEngine.Unify(compound1, compound2);
            
            Assert.That(result.Success, Is.True);
            Assert.That(result.Bindings.Count, Is.EqualTo(0));
        }

        [Test]
        public void UnificationEngine_FailsOnDifferentFunctors()
        {
            var compound1 = new Compound("parent", new Atom("tom"), new Atom("bob"));
            var compound2 = new Compound("likes", new Atom("tom"), new Atom("bob"));
            
            var result = unificationEngine.Unify(compound1, compound2);
            
            Assert.That(result.Success, Is.False);
        }

        [Test]
        public void UnificationEngine_FailsOnDifferentArities()
        {
            var compound1 = new Compound("parent", new Atom("tom"));
            var compound2 = new Compound("parent", new Atom("tom"), new Atom("bob"));
            
            var result = unificationEngine.Unify(compound1, compound2);
            
            Assert.That(result.Success, Is.False);
        }

        [Test]
        public void UnificationEngine_UnifiesCompoundsWithVariables()
        {
            var compound1 = new Compound("parent", new Variable("X"), new Atom("bob"));
            var compound2 = new Compound("parent", new Atom("tom"), new Atom("bob"));
            
            var result = unificationEngine.Unify(compound1, compound2);
            
            Assert.That(result.Success, Is.True);
            Assert.That(result.Bindings.Count, Is.EqualTo(1));
            Assert.That(result.Bindings["X"], Is.EqualTo(new Atom("tom")));
        }

        [Test]
        public void UnificationEngine_UnifiesNestedCompounds()
        {
            var compound1 = new Compound("likes", new Compound("person", new Variable("X")), new Atom("pizza"));
            var compound2 = new Compound("likes", new Compound("person", new Atom("tom")), new Atom("pizza"));
            
            var result = unificationEngine.Unify(compound1, compound2);
            
            Assert.That(result.Success, Is.True);
            Assert.That(result.Bindings.Count, Is.EqualTo(1));
            Assert.That(result.Bindings["X"], Is.EqualTo(new Atom("tom")));
        }

        [Test]
        public void UnificationEngine_HandlesExistingBindings()
        {
            var variable = new Variable("X");
            var atom = new Atom("tom");
            var existingBindings = new Dictionary<string, Term> { { "X", atom } };
            
            var result = unificationEngine.Unify(variable, atom, existingBindings);
            
            Assert.That(result.Success, Is.True);
            Assert.That(result.Bindings["X"], Is.EqualTo(atom));
        }

        [Test]
        public void UnificationEngine_FailsOnConflictingBindings()
        {
            var variable = new Variable("X");
            var atom1 = new Atom("tom");
            var atom2 = new Atom("bob");
            var existingBindings = new Dictionary<string, Term> { { "X", atom1 } };
            
            var result = unificationEngine.Unify(variable, atom2, existingBindings);
            
            Assert.That(result.Success, Is.False);
        }

        [Test]
        public void UnificationEngine_FollowsBindingChains()
        {
            var varX = new Variable("X");
            var varY = new Variable("Y");
            var atom = new Atom("tom");
            
            // First unify X with Y
            var result1 = unificationEngine.Unify(varX, varY);
            Assert.That(result1.Success, Is.True);
            
            // Then unify Y with tom using the bindings from first unification
            var result2 = unificationEngine.Unify(varY, atom, result1.Bindings);
            Assert.That(result2.Success, Is.True);
            
            // X should now be bound to tom (through Y)
            var finalX = varX.Substitute(result2.Bindings);
            Assert.That(finalX, Is.EqualTo(atom));
        }

        [Test]
        public void UnificationEngine_PreventsCyclicBindings()
        {
            var varX = new Variable("X");
            var compound = new Compound("f", varX);
            
            // Try to unify X with f(X) - should fail due to occurs check
            var result = unificationEngine.Unify(varX, compound);
            
            Assert.That(result.Success, Is.False);
        }

        [Test]
        public void UnificationEngine_AllowsCyclicBindingsWhenOccursCheckDisabled()
        {
            var engineWithoutOccursCheck = new UnificationEngine(enableOccursCheck: false);
            var varX = new Variable("X");
            var compound = new Compound("f", varX);
            
            // Should succeed when occurs check is disabled
            var result = engineWithoutOccursCheck.Unify(varX, compound);
            
            Assert.That(result.Success, Is.True);
            Assert.That(result.Bindings["X"], Is.EqualTo(compound));
        }

        [Test]
        public void UnificationEngine_UnifiesComplexTerms()
        {
            // grandparent(X, Z) with grandparent(tom, alice)
            var term1 = new Compound("grandparent", new Variable("X"), new Variable("Z"));
            var term2 = new Compound("grandparent", new Atom("tom"), new Atom("alice"));
            
            var result = unificationEngine.Unify(term1, term2);
            
            Assert.That(result.Success, Is.True);
            Assert.That(result.Bindings.Count, Is.EqualTo(2));
            Assert.That(result.Bindings["X"], Is.EqualTo(new Atom("tom")));
            Assert.That(result.Bindings["Z"], Is.EqualTo(new Atom("alice")));
        }

        [Test]
        public void UnificationEngine_HandlesZeroArityCompounds()
        {
            var compound1 = new Compound("fact", new List<Term>());
            var compound2 = new Compound("fact", new List<Term>());
            
            var result = unificationEngine.Unify(compound1, compound2);
            
            Assert.That(result.Success, Is.True);
            Assert.That(result.Bindings.Count, Is.EqualTo(0));
        }

        [Test]
        public void UnificationEngine_CanUnifyConvenienceMethods()
        {
            var atom1 = new Atom("tom");
            var atom2 = new Atom("tom");
            var atom3 = new Atom("bob");
            
            Assert.That(unificationEngine.CanUnify(atom1, atom2), Is.True);
            Assert.That(unificationEngine.CanUnify(atom1, atom3), Is.False);
        }

        [Test]
        public void UnificationEngine_ThrowsOnNullTerms()
        {
            var atom = new Atom("tom");
            
            Assert.Throws<ArgumentNullException>(() => unificationEngine.Unify(null!, atom));
            Assert.Throws<ArgumentNullException>(() => unificationEngine.Unify(atom, null!));
        }

        [Test]
        public void UnificationEngine_ThrowsOnNullBindings()
        {
            var atom1 = new Atom("tom");
            var atom2 = new Atom("tom");
            
            Assert.Throws<ArgumentNullException>(() => unificationEngine.Unify(atom1, atom2, null!));
        }

        [Test]
        public void UnificationEngine_PreservesOriginalBindings()
        {
            var variable = new Variable("X");
            var atom = new Atom("tom");
            var originalBindings = new Dictionary<string, Term> { { "Y", new Atom("bob") } };
            
            var result = unificationEngine.Unify(variable, atom, originalBindings);
            
            Assert.That(result.Success, Is.True);
            // Original bindings should be preserved
            Assert.That(originalBindings.Count, Is.EqualTo(1));
            Assert.That(originalBindings["Y"], Is.EqualTo(new Atom("bob")));
            // Result should have both bindings
            Assert.That(result.Bindings.Count, Is.EqualTo(2));
            Assert.That(result.Bindings["Y"], Is.EqualTo(new Atom("bob")));
            Assert.That(result.Bindings["X"], Is.EqualTo(atom));
        }
    }
}