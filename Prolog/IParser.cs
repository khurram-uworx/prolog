namespace Prolog
{
    /// <summary>
    /// Interface for parsing Prolog source code into internal structures
    /// </summary>
    public interface IParser
    {
        /// <summary>
        /// Parses a Prolog program (facts and rules) from source text
        /// </summary>
        ParseResult ParseProgram(string source);

        /// <summary>
        /// Parses a Prolog query from source text
        /// </summary>
        ParseResult ParseQuery(string source);
    }
}