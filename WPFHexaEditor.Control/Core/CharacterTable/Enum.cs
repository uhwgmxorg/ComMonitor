//////////////////////////////////////////////
// Apache 2.0  - 2003-2017
// Author : Derek Tremblay (derektremblay666@gmail.com)
//////////////////////////////////////////////

namespace WpfHexaEditor.Core.CharacterTable
{
    /// <summary>
    /// Type de DTE qui sera utilis� dans les classe de DTE
    /// </summary>
    public enum DteType
    {
        Invalid = -1,
        Ascii = 0,
        Japonais,
        DualTitleEncoding,
        MultipleTitleEncoding,
        EndLine,
        EndBlock
    }

    public enum DefaultCharacterTableType
    {
        Ascii
        //ADD OTHERTYPE...
        //EBCDIC
        //MACINTOSH
        //DOS/IBM-ASCII
    }
}