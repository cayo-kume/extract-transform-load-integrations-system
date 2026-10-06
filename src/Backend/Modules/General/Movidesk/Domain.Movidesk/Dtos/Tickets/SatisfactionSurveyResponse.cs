namespace Domain.Movidesk.Dtos.Tickets
{
    public class SatisfactionSurveyResponse
    {
        public string id { get; init; }
        public string responseDate { get; init; }
        public string satisfactionSurveyModel { get; init; }
        public string satisfactionSurveyNetPromoterScoreResponse { get; init; }
        public string satisfactionSurveyPositiveNegativeResponse { get; init; }
        public string satisfactionSurveySmileyFacesResponse { get; init; }
        public string comments { get; init; }
        
        public Person responsedBy { get; init; }
    }
}
