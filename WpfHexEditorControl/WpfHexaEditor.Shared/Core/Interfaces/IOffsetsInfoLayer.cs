using System.Windows.Controls;

namespace WpfHexaEditor.Core.Interfaces
{
    public interface IOffsetsInfoLayer
    {
        Orientation Orientation { get; set; }
        long StartStepIndex { get; set; }

        //Count of rows/cols that will be shown;
        int StepsCount { get; set; }

        //The subtitution of  two neibor row line number;
        int StepLength { get; set; }

        /// <summary>
        /// To Show how "wide" the char will be displayed.
        /// </summary>
        int SavedBits { get; set; }

        DataVisualType DataVisualType { get; set; }
    }
}
