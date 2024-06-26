namespace SharedResources
{
    public enum eQuestionType
    {
        Stars = 0,
        Smiley = 1,
        Slider = 2,
    }

    public class SharedData
    {
        /// <summary>
        /// this class contains resources used across the application layers
        /// such as enums or constants
        /// </summary>

        //constant
        //Question objects constants
        public const int cQuestionTextLength = 350;

        public const int cMinNumberOfStars = 1;
        public const int cMaxNumberOfStars = 10;

        public const int cMinNumberOfSmileyFaces = 2;
        public const int cMaxNumberOfSmileyFaces = 5;

        public const int cMinStartValue = 0;
        public const int cMaxStartValue = 100;
        public const int cMinEndValue = 0;
        public const int cMaxEndValue = 100;
        public const string cDefaultStartValueCaption = "Min";
        public const string cDefaultEndValueCaption = "Max";
        

    }
}
