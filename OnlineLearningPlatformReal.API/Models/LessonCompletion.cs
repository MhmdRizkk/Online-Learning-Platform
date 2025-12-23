namespace OnlineLearningPlatform.API.Models
{
    public class LessonCompletion
    {
        public int Id { get; set; }

        public int LessonId { get; set; }
        public int UserId { get; set; }
        public DateTime CompletionDate { get; set; }

        public Lesson Lesson { get; set; }
        public User User { get; set; }

    }

}
