using System.Collections.Generic;
using System.Linq;

namespace Prolog
{
    /// <summary>
    /// Represents a Prolog compound term with a functor and arguments
    /// Examples: parent(tom, bob), likes(X, Y), f(a, g(b, c))
    /// </summary>
    public class Compound : Term
    {
        public string Functor { get; }
        public List<Term> Arguments { get; }
        public int Arity => Arguments.Count;

        public Compound(string functor, List<Term> arguments)
        {
            Functor = functor ?? throw new ArgumentNullException(nameof(functor));
            Arguments = arguments ?? throw new ArgumentNullException(nameof(arguments));
        }

        public Compound(string functor, params Term[] arguments)
            : this(functor, arguments.ToList())
        {
        }

        public override bool IsVariable => false;

        public override Term Substitute(Dictionary<string, Term> bindings)
        {
            // Substitute all arguments recursively
            var substitutedArgs = Arguments.Select(arg => arg.Substitute(bindings)).ToList();
            return new Compound(Functor, substitutedArgs);
        }

        public override string ToString()
        {
            if (Arguments.Count == 0)
            {
                return Functor;
            }

            var argStrings = Arguments.Select(arg => arg.ToString());
            return $"{Functor}({string.Join(", ", argStrings)})";
        }

        public override bool Equals(object? obj)
        {
            if (obj is not Compound other)
                return false;

            if (Functor != other.Functor || Arity != other.Arity)
                return false;

            for (int i = 0; i < Arguments.Count; i++)
            {
                if (!Arguments[i].Equals(other.Arguments[i]))
                    return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            var hash = Functor.GetHashCode();
            foreach (var arg in Arguments)
            {
                hash = HashCode.Combine(hash, arg.GetHashCode());
            }
            return hash;
        }
    }
}