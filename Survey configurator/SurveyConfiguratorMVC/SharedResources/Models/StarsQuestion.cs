
namespace SharedResources.Models
{
    public class StarsQuestion : Question
    {
        public string Text { get; set; }
        public int Order { get; set; }
        public int NumberOfStars { get; set; }

        public StarsQuestion(int pId, string pText, int pOrder, int pNumberOfStars = SharedData.cMaxNumberOfStars) : base(pId, pText, pOrder, eQuestionType.Stars)
        {
            NumberOfStars = pNumberOfStars;
        }

        public StarsQuestion(Question pQuestionData, int pNumberOfstars = SharedData.cMaxNumberOfStars) : base(pQuestionData.Id, pQuestionData.Text, pQuestionData.Order, eQuestionType.Stars)
        {
            NumberOfStars = pNumberOfstars;
        }
    }
}
