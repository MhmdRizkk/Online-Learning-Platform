using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace OnlineLearningPlatform.API.Services
{
    public class CertificatePdfService
    {
        public string GenerateCertificate(
            string studentName,
            string courseTitle,
            DateTime generatedAt)
        {
            var folderPath = Path.Combine("wwwroot", "certificates");
            Directory.CreateDirectory(folderPath);

            var fileName = $"{Guid.NewGuid()}.pdf";
            var fullPath = Path.Combine(folderPath, fileName);

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(50);
                    page.DefaultTextStyle(x => x.FontSize(16));

                    page.Content()
                        .Column(column =>
                        {
                            column.Spacing(20);

                            column.Item().AlignCenter().Text("Certificate of Completion")
                                .FontSize(32).Bold();

                            column.Item().AlignCenter().Text("This certifies that");

                            column.Item().AlignCenter().Text(studentName)
                                .FontSize(24).Bold();

                            column.Item().AlignCenter().Text("has successfully completed the course");

                            column.Item().AlignCenter().Text(courseTitle)
                                .FontSize(20).Bold();

                            column.Item().AlignCenter()
                                .Text($"Date: {generatedAt:yyyy-MM-dd}");

                            column.Item().PaddingTop(40)
                                .AlignCenter()
                                .Text("Online Learning Platform")
                                .Italic();
                        });
                });
            }).GeneratePdf(fullPath);

            return $"certificates/{fileName}";
        }
    }
}
