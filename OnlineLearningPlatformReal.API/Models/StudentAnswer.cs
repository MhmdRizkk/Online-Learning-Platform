namespace OnlineLearningPlatform.API.Models
{
    public class StudentAnswer
    {
        public int Id { get; set; }

        public int QuizAttemptId { get; set; }
        public int QuestionId { get; set; }
        public int? AnswerId { get; set; } 

        public string StudentTextAnswer { get; set; }
        public bool IsCorrect { get; set; }
        public DateTime AnsweredAt { get; set; }

        public QuizAttempt QuizAttempt { get; set; }
        public Question Question { get; set; }
        public Answer Answer { get; set; }
    }

}
